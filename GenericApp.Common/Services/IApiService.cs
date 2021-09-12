using GenericApp.Common.Requests;
using GenericApp.Common.Responses;
using System.IO;
using System.Threading.Tasks;

namespace GenericApp.Common.Services
{
    public interface IApiService
    {
        Task<ResponseT<UsuarioAppResponse>> GetUserByEmailAsync(
           string urlBase,
           string servicePrefix,
           string controller,
           string email,
           string password);

        Task<ResponseT<object>> GetUsuarios(
          string urlBase,
          string servicePrefix,
          string controller);

        Task<ResponseT<object>> GetObras(
          string urlBase,
          string servicePrefix,
          string controller);


        Task<Response> GetListAsync<T>(string urlBase, string servicePrefix, string controller);

        Task<Response> GetTokenAsync(string urlBase, string servicePrefix, string controller, TokenRequest request);

        Task<Response> RegisterUserAsync(string urlBase, string servicePrefix, string controller, UserRequest userRequest);

        Task<Response> RecoverPasswordAsync(string urlBase, string servicePrefix, string controller, EmailRequest emailRequest);

        Task<Response> ModifyUserAsync(string urlBase, string servicePrefix, string controller, UserRequest userRequest, string token);

        Task<Response> ChangePasswordAsync(string urlBase, string servicePrefix, string controller, ChangePasswordRequest changePasswordRequest, string token);

        //Task<Response> PostAsync<T>(string urlBase, string servicePrefix, string controller, T model, string token);

        Task<Response> GetListAsync<T>(string urlBase, string servicePrefix, string controller, string token);

        Task<Response> PutAsync<T>(string urlBase, string servicePrefix, string controller, T model, string token);

        Task<Stream> GetPictureAsync(string urlBase, string servicePrefix);
        
        Task<ResponseT<object>> PostAsync<T>(
          string urlBase,
          string servicePrefix,
          string controller,
          T model);

        Task<ResponseT<object>> PutAsync<T>(
           string urlBase,
           string servicePrefix,
           string controller,
           int id,
           T model,
           string tokenType,
           string accessToken);

        Task<Response> DeleteAsync(
           string urlBase,
           string servicePrefix,
           string controller,
           int id);

        Task<Response> DeleteAsync(
           string urlBase,
           string servicePrefix,
           string controller,
           int id,
           string tokenType,
           string accessToken);
    }
}