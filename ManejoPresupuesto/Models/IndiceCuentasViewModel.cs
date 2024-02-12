namespace ManejoPresupuesto.Models
{
    public class IndiceCuentasViewModel
    {
        /// <summary>
        /// Obtiene o establece el tipo de cuenta.
        /// </summary>
        public string TipoCuenta { get; set; }

        /// <summary>
        /// Obtiene o establece las cuentas del usuario.
        /// </summary>
        public IEnumerable<Cuenta> Cuentas { get; set; }

        /// <summary>
        /// Obtiene el balance total de todas las cuentas.
        /// </summary>
        public decimal TotalBalance => Cuentas.Sum(c => c.Balance);
    }
}
