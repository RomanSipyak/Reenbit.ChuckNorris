using Microsoft.AspNetCore.Identity;

namespace Reenbit.ChuckNorris.Domain.Entities
{
    public class UserRole : IdentityUserRole<int>
    {
        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}
