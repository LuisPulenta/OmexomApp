using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GenericApp.Web.Data.Entities
{
    public class Obra
    {
        [Key]
        public int NroObra { get; set; }
        public string NombreObra { get; set; }
        public string ELEMPEP { get; set; }
        public int Finalizada { get; set; }
        public string SUPERVISORE { get; set; }
        public string CodigoEstado { get; set; }
        public string Modulo { get; set; }
        public string GrupoAlmacen { get; set; }
        public string GrupoCausante { get; set; }
        public ICollection<ObrasDocumento> ObrasDocumentos { get; set; }
    }
}