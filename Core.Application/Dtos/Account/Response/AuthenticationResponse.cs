namespace Core.Application.Dtos.Account.Response
{
    public class AuthenticationResponse
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }


        public bool IsValified { get; set; }

        public bool HasError { get; set; }

        public string Error { get; set; }





    }
}
