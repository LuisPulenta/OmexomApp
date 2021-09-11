namespace GenericApp.Common.Responses
{
    public class ProductImageResponse
    {
        public int Id { get; set; }

        public string ImagePath { get; set; }

        public int ProductId { get; set; }

        public string ImageFullPath => string.IsNullOrEmpty(ImagePath)
           ? $"http://keypress.serveftp.net:88/GenericAppApi/images/Products/noimage.png"
        : $"http://keypress.serveftp.net:88/GenericAppApi{ImagePath.Substring(1)}";
    }
}