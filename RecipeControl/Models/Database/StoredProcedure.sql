-- ====================================================================================================================================
-- Script de Creación de Base de Datos
-- Sistema de Balanzas
-- Autor: Juan Armando Joyo Taype
-- Fecha: 2025/11/20
-- Versión: 1.1.0
-- Descripción: Script para la creación de procedimientos almacenados.
-- ====================================================================================================================================

USE REPRECIPE;
GO

-- ====================================================================================================================================
-- Stored Procedure: sp_InsertarNuevoRegistroPeso
-- ====================================================================================================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_InsertarNuevoRegistroPeso')
	DROP PROCEDURE sp_InsertarNuevoRegistroPeso;
GO

CREATE PROCEDURE sp_InsertarNuevoRegistroPeso
	@RecetaVersionId	INT,
	@InsumoId			INT,
	@BalanzaId			INT,
	@UsuarioId			INT,
	@FechaPesado		DATETIME,
	@CantidadPesada		DECIMAL(10,4)
AS
BEGIN
	SET NOCOUNT ON;

	-- Variables para generar el código del nuevo registro de peso
	DECLARE @CodigoInsumo VARCHAR(20);
	DECLARE @NuevoId INT;
	DECLARE @Codigo VARCHAR(20);

	-- Obtener el código del insumo
	SELECT @CodigoInsumo = Codigo 
	FROM Insumo 
	WHERE InsumoId = @InsumoId;

	-- Obetener el siguiente registro de peso
	SELECT @NuevoId = ISNULL(COUNT(RegistroPesoId), 0) + 1
	FROM RegistroPeso
	WHERE InsumoId = @InsumoId;

	-- Generar el código del nuevo registro de peso
	SET @Codigo = @CodigoInsumo + '-' + RIGHT('000000' + CAST(@NuevoId AS VARCHAR(6)), 6);

	INSERT INTO RegistroPeso (Codigo, RecetaVersionId, InsumoId, BalanzaId, UsuarioId, FechaPesado, CantidadPesada, UsuarioCreacionId, UsuarioModificacionId)
	OUTPUT INSERTED.*
	VALUES (@Codigo, @RecetaVersionId, @InsumoId, @BalanzaId, @UsuarioId, @FechaPesado, @CantidadPesada, @UsuarioId, @UsuarioId)
END
GO

-- ====================================================================================================================================
-- Stored Procedure: sp_GetInsumosPorRecetaYTipo
-- ====================================================================================================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetInsumosPorRecetaYTipo')
	DROP PROCEDURE sp_GetInsumosPorRecetaYTipo
GO

CREATE PROCEDURE sp_GetInsumosPorRecetaYTipo
	@RecetaVersionId	INT,
	@TipoInsumoId		INT
AS
BEGIN
	SET NOCOUNT ON;

	-- CTE para filtrar los RecetaVersionDetalle por RecetaVersionId
	WITH RecetaVersionDetalleTable AS
	(
		SELECT
			RecetaVersionDetalleId,
			InsumoId
		FROM RecetaVersionDetalle
		WHERE RecetaVersionId = @RecetaVersionId
	)

	-- Seleccionar los insumos que coinciden con el TipoInsumoId
	SELECT 
		i.InsumoId AS InsumoId,
		i.Codigo AS Codigo,
		i.Descripcion AS Descripcion,
		i.TipoInsumoId AS TipoInsumoId,
		i.Unidad AS Unidad,
		i.FechaCreacion AS FechaCreacion,
		i.FechaModificacion AS FechaModificacion
	FROM RecetaVersionDetalleTable d
	INNER JOIN Insumo i ON d.InsumoId = i.InsumoId
	WHERE i.TipoInsumoId = @TipoInsumoId;
	
END
GO

-- ====================================================================================================================================
-- Stored Procedure: sp_GetRegistrosDisponiblesPorCodigo
-- ====================================================================================================================================
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
		rp.CantidadPesada AS RegistroPesoCantidadPesada,
		i.Unidad AS InsumoUnidad
	FROM RegistroPeso rp
	JOIN Insumo i ON rp.InsumoId = i.InsumoId
	WHERE i.Codigo = @InsumoCodigo
		AND rp.Estado = 1
	ORDER BY rp.FechaPesado DESC;
END
GO

-- ====================================================================================================================================
-- Stored Procedure: sp_LeerDetalleRecetaMinimo
-- ====================================================================================================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_LeerDetalleRecetaMinimo')
	DROP PROCEDURE sp_LeerDetalleRecetaMinimo
GO
CREATE PROCEDURE sp_LeerDetalleRecetaMinimo
	@RecetaId	INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		rvd.RecetaVersionId AS RecetaVersionId,
		r.RecetaId AS RecetaId,
		r.Codigo AS RecetaCodigo,
		i.InsumoId AS InsumoId,
		i.Unidad AS InsumoUnidad,
		i.Codigo AS InsumoCodigo,
		rvd.CantidadRequerida AS CantidadRequerida,
		rvd.ToleranciaMaxima AS ToleranciaMaxima
		FROM RecetaVersionDetalle rvd
		JOIN RecetaVersion rv ON dr.RecetaVersionId = rv.RecetaVersionId
		JOIN Receta r ON rv.RecetaId = r.RecetaId
		JOIN Insumo i ON dr.InsumoId = i.InsumoId
		WHERE rv.RecetaId = @RecetaId
			AND rv.Estado = 1
			AND dr.Valor > 0
END
GO

-- ====================================================================================================================================
-- Stored Procedure: sp_GuardarRegistroBatch
-- ====================================================================================================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GuardarRegistroBatch')
	DROP PROCEDURE sp_GuardarRegistroBatch
GO
CREATE PROCEDURE sp_GuardarRegistroBatch
	@Lote					INT,
	@FormulaId				INT,
	@RecetaVersionId		INT,
	@InsumoId				INT,
	@Usuario				VARCHAR(20),
	@ValorSetpoint			DECIMAL(10,4),
	@Variacion				DECIMAL(5,2),
	@ValorReal				DECIMAL(10,4),
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
	INSERT INTO RegistroBatchWarehouse (Lote, FormulaId, RecetaVersionId, InsumoId, Usuario, ValorSetpoint, Variacion, ValorReal, RegistroPesoId, FechaPreparacion)
	VALUES (@Lote, @FormulaId, @RecetaVersionId, @InsumoId, @Usuario, @ValorSetpoint, @Variacion, @ValorReal, @RegistroPesoId, @FechaPreparacion);

	-- Marcar el registro de peso como utilizado
	UPDATE RegistroPeso
	SET Estado = 0
	WHERE RegistroPesoId = @RegistroPesoId;

	RETURN 1
END
GO