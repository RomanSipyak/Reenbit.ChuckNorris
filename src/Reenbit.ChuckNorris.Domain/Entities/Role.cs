using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Reenbit.ChuckNorris.Domain.Entities
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
