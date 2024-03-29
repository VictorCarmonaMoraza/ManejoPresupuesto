﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace ManejoPresupuesto.Servicios
{
    //INTERFACE
    public interface IRepositorioTiposCuentas
    {
        Task Crear(TipoCuenta tipoCuenta);

        Task<bool> Existe(string nombre, int usuarioId);

        Task <IEnumerable<TipoCuenta>> Listar();

        Task <IEnumerable<TipoCuenta>> ListarPorUsuarioId(int usuarioId);

        Task Actualizar(TipoCuenta tipoCuenta);

        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);

        Task Eliminar(int id);

        Task OrdenarForma1(int usuarioId, int[] ids);
        Task OrdenarForma2(IEnumerable<TipoCuenta> listatiposCuentasOrdenados);
    }


    //REPOSITORIO
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {

        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("RutaServerSQL");
        }
        /// <summary>
        /// Crear un tipo de Cuenta
        /// </summary>
        /// <param name="tipoCuenta">model tipocuenta</param>
        /// <returns></returns>
        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);

            //Asegurarnos que la conexion esta abierta antes de ejecutar la query
            await connection.OpenAsync();

            var id = await connection.QuerySingleAsync<int>(
                        "TiposCuentas_Insertar",
                        new {nombre= tipoCuenta.Nombre, usuarioId=tipoCuenta.UsuarioId },
                        commandType:System.Data.CommandType.StoredProcedure);

                        tipoCuenta.Id = id;
        }

        /// <summary>
        /// Validacion si un usuario ya existe
        /// </summary>
        /// <param name="nombre">nombre</param>
        /// <param name="usuarioId">Id del usuario</param>
        /// <returns></returns>
        public async Task<bool> Existe(string nombre, int usuarioId)
        {

            using var connection = new SqlConnection(connectionString);

            //Asegurarnos que la conexion esta abierta antes de ejecutar la query
            await connection.OpenAsync();
            //Opcion 1            
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@$"select 1 from TiposCuentas where Nombre=@Nombre and UsuarioId =@UsuarioId;",
                new {nombre,usuarioId});

            //Opcion 2
            //var parametros  = new { Nombre = nombre, UsuarioId = usuarioId };
            //var existe = await connection.QueryFirstOrDefaultAsync<int>(
            //            @"SELECT 1 FROM TiposCuentas WHERE Nombre=@Nombre AND UsuarioId=@UsuarioId;",
            //            parametros);

            bool registroExiste = existe == 1;

            return registroExiste;
        }

        /// <summary>
        /// Listar todos los tipos de cuentas
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TipoCuenta>> Listar()
        {
            using var connection = new SqlConnection(connectionString);

            //Asegurarnos que la conexion esta abierta antes de ejecutar la query
            await connection.OpenAsync();

            var lista = await connection.QueryAsync<TipoCuenta>(@"SELECT * FROM TiposCuentas;");

            return lista;
        }


        /// <summary>
        /// Lista tipos de cuentas por usuario id
        /// </summary>
        /// <param name="usuarioId">usuario id</param>
        /// <returns></returns>
        public async Task<IEnumerable<TipoCuenta>> ListarPorUsuarioId(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            //Asegurarnos que la conexion esta abierta antes de ejecutar la query
            await connection.OpenAsync();

            var lista = await connection.QueryAsync<TipoCuenta>(@"SELECT * FROM TiposCuentas WHERE UsuarioId=@usuarioId ORDER BY Orden;",
                               new { usuarioId });

            return lista;
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);

            //Asegurarnos que la conexion esta abierta antes de ejecutar la query
            await connection.OpenAsync();

            await connection.ExecuteAsync(@"UPDATE TiposCuentas SET Nombre=@Nombre WHERE Id=@Id;",
                               new { tipoCuenta.Nombre, tipoCuenta.Id });
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT * FROM TiposCuentas WHERE Id=@id AND UsuarioId=@usuarioId;",
                               new { id, usuarioId });
        }

        public async Task Eliminar(int id)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();
            await connection.ExecuteAsync(@"DELETE FROM TiposCuentas WHERE Id=@id;",
                                              new { id });
        }

        /// <summary>
        /// Ordenacion Forma 1
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task OrdenarForma1(int usuarioId, int[] ids)
        {
            using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();

            for (int i = 0; i < ids.Length; i++)
            {
                await connection.ExecuteAsync(@"UPDATE TiposCuentas SET Orden=@orden WHERE Id=@id AND UsuarioId=@usuarioId;",
                                       new { orden = i, id = ids[i], usuarioId });
            }

            //Obtener los nombres de los elementos ordenados
            var lista = await connection.QueryAsync<TipoCuenta>(@"SELECT * FROM TiposCuentas WHERE UsuarioId=@usuarioId;");
        }

        public async Task OrdenarForma2 (IEnumerable<TipoCuenta> listatiposCuentasOrdenados)
        {
            var query = @"UPDATE TiposCuentas SET Orden=@orden WHERE Id=@id";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query,listatiposCuentasOrdenados);
        }
    }
}
