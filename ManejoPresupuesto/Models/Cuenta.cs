using ManejoPresupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    /// <summary>
    /// Representa una cuenta financiera en el sistema.
    /// </summary>
    public class Cuenta
    {
        /// <summary>
        /// Identificador único de la cuenta.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la cuenta. Es un campo obligatorio, debe tener una longitud máxima de 50 caracteres,
        /// y su primera letra debe ser mayúscula.
        /// </summary>
        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [StringLength(maximumLength: 50, ErrorMessage = "La longitud máxima del nombre es de 50 caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        /// <summary>
        /// Identificador del tipo de cuenta asociado a esta cuenta. Este campo se muestra como 'Tipo de Cuenta' en la interfaz de usuario.
        /// </summary>
        [Display(Name = "Tipo de Cuenta")]
        public int TipoCuentaId { get; set; }

        /// <summary>
        /// Balance actual de la cuenta.
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Descripción opcional de la cuenta.
        /// </summary>
        public string Descripcion { get; set; }
    }

}
