using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);
        Task Crear(Categoria categoria);
        Task<Categoria> ObtenerCategoriaPorId(int id, int usuarioId);
        Task<IEnumerable<Categoria>> ObtenerCategoriasPorUsuario(int usuarioId);
    }
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string _connectionString;
        public RepositorioCategorias(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("RutaServerSQL");
        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(_connectionString);
            var id = await connection.QuerySingleAsync<int>(@"insert into Categorias (nombre, TipoOperacionId, UsuarioId)
                                                            values (@Nombre, @TipoOperacionId, @UsuarioId);
                                                            select SCOPE_IDENTITY();", categoria);

            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> ObtenerCategoriasPorUsuario(int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Categoria>(@"select * from Categorias where UsuarioId = @usuarioId", new { usuarioId });
        }

        public async Task<Categoria> ObtenerCategoriaPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(@"select * from Categorias where Id = @id and UsuarioId=@usuarioId", new { id, usuarioId });
        }

        public async Task Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"update Categorias set Nombre = @Nombre, TipoOperacionId = @TipoOperacionId where Id = @Id", categoria);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"delete from Categorias where Id = @id", new { id });
        }
    }
}
