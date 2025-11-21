-- ====================================================================================================================================
-- Script de Creación de Base de Datos
-- Sistema de Balanzas
-- Autor: Juan Armando Joyo Taype
-- Fecha: 2025/11/20
-- Versión: 1.1.0
-- Descripción: Script para la creación de la base de datos REPRECIPE, tablas e índices.
-- ====================================================================================================================================

-- ====================================================================================================================================
-- Database: Database creation
-- ====================================================================================================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'REPRECIPE')
BEGIN
	CREATE DATABASE REPRECIPE;
	PRINT 'Base de datos creada exitosamente';
END
ELSE
BEGIN
	PRINT 'Base de datos ya existe'
END
GO

USE REPRECIPE;
GO

-- ====================================================================================================================================
-- Tabla: Usuario
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuario')
BEGIN
	CREATE TABLE Usuario (
		UsuarioId INT PRIMARY KEY IDENTITY(1001, 1),
		Nombre VARCHAR(20) NOT NULL,
		ClaveHash VARCHAR(100) NOT NULL,
		EstadoRegistro SMALLINT DEFAULT 1,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
		UsuarioCreacion INT NOT NULL,
		UsuarioModificacion INT NOT NULL,

		CONSTRAINT UK_Nombre UNIQUE (Nombre)
	);
END
GO

CREATE INDEX IDX_Usuario_Nombre
	ON Usuario (Nombre);

CREATE INDEX IDX_Usuario_EstadoRegistro
	ON Usuario (EstadoRegistro);

CREATE INDEX IDX_Usuario_FechaCreacion
	ON Usuario (FechaCreacion);

-- ====================================================================================================================================
-- Tabla: Receta
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Receta')
BEGIN
	CREATE TABLE Receta (
		RecetaId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40) NULL,
		EstadoRegistro SMALLINT DEFAULT 1,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
		UsuarioCreacion INT NOT NULL,
		UsuarioModificacion INT NOT NULL,

		CONSTRAINT UK_Receta_Codigo UNIQUE (Codigo),
		CONSTRAINT FK_Receta_UsuarioCreacion FOREIGN KEY (UsuarioCreacion) REFERENCES Usuario(UsuarioId),
		CONSTRAINT FK_Receta_UsuarioModificacion FOREIGN KEY (UsuarioModificacion) REFERENCES Usuario(UsuarioId)
	);
END
GO

CREATE INDEX IDX_Receta_Codigo 
	ON Receta (Codigo);

CREATE INDEX IDX_Receta_EstadoRegistro
	ON Receta (EstadoRegistro);

CREATE INDEX IDX_Receta_FechaCreacion 
	ON Receta (FechaCreacion);

-- ====================================================================================================================================
-- Tabla: TipoInsumo
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TipoInsumo')
BEGIN
	CREATE TABLE TipoInsumo (
		TipoInsumoId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40) NULL,
		EstadoRegistro SMALLINT DEFAULT 1,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
		UsuarioCreacion INT NOT NULL,
		UsuarioModificacion INT NOT NULL,

		CONSTRAINT UK_TipoInsumo_Codigo UNIQUE (Codigo),
		CONSTRAINT FK_TipoInsumo_UsuarioCreacion FOREIGN KEY (UsuarioCreacion) REFERENCES Usuario(UsuarioId),
		CONSTRAINT FK_TipoInsumo_UsuarioModificacion FOREIGN KEY (UsuarioModificacion) REFERENCES Usuario(UsuarioId)
	);
END
GO

CREATE INDEX IDX_TipoInsumo_Codigo 
	ON TipoInsumo (Codigo);

CREATE INDEX IDX_TipoInsumo_EstadoRegistro
	ON TipoInsumo (EstadoRegistro);

CREATE INDEX IDX_TipoInsumo_FechaCreacion 
	ON TipoInsumo (FechaCreacion);

-- ====================================================================================================================================
-- Tabla: Balanza
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Balanza')
BEGIN
	CREATE TABLE Balanza (
		BalanzaId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40) NULL,
		EstadoRegistro SMALLINT DEFAULT 1,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
		UsuarioCreacion INT NOT NULL,
		UsuarioModificacion INT NOT NULL,

		CONSTRAINT UK_Balanza_Codigo UNIQUE (Codigo),
		CONSTRAINT FK_Balanza_UsuarioCreacion FOREIGN KEY (UsuarioCreacion) REFERENCES Usuario(UsuarioId),
		CONSTRAINT FK_Balanza_UsuarioModificacion FOREIGN KEY (UsuarioModificacion) REFERENCES Usuario(UsuarioId)
	);
END
GO

CREATE INDEX IDX_Balanza_Codigo 
	ON Balanza (Codigo);

CREATE INDEX IDX_Balanza_EstadoRegistro
	ON Balanza (EstadoRegistro);

CREATE INDEX IDX_Balanza_FechaCreacion
	ON Balanza (FechaCreacion);

-- ====================================================================================================================================
-- Tabla: Formula
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Formula')
BEGIN
	CREATE TABLE Formula (
		FormulaId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40) NULL,
		EstadoRegistro SMALLINT DEFAULT 1,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
		UsuarioCreacion INT NOT NULL,
		UsuarioModificacion INT NOT NULL,

		CONSTRAINT UK_Formula_Codigo UNIQUE (Codigo),
		CONSTRAINT FK_Formula_UsuarioCreacion FOREIGN KEY (UsuarioCreacion) REFERENCES Usuario(UsuarioId),
		CONSTRAINT FK_Formula_UsuarioModificacion FOREIGN KEY (UsuarioModificacion) REFERENCES Usuario(UsuarioId)
	);
END
GO

CREATE INDEX IDX_Formula_Codigo
	ON Formula (Codigo);

CREATE INDEX IDX_Formula_EstadoRegistro
	ON Formula (EstadoRegistro);

CREATE INDEX IDX_Formula_FechaCreacion
	ON Formula (FechaCreacion);

-- ====================================================================================================================================
-- Tabla: RecetaVersion
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RecetaVersion')
BEGIN
	CREATE TABLE RecetaVersion (
		RecetaVersionId INT PRIMARY KEY IDENTITY(1001, 1),
		FormulaId INT NOT NULL,
		RecetaId INT NOT NULL,
		NumeroVersion INT NOT NULL,
		Estado SMALLINT NOT NULL DEFAULT 1,
		EstadoRegistro SMALLINT DEFAULT 1,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
		UsuarioCreacion INT NOT NULL,
		UsuarioModificacion INT NOT NULL,

		CONSTRAINT UK_RecetaVersion_FormulaId_RecetaId_NumeroVersion UNIQUE (FormulaId, RecetaId, NumeroVersion),
		CONSTRAINT FK_RecetaVersion_FormulaId FOREIGN KEY (FormulaId) REFERENCES Formula(FormulaId),
		CONSTRAINT FK_RecetaVersion_RecetaId FOREIGN KEY (RecetaId) REFERENCES Receta(RecetaId),
		CONSTRAINT FK_RecetaVersion_UsuarioCreacion FOREIGN KEY (UsuarioCreacion) REFERENCES Usuario(UsuarioId),
		CONSTRAINT FK_RecetaVersion_UsuarioModificacion FOREIGN KEY (UsuarioModificacion) REFERENCES Usuario(UsuarioId)
	);
END
GO

CREATE INDEX IDX_RecetaVersion_RecetaId
	ON RecetaVersion (RecetaId);

CREATE INDEX IDX_RecetaVersion_FormulaId
	ON RecetaVersion (FormulaId);

CREATE INDEX IDX_RecetaVersion_FormulaId_RecetaId_NumeroVersion
	ON RecetaVersion (FormulaId, RecetaId, NumeroVersion);

CREATE INDEX IDX_RecetaVersion_EstadoRegistro
	ON RecetaVersion (EstadoRegistro);

CREATE INDEX IDX_RecetaVersion_Estado
	ON RecetaVersion (Estado);

-- ====================================================================================================================================
-- Tabla: Insumo
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Insumo')
BEGIN
	CREATE TABLE Insumo (
		InsumoId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40) NULL,
		TipoInsumoId INT NOT NULL,
		Unidad VARCHAR(5) NOT NULL DEFAULT 'kg',
		EstadoRegistro SMALLINT DEFAULT 1,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
		UsuarioCreacion INT NOT NULL,
		UsuarioModificacion INT NOT NULL,

		CONSTRAINT UK_Insumo_Codigo UNIQUE (Codigo),
		CONSTRAINT FK_Insumo_TipoInsumoId FOREIGN KEY (TipoInsumoId) REFERENCES TipoInsumo(TipoInsumoId),
		CONSTRAINT FK_Insumo_UsuarioCreacion FOREIGN KEY (UsuarioCreacion) REFERENCES Usuario(UsuarioId),
		CONSTRAINT FK_Insumo_UsuarioModificacion FOREIGN KEY (UsuarioModificacion) REFERENCES Usuario(UsuarioId)
	);
END
GO

CREATE INDEX IDX_Insumo_Codigo
	ON Insumo (Codigo);

CREATE INDEX IDX_Insumo_TipoInsumoId
	ON Insumo (TipoInsumoId);

CREATE INDEX IDX_Insumo_EstadoRegistro
	ON Insumo (EstadoRegistro);

CREATE INDEX IDX_Insumo_FechaCreacion
	ON Insumo (FechaCreacion);

-- ====================================================================================================================================
-- Tabla: RecetaVersionDetalle
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RecetaVersionDetalle')
BEGIN
	CREATE TABLE RecetaVersionDetalle (
		RecetaVersionDetalleId INT PRIMARY KEY IDENTITY(1001, 1),
		RecetaVersionId INT NOT NULL,
		InsumoId INT NOT NULL,
		CantidadRequerida DECIMAL(10, 4) NOT NULL,
		ToleranciaMaxima DECIMAL(5, 2) NOT NULL DEFAULT 0.01,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
		UsuarioCreacion INT NOT NULL,
		UsuarioModificacion INT NOT NULL,

		CONSTRAINT UK_RecetaVersionDetalle UNIQUE (FormulaId, RecetaVersionId, InsumoId),
		CONSTRAINT FK_RecetaVersionDetalle_RecetaVersionId FOREIGN KEY (RecetaVersionId) REFERENCES RecetaVersion(RecetaVersionId),
		CONSTRAINT FK_RecetaVersionDetalle_InsumoId FOREIGN KEY (InsumoId) REFERENCES Insumo(InsumoId),
		CONSTRAINT FK_RecetaVersionDetalle_UsuarioCreacion FOREIGN KEY (UsuarioCreacion) REFERENCES Usuario(UsuarioId),
		CONSTRAINT FK_RecetaVersionDetalle_UsuarioModificacion FOREIGN KEY (UsuarioModificacion) REFERENCES Usuario(UsuarioId)
	);
END
GO

CREATE INDEX IDX_RecetaVersionDetalle_FormulaId 
	ON RecetaVersionDetalle (FormulaId);

CREATE INDEX IDX_RecetaVersionDetalle_RecetaVersionId 
	ON RecetaVersionDetalle (RecetaVersionId);

CREATE INDEX IDX_RecetaVersionDetalle_InsumoId 
	ON RecetaVersionDetalle (InsumoId);

CREATE INDEX IDX_RecetaVersionDetalle_FechaCreacion 
	ON RecetaVersionDetalle (FechaCreacion);

-- ====================================================================================================================================
-- Tabla: RegistroPeso
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RegistroPeso')
BEGIN
	CREATE TABLE RegistroPeso (
		RegistroPesoId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		RecetaVersionId INT NOT NULL,
		InsumoId INT NOT NULL,
		BalanzaId INT NOT NULL,
		UsuarioId INT NOT NULL,
		FechaPesado DATETIME NOT NULL DEFAULT GETDATE(),
		CantidadPesada DECIMAL(10,4) NOT NULL,
		Estado SMALLINT NOT NULL DEFAULT 1,
		EstadoRegistro SMALLINT DEFAULT 1,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
		UsuarioCreacion INT NOT NULL,
		UsuarioModificacion INT NOT NULL,

		CONSTRAINT UK_Codigo UNIQUE (Codigo),
		CONSTRAINT FK_RegistroPeso_RecetaVersionId FOREIGN KEY (RecetaVersionId) REFERENCES RecetaVersion(RecetaVersionId),
		CONSTRAINT FK_RegistroPeso_InsumoId FOREIGN KEY (InsumoId) REFERENCES INSUMO(InsumoId),
		CONSTRAINT FK_RegistroPeso_BalanzaId FOREIGN KEY (BalanzaId) REFERENCES Balanza(BalanzaId),
		CONSTRAINT FK_RegistroPeso_UsuarioId FOREIGN KEY (UsuarioId) REFERENCES Usuario(UsuarioId),
		CONSTRAINT FK_RegistroPeso_UsuarioCreacion FOREIGN KEY (UsuarioCreacion) REFERENCES Usuario(UsuarioId),
		CONSTRAINT FK_RegistroPeso_UsuarioModificacion FOREIGN KEY (UsuarioModificacion) REFERENCES Usuario(UsuarioId)
	);
END
GO

CREATE INDEX IDX_RegistroPeso_Codigo 
	ON RegistroPeso (Codigo);

CREATE INDEX IDX_RegistroPeso_RecetaVersionId 
	ON RegistroPeso (RecetaVersionId);

CREATE INDEX IDX_RegistroPeso_InsumoId 
	ON RegistroPeso (InsumoId);

CREATE INDEX IDX_RegistroPeso_BalanzaId
	ON RegistroPeso (BalanzaId);

CREATE INDEX IDX_RegistroPeso_UsuarioId
	ON RegistroPeso (UsuarioId);

CREATE INDEX IDX_RegistroPeso_FechaPesado
	ON RegistroPeso (FechaPesado);

CREATE INDEX IDX_RegistroPeso_Estado
	ON RegistroPeso (Estado);

CREATE INDEX IDX_RegistroPeso_EstadoRegistro
	ON RegistroPeso (EstadoRegistro);

CREATE INDEX IDX_RegistroPeso_FechaCreacion
	ON RegistroPeso (FechaCreacion);

-- ====================================================================================================================================
-- Tabla: RegistroBatch
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RegistroBatch')
BEGIN
	CREATE TABLE RegistroBatch (
		RegistroBatchId INT PRIMARY KEY IDENTITY(1001, 1),
		CodigoLote INT NOT NULL,
		RecetaVersionId INT NOT NULL,
		FechaInicio DATETIME NULL,
		FechaFin DATETIME NULL,
		Usuario VARCHAR(20) NOT NULL,
		Estado SMALLINT NOT NULL DEFAULT 0,
		EstadoRegistro SMALLINT DEFAULT 1,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
		UsuarioCreacion INT NOT NULL,
		UsuarioModificacion INT NOT NULL,

		CONSTRAINT FK_RegistroBatch_RecetaVersionId FOREIGN KEY (RecetaVersionId) REFERENCES RecetaVersion(RecetaVersionId),
		CONSTRAINT FK_RegistroBatch_UsuarioCreacion FOREIGN KEY (UsuarioCreacion) REFERENCES Usuario(UsuarioId),
		CONSTRAINT FK_RegistroBatch_UsuarioModificacion FOREIGN KEY (UsuarioModificacion) REFERENCES Usuario(UsuarioId)
	);
END
GO

CREATE INDEX IDX_RegistroBatch_CodigoLote
	ON RegistroBatch (CodigoLote);

CREATE INDEX IDX_RegistroBatch_RecetaVersionId
	ON RegistroBatch (RecetaVersionId);

CREATE INDEX IDX_RegistroBatch_FechaInicio
	ON RegistroBatch (FechaInicio);

CREATE INDEX IDX_RegistroBatch_FechaFin
	ON RegistroBatch (FechaFin);

CREATE INDEX IDX_RegistroBatch_EstadoRegistro
	ON RegistroBatch (EstadoRegistro);

CREATE INDEX IDX_RegistroBatch_FechaCreacion
	ON RegistroBatch (FechaCreacion);

-- ====================================================================================================================================
-- Tabla: RegistroBatchDetalle
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RegistroBatchDetalle')
BEGIN
	CREATE TABLE RegistroBatchDetalle (
		RegistroBatchDetalleId INT PRIMARY KEY IDENTITY(1001, 1),
		RegistroBatchId INT NOT NULL,
		InsumoId INT NOT NULL,
		RegistroPesoId INT NOT NULL,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
		UsuarioCreacion INT NOT NULL,
		UsuarioModificacion INT NOT NULL,

		CONSTRAINT FK_RegistroBatchDetalle_RegistroBatchId FOREIGN KEY (RegistroBatchId) references RegistroBatch(RegistroBatchId),
		CONSTRAINT FK_RegistroBatchDetalle_InsumoId FOREIGN KEY (InsumoId) references Insumo(InsumoId),
		CONSTRAINT FK_RegistroBatchDetalle_RegistroPesoId FOREIGN KEY (RegistroPesoId) references RegistroPeso(RegistroPesoId),
		CONSTRAINT FK_RegistroBatchDetalle_UsuarioCreacion FOREIGN KEY (UsuarioCreacion) REFERENCES Usuario(UsuarioId),
		CONSTRAINT FK_RegistroBatchDetalle_UsuarioModificacion FOREIGN KEY (UsuarioModificacion) REFERENCES Usuario(UsuarioId)
	);
END
GO

CREATE INDEX IDX_RegistroBatchDetalle_RegistroBatchId
	ON RegistroBatchDetalle (RegistroBatchId);

CREATE INDEX IDX_RegistroBatchDetalle_InsumoId
	ON RegistroBatchDetalle (InsumoId);

CREATE INDEX IDX_RegistroBatchDetalle_RegistroPesoId
	ON RegistroBatchDetalle (RegistroPesoId);

CREATE INDEX IDX_RegistroBatchDetalle_FechaCreacion
	ON RegistroBatchDetalle (FechaCreacion);