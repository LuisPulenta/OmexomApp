using System.Collections.Generic;
using System.Linq;

namespace GenericApp.Common.Responses
{
    public class ProductResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public CategoryResponse Category { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public StateResponse State { get; set; }

        public ICollection<ProductImageResponse> ProductImages { get; set; }

        public int ProductImagesNumber => ProductImages == null ? 0 : ProductImages.Count;

        public string ImageFullPath => ProductImages == null || ProductImages.Count == 0
            ? $"http://keypress.serveftp.net:88/GenericAppApi/images/Products/noimage.png"
            : ProductImages.FirstOrDefault().ImageFullPath;
    }
}