using System;
using Xunit;
using System.Threading.Tasks;
using SupportApi.Data;
// using SupportApi.Models;
using SupportApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SupportApi.Models;
using SupportApi.Services;

namespace SupportApi.Tests.Controllers
{
    public class UsuarioControllersTests
    {
        [Fact]
        public async Task DeleteUsuario_DeberiaRetornarOk()
        {
            var options = new DbContextOptionsBuilder<SupportDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbUsuarioDelete")
                .Options;
            var context = new SupportDbContext(options);
                var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<SupportApi.Models.Usuario>();
                var usuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Test",
                    CorreoElectronico = "test@mail.com",
                    Rol = "Usuario"
                };
                usuario.HashContrasena = passwordHasher.HashPassword(usuario, "123456");
            context.Usuarios.Add(usuario);
            context.SaveChanges();
            var controller = new UsuariosController(context);

            var result = await controller.DeleteUsuario(usuario.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
