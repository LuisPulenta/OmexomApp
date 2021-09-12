using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Data.Entities
{
    public class SubContratistasUsrWeb
    {
        [Key]
        public int ID { get; set; }
        public string CODIGO { get; set; }
        public string APELLIDONOMBRE { get; set; }
        public string USRLOGIN { get; set; }
        public string USRCONTRASENA { get; set; }
        public int? PERFIL { get; set; }
    }
}