using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task Eliminar(int id);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
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

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var cuentas = await connection.QueryAsync<Cuenta>(@"select Cuentas.Id,Cuentas.Nombre,Balance,tc.Nombre as TipoCuenta from Cuentas
                                            inner join TiposCuentas tc 
                                            on tc.Id = Cuentas.TipoCuentaId
                                            where tc.UsuarioId =@UsuarioId
                                            order by tc.Orden", new {usuarioId});
            return cuentas;
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"Select a.Id,a.Nombre,a.Balance,a.Descripcion,a.TipoCuentaId
                                                                        From Cuentas a
                                                                        inner join  TiposCuentas b
                                                                        on b.Id = a.TipoCuentaId
                                                                        where b.UsuarioId = @UsuarioId and a.Id = @Id", new {id, usuarioId});
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"update Cuentas set Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion,
                                    TipoCuentaId = @TipoCuentaId
                                    where Id = @Id", cuenta);
        }

        public async Task Eliminar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"delete from Cuentas where Id = @Id", new {id});
        }
    }
}
