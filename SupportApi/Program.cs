using Microsoft.Extensions.FileProviders;
using SupportApi.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SupportApi.Configurations;
using SupportApi.Data;
using SupportApi.Services;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using SupportApi.Hubs;


var builder = WebApplication.CreateBuilder(args);

// Forzar Kestrel a escuchar solo en HTTP puerto 8080 (evita problemas de binding en Docker)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

// Agregar soporte para encabezados reenviados (necesario para SignalR detrás de proxy/Docker)
builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Repositorios
builder.Services.AddScoped<SupportApi.Repositories.IUsuarioRepository, SupportApi.Repositories.UsuarioRepository>();
builder.Services.AddScoped<SupportApi.Repositories.IReclamoRepository, SupportApi.Repositories.ReclamoRepository>();
builder.Services.AddScoped<SupportApi.Repositories.IRespuestaRepository, SupportApi.Repositories.RespuestaRepository>();

// Servicios
builder.Services.AddScoped<SupportApi.Services.IRespuestaService, SupportApi.Services.RespuestaService>();
builder.Services.AddScoped<SupportApi.Services.IDashboardService, SupportApi.Services.DashboardService>();
// CORS: permitir peticiones desde el frontend
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5178", "http://127.0.0.1:8080", "http://localhost:8080", "http://localhost:8081")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddTransient<FluentValidation.IValidator<SupportApi.DTOs.ReclamoDto>, SupportApi.Validators.ReclamoDtoValidator>();

// 1. Configuración del DbContext
builder.Services.AddDbContext<SupportDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null)
    ));

// 2. Configuración de controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 3. Configuración de JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
    if (jwtSettings == null)
    {
        throw new InvalidOperationException("JwtSettings configuration is missing or invalid");
    }
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SupportAndAdmin", policy => policy.RequireRole("Soporte", "Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("Usuario"));
});

// 4. Swagger con JWT integrado
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Support API",
        Version = "v1",
        Description = "API RESTful para la gestión de usuarios, reclamos y respuestas en un sistema de soporte técnico.",
        Contact = new OpenApiContact
        {
            Name = "Santiago Segal",
            Url = new Uri("https://github.com/Santucho12"),
            Email = "tucorreo@ejemplo.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // JWT auth config para Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT usando el esquema Bearer. Ejemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Comentarios XML
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Mover la línea de AddAutoMapper justo después de la configuración de servicios
builder.Services.AddAutoMapper(typeof(ReclamoProfile), typeof(UsuarioProfile), typeof(RespuestaProfile));

// SignalR y CustomUserIdProvider
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();



var app = builder.Build();


// Migraciones automáticas al iniciar (comentado temporalmente)
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<SupportDbContext>();
//     db.Database.Migrate();
// }

// Usar encabezados reenviados antes de routing (importante para SignalR en Docker)
app.UseForwardedHeaders();

// Exponer archivos adjuntos como recursos estáticos en /Archivos
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Archivos")),
    RequestPath = "/Archivos"
});

// Swagger siempre habilitado (para pruebas locales)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Support API V1");
});

// Middleware de routing y CORS (el orden es importante)
app.UseRouting();
app.UseCors(myAllowSpecificOrigins);
// Log para verificar si la política de CORS se aplica
app.Use(async (context, next) =>
{
    Console.WriteLine($"[CORS] Origin: {context.Request.Headers["Origin"]}");
    await next();
});

// Middleware personalizado para loguear y mostrar detalles de errores 403
app.UseAuthentication();
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 403)
    {
        var user = context.User;
        var roles = user?.Claims?.Where(c => c.Type.Contains("role", StringComparison.OrdinalIgnoreCase)).Select(c => c.Value).ToList();
        var msg = $"Acceso denegado. Roles detectados: [{string.Join(", ", roles ?? new List<string>())}]";
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync($"{{\"error\":\"{msg}\"}}\n");
    }
});
app.UseAuthorization();


app.MapControllers();
// Mapear el hub de SignalR
app.MapHub<SoporteHub>("/hub/soporte");


// Migración automática al iniciar (solo para Docker/desarrollo)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SupportDbContext>();
    db.Database.Migrate();
}

app.Run();

