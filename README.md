# SupportAPI finalizada

Este proyecto es una API pensada para que puedas gestionar reclamos y soporte de manera facil y directa. Los usuarios pueden registrarse, crear reclamos, recibir respuestas y ver notificaciones en tiempo real. Todo esta armado para que la comunicacion entre clientes y soporte sea lo mas agil posible.

---

## Documentación Interactiva con Swagger

La API expone su documentación y permite probar los endpoints usando Swagger UI. Accedé a:

- `http://localhost:8080/swagger/index.html` (si usás Docker)
- `http://localhost:5138/swagger` o `https://localhost:7007/swagger` (si lo corres local)

Desde ahí podés ver los modelos, probar los endpoints y ver ejemplos de request/response.

---

## Estructura del Proyecto

```
SupportApi/
├── Controllers/         # Controladores de la API REST (Auth, Usuarios, Reclamos, Respuestas, Notificaciones, Admin)
├── Models/              # Modelos de datos: Usuario, Reclamo, Respuesta, Notificacion
├── DTOs/                # Objetos para transferir datos y validar
├── Data/                # DbContext y configuracion de EF Core
├── Migrations/          # Migraciones de la base de datos
├── Services/            # Servicios y logica de negocio (TokenService, SignalR, reclamos)
├── Validators/          # Validaciones automaticas con FluentValidation
├── Mappings/            # Perfiles de AutoMapper para transformar modelos y DTOs
├── Hubs/                # SignalR para notificaciones en tiempo real
├── Properties/          # launchSettings.json y configuracion de inicio
SupportApi.Tests/        # Pruebas unitarias con xUnit
```

---

## Requisitos Previos

- .NET 8 SDK
- Docker (opcional, para levantar los servicios con docker-compose)
- SQL Server (local o en Docker)

---

## Como Compilar y Ejecutar

1. Clona el repo en tu maquina.
2. Configura la cadena de conexion en el archivo `appsettings.json`.
3. Si usas Docker, levanta los servicios con:
	```
	docker-compose up --build
	```
4. Si lo corres local, primero aplica las migraciones:
	```
	dotnet ef database update
	```
5. Inicia la API:
	```
	dotnet run
	```
6. Accede a Swagger UI en `http://localhost:8080/` para probar los endpoints.

---

## Endpoints y Que Hace Cada Uno

### Autenticacion y Usuarios
- `POST /api/auth/register`: Registra un usuario nuevo. Necesitas nombre, correo, contrasena y rol.
- `POST /api/auth/login`: Loguea al usuario y te devuelve el token JWT.
- `GET /api/usuarios`: Lista todos los usuarios (solo para roles autorizados).
- `GET /api/usuarios/{id}`: Trae los datos de un usuario por su ID.
- `GET /api/usuarios/actualizar-rol-temporal`: Permite cambiar el rol de un usuario por email (solo admins, temporal).

### Reclamos
- `GET /api/reclamos`: Lista los reclamos. Si sos usuario ves los tuyos, si sos soporte/admin ves todos.
- `POST /api/reclamos`: Crea un reclamo nuevo (necesita token).
- `PUT /api/reclamos/{id}`: Actualiza un reclamo existente.
- `DELETE /api/reclamos/{id}`: Elimina un reclamo.

### Respuestas
- `GET /api/respuestas/notificaciones?usuarioId=...`: Trae las respuestas no vistas (notificaciones) para el usuario.
- `PATCH /api/respuestas/{id}/visto`: Marca una respuesta como vista.
- `POST /api/respuestas`: Crea una respuesta a un reclamo.

### Notificaciones
- `GET /api/notificaciones/soporte`: Lista las notificaciones relevantes para el equipo de soporte.

---

## Como Correr los Tests Unitarios

1. Abri una terminal en la carpeta del proyecto.
2. Ejecuta:
	```
	dotnet test
	```
3. Vas a ver el resultado de todos los tests definidos en SupportApi.Tests.

---

## Consejos y Buenas Practicas

- No subas archivos sensibles (appsettings.*.json) al repo.
- Usa HTTPS en produccion.
- Cambia las claves y JWT en cada ambiente.
- Limita permisos de los roles y revisa los claims en cada endpoint.
- Mete logging y monitoreo (Application Insights, ELK, lo que uses).
- Hace backups de la base cada tanto.

---

## Contacto
- GitHub: Santucho12 (https://github.com/Santucho12)
- Email: santysegal@gmail.com
