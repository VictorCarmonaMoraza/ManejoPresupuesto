using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    //INTERFACE
    public interface IRepositorioTiposCuentas
    {
        Task Crear(TipoCuenta tipoCuenta);
    }


    //REPOSITORIO
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {

        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("RutaServerSQL");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);

            //Asegurarnos que la conexion esta abierta antes de ejecutar la query
            await connection.OpenAsync();

            //Hacer un query que solo trae un resultado
            //var id =await connection.QuerySingleAsync<int>($@"insert into TiposCuentas (Nombre,UsuarioId,Orden) Values(@Nombre,@UsuarioId,0);
            //                    SELECT SCOPE_IDENTITY();",tipoCuenta);

            var id = await connection.QuerySingleAsync<int>(
                        @"INSERT INTO TiposCuentas (Nombre, UsuarioId, Orden) VALUES (@Nombre, @UsuarioId, 0);
                        SELECT SCOPE_IDENTITY();",
                        new { tipoCuenta.Nombre, tipoCuenta.UsuarioId });

                        tipoCuenta.Id = id;
        }
    }
}
