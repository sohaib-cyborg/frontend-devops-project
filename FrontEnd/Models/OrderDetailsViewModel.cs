namespace FrontEnd.Models
{
    public class OrderDetailsViewModel
    {
        public Guid ProductId { get; set; }
        public int ItemQuantity { get; set; }
        public string name { get; set; }
        public double total { get; set; }
    }
}
