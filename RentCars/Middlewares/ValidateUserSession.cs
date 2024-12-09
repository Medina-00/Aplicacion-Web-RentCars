
using Core.Application.Helpers;
using Core.Application.Dtos.Account.Response;

namespace RentCars.Middlewares
{
    public class ValidateUserSession
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ValidateUserSession(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public bool HasUser()
        {
            AuthenticationResponse authentication = httpContextAccessor.HttpContext.Session.Get<AuthenticationResponse>("user");

            if (authentication == null)
            {
                return false;
            }
            return true;
        }

    }
}
