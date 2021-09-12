using GenericApp.Common.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenericApp.Prism.Models
{
    class UsuarioAppResponseRepository
    {
        public IList<UsuarioAppResponse> Users { get; set; }

        public UsuarioAppResponseRepository()
        {
            Task.Run(async () => Users = await App.Database.GetItemsAsync()).Wait();
        }

        public IList<UsuarioAppResponse> GetAll()
        {
            return Users;
        }
    }
}
