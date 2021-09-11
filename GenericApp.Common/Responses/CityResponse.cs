namespace GenericApp.Common.Responses
{
    public class CityResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DepartmentResponse Department { get; set; }
    }
}