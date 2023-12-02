namespace FrontEnd.Models
{
    public class OrderViewModel
    {
        public Guid OrderId { get; set; }
        public string CompleteOrderStatus { get; set; }
        public ICollection<TrackingViewModel> TrackingList { get; set; }
    }
}
