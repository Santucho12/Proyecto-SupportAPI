using Xunit;
using Moq;
using SupportApi.Repositories;
using SupportApi.Services;

namespace SupportApi.Tests.Services
{
public class ReclamoServiceTests
{
    [Fact]
    public void ReclamoService_ClassExists()
    {
        Assert.True(typeof(ReclamoService) != null);
    }
}}
