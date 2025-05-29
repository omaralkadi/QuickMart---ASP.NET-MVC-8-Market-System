namespace QuickMart.Models.Payment
{
    public class PaymobCallbackData
    {
        public bool success { get; set; }
        public int id { get; set; }
        public PaymobCallbackOrder order { get; set; }
        public string pending { get; set; }
        public int amount_cents { get; set; }
    }
}
