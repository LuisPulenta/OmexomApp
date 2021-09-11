using GenericApp.Common.Responses;

namespace GenericApp.Common.Requests
{
    public class ProductImageRequest
    {
        public int Id { get; set; }

        public byte[] ImageArray { get; set; }

        public string ImageUrl { get; set; }

        public ProductResponse Product { get; set; }
    }
}