-- ====================================================================================================================================
-- Script de Creación de Base de Datos
-- Sistema de Balanzas
-- Autor: Juan Armando Joyo Taype
-- Fecha: 2025/11/20
-- Versión: 1.1.0
-- Descripción: Script para la creación de procedimientos almacenados.
-- ====================================================================================================================================

-- ====================================================================================================================================
-- View window : vw_RegistroPesoDataGrid
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

-- ====================================================================================================================================
-- View window : vw_RegistroPesoActivoDataGrid
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- View window : vw_GetAllRecetasActivas
-- ====================================================================================================================================
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

-- ====================================================================================================================================
-- View window : vw_ObtenerRegistrosBatchWarehouse
-- ====================================================================================================================================
IF EXISTS (SELECT * FROM sys. views WHERE name = 'vw_ObtenerRegistrosBatchWarehouse')
	DROP VIEW vw_ObtenerRegistrosBatchWarehouse;
GO

CREATE VIEW vw_ObtenerRegistrosBatchWarehouse
AS
SELECT
	rb.RegistroBatchWarehouseId AS RegistroBatchWarehouseId,
	rb.Lote AS Lote,
	rb.FormulaId AS FormulaId,
	f.Codigo AS FormulaCodigo,
	r.RecetaId AS RecetaId,
	r.Codigo AS RecetaCodigo,
	rb.InsumoId AS InsumoId,
	rb.Usuario AS Usuario,
	i.Codigo AS InsumoCodigo,
	rb.ValorSetpoint AS DetalleRecetaValor,
	i.Unidad AS InsumoUnidad,
	rb.Variacion AS DetalleRecetaVariacion,
	rp.RegistroPesoId AS RegistroPesoId,
	rp.Codigo AS RegistroPesoCodigo,
	rp.Valor AS RegistroPesoValor,
	rb.ValorReal AS RegistroBatchWarehouseValorReal,
	rb.FechaPreparacion AS FechaPreparacion
FROM RegistroBatchWarehouse rb
JOIN Formula f ON rb.FormulaId = f.FormulaId
JOIN RecetaVersion rv ON rb.RecetaVersionId = rv.RecetaVersionId
JOIN Receta r ON rv.RecetaId = r.RecetaId
JOIN Insumo i ON rb.InsumoId = i.InsumoId
JOIN RegistroPeso rp ON rb.RegistroPesoId = rp.RegistroPesoId;
GO


