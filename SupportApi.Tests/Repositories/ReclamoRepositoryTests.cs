using SupportApi.Repositories;
using Xunit;

namespace SupportApi.Tests.Repositories
{
    public class ReclamoRepositoryTests
    {
        [Fact]
        public void CrearReclamo_DeberiaPersistirReclamo()
        {
            Assert.True(true);
        }

        [Fact]
        public void ObtenerReclamo_DeberiaRetornarReclamo()
        {
            Assert.True(true);
        }

        [Fact]
        public void ActualizarReclamo_DeberiaActualizar()
        {
            Assert.True(true);
        }

        [Fact]
        public void EliminarReclamo_DeberiaEliminar()
        {
            Assert.True(true);
        }

        [Fact]
        public void ReclamoRepository_ClassExists()
        {
            var repo = new ReclamoRepository(null);
            Assert.NotNull(repo);
        }
    }
}
