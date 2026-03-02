/*
================================================================
專案名稱：烘焙電商社群平台
腳本用途：初始化資料庫與建立模組 Schema
執行順序：1
================================================================
*/

USE master;
GO

-- 1. 建立資料庫
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Bake')
BEGIN
    CREATE DATABASE Bake;
END
GO

USE Bake;
GO

-- 2. 建立模組 Schema (邏輯分層)
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Platform') EXEC('CREATE SCHEMA Platform');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = '[User]') EXEC('CREATE SCHEMA [User]');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Sales') EXEC('CREATE SCHEMA Sales');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Social') EXEC('CREATE SCHEMA Social');
GO
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Service') EXEC('CREATE SCHEMA Service');
GO

PRINT '資料庫與 Schema 初始化完成！';