using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERP.DBAccess.Context
{
    public class IdentityContext : IdentityDbContext
    {
        public IdentityContext(DbContextOptions options):base(options)
        {
            
        }
    }
}
