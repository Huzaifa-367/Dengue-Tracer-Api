
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/06/2023 22:42:50
-- Generated from EDMX file: E:\FYP\Dengue-Tracer-Api\FYP_Api\Models\Project.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Project];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[ACTION_LOGS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ACTION_LOGS];
GO
IF OBJECT_ID(N'[dbo].[ASSIGNSECTORS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ASSIGNSECTORS];
GO
IF OBJECT_ID(N'[dbo].[CASES_LOGS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CASES_LOGS];
GO
IF OBJECT_ID(N'[dbo].[NOTIFICATIONs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NOTIFICATIONs];
GO
IF OBJECT_ID(N'[dbo].[POLYGONS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[POLYGONS];
GO
IF OBJECT_ID(N'[dbo].[SECTORS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SECTORS];
GO
IF OBJECT_ID(N'[dbo].[USERs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[USERs];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'ACTION_LOGS'
CREATE TABLE [dbo].[ACTION_LOGS] (
    [action_id] int IDENTITY(1,1) NOT NULL,
    [sec_id] int  NULL,
    [date] datetime  NULL,
    [act_status] bit  NULL
);
GO

-- Creating table 'ASSIGNSECTORS'
CREATE TABLE [dbo].[ASSIGNSECTORS] (
    [user_id] int  NULL,
    [sec_id] int  NULL,
    [ass_id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'CASES_LOGS'
CREATE TABLE [dbo].[CASES_LOGS] (
    [case_id] int IDENTITY(1,1) NOT NULL,
    [user_id] int  NULL,
    [status] bit  NULL,
    [startdate] datetime  NULL,
    [enddate] datetime  NULL
);
GO

-- Creating table 'NOTIFICATIONs'
CREATE TABLE [dbo].[NOTIFICATIONs] (
    [notif_id] int IDENTITY(1,1) NOT NULL,
    [case_id] int  NULL,
    [title] varchar(30)  NULL,
    [type] bit  NULL,
    [date] datetime  NULL
);
GO

-- Creating table 'POLYGONS'
CREATE TABLE [dbo].[POLYGONS] (
    [poly_id] int IDENTITY(1,1) NOT NULL,
    [sec_id] int  NULL,
    [lat_long] varchar(max)  NULL
);
GO

-- Creating table 'SECTORS'
CREATE TABLE [dbo].[SECTORS] (
    [sec_id] int IDENTITY(1,1) NOT NULL,
    [sec_name] varchar(50)  NULL,
    [threshold] int  NULL,
    [description] varchar(max)  NULL
);
GO

-- Creating table 'USERs'
CREATE TABLE [dbo].[USERs] (
    [user_id] int IDENTITY(1,1) NOT NULL,
    [sec_id] int  NULL,
    [name] varchar(20)  NULL,
    [email] varchar(40)  NULL,
    [phone_number] varchar(20)  NULL,
    [password] varchar(20)  NULL,
    [role] varchar(10)  NULL,
    [image] varchar(max)  NULL,
    [home_location] varchar(max)  NULL,
    [office_location] varchar(max)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [action_id] in table 'ACTION_LOGS'
ALTER TABLE [dbo].[ACTION_LOGS]
ADD CONSTRAINT [PK_ACTION_LOGS]
    PRIMARY KEY CLUSTERED ([action_id] ASC);
GO

-- Creating primary key on [ass_id] in table 'ASSIGNSECTORS'
ALTER TABLE [dbo].[ASSIGNSECTORS]
ADD CONSTRAINT [PK_ASSIGNSECTORS]
    PRIMARY KEY CLUSTERED ([ass_id] ASC);
GO

-- Creating primary key on [case_id] in table 'CASES_LOGS'
ALTER TABLE [dbo].[CASES_LOGS]
ADD CONSTRAINT [PK_CASES_LOGS]
    PRIMARY KEY CLUSTERED ([case_id] ASC);
GO

-- Creating primary key on [notif_id] in table 'NOTIFICATIONs'
ALTER TABLE [dbo].[NOTIFICATIONs]
ADD CONSTRAINT [PK_NOTIFICATIONs]
    PRIMARY KEY CLUSTERED ([notif_id] ASC);
GO

-- Creating primary key on [poly_id] in table 'POLYGONS'
ALTER TABLE [dbo].[POLYGONS]
ADD CONSTRAINT [PK_POLYGONS]
    PRIMARY KEY CLUSTERED ([poly_id] ASC);
GO

-- Creating primary key on [sec_id] in table 'SECTORS'
ALTER TABLE [dbo].[SECTORS]
ADD CONSTRAINT [PK_SECTORS]
    PRIMARY KEY CLUSTERED ([sec_id] ASC);
GO

-- Creating primary key on [user_id] in table 'USERs'
ALTER TABLE [dbo].[USERs]
ADD CONSTRAINT [PK_USERs]
    PRIMARY KEY CLUSTERED ([user_id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------