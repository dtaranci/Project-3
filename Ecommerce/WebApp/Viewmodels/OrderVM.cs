namespace WebApp.Viewmodels
{
    public class OrderVM
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int PaymentMethodId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal? Total { get; set; }
    }
}
