using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Comprobación.Models
{
    public class Cliente
    {
        /*Id*/
        [Key]
        [Column("cliente_id")]
        public int IdCliente { get; set; }
        /*Nit*/
        [Column("nit")]
        [StringLength(20, ErrorMessage = "No mayor de 20 caracteres")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public string NitCliente { get; set; }
        /*Nombre*/
        [Column("nombre")]
        [StringLength(50, ErrorMessage = "No mayor a 50 caracteres")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public string NombreCliente { get; set; }
        /*Apellido*/
        [Column("apellido")]
        [StringLength(50, ErrorMessage = "No mayor a 50 caracteres")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public string ApellidoCliente { get; set; }
        /*dirrecion*/
        [Column("direccion")]
        [StringLength(200, ErrorMessage = "No mayor a 200 caracteres")]
        public string? DireccionCliente { get; set; }
        /*telefono*/
        [Column("telefono")]
        [StringLength(20, ErrorMessage = "No mayor de 20 caracteres")]
        public string? TelefonoCliente { get; set; }

        /*De cliente a otros*/
        public ICollection<Venta>? Ventas { get; set; }
        /*De otros a cliente*/
    }
}
