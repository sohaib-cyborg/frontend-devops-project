namespace FrontEnd.Models
{
    public class AddressViewModel
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;
        public string houseNum { get; set; } = string.Empty;

        public string areaCode { get; set; } = string.Empty;

        public string area { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
    }
}
