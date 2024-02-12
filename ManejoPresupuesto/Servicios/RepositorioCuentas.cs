using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCuentas
    {

        Task Crear(Cuenta cuenta);
    }
    public class RepositorioCuentas: IRepositorioCuentas
    {
        private readonly string connectionString;
        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("RutaServerSQL");
        }

        /// <summary>
        /// Crea una nueva cuenta en la base de datos.
        /// </summary>
        /// <param name="cuenta">La instancia de <see cref="Cuenta"/> que contiene la información de la nueva cuenta a crear. 
        /// Debe incluir los siguientes campos: Nombre, TipoCuentaId, Descripción y Balance.</param>
        /// <remarks>
        /// Este método inserta una nueva cuenta en la base de datos utilizando los datos proporcionados en el parámetro <paramref name="cuenta"/>.
        /// Después de la inserción, recupera el ID generado automáticamente para la nueva cuenta (usando SCOPE_IDENTITY()) y actualiza la propiedad Id del objeto <paramref name="cuenta"/> con este valor.
        /// </remarks>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// var cuenta = new Cuenta { Nombre = "Cuenta Nueva", TipoCuentaId = 1, Descripcion = "Descripción de la cuenta", Balance = 100.00 };
        /// await Crear(cuenta);
        /// Console.WriteLine(cuenta.Id); // Imprime el ID generado para la nueva cuenta
        /// </code>
        /// </example>
        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"insert into cuentas (Nombre, TipoCuentaId,Descripcion, Balance)
                            values (@Nombre,@TipoCuentaId,@Descripcion,@Balance);
                            select SCOPE_IDENTITY();", cuenta);
            cuenta.Id = id;
        }
    }
}
