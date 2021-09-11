namespace GenericApp.Common.Responses
{
    public class UsuarioAppResponse
    {
        public int IDUsuario { get; set; }

        public string Login { get; set; }

        public string Contrasena { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public int? AutorWOM { get; set; }

        public byte Estado { get; set; }

        public string FullName => $"{Nombre} {Apellido}";
    }
}