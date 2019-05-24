﻿USE [FOK.User]
GO

/****** Object:  Table [dbo].[Favorite]    Script Date: 24/05/2019 14:55:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Favorite](
	[RestaurantId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_Favorite] PRIMARY KEY CLUSTERED 
(
	[RestaurantId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

