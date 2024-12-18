
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Entities
{
    public class ApplitationUser : IdentityUser
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }


        
    }
}
