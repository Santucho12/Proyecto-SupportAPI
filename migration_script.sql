USE SupportDB;

-- Crear tabla __EFMigrationsHistory si no existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='__EFMigrationsHistory' AND xtype='U')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

-- Crear tabla Usuarios
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Usuarios' AND xtype='U')
BEGIN
    CREATE TABLE [Usuarios] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(max) NOT NULL,
        [CorreoElectronico] nvarchar(max) NOT NULL,
        [HashContrasena] nvarchar(max) NOT NULL,
        [Rol] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
    );
END;

-- Crear tabla Reclamos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Reclamos' AND xtype='U')
BEGIN
    CREATE TABLE [Reclamos] (
        [Id] uniqueidentifier NOT NULL,
        [Titulo] nvarchar(max) NOT NULL,
        [Descripcion] nvarchar(max) NOT NULL,
        [Estado] nvarchar(max) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [UsuarioId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Reclamos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reclamos_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE CASCADE
    );
END;

-- Crear tabla Respuestas
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Respuestas' AND xtype='U')
BEGIN
    CREATE TABLE [Respuestas] (
        [Id] uniqueidentifier NOT NULL,
        [Contenido] nvarchar(max) NOT NULL,
        [FechaRespuesta] datetime2 NOT NULL,
        [ReclamoId] uniqueidentifier NOT NULL,
        [UsuarioId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Respuestas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Respuestas_Reclamos_ReclamoId] FOREIGN KEY ([ReclamoId]) REFERENCES [Reclamos] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Respuestas_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
    );
END;

-- Crear índices
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reclamos_UsuarioId')
BEGIN
    CREATE INDEX [IX_Reclamos_UsuarioId] ON [Reclamos] ([UsuarioId]);
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Respuestas_ReclamoId')
BEGIN
    CREATE INDEX [IX_Respuestas_ReclamoId] ON [Respuestas] ([ReclamoId]);
END;

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Respuestas_UsuarioId')
BEGIN
    CREATE INDEX [IX_Respuestas_UsuarioId] ON [Respuestas] ([UsuarioId]);
END;

-- Insertar registro de migración
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250725235024_CreacionInicial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250725235024_CreacionInicial', '8.0.7');
END;

SELECT 'Migración completada exitosamente' AS Resultado;
