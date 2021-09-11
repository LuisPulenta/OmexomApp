namespace GenericApp.Common.Responses
{
    public class CategoryResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImagePath { get; set; }

        public string ImageFullPath => string.IsNullOrEmpty(ImagePath)
           ? $"http://keypress.serveftp.net:88/GenericAppApi/images/Categories/noimage.png"
           : $"http://keypress.serveftp.net:88/GenericAppApi{ImagePath.Substring(1)}";
    }
}