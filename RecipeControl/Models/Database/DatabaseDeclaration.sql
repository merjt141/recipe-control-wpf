-- ============================================
-- Script de Creación de Base de Datos
-- Sistema de Balanzas
-- ============================================

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
go

USE REPRECIPE;
GO

-- ============================================
-- Tabla: Receta
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Receta')
BEGIN
	CREATE TABLE Receta (
		RecetaId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40),
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),

		CONSTRAINT UK_CODE UNIQUE (Codigo)
	);
END
GO

-- ============================================
-- Tabla: TipoInsumo
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TipoInsumo')
BEGIN
	CREATE TABLE TipoInsumo (
		TipoInsumoId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40),
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
	);
END
GO

-- ============================================
-- Tabla: Balanza
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Balanza')
BEGIN
	CREATE TABLE Balanza (
		BalanzaId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40),
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
	);
END
GO

-- ============================================
-- Tabla: Usuario
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuario')
BEGIN
	CREATE TABLE Usuario (
		UsuarioId INT PRIMARY KEY IDENTITY(1001, 1),
		Nombre VARCHAR(20) NOT NULL,
		ClaveHash VARCHAR(100) NOT NULL,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),

		CONSTRAINT UK_Nombre UNIQUE (Nombre)
	);
END
GO

-- ============================================
-- Tabla: RecetaVersion
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RecetaVersion')
BEGIN
	CREATE TABLE RecetaVersion (
		RecetaVersionId INT PRIMARY KEY IDENTITY(1001, 1),
		RecetaId INT NOT NULL,
		VersionNum INT NOT NULL,
		Descripcion VARCHAR(40),
		Estado SMALLINT NOT NULL DEFAULT 1,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),

		CONSTRAINT FK_RecetaId FOREIGN KEY (RecetaId) REFERENCES Receta(RecetaId),
		CONSTRAINT UK_RecetaVersion UNIQUE (RecetaId, VersionNum)
	);
END
GO

-- ============================================
-- Tabla: Formula
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Formula')
BEGIN
	CREATE TABLE Formula (
		FormulaId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40),
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),
	);
END
GO

-- ============================================
-- Tabla: Insumo
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Insumo')
BEGIN
	CREATE TABLE Insumo (
		InsumoId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40),
		TipoInsumoId INT NOT NULL,
		Unidad VARCHAR(5) NOT NULL DEFAULT 'kg',
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),

		CONSTRAINT FK_TipoInsumo FOREIGN KEY (TipoInsumoId) REFERENCES TipoInsumo(TipoInsumoId)
	);
END
GO

-- ============================================
-- Tabla: DetalleReceta
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DetalleReceta')
BEGIN
	CREATE TABLE DetalleReceta (
		DetalleRecetaId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40),
		FormulaId INT NOT NULL,
		RecetaVersionId INT NOT NULL,
		InsumoId INT NOT NULL,
		Valor DECIMAL(10, 4) NOT NULL,
		Variacion DECIMAL(5, 2) NOT NULL DEFAULT 0.01,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),

		CONSTRAINT FK_FormulaId FOREIGN KEY (FormulaId) REFERENCES Formula(FormulaId),
		CONSTRAINT FK_RecetaVersionId FOREIGN KEY (RecetaVersionId) REFERENCES RecetaVersion(RecetaVersionId),
		CONSTRAINT FK_InsumoId FOREIGN KEY (InsumoId) REFERENCES Insumo(InsumoId),
	);
END
GO

-- ============================================
-- Tabla: RegistroPeso
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RegistroPeso')
BEGIN
	CREATE TABLE RegistroPeso (
		RegistroPesoId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40),
		RecetaVersionId INT NOT NULL,
		InsumoId INT NOT NULL,
		BalanzaId INT NOT NULL,
		UsuarioId INT NOT NULL,
		FechaPesado DATETIME NOT NULL DEFAULT GETDATE(),
		Valor DECIMAL(10,4) NOT NULL,
		Estado SMALLINT NOT NULL DEFAULT 1,
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),

		CONSTRAINT UK_Codigo UNIQUE (Codigo),
		CONSTRAINT FK_RecetaVersionWeightId FOREIGN KEY (RecetaVersionId) REFERENCES RecetaVersion(RecetaVersionId),
		CONSTRAINT FK_InsumoRegistroPesoId FOREIGN KEY (InsumoId) REFERENCES INSUMO(InsumoId),
		CONSTRAINT FK_BalanzaRegistroPesoId FOREIGN KEY (BalanzaId) REFERENCES Balanza(BalanzaId),
		CONSTRAINT FK_UsuarioRegistroPesoId FOREIGN KEY (UsuarioId) REFERENCES Usuario(UsuarioId),
	);
END
GO

-- ============================================
-- Tabla: RegistroBatch
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RegistroBatch')
BEGIN
	CREATE TABLE RegistroBatch (
		RegistroBatchId INT PRIMARY KEY IDENTITY(1001, 1),
		Codigo VARCHAR(20) NOT NULL,
		Descripcion VARCHAR(40),
		Lote INT NOT NULL,
		DetalleRecetaId INT NOT NULL,
		RegistroPesoId INT NOT NULL,
		FechaPreparacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),

		CONSTRAINT FK_DetalleRecetaRegistroBatchId FOREIGN KEY (DetalleRecetaId) references DetalleReceta(DetalleRecetaId),
		CONSTRAINT FK_RegistroPesoRegistroBatchId FOREIGN KEY (RegistroPesoId) references RegistroPeso(RegistroPesoId),
	);
END
GO

-- ============================================
-- Tabla: RegistroBatchWarehouse
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RegistroBatchWarehouse')
BEGIN
	CREATE TABLE RegistroBatchWarehouse (
		RegistroBatchWarehouseId INT PRIMARY KEY IDENTITY(1001, 1),
		Lote INT NOT NULL,
		FormulaId INT NOT NULL,
		RecetaVersionId INT NOT NULL,
		InsumoId INT NOT NULL,
		ValorSetpoint DECIMAL(10,4) NOT NULL,
		Variacion DECIMAL(5,2) NOT NULL,
		RegistroPesoId INT NOT NULL,
		FechaPreparacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
		FechaModificacion DATETIME NOT NULL DEFAULT GETDATE(),

		CONSTRAINT FK_FormulaWarehouseId FOREIGN KEY (FormulaId) references Formula(FormulaId),
		CONSTRAINT FK_RecetaVersionWarehouseId FOREIGN KEY (RecetaVersionId) references RecetaVersion(RecetaVersionId),
		CONSTRAINT FK_InsumoWarehouseId FOREIGN KEY (InsumoId) references Insumo(InsumoId),
		CONSTRAINT FK_RegistroPesoWarehouseId FOREIGN KEY (RegistroPesoId) references RegistroPeso(RegistroPesoId),
	);
END
GO

-- ============================================
-- Login: Login creation
-- ============================================

USE MASTER;
GO

IF NOT EXISTS (SELECT * FROM sys.syslogins WHERE name = 'RecipeOperator')
BEGIN
	CREATE LOGIN RecipeOperator
	WITH PASSWORD = 'Aex%5td123'
END

USE REPRECIPE;
GO

IF NOT EXISTS (SELECT * FROM REPRECIPE.sys.sysusers WHERE name = 'RecipeOperator')
BEGIN
	CREATE USER RecipeOperator FOR LOGIN RecipeOperator;
	exec sp_addrolemember 'db_datareader', 'RecipeOperator';
	exec sp_addrolemember 'db_datawriter', 'RecipeOperator';
END


-- ============================================
-- Ingreso de datos de pruebas
-- ============================================

IF NOT EXISTS (SELECT * FROM TipoInsumo)
BEGIN
	INSERT INTO TipoInsumo(Codigo, Descripcion)
	VALUES
		('LIQUIDO',''),
		('SOLIDO',''),
		('GRANULAR','');

	PRINT('Datos de ejemplo TipoInsumo insertados')
END

IF NOT EXISTS (SELECT * FROM Receta)
BEGIN
	INSERT INTO Receta(Codigo, Descripcion)
	VALUES
		('HAMAYONEAH1',''),
		('HAMAYOLIGHTLAS','');

	PRINT('Datos de ejemplo Receta insertados')
END

IF NOT EXISTS (SELECT * FROM Formula)
BEGIN
	INSERT INTO Formula(Codigo, Descripcion)
	VALUES
		('GENESIS',''),
		('FRIMA','');

	PRINT('Datos de ejemplo Formula insertados')
END

IF NOT EXISTS (SELECT * FROM Balanza)
BEGIN
	INSERT INTO Balanza(Codigo, Descripcion)
	VALUES
		('R71MD15',''),
		('R71MD6','');

	PRINT('Datos de ejemplo Balanza insertados')
END

IF NOT EXISTS (SELECT * FROM Insumo)
BEGIN
	INSERT INTO Insumo(Codigo, Descripcion, TipoInsumoId, Unidad)
	VALUES
		('HDAEVEG2','', 1004, 'kg'),
		('MC31220003','', 1001, 'kg'),
		('MC31220004','', 1001, 'kg'),
		('MS004RALP','', 1002, 'kg'),
		('MC31220005','', 1002, 'kg'),
		('MC31220001','', 1002, 'kg'),
		('MC31220270','', 1002, 'kg'),
		('CMC31220002','', 1001, 'kg'),
		('MS00ATX01','', 1001, 'kg'),
		('MC31220006','', 1001, 'kg'),
		('MC31220007','', 1002, 'kg');

	PRINT('Datos de ejemplo Insumo insertados')
END