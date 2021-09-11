namespace GenericApp.Common.Responses
{
    public class TeamResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public CountryResponse Country { get; set; }

        public string LogoImagePath { get; set; }

        public string LogoImageFullPath => string.IsNullOrEmpty(LogoImagePath)
          ? $"http://keypress.serveftp.net:88/GenericAppApi/images/Logos/noimage.png"
          : $"http://keypress.serveftp.net:88/GenericAppApi{LogoImagePath.Substring(1)}";
    }
}