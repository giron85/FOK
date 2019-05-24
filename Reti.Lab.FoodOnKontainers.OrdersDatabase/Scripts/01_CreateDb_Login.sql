﻿-- CREATE DATABASE
CREATE DATABASE [Fok_Orders]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Fok_Orders', FILENAME = N'/var/opt/mssql/data/Fok_Orders.mdf' , SIZE = 8192KB , FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'Fok_Orders_log', FILENAME = N'/var/opt/mssql/data/Fok_Orders_log.ldf' , SIZE = 8192KB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [Fok_Orders] SET COMPATIBILITY_LEVEL = 140
GO
ALTER DATABASE [Fok_Orders] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Fok_Orders] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Fok_Orders] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Fok_Orders] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Fok_Orders] SET ARITHABORT OFF 
GO
ALTER DATABASE [Fok_Orders] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Fok_Orders] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Fok_Orders] SET AUTO_CREATE_STATISTICS ON(INCREMENTAL = OFF)
GO
ALTER DATABASE [Fok_Orders] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Fok_Orders] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Fok_Orders] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Fok_Orders] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Fok_Orders] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Fok_Orders] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Fok_Orders] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Fok_Orders] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Fok_Orders] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Fok_Orders] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Fok_Orders] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Fok_Orders] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Fok_Orders] SET  READ_WRITE 
GO
ALTER DATABASE [Fok_Orders] SET RECOVERY FULL 
GO
ALTER DATABASE [Fok_Orders] SET  MULTI_USER 
GO
ALTER DATABASE [Fok_Orders] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Fok_Orders] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [Fok_Orders] SET DELAYED_DURABILITY = DISABLED 
GO
USE [Fok_Orders]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = Off;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = Primary;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = On;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = Primary;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = Off;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = Primary;
GO
USE [Fok_Orders]
GO
IF NOT EXISTS (SELECT name FROM sys.filegroups WHERE is_default=1 AND name = N'PRIMARY') ALTER DATABASE [Fok_Orders] MODIFY FILEGROUP [PRIMARY] DEFAULT
GO

USE [master]
GO
CREATE LOGIN [orderSvc] WITH PASSWORD=N'PasswordOrder01!', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
use [Fok_Orders]

-- LOGIN
GO
USE [Fok_Orders]
GO
CREATE USER [orderSvc] FOR LOGIN [orderSvc]
GO
USE [Fok_Orders]
GO
ALTER ROLE [db_datareader] ADD MEMBER [orderSvc]
GO
USE [Fok_Orders]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [orderSvc]
GO
