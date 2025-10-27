-- ============================================
-- Script de Creaci�n de Base de Datos
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
	GRANT EXECUTE TO RecipeOperator;
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
		('HDAEVEG2','Medición por flujómetro', 1001, 'kg'),
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

IF NOT EXISTS (SELECT * FROM Usuario)
BEGIN
	INSERT INTO Usuario(Nombre, ClaveHash)
	VALUES
		('Admin','21232f297a57a5a743894a0e4a801fc3'),
		('Operador','d3d9446802a44259755d38e6d163e820');
	PRINT('Datos de ejemplo Usuario insertados')
END

IF NOT EXISTS (SELECT * FROM RecetaVersion)
BEGIN
	INSERT INTO RecetaVersion(RecetaId, VersionNum, Descripcion, Estado)
	VALUES
		(1001, 1, 'Primera version de Hamayoneah', 0),
		(1001, 2, 'Segunda version de Hamayoneah', 1),
		(1002, 1, 'Primera version de Hamayolight', 1);
	PRINT('Datos de ejemplo RecetaVersion insertados')
END

IF NOT EXISTS (SELECT * FROM DetalleReceta)
BEGIN
	INSERT INTO DetalleReceta(Codigo, Descripcion, FormulaId, RecetaVersionId, InsumoId, Valor, Variacion)
	VALUES
		('DR1001','', 1001, 1001, 1002, 13.0000, 0.50),
		('DR1002','', 1001, 1001, 1003, 10.3000, 0.50),
		('DR1003','', 1001, 1001, 1004, 10.5000, 0.20),
		('DR1004','', 1001, 1001, 1005, 12.1000, 0.50),
		('DR1005','', 1001, 1002, 1002, 13.7000, 0.10),
		('DR1006','', 1001, 1002, 1003, 10.3000, 0.50),
		('DR1007','', 1001, 1002, 1004, 10.5400, 0.05),
		('DR1008','', 1001, 1002, 1005, 12.1000, 0.50),
		('DR1009','', 1001, 1003, 1003, 8.2000, 0.08),
		('DR1010','', 1001, 1003, 1005, 7.0100, 0.07);
	PRINT('Datos de ejemplo DetalleReceta insertados')
END

IF NOT EXISTS (SELECT * FROM RegistroPeso)
BEGIN
	INSERT INTO RegistroPeso(Codigo, Descripcion, RecetaVersionId, InsumoId, BalanzaId, UsuarioId, Valor, Estado)
	VALUES
		('RP1001','', 1002, 1002, 1001, 1001, 13.2000, 0),
		('RP1002','', 1002, 1003, 1001, 1001, 10.1000, 0),
		('RP1003','', 1002, 1004, 1002, 1002, 10.6000, 0),
		('RP1004','', 1002, 1005, 1002, 1001, 12.3000, 0),
		('RP1005','', 1002, 1002, 1001, 1002, 13.0000, 1),
		('RP1006','', 1002, 1003, 1001, 1001, 11.1000, 1),
		('RP1007','', 1002, 1004, 1002, 1002, 11.6000, 1),
		('RP1008','', 1002, 1005, 1002, 1001, 12.1000, 1);
	PRINT('Datos de ejemplo RegistroPeso insertados')
END

IF NOT EXISTS (SELECT * FROM RegistroBatch)
BEGIN
	INSERT INTO RegistroBatch(Codigo, Descripcion, Lote, DetalleRecetaId, RegistroPesoId, FechaPreparacion)
	VALUES
		('RB1001','', 5001, 1005, 1001, GETDATE()),
		('RB1002','', 5001, 1006, 1002, GETDATE()),
		('RB1003','', 5001, 1007, 1003, GETDATE()),
		('RB1004','', 5001, 1008, 1004, GETDATE());
	PRINT('Datos de ejemplo RegistroBatch insertados')
END

IF NOT EXISTS (SELECT * FROM RegistroBatchWarehouse)
BEGIN
	INSERT INTO RegistroBatchWarehouse(Lote, FormulaId, RecetaVersionId, InsumoId, ValorSetpoint, Variacion, RegistroPesoId, FechaPreparacion)
	VALUES
		(5001, 1001, 1002, 1002, 13.7000, 0.10, 1001, GETDATE()),
		(5001, 1001, 1002, 1003, 10.3000, 0.50, 1002, GETDATE()),
		(5001, 1001, 1002, 1004, 10.5400, 0.05, 1003, GETDATE()),
		(5001, 1001, 1002, 1005, 12.1000, 0.50, 1004, GETDATE());
	PRINT('Datos de ejemplo RegistroBatchWarehouse insertados')
END

-- ============================================
-- Creación de procedimientos almacenados
-- ============================================

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_InsertarNuevoRegistroPeso')
	DROP PROCEDURE sp_InsertarNuevoRegistroPeso;
GO

CREATE PROCEDURE sp_InsertarNuevoRegistroPeso
	@Descripcion		VARCHAR(40),
	@RecetaVersionId	INT,
	@InsumoId			INT,
	@BalanzaId			INT,
	@UsuarioId			INT,
	@FechaPesado		DATETIME,
	@Valor				DECIMAL(10,4)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CodigoInsumo VARCHAR(20);
	DECLARE @NuevoId INT;
	DECLARE @Codigo VARCHAR(20);

	-- Obtener el código del insumo
	SELECT @CodigoInsumo = Codigo 
	FROM Insumo 
	WHERE InsumoId = @InsumoId;

	-- Obetener el siguiente registro de peso
	SELECT @NuevoId = ISNULL(COUNT(RegistroPesoId), 1000) + 1
	FROM RegistroPeso
	WHERE InsumoId = @InsumoId;

	-- Generar el código del nuevo registro de peso
	SET @Codigo = @CodigoInsumo + '-' + RIGHT('000000' + CAST(@NuevoId AS VARCHAR(6)), 6);

	INSERT INTO RegistroPeso (Codigo, Descripcion, RecetaVersionId, InsumoId, BalanzaId, UsuarioId, FechaPesado, Valor)
	OUTPUT INSERTED.*
	VALUES (@Codigo, @Descripcion, @RecetaVersionId, @InsumoId, @BalanzaId, @UsuarioId, @FechaPesado, @Valor)
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetInsumosPorRecetaYTipo')
	DROP PROCEDURE sp_GetInsumosPorRecetaYTipo
GO

CREATE PROCEDURE sp_GetInsumosPorRecetaYTipo
	@RecetaVersionId	INT,
	@TipoInsumoId		INT
AS
BEGIN
	SET NOCOUNT ON;

	WITH DetalleRecetaFiltrado AS
	(
		SELECT
			DetalleRecetaId,
			InsumoId
		FROM DetalleReceta
		WHERE RecetaVersionId = @RecetaVersionId
	)
	SELECT 
		i.InsumoId AS InsumoId,
		i.Codigo AS Codigo,
		i.Descripcion AS Descripcion,
		i.TipoInsumoId AS TipoInsumoId,
		i.Unidad AS Unidad,
		i.FechaCreacion AS FechaCreacion,
		i.FechaModificacion AS FechaModificacion
	FROM DetalleRecetaFiltrado d
	INNER JOIN Insumo i ON d.InsumoId = i.InsumoId
	WHERE i.TipoInsumoId = @TipoInsumoId;
	
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetRegistrosDisponibles')
	DROP PROCEDURE sp_GetRegistrosDisponibles
GO

CREATE PROCEDURE sp_GetRegistrosDisponibles
	@InsumoId			INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		Codigo,
		Valor
	FROM RegistroPeso
	WHERE InsumoId = @InsumoId
		AND Estado = 1
	ORDER BY FechaPesado DESC;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_LeerDatosReceta')
	DROP PROCEDURE sp_LeerDatosReceta
GO
CREATE PROCEDURE sp_LeerDatosReceta
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		r.Codigo AS RecetaCodigo,
		i.Codigo AS InsumoCodigo,
		dr.Valor AS Valor
		FROM DetalleReceta dr
		JOIN RecetaVersion rv ON dr.RecetaVersionId = rv.RecetaVersionId
		JOIN Receta r ON rv.RecetaId = r.RecetaId
		JOIN Insumo i ON dr.InsumoId = i.InsumoId
		WHERE rv.Estado = 1
END
GO

-- ============================================
-- Creación de vistas
-- ============================================
IF EXISTS (SELECT * FROM sys. views WHERE name = 'vw_RegistroPesoDataGrid')
	DROP VIEW vw_RegistroPesoDataGrid;
GO

CREATE VIEW vw_RegistroPesoDataGrid
AS
SELECT TOP 100
    rp.RegistroPesoId,
    i.Codigo AS InsumoCodigo,
    ti.Codigo AS TipoInsumoCodigo,
    rp.FechaPesado,
    rp.Valor,
    u.Nombre AS UsuarioNombre,
    rp.Codigo,
    CAST(rp.Estado AS INT) AS Estado
FROM RegistroPeso rp
JOIN Insumo i ON rp.InsumoId = i.InsumoId
JOIN TipoInsumo ti ON i.TipoInsumoId = ti.TipoInsumoId
JOIN Usuario u ON rp.UsuarioId = u.UsuarioId
ORDER BY rp.FechaPesado DESC, rp.RegistroPesoId DESC;
GO

IF EXISTS (SELECT * FROM sys. views WHERE name = 'vw_RegistroPesoActivoDataGrid')
	DROP VIEW vw_RegistroPesoActivoDataGrid;
GO

CREATE VIEW vw_RegistroPesoActivoDataGrid
AS
SELECT TOP 100
    rp.RegistroPesoId,
    i.Codigo AS InsumoCodigo,
    ti.Codigo AS TipoInsumoCodigo,
    rp.FechaPesado,
    rp.Valor,
    u.Nombre AS UsuarioNombre,
    rp.Codigo,
    CAST(rp.Estado AS INT) AS Estado
FROM RegistroPeso rp
JOIN Insumo i ON rp.InsumoId = i.InsumoId
JOIN TipoInsumo ti ON i.TipoInsumoId = ti.TipoInsumoId
JOIN Usuario u ON rp.UsuarioId = u.UsuarioId
WHERE Estado = 1
ORDER BY rp.FechaPesado DESC, rp.RegistroPesoId DESC;
GO

IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_GetAllRecetasActivas')
	DROP VIEW vw_GetAllRecetasActivas;
GO

CREATE VIEW vw_GetAllRecetasActivas
AS
SELECT
	rv.RecetaVersionId as RecetaVersionId,
	rv.RecetaId as RecetaId,
	r.Codigo as RecetaCodigo,
	rv.VersionNum as VersionNum,
	rv.Estado as Estado
FROM RecetaVersion rv
JOIN Receta r ON rv.RecetaId = r.RecetaId
WHERE rv.Estado = 1;
GO