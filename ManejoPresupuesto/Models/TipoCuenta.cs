using ManejoPresupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
<<<<<<< HEAD
        //[StringLength(maximumLength:50,MinimumLength =3,ErrorMessage ="La longitud del campo {0} debe estar entre {2} y {1}")]
        [PrimeraLetraMayuscula]
=======
        [StringLength(maximumLength:50,MinimumLength =3,ErrorMessage ="La longitud del campo {0} debe estar entre {2} y {1}")]
        [Display(Name ="Nombre del tipo cuenta")]
>>>>>>> ac82a545c506becf278405083b1586bf9e64de6b
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }

        /*Prueba de otra validaciones por defecto*/
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [EmailAddress(ErrorMessage ="El campo debe ser un correo electronico válido")]
        public string Email { get; set; }
        [Range(minimum:19, maximum:130, ErrorMessage ="El valor debe estar entre {1} y {2}")]
        public int Edad { get; set; }
        [Url(ErrorMessage ="El campo debe ser una URL valida")]
        public string Url { get; set; }
        [CreditCard(ErrorMessage ="La tarjeta de credito no es valida")]
        [Display(Name ="Tarjeta de Credito")]
        public string TarjetaCredito { get; set; }
    }
}
