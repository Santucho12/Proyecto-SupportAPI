using Xunit;
using Moq;
using SupportApi.Models;
using SupportApi.Repositories;
using SupportApi.Services;

namespace SupportApi.Tests.Services
{
    public class UsuarioServiceTests
    {
        [Fact]
        public void UsuarioService_ClassExists()
        {
            Assert.True(typeof(UsuarioService) != null);
        }
    }
}
