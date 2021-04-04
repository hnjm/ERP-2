using Microsoft.EntityFrameworkCore;

namespace ERP.DBAccess.Context
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions<AppContext> options) : base(options)
        {

        }
    }
}
