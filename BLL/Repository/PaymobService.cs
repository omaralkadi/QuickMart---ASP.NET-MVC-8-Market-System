// Services/PaymobService.cs
using System.Security.Cryptography;
using System.Text;
using BLL.Interface;
using DAL.Entities;
using DAL.Entities.Payment;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickMart.Models.Payment;

public class PaymobService : IPaymobService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly int _integrationId;
    private readonly string _hmacSecret;
    private readonly string _baseUrl;

    public PaymobService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _apiKey = _configuration["PaymobSettings:ApiKey"];
        _integrationId = int.Parse(_configuration["PaymobSettings:IntegrationId"]);
        _hmacSecret = _configuration["PaymobSettings:HmacSecret"];
        _baseUrl = _configuration["PaymobSettings:BaseUrl"];
    }

    public async Task<PaymentResult> CreatePaymentTokenAsync(Order order, ApplicationUser user)
    {
        try
        {
            // Step 1: Get Authentication Token
            var authToken = await GetAuthTokenAsync();

            // Step 2: Create Order
            var paymobOrder = await CreateOrderAsync(authToken, order);

            // Step 3: Generate Payment Key
            var paymentKey = await GetPaymentKeyAsync(authToken, order, paymobOrder.id, user);

            var c = paymentKey;

            return new PaymentResult
            {
                PaymentToken = paymentKey,
                PaymobOrderId = paymobOrder.id.ToString(),
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new PaymentResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private async Task<string> GetAuthTokenAsync()
    {
        var request = new PaymobAuthRequest { api_key = _apiKey };
        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}auth/tokens", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to get auth token: {responseContent}");

        var authResponse = JsonConvert.DeserializeObject<PaymobAuthResponse>(responseContent);
        return authResponse.token;
    }

    private async Task<PaymobOrderResponse> CreateOrderAsync(string authToken, Order order)
    {
        var request = new PaymobOrderRequest
        {
            auth_token = authToken,
            delivery_needed = false,
            amount_cents = (int)(order.TotalPrice * 100), // Convert to cents
            currency = "EGP",
            items = new List<PaymobOrderItem>
            {
                new PaymobOrderItem
                {
                    name = $"Order #{order.Id}",
                    description = $"Payment for order #{order.Id}",
                    amount_cents = (int)(order.TotalPrice * 100),
                    quantity = 1
                }
            }
        };

        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}ecommerce/orders", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to create order: {responseContent}");

        return JsonConvert.DeserializeObject<PaymobOrderResponse>(responseContent);
    }

    private async Task<string> GetPaymentKeyAsync(string authToken, Order order, int paymobOrderId, ApplicationUser user)
    {
        var nameParts = (user.UserName ?? "Unknown User").Split(' ');
        var firstName = nameParts.Length > 0 ? nameParts[0] : "Unknown";
        var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "User";

        var request = new PaymobPaymentKeyRequest
        {
            auth_token = authToken,
            amount_cents = (int)(order.TotalPrice * 100),
            expiration = 3600, // 1 hour
            order_id = paymobOrderId,
            billing_data = new PaymobBillingData
            {
                apartment = "NA",
                email = user.Email ?? "test@example.com",
                floor = "NA",
                first_name = firstName,
                street = order.BillingAddress ?? "NA",
                building = "NA",
                phone_number = order.Phone ?? "+201234567890",
                shipping_method = "PKG",
                postal_code = "11511",
                city = "Cairo",
                country = "Egypt",
                last_name = lastName,
                state = "Cairo"
            },
            currency = "EGP",
            integration_id = _integrationId
        };

        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}acceptance/payment_keys", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Failed to get payment key: {responseContent}");

        var keyResponse = JsonConvert.DeserializeObject<PaymobPaymentKeyResponse>(responseContent);
        return keyResponse.token;
    }

}