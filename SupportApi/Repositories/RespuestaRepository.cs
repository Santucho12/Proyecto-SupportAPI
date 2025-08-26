using Microsoft.EntityFrameworkCore;
namespace SupportApi.Repositories
{
    public class RespuestaRepository : IRespuestaRepository
    {
        private readonly Data.SupportDbContext _context;
        public RespuestaRepository(Data.SupportDbContext context)
        {
            _context = context;
        }

        public async Task<List<Models.Respuesta>> ObtenerNoVistasAsync(Guid usuarioId)
        {
            return await _context.Respuestas
                .Where(r => r.Reclamo != null && r.Reclamo.UsuarioId == usuarioId && !r.Visto)
                .Include(r => r.Usuario)
                .Include(r => r.Reclamo)
                .ToListAsync();
        }

        public async Task MarcarComoVistoAsync(Guid respuestaId)
        {
            var respuesta = await _context.Respuestas.FindAsync(respuestaId);
            if (respuesta != null && !respuesta.Visto)
            {
                respuesta.Visto = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Models.Respuesta> CrearRespuestaAsync(Models.Respuesta respuesta)
        {
            // Asumimos que respuesta.Id, FechaRespuesta, etc. ya están seteados
            _context.Respuestas.Add(respuesta);
            await _context.SaveChangesAsync();
            // Cargar navegación para Reclamo y Usuario
            await _context.Entry(respuesta).Reference(r => r.Reclamo).LoadAsync();
            await _context.Entry(respuesta).Reference(r => r.Usuario).LoadAsync();
            return respuesta;
        }
    }
}
