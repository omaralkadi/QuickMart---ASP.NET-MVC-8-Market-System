namespace QuickMart.Models.Payment
{
    public class PaymobOrderRequest
    {
        public string auth_token { get; set; }
        public bool delivery_needed { get; set; }
        public int amount_cents { get; set; }
        public string currency { get; set; }
        public List<PaymobOrderItem> items { get; set; }
    }
}
