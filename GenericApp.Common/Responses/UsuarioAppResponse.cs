using SQLite;

namespace GenericApp.Common.Responses
{
    public class UsuarioAppResponse
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string CODIGO { get; set; }
        public string APELLIDONOMBRE { get; set; }
        public string USRLOGIN { get; set; }
        public string USRCONTRASENA { get; set; }
        public int? PERFIL { get; set; }
    }
}