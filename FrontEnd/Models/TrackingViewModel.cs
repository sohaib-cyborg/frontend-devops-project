namespace FrontEnd.Models
{
    public class TrackingViewModel
    {
        public Guid TrackingId { get; set; }
        public DateTime ShippingDate { get; set; }
        public string Status { get; set; }
        public string Total { get; set; }
    }
}
