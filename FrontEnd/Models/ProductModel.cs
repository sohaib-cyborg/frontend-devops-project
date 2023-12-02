﻿namespace FrontEnd.Models
{
    public class ProductModel
    {
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string category { get; set; } = string.Empty;
        public double price { get; set; }
        public int quantity { get; set; }
        public string? image { get; set; }
    }
}
