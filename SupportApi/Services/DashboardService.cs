namespace SupportApi.Services
{
    public class DashboardService : IDashboardService
    {
        public async Task<object> GetDashboardDataAsync()
        {
            await Task.CompletedTask;

            return new
            {
                TotalReclamos = 0,
                ReclamosPendientes = 0,
                ReclamosResueltos = 0,
                TotalUsuarios = 0
            };
        }

        public async Task<object> GetEstadisticasAsync()
        {
            await Task.CompletedTask;

            return new
            {
                PorEstado = new[] { new { Estado = "Pendiente", Cantidad = 0 } },
                PorPrioridad = new[] { new { Prioridad = "Media", Cantidad = 0 } }
            };
        }
    }
}
