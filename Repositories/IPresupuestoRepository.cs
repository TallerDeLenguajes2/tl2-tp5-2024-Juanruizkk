using TiendaAPI.Models;

namespace TiendaAPI.Repositories
{
    public interface IPresupuestoRepository
    {
        Task<List<Presupuestos>> GetAllAsync();
        Task<Presupuestos> GetByIdAsync(int id);
        Task AddAsync(Presupuestos presupuesto);
        Task UpdateAsync(int idPresupuesto, Presupuestos presupuesto);
        Task DeleteAsync(int id);
    }
}

/* Por que uso TASK?
Task es un tipo que representa una operación asíncrona en C#. Es como una "promesa" de que el resultado estará disponible en el futuro.

Método SÍNCRONO (sin Task):
public List<Presupuestos> GetAll()
{
    // El hilo se BLOQUEA aquí hasta completar la operación
    var result = database.Query("SELECT * FROM Presupuestos");
    return result; // Devuelve el resultado inmediatamente
}

Analogía simple:
Sin Task (síncrono): Como hacer una llamada telefónica y esperar en línea hasta que contesten
Con Task (asíncrono): Como enviar un WhatsApp y seguir haciendo otras cosas hasta que respondan

 */