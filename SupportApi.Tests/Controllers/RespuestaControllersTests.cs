using SupportApi.Services;
using Xunit;
using System.Threading.Tasks;
using SupportApi.Data;
using SupportApi.Models;
using SupportApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace SupportApi.Tests.Controllers
{
using System;
    public class RespuestaControllersTests
    {
        [Fact]
        public async Task DeleteRespuesta_DeberiaRetornarOk()
        {
            var options = new DbContextOptionsBuilder<SupportDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbRespuestaDelete")
                .Options;
            var context = new SupportDbContext(options);
            var respuesta = new Respuesta { Id = Guid.NewGuid(), Contenido = "Test", ReclamoId = Guid.NewGuid() };
            context.Respuestas.Add(respuesta);
            context.SaveChanges();
        var respuestaServiceMock = new Moq.Mock<IRespuestaService>();
        var controller = new RespuestasController(context, respuestaServiceMock.Object);

            var result = await controller.DeleteRespuesta(respuesta.Id);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
