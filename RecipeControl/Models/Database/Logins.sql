-- ====================================================================================================================================
-- Script de Creación de Base de Datos
-- Sistema de Balanzas
-- Autor: Juan Armando Joyo Taype
-- Fecha: 2025/11/20
-- Versión: 1.1.0
-- Descripción: Script para la creación de usuarios.
-- ====================================================================================================================================

USE MASTER;
GO

-- ====================================================================================================================================
-- Login: Login creation
-- ====================================================================================================================================

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