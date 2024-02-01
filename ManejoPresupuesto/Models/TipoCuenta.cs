using ManejoPresupuesto.Properties;
using ManejoPresupuesto.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models
{
    //public class TipoCuenta:IValidatableObject
    public class TipoCuenta
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [PrimeraLetraMayuscula]
        //[StringLength(maximumLength:50,MinimumLength =3,ErrorMessage ="La longitud del campo {0} debe estar entre {2} y {1}")]
        [Display(Name ="Nombre del tipo cuenta")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }

        //Validacion de Modelo
        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{

        //    if (Nombre !=null && Nombre.Length>0)
        //    {
        //        var primeraLetra = Nombre[0].ToString();

        //        if (primeraLetra !=primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra debe ser mayuscula", new[] { nameof(Nombre) });
        //        }
        //    }


        //    //// Lista para almacenar los resultados de la validación
        //    //List<ValidationResult> results = new List<ValidationResult>();

        //    //// Validación 1: Asegurarse de que el nombre no esté vacío
        //    //if (string.IsNullOrWhiteSpace(Nombre))
        //    //{
        //    //    results.Add(new ValidationResult("El nombre del producto no puede estar vacío.", new[] { "Nombre" }));
        //    //}

        //    //// Validación 2: El precio debe ser mayor que 0
        //    //if (Precio <= 0)
        //    //{
        //    //    results.Add(new ValidationResult("El precio del producto debe ser mayor que 0.", new[] { "Precio" }));
        //    //}

        //    //// Validación 3: La fecha de expiración no debe ser en el pasado
        //    //if (FechaDeExpiracion <= DateTime.Now)
        //    //{
        //    //    results.Add(new ValidationResult("La fecha de expiración debe ser una fecha futura.", new[] { "FechaDeExpiracion" }));
        //    //}

        //    //// Devuelve la lista de resultados de la validación
        //    //return results;
        //}
    }
}
