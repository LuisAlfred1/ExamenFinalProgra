using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Comprobación.Models
{
    public class Empleado
    {
        /*Id*/
        [Key]
        [Column("empleado_id")]
        public int IdEmpleado { get; set; }
        /*Nombre Empleado*/
        [Column("nombre")]
        [StringLength(50, ErrorMessage = "No mayor a 50 caracteres")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public string NombreEmpleado { get; set; }
        /*Apellido*/
        [Column("apellido")]
        [StringLength(50, ErrorMessage = "No mayor a 50 caracteres")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public string ApellidoEmpleado { get; set; }
        /*DPI*/
        [Column("dpi")]
        [StringLength(20, ErrorMessage = "No puede ser mayor a 20 caracteres")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public string DpiEmpleado { get; set; }
        /*Cargo*/
        [Column("cargo")]
        [StringLength(50, ErrorMessage = "No mayor a 50 caracteres")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public string CargoEmpleado { get; set; }
        /*Relaciones*/
        /*Id del usuario*/
        [Column("usuario_id")]
        [ForeignKey("ApplicationUser")]
        [Required]
        public string UsuarioId { get; set; }

        //De empleado a otros
        public ICollection<Venta>? Ventas { get; set; }

        //Variables de las relaciones de otros a empleado
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
