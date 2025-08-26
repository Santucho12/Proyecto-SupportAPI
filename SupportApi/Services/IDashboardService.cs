namespace SupportApi.Services
{
    public interface IDashboardService
    {
        Task<object> GetDashboardDataAsync();
        Task<object> GetEstadisticasAsync();
    }
}
