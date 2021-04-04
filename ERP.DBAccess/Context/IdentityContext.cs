using ERP.Models.DBModels.Claim;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERP.DBAccess.Context
{
    public class IdentityContext : IdentityDbContext
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ClaimDbModel> Claims { get; set; }
    }
}
