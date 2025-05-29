namespace QuickMart.Models.Payment
{
    public class PaymobPaymentKeyRequest
    {
        public string auth_token { get; set; }
        public int amount_cents { get; set; }
        public int expiration { get; set; }
        public int order_id { get; set; }
        public PaymobBillingData billing_data { get; set; }
        public string currency { get; set; }
        public int integration_id { get; set; }
    }
}
