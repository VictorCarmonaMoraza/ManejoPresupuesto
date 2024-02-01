using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    //INTERFACE
    public interface IRepositorioTiposCuentas
    {
        void Crear(TipoCuenta tipoCuenta);
    }


    //REPOSITORIO
    public class RepositorioTiposCuentas: IRepositorioTiposCuentas
    {

        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("RutaServerSQL");
        }

        public void Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            //Hacer un query que solo trae un resultado
            var id = connection.QuerySingle<int>($@"insert into TiposCuentas (Nombre,UsuarioId,Orden) Values(@Nombre,@UsuarioId,0);
                                SELECT SCOPE_IDENTITY();",tipoCuenta);
            tipoCuenta.Id = id;
        }
    }
}
