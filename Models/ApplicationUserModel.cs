using Microsoft.AspNetCore.Identity;
using System;

namespace Comprobación.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            // Generar un GUID único cuando se crea una nueva instancia de ApplicationUser.
            Id = Guid.NewGuid().ToString();
        }

        public string Nombre { get; set; }
        public string Rol { get; set; }

        // Sobrescribir la propiedad Id heredada de IdentityUser
        public override string? Id { get; set; }
    }
}
