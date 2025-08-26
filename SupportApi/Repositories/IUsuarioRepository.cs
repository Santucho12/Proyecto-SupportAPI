namespace SupportApi.Repositories
{
    public interface IUsuarioRepository
    {
        // Métodos CRUD para Usuario
        Task<int> ContarAsync();
    }
}
