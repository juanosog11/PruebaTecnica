IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Users] (
    [UsuarioId] int NOT NULL IDENTITY,
    [Usuario] varchar(50) NOT NULL,
    [Email] varchar(50) NOT NULL,
    [Numero] varchar(50) NOT NULL,
    [Clave] varchar(100) NOT NULL,
    CONSTRAINT [PK__Users__2B3DE7B839539EFE] PRIMARY KEY ([UsuarioId])
);
GO

CREATE TABLE [Follows] (
    [FollowerId] int NOT NULL,
    [FolloweeId] int NOT NULL,
    CONSTRAINT [PK__Follows__67A1FC7C82A23FCE] PRIMARY KEY ([FollowerId], [FolloweeId]),
    CONSTRAINT [FK_Follows_FolloweeId_Users_UsuarioId] FOREIGN KEY ([FolloweeId]) REFERENCES [Users] ([UsuarioId]),
    CONSTRAINT [FK_Follows_FollowerId_Users_UsuarioId] FOREIGN KEY ([FollowerId]) REFERENCES [Users] ([UsuarioId])
);
GO

CREATE TABLE [Posts] (
    [PostId] int NOT NULL IDENTITY,
    [Content] nvarchar(280) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [UsuarioId] int NOT NULL,
    [UserUsuarioId] int NULL,
    CONSTRAINT [PK__Posts__AA126018436802A7] PRIMARY KEY ([PostId]),
    CONSTRAINT [FK_Post_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [Users] ([UsuarioId]),
    CONSTRAINT [FK_Posts_Users_UserUsuarioId] FOREIGN KEY ([UserUsuarioId]) REFERENCES [Users] ([UsuarioId])
);
GO

CREATE INDEX [IX_Follows_FolloweeId] ON [Follows] ([FolloweeId]);
GO

CREATE INDEX [IX_Posts_UserUsuarioId] ON [Posts] ([UserUsuarioId]);
GO

CREATE INDEX [IX_Posts_UsuarioId] ON [Posts] ([UsuarioId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

CREATE UNIQUE INDEX [IX_Users_Numero] ON [Users] ([Numero]);
GO

CREATE UNIQUE INDEX [IX_Users_Usuario] ON [Users] ([Usuario]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240629014133_InitialCreate', N'8.0.6');
GO

COMMIT;
GO

