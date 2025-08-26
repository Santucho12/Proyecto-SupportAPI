namespace SupportApi.Services
{
    public interface IRespuestaService
    {
        // Métodos de negocio para Respuesta
        Task<List<Models.Respuesta>> ObtenerNoVistasAsync(Guid usuarioId);
        Task MarcarComoVistoAsync(Guid respuestaId);
        Task<Models.Respuesta> CrearRespuestaAsync(Models.Respuesta respuesta);
    }
}
