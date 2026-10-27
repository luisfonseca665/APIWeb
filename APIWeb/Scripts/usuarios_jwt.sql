-- =============================================================
--  Migración de la tabla Usuarios: Google OAuth  ->  JWT propio
--  Ejecutar en la base de datos "nwind" (MySQL Workbench o CLI)
-- =============================================================
USE nwind;

-- Los usuarios que venían de Google no tienen contraseña, así que no sirven
-- para el login con JWT. Se recrea la tabla con la nueva estructura.
DROP TABLE IF EXISTS Usuarios;

CREATE TABLE Usuarios (
    Id            INT          NOT NULL AUTO_INCREMENT,
    Nombre        VARCHAR(150) NOT NULL DEFAULT '',
    Email         VARCHAR(150) NOT NULL,
    PasswordHash  VARCHAR(255) NOT NULL,
    UltimoAcceso  DATETIME(6)  NOT NULL,
    FechaRegistro DATETIME(6)  NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE KEY IX_Usuarios_Email (Email)
);
