namespace QuickMart.Models.Payment
{
    public class PaymobOrderItem
    {
        public string name { get; set; }
        public string description { get; set; }
        public int amount_cents { get; set; }
        public int quantity { get; set; }
    }
}
