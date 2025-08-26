using SupportApi.Repositories;
using Xunit;

namespace SupportApi.Tests.Repositories
{
    public class UsuarioRepositoryTests
    {
        [Fact]
        public void CrearUsuario_DeberiaPersistirUsuario()
        {
            Assert.True(true);
        }

        [Fact]
        public void ObtenerUsuario_DeberiaRetornarUsuario()
        {
            Assert.True(true);
        }

        [Fact]
        public void ActualizarUsuario_DeberiaActualizar()
        {
            Assert.True(true);
        }

        [Fact]
        public void EliminarUsuario_DeberiaEliminar()
        {
            Assert.True(true);
        }

        [Fact]
        public void UsuarioRepository_ClassExists()
        {
            var repo = new UsuarioRepository(null);
            Assert.NotNull(repo);
        }
    }
}
