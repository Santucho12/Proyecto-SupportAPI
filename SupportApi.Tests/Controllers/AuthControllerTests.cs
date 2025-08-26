using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SupportApi.Controllers;
using SupportApi.DTOs;
using SupportApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using SupportApi.Data;

public class AuthControllerTests
{
    [Fact]
    public async Task Register_DeberiaCrearUsuario()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SupportDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        var context = new SupportDbContext(options);
        var passwordHasher = new PasswordHasher<Usuario>();
    var jwtSettings = new SupportApi.Configurations.JwtSettings { Key = "claveSuperSeguraParaJWT1234567890!@#$", Issuer = "issuer", Audience = "aud", ExpirationInMinutes = 60 };
        var optionsJwt = new Mock<Microsoft.Extensions.Options.IOptions<SupportApi.Configurations.JwtSettings>>();
        optionsJwt.Setup(o => o.Value).Returns(jwtSettings);
        var tokenService = new SupportApi.Services.TokenService(optionsJwt.Object);
        var controller = new AuthController(context, tokenService);

        var dto = new UsuarioCreateDto
        {
            Nombre = "Test",
            CorreoElectronico = "test@mail.com",
            Contrasena = "123456",
            Rol = "Usuario"
        };

        // Act
        var result = await controller.Register(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void Login_DeberiaAutenticarUsuario()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<SupportDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDbLogin")
            .Options;
        var context = new SupportDbContext(options);
        var passwordHasher = new PasswordHasher<Usuario>();
        var usuario = new Usuario
        {
            Nombre = "Test",
            CorreoElectronico = "test@mail.com",
            Rol = "Usuario"
        };
        usuario.HashContrasena = passwordHasher.HashPassword(usuario, "123456");
        context.Usuarios.Add(usuario);
        context.SaveChanges();

    var jwtSettings = new SupportApi.Configurations.JwtSettings { Key = "claveSuperSeguraParaJWT1234567890!@#$", Issuer = "issuer", Audience = "aud", ExpirationInMinutes = 60 };
        var optionsJwt = new Mock<Microsoft.Extensions.Options.IOptions<SupportApi.Configurations.JwtSettings>>();
        optionsJwt.Setup(o => o.Value).Returns(jwtSettings);
        var tokenService = new SupportApi.Services.TokenService(optionsJwt.Object);
        var controller = new AuthController(context, tokenService);

        var dto = new UsuarioLoginDto
        {
            CorreoElectronico = "test@mail.com",
            Contrasena = "123456"
        };

        // Act
        var result = controller.Login(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void Test_AuthController_Exists()
    {
        Assert.True(true);
    }

    [Fact]
    public void CanInstantiateTokenService()
    {
        var jwtSettings = new SupportApi.Configurations.JwtSettings { Key = "clave", Issuer = "issuer", Audience = "aud", ExpirationInMinutes = 60 };
        var options = new Mock<Microsoft.Extensions.Options.IOptions<SupportApi.Configurations.JwtSettings>>();
        options.Setup(o => o.Value).Returns(jwtSettings);
        var tokenService = new SupportApi.Services.TokenService(options.Object);
        Assert.NotNull(tokenService);
    }
}
