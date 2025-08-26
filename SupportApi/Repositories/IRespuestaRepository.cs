namespace SupportApi.Repositories
{
    public interface IRespuestaRepository
    {
        // Métodos CRUD para Respuesta
        Task<List<Models.Respuesta>> ObtenerNoVistasAsync(Guid usuarioId);
        Task MarcarComoVistoAsync(Guid respuestaId);
        Task<Models.Respuesta> CrearRespuestaAsync(Models.Respuesta respuesta);
    }
}
