using System;

namespace GenericApp.Common.Responses
{
    public class ObrasDocumentoResponse
    {
        public int NROREGISTRO { get; set; }
        public int NROOBRA { get; set; }
        public string OBSERVACION { get; set; }
        public string LINK { get; set; }
        public DateTime FECHA { get; set; }
        public string MODULO { get; set; }
        public string NroLote { get; set; }
        public string Sector { get; set; }
        public string Estante { get; set; }
        public string GeneradoPor { get; set; }
        public string ImageFullPath => string.IsNullOrEmpty(LINK)
          ? $"http://keypress.serveftp.net:88/RowingAppApi/images/Obras/noimage.png"
       : $"http://keypress.serveftp.net:88/RowingAppApi{LINK.Substring(1)}";
    }
}