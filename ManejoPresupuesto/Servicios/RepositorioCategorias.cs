﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCategorias
    {
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> ObtenerCategoriasPorUsuario(int usuarioId);
    }
    public class RepositorioCategorias: IRepositorioCategorias
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
    }
}
