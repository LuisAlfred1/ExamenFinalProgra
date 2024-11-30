using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Comprobación.Models;

namespace Comprobación.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<DetallesVentas> Detalle_Ventas { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

    }
}
