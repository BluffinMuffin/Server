ALTER TABLE [TableParams] DROP CONSTRAINT [FK_TableParams_Servers]
GO
ALTER TABLE [TableParams] DROP CONSTRAINT [FK_TableParams_Control.LobbyTypes]
GO
ALTER TABLE [TableParams] DROP CONSTRAINT [FK_TableParams_Control.LimitTypes]
GO
ALTER TABLE [TableParams] DROP CONSTRAINT [FK_TableParams_Control.GameSubTypes]
GO
ALTER TABLE [TableParams] DROP CONSTRAINT [FK_TableParams_Control.BlindTypes]
GO
ALTER TABLE [Games] DROP CONSTRAINT [FK_Games_TableParams]
GO
ALTER TABLE [Control.GameSubTypes] DROP CONSTRAINT [FK_Control.GameSubTypes_Control.GameTypes]
GO
ALTER TABLE [Commands] DROP CONSTRAINT [FK_Commands_Servers]
GO
ALTER TABLE [Commands] DROP CONSTRAINT [FK_Commands_Games]
GO
ALTER TABLE [Commands] DROP CONSTRAINT [FK_Commands_Clients]
GO
/****** Object:  Table [TableParams]    Script Date: 2015-09-20 16:54:56 ******/
DROP TABLE [TableParams]
GO
/****** Object:  Table [Servers]    Script Date: 2015-09-20 16:54:56 ******/
DROP TABLE [Servers]
GO
/****** Object:  Table [Games]    Script Date: 2015-09-20 16:54:56 ******/
DROP TABLE [Games]
GO
/****** Object:  Table [Control.LobbyTypes]    Script Date: 2015-09-20 16:54:56 ******/
DROP TABLE [Control.LobbyTypes]
GO
/****** Object:  Table [Control.LimitTypes]    Script Date: 2015-09-20 16:54:56 ******/
DROP TABLE [Control.LimitTypes]
GO
/****** Object:  Table [Control.GameTypes]    Script Date: 2015-09-20 16:54:56 ******/
DROP TABLE [Control.GameTypes]
GO
/****** Object:  Table [Control.GameSubTypes]    Script Date: 2015-09-20 16:54:56 ******/
DROP TABLE [Control.GameSubTypes]
GO
/****** Object:  Table [Control.BlindTypes]    Script Date: 2015-09-20 16:54:56 ******/
DROP TABLE [Control.BlindTypes]
GO
/****** Object:  Table [Commands]    Script Date: 2015-09-20 16:54:56 ******/
DROP TABLE [Commands]
GO
/****** Object:  Table [Clients]    Script Date: 2015-09-20 16:54:56 ******/
DROP TABLE [Clients]
GO
/****** Object:  Table [Clients]    Script Date: 2015-09-20 16:54:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Clients](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClientIdentification] [nvarchar](200) NULL,
	[ImplementedProtocol] [nvarchar](20) NULL,
	[ClientStartedAt] [datetime] NOT NULL,
	[Hostname] [nvarchar](200) NOT NULL,
	[DisplayName] [nvarchar](200) NULL,
 CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Commands]    Script Date: 2015-09-20 16:54:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Commands](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExecutionTime] [datetime] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[IsFromServer] [bit] NOT NULL,
	[ServerId] [int] NOT NULL,
	[ClientId] [int] NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[GameId] [int] NULL,
	[Detail] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Commands] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [Control.BlindTypes]    Script Date: 2015-09-20 16:54:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Control.BlindTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](200) NULL,
 CONSTRAINT [PK_Control.BlindTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Control.GameSubTypes]    Script Date: 2015-09-20 16:54:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Control.GameSubTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GameTypeId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](200) NULL,
 CONSTRAINT [PK_Control.GameSubTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Control.GameTypes]    Script Date: 2015-09-20 16:54:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Control.GameTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](200) NULL,
 CONSTRAINT [PK_Control.GameTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Control.LimitTypes]    Script Date: 2015-09-20 16:54:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Control.LimitTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](200) NULL,
 CONSTRAINT [PK_Control.LimitTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Control.LobbyTypes]    Script Date: 2015-09-20 16:54:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Control.LobbyTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](200) NULL,
 CONSTRAINT [PK_Control.LobbyTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Games]    Script Date: 2015-09-20 16:54:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Games](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TableParamId] [int] NOT NULL,
	[GameStartedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_Games] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [Servers]    Script Date: 2015-09-20 16:54:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [Servers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ServerIdentification] [nvarchar](200) NOT NULL,
	[ImplementedProtocol] [nvarchar](20) NOT NULL,
	[ServerStartedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_Servers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [TableParams]    Script Date: 2015-09-20 16:54:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TableParams](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TableName] [nvarchar](200) NOT NULL,
	[GameSubTypeId] [int] NOT NULL,
	[MinPlayerToStart] [int] NOT NULL,
	[MaxPlayer] [int] NOT NULL,
	[Arguments] [nvarchar](2000) NULL,
	[LobbyTypeId] [int] NOT NULL,
	[BlindTypeId] [int] NOT NULL,
	[LimitTypeId] [int] NOT NULL,
	[ServerId] [int] NOT NULL,
	[TableStartedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_TableParams] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [Commands]  WITH CHECK ADD  CONSTRAINT [FK_Commands_Clients] FOREIGN KEY([ClientId])
REFERENCES [Clients] ([Id])
GO
ALTER TABLE [Commands] CHECK CONSTRAINT [FK_Commands_Clients]
GO
ALTER TABLE [Commands]  WITH CHECK ADD  CONSTRAINT [FK_Commands_Games] FOREIGN KEY([GameId])
REFERENCES [Games] ([Id])
GO
ALTER TABLE [Commands] CHECK CONSTRAINT [FK_Commands_Games]
GO
ALTER TABLE [Commands]  WITH CHECK ADD  CONSTRAINT [FK_Commands_Servers] FOREIGN KEY([ServerId])
REFERENCES [Servers] ([Id])
GO
ALTER TABLE [Commands] CHECK CONSTRAINT [FK_Commands_Servers]
GO
ALTER TABLE [Control.GameSubTypes]  WITH CHECK ADD  CONSTRAINT [FK_Control.GameSubTypes_Control.GameTypes] FOREIGN KEY([GameTypeId])
REFERENCES [Control.GameTypes] ([Id])
GO
ALTER TABLE [Control.GameSubTypes] CHECK CONSTRAINT [FK_Control.GameSubTypes_Control.GameTypes]
GO
ALTER TABLE [Games]  WITH CHECK ADD  CONSTRAINT [FK_Games_TableParams] FOREIGN KEY([TableParamId])
REFERENCES [TableParams] ([Id])
GO
ALTER TABLE [Games] CHECK CONSTRAINT [FK_Games_TableParams]
GO
ALTER TABLE [TableParams]  WITH CHECK ADD  CONSTRAINT [FK_TableParams_Control.BlindTypes] FOREIGN KEY([BlindTypeId])
REFERENCES [Control.BlindTypes] ([Id])
GO
ALTER TABLE [TableParams] CHECK CONSTRAINT [FK_TableParams_Control.BlindTypes]
GO
ALTER TABLE [TableParams]  WITH CHECK ADD  CONSTRAINT [FK_TableParams_Control.GameSubTypes] FOREIGN KEY([GameSubTypeId])
REFERENCES [Control.GameSubTypes] ([Id])
GO
ALTER TABLE [TableParams] CHECK CONSTRAINT [FK_TableParams_Control.GameSubTypes]
GO
ALTER TABLE [TableParams]  WITH CHECK ADD  CONSTRAINT [FK_TableParams_Control.LimitTypes] FOREIGN KEY([LimitTypeId])
REFERENCES [Control.LimitTypes] ([Id])
GO
ALTER TABLE [TableParams] CHECK CONSTRAINT [FK_TableParams_Control.LimitTypes]
GO
ALTER TABLE [TableParams]  WITH CHECK ADD  CONSTRAINT [FK_TableParams_Control.LobbyTypes] FOREIGN KEY([LobbyTypeId])
REFERENCES [Control.LobbyTypes] ([Id])
GO
ALTER TABLE [TableParams] CHECK CONSTRAINT [FK_TableParams_Control.LobbyTypes]
GO
ALTER TABLE [TableParams]  WITH CHECK ADD  CONSTRAINT [FK_TableParams_Servers] FOREIGN KEY([ServerId])
REFERENCES [Servers] ([Id])
GO
ALTER TABLE [TableParams] CHECK CONSTRAINT [FK_TableParams_Servers]
GO
