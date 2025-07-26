using System.Collections.Generic;
using System.Threading.Tasks;
using TiendaAPI.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace TiendaAPI.Repositories
{
    public class PresupuestoRepository : IPresupuestoRepository
    {
        private readonly string _cs;
        public PresupuestoRepository(IConfiguration configuration)
        {
            _cs = configuration.GetConnectionString("ConexionSQLite")!;
        }

        public async Task<List<Presupuestos>> GetAllAsync()
        {
            var presupuestos = new List<Presupuestos>();

            using var connection = new SqliteConnection(_cs);
            await connection.OpenAsync();

            // 1. Obtener todos los presupuestos
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT IdPresupuesto, NombreDestinatario 
                FROM Presupuestos";

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var presupuesto = new Presupuestos
                {
                    IdPresupuesto = reader.GetInt32("IdPresupuesto"),
                    NombreDestinatario = reader.GetString("NombreDestinatario"),
                    Detalles = new List<PresupuestoDetalle>()
                };
                presupuestos.Add(presupuesto);
            }

            // 2. Para cada presupuesto, obtener sus detalles
            foreach (var presupuesto in presupuestos)
            {
                presupuesto.Detalles = await GetDetallesPresupuesto(connection, presupuesto.IdPresupuesto);
            }

            return presupuestos;
        }

        private async Task<List<PresupuestoDetalle>> GetDetallesPresupuesto(SqliteConnection connection, int idPresupuesto)
        {
            var detalles = new List<PresupuestoDetalle>();

            var command = connection.CreateCommand();
            command.CommandText = @"
                  SELECT 
                      pd.idProducto,
                      pd.Cantidad,
                      p.idProducto,
                      p.Descripcion,
                      p.Precio
                  FROM PresupuestosDetalle pd
                  INNER JOIN Productos p ON pd.idProducto = p.idProducto
                  WHERE pd.idPresupuesto = @idPresupuesto";

            command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var detalle = new PresupuestoDetalle
                {
                    Cantidad = reader.GetInt32("Cantidad"),
                    Producto = new Productos
                    {
                        Id = reader.GetInt32("IdProducto"),
                        Descripcion = reader.GetString("Descripcion"),
                        Precio = reader.GetInt32("Precio")
                    }
                };
                detalles.Add(detalle);
            }

            return detalles;
        }

        public async Task AddAsync(Presupuestos presupuesto)
        {
            using var connection = new SqliteConnection(_cs);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) 
                VALUES ($nombreDestinatario, $fechaCreacion)";
            command.Parameters.AddWithValue("$nombreDestinatario", presupuesto.NombreDestinatario);
            command.Parameters.AddWithValue("$fechaCreacion", presupuesto.FechaCreacion);

            await command.ExecuteNonQueryAsync();
        }
        public async Task<bool> AddProductoDetalleAsync(int idPresupuesto, int idProducto, int cantidad)
        {
            var presupuestoExistente = await VerificarPresupuestoExistenteAsync(idPresupuesto);
            if (!presupuestoExistente)
                return false;
                
            var productoExistente = await VerificarProductoExistenteAsync(idProducto);
            if (!productoExistente)
                return false;

            using var connection = new SqliteConnection(_cs);
            await connection.OpenAsync();

            // 1. Verificar si ya existe el detalle
            var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = @"
                SELECT COUNT(*) FROM PresupuestosDetalle 
                WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto";
            checkCommand.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
            checkCommand.Parameters.AddWithValue("@idProducto", idProducto);
            
            var existe = Convert.ToInt32(await checkCommand.ExecuteScalarAsync()) > 0;

            var command = connection.CreateCommand();
            
            if (existe)
            {
                // 2. Si existe, actualizar la cantidad (sumar)
                command.CommandText = @"
                    UPDATE PresupuestosDetalle 
                    SET Cantidad = Cantidad + @cantidad 
                    WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto";
            }
            else
            {
                // 3. Si no existe, insertar nuevo registro
                command.CommandText = @"
                    INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) 
                    VALUES (@idPresupuesto, @idProducto, @cantidad)";
            }
            
            command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
            command.Parameters.AddWithValue("@idProducto", idProducto);
            command.Parameters.AddWithValue("@cantidad", cantidad);
            
            await command.ExecuteNonQueryAsync();
            return true;
        }

        private async Task<bool> VerificarPresupuestoExistenteAsync(int idPresupuesto)
        {
            using var connection = new SqliteConnection(_cs);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*) FROM Presupuestos WHERE IdPresupuesto= @idPresupuesto";
            command.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
            var resultado = await command.ExecuteScalarAsync();
            /* ejecuta el comando y devuelve la primera columna de la primera fila del primer conjunto de resultados devuelto. */
            return Convert.ToInt32(resultado) > 0;

        }
        private async Task<bool> VerificarProductoExistenteAsync(int idProducto)
        {
            using var connection = new SqliteConnection(_cs);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*) FROM Productos WHERE idProducto = @idProducto";
            command.Parameters.AddWithValue("@idProducto", idProducto);
            var resultado = await command.ExecuteScalarAsync();
            /* ejecuta el comando y devuelve la primera columna de la primera fila del primer conjunto de resultados devuelto. */
            return Convert.ToInt32(resultado) > 0;
        }





    }
}
