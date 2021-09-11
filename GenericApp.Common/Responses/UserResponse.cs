using GenericApp.Common.Enums;

namespace GenericApp.Common.Responses
{
    public class UserResponse
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Document { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string PicturePath { get; set; }

        public string PictureFullPath => string.IsNullOrEmpty(PicturePath)
            ? $"http://keypress.serveftp.net:88/GenericAppiApi/images/Users/nouser.png"
            : $"http://keypress.serveftp.net:88/GenericAppApi{PicturePath.Substring(1)}";

        public UserType UserType { get; set; }

        public CityResponse City { get; set; }

        public TeamResponse FavoriteTeam { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string FullNameWithDocument => $"{FirstName} {LastName} - {Document}";
    }
}