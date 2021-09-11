using System.Collections.Generic;

namespace GenericApp.Common.Responses
{
    public class CountryResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string FlagImagePath { get; set; }

        public string FlagImageFullPath => string.IsNullOrEmpty(FlagImagePath)
          ? $"http://keypress.serveftp.net:88/GenericAppApi/images/Flags/noimage.png"
          : $"http://keypress.serveftp.net:88/GenericAppApi{FlagImagePath.Substring(1)}";

        public ICollection<DepartmentResponse> Departments { get; set; }

        public ICollection<TeamResponse> Teams { get; set; }
    }
}