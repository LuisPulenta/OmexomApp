using GenericApp.Common.Responses;

namespace GenericApp.Web.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string to, string subject, string body);
    }
}