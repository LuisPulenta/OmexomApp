using GenericApp.Common.Responses;
using System.Collections.Generic;

namespace GenericApp.Common.Requests
{
    public class ProductRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public CategoryResponse Category { get; set; }
        public StateResponse State { get; set; }
        public byte[] PhotoArray { get; set; }
        public string PhotoUrl { get; set; }
        public int ProductId { get; set; }
        public ICollection<ProductImageResponse> ProductImages { get; set; }
    }
}