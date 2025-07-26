using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaAPI.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace TiendaAPI.Repositories
{
    public class ProductosRepository : IProductosRepository
    {
        private readonly string _cs;

        public ProductosRepository(IConfiguration configuration)
        {
            _cs = configuration.GetConnectionString("ConexionSQLite")!;
        }

        public async Task<List<Productos>> GetAllAsync()
        {
            var productos = new List<Productos>();

            using var connection = new SqliteConnection(_cs);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT IdProducto, Descripcion, Precio 
                FROM Productos";

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var producto = new Productos
                {
                    Id = reader.GetInt32("IdProducto"),
                    Descripcion = reader.GetString("Descripcion"),
                    Precio = reader.GetDecimal("Precio")
                };
                productos.Add(producto);
            }

            return productos;
        }

        public async Task AddAsync(Productos prod)
        {
            using var connection = new SqliteConnection(_cs);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Productos (Descripcion, Precio) 
                VALUES ($descripcion, $precio)";
            command.Parameters.AddWithValue("$descripcion", prod.Descripcion);
            command.Parameters.AddWithValue("$precio", prod.Precio);

            await command.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(int idProd, Productos producto)
        {
            using var connection = new SqliteConnection(_cs);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Productos 
                SET Descripcion = $descripcion, Precio = $precio 
                WHERE IdProducto = $idProd";
            command.Parameters.AddWithValue("$descripcion", producto.Descripcion);
            command.Parameters.AddWithValue("$precio", producto.Precio);
            command.Parameters.AddWithValue("$idProd", idProd);

            await command.ExecuteNonQueryAsync();
        }
        public async Task<Productos> GetByIdAsync(int id)
        {
            using var connection = new SqliteConnection(_cs);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM Productos WHERE IdProducto = $id";
            command.Parameters.AddWithValue("$id", id);
            using var reader = await command.ExecuteReaderAsync();
            var producto = new Productos();
            while (await reader.ReadAsync())
            {
                producto.Id = reader.GetInt32("IdProducto");
                producto.Descripcion = reader.GetString("Descripcion");

                producto.Precio = reader.GetDecimal("Precio");
            }
            ;
            return producto;
        }


        public async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(_cs);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM Productos WHERE IdProducto = $id";
            command.Parameters.AddWithValue("$id", id);
            await command.ExecuteNonQueryAsync();
        }
    }
}