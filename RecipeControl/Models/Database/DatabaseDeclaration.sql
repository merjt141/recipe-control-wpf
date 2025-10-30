-- ====================================================================================================================================
-- Script de Creación de Base de Datos
-- Sistema de Balanzas
-- Autor: Juan Armando Joyo Taype
-- Fecha: 2025/10/30
-- Versión: 1.0.0
-- Descripción: Script para la creación de la base de datos REPRECIPE, tablas, índices, procedimientos almacenados y vistas.
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
-- Tabla: Receta
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- Tabla: TipoInsumo
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- Tabla: Balanza
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- Tabla: Usuario
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- Tabla: RecetaVersion
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- Tabla: Formula
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- Tabla: Insumo
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- Tabla: DetalleReceta
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- Tabla: RegistroPeso
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- Tabla: RegistroBatch
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- Tabla: RegistroBatchWarehouse
-- ====================================================================================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RegistroBatchWarehouse')
BEGIN
	CREATE TABLE RegistroBatchWarehouse (
		RegistroBatchWarehouseId INT PRIMARY KEY IDENTITY(1001, 1),
		Lote INT NOT NULL,
		FormulaId INT NOT NULL,
		RecetaVersionId INT NOT NULL,
		InsumoId INT NOT NULL,
		Usuario VARCHAR(20) NOT NULL,
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

-- ====================================================================================================================================
-- Login: Login creation
-- ====================================================================================================================================

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

	-- Asignar roles y permisos
	EXEC sp_addrolemember 'db_datareader', 'RecipeOperator';
	EXEC sp_addrolemember 'db_datawriter', 'RecipeOperator';

	-- Permisos adicionales si es necesario
	GRANT EXECUTE TO RecipeOperator;
END


-- ====================================================================================================================================
-- Ingreso de datos de pruebas
-- ====================================================================================================================================

IF NOT EXISTS (SELECT * FROM TipoInsumo)
BEGIN
	INSERT INTO TipoInsumo(Codigo, Descripcion)
	VALUES
		('LIQUIDO',''),
		('GRANULAR',''),
		('POLVO','');
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
		('R71MD15','Balanza de 15kg FRIMA'),
		('R71MD6','Balanza de 6 kg FRIMA');
	PRINT('Datos de ejemplo Balanza insertados')
END

IF NOT EXISTS (SELECT * FROM Insumo)
BEGIN
	INSERT INTO Insumo(Codigo, Descripcion, TipoInsumoId, Unidad)
	VALUES
		('HDAEVEG2','Medición por flujómetro', 1001, 'kg'),		-- 1001
		('MC31220003','', 1001, 'kg'),							-- 1002
		('MC31220004','', 1001, 'kg'),							-- 1003
		('MS004RALP','', 1002, 'kg'),							-- 1004
		('MC31220005','', 1002, 'kg'),							-- 5
		('MC31220001','', 1002, 'kg'),							-- 6
		('MC31220270','', 1002, 'kg'),							-- 7
		('CMC31220002','', 1001, 'kg'),							-- 8
		('MS00ATX01','', 1001, 'kg'),							-- 9
		('MC31220006','', 1001, 'kg'),							-- 1010
		('MC31220007','', 1002, 'kg'),
		('MS00CCB02','', 1001, 'kg'),
		('MC31220009','', 1001, 'kg'),
		('MC31220010','', 1001, 'kg'),
		('MC31220050','', 1002, 'kg'),							-- 1015
		('H20MF00001','Medición por flujómetro', 1001, 'kg'),
		('MC31220011','', 1001, 'kg'),
		('MC31220015','', 1001, 'kg'),
		('MS005RALP','', 1002, 'kg'),
		('MS006RALP','', 1002, 'kg'),							-- 1020
		('MC31660402','', 1002, 'kg'),
		('MS00CCB02','', 1001, 'kg');
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
		('DR00101','', 1001, 1001, 1001, 16.0000, 0.50),
		('DR00102','', 1001, 1001, 1002, 13.0000, 0.50),
		('DR00103','', 1001, 1001, 1003, 10.3000, 0.50),
		('DR00104','', 1001, 1001, 1004, 11.5000, 0.50),
		('DR00105','', 1001, 1001, 1005, 12.1000, 0.50),
		('DR00106','', 1001, 1001, 1006, 13.1000, 0.50),
		('DR00107','', 1001, 1001, 1007, 14.1000, 0.50),
		('DR00108','', 1001, 1001, 1008, 12.1000, 0.50),
		('DR00109','', 1001, 1001, 1009, 11.1000, 0.50),
		('DR00110','', 1001, 1001, 1010, 13.1000, 0.50),
		('DR00111','', 1001, 1001, 1011, 12.1000, 0.50),
		('DR00112','', 1001, 1001, 1012, 10.9000, 0.50),
		('DR00113','', 1001, 1001, 1013, 11.8000, 0.50),
		('DR00114','', 1001, 1001, 1014, 13.4000, 0.50),
		('DR00115','', 1001, 1001, 1015, 14.6000, 0.50),
		('DR00116','', 1001, 1001, 1016, 12.7000, 0.50),

		('DR00201','', 1001, 1002, 1001, 10.0000, 0.50),
		('DR00202','', 1001, 1002, 1002, 13.0000, 0.50),
		('DR00203','', 1001, 1002, 1003, 10.3000, 0.50),
		('DR00204','', 1001, 1002, 1004, 10.5000, 0.50),
		('DR00205','', 1001, 1002, 1005, 12.1000, 0.50),
		('DR00206','', 1001, 1002, 1006, 12.3000, 0.50),
		('DR00207','', 1001, 1002, 1007, 12.1400, 0.50),
		('DR00208','', 1001, 1002, 1008, 12.1200, 0.50),
		('DR00209','', 1001, 1002, 1009, 12.4000, 0.50),
		('DR00210','', 1001, 1002, 1010, 12.6000, 0.50),
		('DR00211','', 1001, 1002, 1011, 13.8000, 0.50),
		('DR00212','', 1001, 1002, 1012, 12.9000, 0.50),
		('DR00213','', 1001, 1002, 1013, 11.1300, 0.50),
		('DR00214','', 1001, 1002, 1014, 12.4000, 0.50),
		('DR00215','', 1001, 1002, 1015, 12.3560, 0.50),
		('DR00216','', 1001, 1002, 1016, 12.1000, 0.50),

		('DR00301','', 1001, 1003, 1001, 4.0500, 0.50),
		('DR00301','', 1001, 1003, 1017, 6.4000, 0.50),
		('DR00301','', 1001, 1003, 1018, 2.0500, 0.50),
		('DR00301','', 1001, 1003, 1019, 8.8600, 0.50),
		('DR00301','', 1001, 1003, 1004, 6.0400, 0.50),
		('DR00301','', 1001, 1003, 1005, 12.3000, 0.50),
		('DR00301','', 1001, 1003, 1020, 14.0200, 0.50),
		('DR00301','', 1001, 1003, 1021, 12.0300, 0.50),
		('DR00301','', 1001, 1003, 1006, 13.5000, 0.50),
		('DR00301','', 1001, 1003, 1007, 12.6400, 0.50),
		('DR00301','', 1001, 1003, 1008, 11.0300, 0.50),
		('DR00301','', 1001, 1003, 1011, 10.0200, 0.50),
		('DR00301','', 1001, 1003, 1010, 12.4000, 0.50),
		('DR00301','', 1001, 1003, 1012, 11.2000, 0.50),
		('DR00301','', 1001, 1003, 1013, 13.1400, 0.50),
		('DR00301','', 1001, 1003, 1014, 10.0500, 0.50),
		('DR00301','', 1001, 1003, 1015, 10.0500, 0.50),
		('DR00301','', 1001, 1003, 1016, 18.3000, 0.50);
	PRINT('Datos de ejemplo DetalleReceta insertados')
END

-- ====================================================================================================================================
-- Creación de índices de base de datos
-- ====================================================================================================================================

CREATE INDEX IDX_Insumo_Codigo ON Insumo (Codigo);
GO

CREATE INDEX IDX_DetalleReceta_RecetaVersionId ON DetalleReceta (RecetaVersionId);
GO

CREATE INDEX IDX_DetalleReceta_InsumoId ON DetalleReceta (InsumoId);
GO

CREATE INDEX IDX_DetalleReceta_FormulaId ON DetalleReceta (FormulaId);
GO

CREATE INDEX IDX_RegistroPeso_Codigo ON RegistroPeso (Codigo);
GO

CREATE INDEX IDX_RegistroPeso_InsumoId ON RegistroPeso (InsumoId);
GO

CREATE INDEX IDX_RegistroPeso_Estado ON RegistroPeso (Estado);
GO

CREATE INDEX IDX_RecetaVersion_RecetaId ON RecetaVersion (RecetaId);
GO

CREATE INDEX IDX_RecetaVersion_Estado ON RecetaVersion (Estado);
GO

CREATE INDEX IDX_Usuario_Nombre ON Usuario (Nombre);
GO

-- ====================================================================================================================================
-- Creación de procedimientos almacenados
-- ====================================================================================================================================

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

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetRegistrosDisponiblesPorCodigo')
	DROP PROCEDURE sp_GetRegistrosDisponiblesPorCodigo
GO

CREATE PROCEDURE sp_GetRegistrosDisponiblesPorCodigo
	@InsumoCodigo			VARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		rp.Codigo AS RegistroPesoCodigo,
		rp.Valor AS RegistroPesoValor,
		i.Unidad AS InsumoUnidad
	FROM RegistroPeso rp
	JOIN Insumo i ON rp.InsumoId = i.InsumoId
	WHERE i.Codigo = @InsumoCodigo
		AND rp.Estado = 1
	ORDER BY rp.FechaPesado DESC;
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_LeerDetalleReceta')
	DROP PROCEDURE sp_LeerDetalleReceta
GO
CREATE PROCEDURE sp_LeerDetalleReceta
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		r.RecetaId AS RecetaId,
		i.InsumoId AS InsumoId,
		dr.Valor AS Valor
		FROM DetalleReceta dr
		JOIN RecetaVersion rv ON dr.RecetaVersionId = rv.RecetaVersionId
		JOIN Receta r ON rv.RecetaId = r.RecetaId
		JOIN Insumo i ON dr.InsumoId = i.InsumoId
		WHERE rv.Estado = 1
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_LeerDetalleRecetaMinimo')
	DROP PROCEDURE sp_LeerDetalleRecetaMinimo
GO
CREATE PROCEDURE sp_LeerDetalleRecetaMinimo
	@RecetaId	INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		dr.RecetaVersionId AS RecetaVersionId,
		r.RecetaId AS RecetaId,
		r.Codigo AS RecetaCodigo,
		i.InsumoId AS InsumoId,
		i.Unidad AS InsumoUnidad,
		i.Codigo AS InsumoCodigo,
		dr.Valor AS Valor,
		dr.Variacion AS Variacion
		FROM DetalleReceta dr
		JOIN RecetaVersion rv ON dr.RecetaVersionId = rv.RecetaVersionId
		JOIN Receta r ON rv.RecetaId = r.RecetaId
		JOIN Insumo i ON dr.InsumoId = i.InsumoId
		WHERE rv.RecetaId = @RecetaId
			AND rv.Estado = 1
			AND dr.Valor > 0
END
GO

IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GuardarRegistroBatchWarehouse')
	DROP PROCEDURE sp_GuardarRegistroBatchWarehouse
GO
CREATE PROCEDURE sp_GuardarRegistroBatchWarehouse
	@Lote					INT,
	@FormulaId				INT,
	@RecetaVersionId		INT,
	@InsumoId				INT,
	@ValorSetpoint			DECIMAL(10,4),
	@Variacion				DECIMAL(5,2),
	@RegistroPesoCodigo		VARCHAR(20),
	@FechaPreparacion		DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @RegistroPesoId	INT;

	-- Obtener el RegistroPesoId a partir del código proporcionado
	SELECT @RegistroPesoId = RegistroPesoId
	FROM RegistroPeso
	WHERE Codigo = @RegistroPesoCodigo;

	-- Insertar el nuevo registro en RegistroBatchWarehouse
	INSERT INTO RegistroBatchWarehouse (Lote, FormulaId, RecetaVersionId, InsumoId, ValorSetpoint, Variacion, RegistroPesoId, FechaPreparacion)
	VALUES (@Lote, @FormulaId, @RecetaVersionId, @InsumoId, @ValorSetpoint, @Variacion, @RegistroPesoId, @FechaPreparacion);

	-- Marcar el registro de peso como utilizado
	UPDATE RegistroPeso
	SET Estado = 0
	WHERE RegistroPesoId = @RegistroPesoId;

	RETURN 1
END
GO

-- ====================================================================================================================================
-- Creación de vistas
-- ====================================================================================================================================
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