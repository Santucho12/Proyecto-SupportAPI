namespace SupportApi.Repositories
{
    public interface IReclamoRepository
    {
        // Métodos CRUD para Reclamo
        Task<List<SupportApi.Models.Reclamo>> ObtenerTodosAsync();
    }
}
