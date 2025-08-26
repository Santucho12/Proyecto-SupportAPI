using System;
using Moq;
using FluentValidation;
using Xunit;
using System.Threading.Tasks;
using SupportApi.Data;
using SupportApi.Models;
using SupportApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace SupportApi.Tests.Controllers
{
    public class ReclamosControllersTests
    {

        [Fact]
        public async Task ActualizarReclamo_DeberiaRetornarOk()
        {
            var options = new DbContextOptionsBuilder<SupportDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbReclamoUpdate")
                .Options;
            var context = new SupportDbContext(options);
    var validatorMock = new Moq.Mock<FluentValidation.IValidator<SupportApi.DTOs.ReclamoDto>>();
    validatorMock.Setup(v => v.Validate(It.IsAny<SupportApi.DTOs.ReclamoDto>())).Returns(new FluentValidation.Results.ValidationResult());
    var controller = new ReclamosController(context, validatorMock.Object);
        var reclamo = new Reclamo { Id = Guid.NewGuid(), Titulo = "Prueba", Descripcion = "Desc", Estado = SupportApi.Models.EstadoReclamo.Nuevo };
        context.Reclamos.Add(reclamo);
        context.SaveChanges();

        reclamo.Titulo = "Actualizado";
        var result = await controller.Actualizar(reclamo.Id, reclamo);
        Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task EliminarReclamo_DeberiaRetornarOk()
        {
            var options = new DbContextOptionsBuilder<SupportDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbReclamoDelete")
                .Options;
            var context = new SupportDbContext(options);
            var validatorMock = new Moq.Mock<FluentValidation.IValidator<SupportApi.DTOs.ReclamoDto>>();
            validatorMock.Setup(v => v.Validate(It.IsAny<SupportApi.DTOs.ReclamoDto>())).Returns(new FluentValidation.Results.ValidationResult());
            var controller = new ReclamosController(context, validatorMock.Object);
            var reclamo = new Reclamo { Id = Guid.NewGuid(), Titulo = "Prueba", Descripcion = "Desc", Estado = SupportApi.Models.EstadoReclamo.Nuevo };
            context.Reclamos.Add(reclamo);
            context.SaveChanges();

            var result = await controller.Eliminar(reclamo.Id);
            Assert.IsType<NoContentResult>(result);
        }
    }
}
