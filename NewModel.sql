USE [master]
GO
/****** Object:  Database [NewModel]    Script Date: 16/04/2021 09:44:49 AM ******/
CREATE DATABASE [NewModel]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'NewModel', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\NewModel.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'NewModel_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\NewModel_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [NewModel] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [NewModel].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [NewModel] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [NewModel] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [NewModel] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [NewModel] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [NewModel] SET ARITHABORT OFF 
GO
ALTER DATABASE [NewModel] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [NewModel] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [NewModel] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [NewModel] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [NewModel] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [NewModel] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [NewModel] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [NewModel] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [NewModel] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [NewModel] SET  DISABLE_BROKER 
GO
ALTER DATABASE [NewModel] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [NewModel] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [NewModel] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [NewModel] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [NewModel] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [NewModel] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [NewModel] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [NewModel] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [NewModel] SET  MULTI_USER 
GO
ALTER DATABASE [NewModel] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [NewModel] SET DB_CHAINING OFF 
GO
ALTER DATABASE [NewModel] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [NewModel] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [NewModel] SET DELAYED_DURABILITY = DISABLED 
GO
USE [NewModel]
GO
/****** Object:  UserDefinedFunction [dbo].[AddWorkingDays]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[AddWorkingDays]
(
	-- Add the parameters for the function here
	@date DATE,
	@days INT
)
RETURNS DATE
AS
BEGIN
	DECLARE @result DATE
	DECLARE @isWorkingDay BIT = 0
	DECLARE @cnt INT = 0
	DECLARE @absoluteDays INT
	DECLARE @integerFactor SMALLINT = 1

	SET @result = @date

	IF @days < 0
		 SET @integerFactor = -1
	SELECT @absoluteDays = @days * @integerFactor

	WHILE @cnt < @absoluteDays
	BEGIN
		SET @isWorkingDay = 0
		WHILE @isWorkingDay = 0
		BEGIN
			SELECT @result = DATEADD(DAY,@integerFactor,@result)
			IF DATEPART(dw,@result) IN (6,7) OR EXISTS (SELECT 1 FROM Holidays where date = @result AND active = 1)
				SET @isWorkingDay = 0
			ELSE
				SET @isWorkingDay = 1
		END
		SET @cnt = @cnt + 1
	END
	
	RETURN @result

END





GO
/****** Object:  UserDefinedFunction [dbo].[DiffWorkingDays]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DiffWorkingDays]
(
	@dateIni DATE, --Fecha inicial, debe ser menor
	@dateEnd DATE  --Fecha final, debe ser mayor
)
RETURNS INT
AS
BEGIN
	DECLARE @result INT
	IF @dateIni < @dateEnd
	BEGIN

		DECLARE @days INT = 0
		DECLARE @holidays INT = 0
		SELECT @days = DATEDIFF(HOUR,@dateIni,@dateEnd)

		SELECT @holidays = COUNT(*) FROM Holidays WHERE date BETWEEN @dateIni AND @dateEnd AND active = 1
		SET @result = @days - (@holidays*24)
		-- Esto puede ocurrir cuando la fecha esta inicial y final son o festivos o no laborales.
		IF @result < 0
			SET @result = 0
	END
	RETURN @result

END




GO
/****** Object:  UserDefinedFunction [dbo].[isWorkingDay]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[isWorkingDay]
(
	@date DATE
)
RETURNS BIT
AS
BEGIN
	DECLARE @result BIT = 1
	
	IF DATEPART(dw,@date) IN (6,7) OR EXISTS (SELECT 1 FROM Holidays where date = @date AND active = 1)
		SET @result = 0
	
	RETURN @result
END





GO
/****** Object:  UserDefinedFunction [dbo].[Split]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[Split] (
      @InputString                  VARCHAR(8000),
      @Delimiter                    VARCHAR(50)
)

RETURNS @Items TABLE (
      Item                          VARCHAR(8000)
)

AS
BEGIN
      IF @Delimiter = ' '
      BEGIN
            SET @Delimiter = ','
            SET @InputString = REPLACE(@InputString, ' ', @Delimiter)
      END

      IF (@Delimiter IS NULL OR @Delimiter = '')
            SET @Delimiter = ','

--INSERT INTO @Items VALUES (@Delimiter) -- Diagnostic
--INSERT INTO @Items VALUES (@InputString) -- Diagnostic

      DECLARE @Item           VARCHAR(8000)
      DECLARE @ItemList       VARCHAR(8000)
      DECLARE @DelimIndex     INT

      SET @ItemList = @InputString
      SET @DelimIndex = CHARINDEX(@Delimiter, @ItemList, 0)
      WHILE (@DelimIndex != 0)
      BEGIN
            SET @Item = SUBSTRING(@ItemList, 0, @DelimIndex)
            INSERT INTO @Items VALUES (@Item)

            -- Set @ItemList = @ItemList minus one less item
            SET @ItemList = SUBSTRING(@ItemList, @DelimIndex+1, LEN(@ItemList)-@DelimIndex)
            SET @DelimIndex = CHARINDEX(@Delimiter, @ItemList, 0)
      END -- End WHILE

      IF @Item IS NOT NULL -- At least one delimiter was encountered in @InputString
      BEGIN
            SET @Item = @ItemList
            INSERT INTO @Items VALUES (@Item)
      END

      -- No delimiters were encountered in @InputString, so just return @InputString
      ELSE INSERT INTO @Items VALUES (@InputString)

      RETURN

END -- End Function





GO
/****** Object:  Table [dbo].[Assistants]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Assistants](
	[IdAssistant] [tinyint] IDENTITY(1,1) NOT NULL,
	[CodAssistant] [varchar](50) NOT NULL,
	[NameAssistant] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Assistants] PRIMARY KEY CLUSTERED 
(
	[IdAssistant] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AuditDB]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AuditDB](
	[IdAuditDB] [int] IDENTITY(1,1) NOT NULL,
	[IdUser] [tinyint] NOT NULL,
	[TableNameAuditDB] [varchar](100) NOT NULL,
	[DateOperationAuditDB] [datetime2](2) NOT NULL,
	[OperationTypeAuditDB] [varchar](50) NOT NULL,
	[OldValueAuditDB] [varchar](max) NULL,
	[NewValueAuditDB] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AuditTickets]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AuditTickets](
	[IdAuditTicket] [int] IDENTITY(1,1) NOT NULL,
	[IdTicket] [varchar](50) NOT NULL,
	[IdUser] [tinyint] NOT NULL,
	[StartDateAuditTicket] [datetime2](2) NOT NULL,
	[EndDateAuditTicket] [datetime2](2) NOT NULL,
	[StartStateAuditTicket] [tinyint] NOT NULL,
	[EndStateAuditTicket] [tinyint] NOT NULL,
	[DescriptionAuditTicket] [varchar](500) NOT NULL,
 CONSTRAINT [PK__AuditTic__FC3F24C1332A3024] PRIMARY KEY CLUSTERED 
(
	[IdAuditTicket] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ChangeLog]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChangeLog](
	[LogId] [int] NOT NULL,
	[DatabaseName] [nvarchar](256) NOT NULL,
	[EventType] [nvarchar](50) NOT NULL,
	[ObjectName] [nvarchar](256) NOT NULL,
	[ObjectType] [nvarchar](25) NOT NULL,
	[SqlCommand] [nvarchar](max) NOT NULL,
	[EventDate] [datetime] NOT NULL,
	[LoginName] [nvarchar](256) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmailParameters]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmailParameters](
	[IdEmailParameter] [tinyint] IDENTITY(1,1) NOT NULL,
	[CodEmailParameter] [varchar](50) NOT NULL,
	[TOEmailParameter] [varchar](max) NOT NULL,
	[CCEmailParameter] [varchar](max) NULL,
	[BCCEmailParameter] [varchar](max) NULL,
	[SubjectEmailParameter] [varchar](250) NOT NULL,
	[BodyEmailParameter] [varchar](max) NOT NULL,
	[IsHTMLEmailParameter] [bit] NOT NULL,
	[IdAssistant] [tinyint] NOT NULL,
 CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED 
(
	[IdEmailParameter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Holidays]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Holidays](
	[DateHoliday] [date] NOT NULL,
	[ActiveHoliday] [bit] NOT NULL,
 CONSTRAINT [PK_Holidays] PRIMARY KEY CLUSTERED 
(
	[DateHoliday] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProcessTime]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProcessTime](
	[IdProcessTime] [tinyint] IDENTITY(1,1) NOT NULL,
	[CodProcessTime] [varchar](50) NOT NULL,
	[NameProcessTime] [varchar](50) NOT NULL,
	[LastExecutionProcessTime] [datetime2](2) NOT NULL,
	[WaitTimeProcessTime] [int] NOT NULL,
 CONSTRAINT [PK_ProcessTime] PRIMARY KEY CLUSTERED 
(
	[IdProcessTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Roles](
	[IdRole] [tinyint] IDENTITY(1,1) NOT NULL,
	[CodRole] [varchar](50) NOT NULL,
	[NameRole] [varchar](50) NOT NULL,
	[DescriptionRole] [varchar](250) NOT NULL,
	[IdAssistant] [tinyint] NOT NULL,
	[RolId] [int] NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[IdRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RpaParameters]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RpaParameters](
	[IdParameter] [tinyint] IDENTITY(1,1) NOT NULL,
	[CodParameter] [varchar](50) NOT NULL,
	[NameParameter] [varchar](50) NOT NULL,
	[ValueParameter] [varchar](max) NULL,
	[DescriptionParameter] [varchar](255) NULL,
	[IsVisibleParameter] [bit] NOT NULL,
	[IdAssistant] [tinyint] NOT NULL,
 CONSTRAINT [PK_Parameters] PRIMARY KEY CLUSTERED 
(
	[IdParameter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[States]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[States](
	[IdState] [tinyint] IDENTITY(1,1) NOT NULL,
	[CodState] [varchar](50) NOT NULL,
	[NameState] [varchar](50) NOT NULL,
	[IdAssistant] [tinyint] NOT NULL,
 CONSTRAINT [PK_States] PRIMARY KEY CLUSTERED 
(
	[IdState] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tickets]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tickets](
	[IdTicket] [varchar](50) NOT NULL,
	[IdState] [tinyint] NOT NULL,
	[CreationDate] [datetime2](2) NOT NULL CONSTRAINT [DF_Tickets_CreationDate]  DEFAULT (getdate()),
	[Locked] [bit] NOT NULL,
	[FinalDate] [datetime2](2) NULL,
	[ExecutionDate] [datetime2](2) NULL,
	[Priority] [bit] NULL,
 CONSTRAINT [PK_Requests3] PRIMARY KEY CLUSTERED 
(
	[IdTicket] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TicketsUsers]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TicketsUsers](
	[IdTicket] [varchar](50) NOT NULL,
	[IdUser] [tinyint] NOT NULL,
	[DateInsertTicketUser] [datetime2](2) NOT NULL,
 CONSTRAINT [PK_TicketsUsers] PRIMARY KEY CLUSTERED 
(
	[IdTicket] ASC,
	[IdUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[IdUser] [tinyint] IDENTITY(1,1) NOT NULL,
	[NameUser] [varchar](50) NOT NULL,
	[ActiveUser] [bit] NOT NULL CONSTRAINT [DF_Users_Active]  DEFAULT ((1)),
	[ComputerNameUser] [varchar](100) NOT NULL CONSTRAINT [DF_Users_ComputerName]  DEFAULT (host_name()),
	[IdType] [tinyint] NOT NULL,
	[FullName] [varchar](100) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[IdUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UsersAssistants]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersAssistants](
	[IdUser] [tinyint] NOT NULL,
	[IdAssistant] [tinyint] NOT NULL,
	[ActiveUserAssistant] [bit] NOT NULL,
 CONSTRAINT [PK_UsersAssistants] PRIMARY KEY CLUSTERED 
(
	[IdUser] ASC,
	[IdAssistant] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersRoles]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UsersRoles](
	[IdUser] [tinyint] NOT NULL,
	[IdRole] [tinyint] NOT NULL,
	[MaxIterationsUserRole] [smallint] NOT NULL CONSTRAINT [DF_UsersRoles_MaxIterations]  DEFAULT ((1)),
	[ActiveUserRole] [bit] NOT NULL CONSTRAINT [DF_UsersRoles_Active]  DEFAULT ((1)),
	[RunnerName] [varchar](10) NULL,
 CONSTRAINT [PK_UsersRoles] PRIMARY KEY CLUSTERED 
(
	[IdUser] ASC,
	[IdRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UsersTypes]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UsersTypes](
	[IdType] [tinyint] IDENTITY(1,1) NOT NULL,
	[CodUserType] [varchar](10) NOT NULL,
	[NameUserType] [varchar](50) NOT NULL,
	[DescriptionUserType] [varchar](100) NOT NULL,
 CONSTRAINT [PK_UsersTypes] PRIMARY KEY CLUSTERED 
(
	[IdType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Assistants] ON 

INSERT [dbo].[Assistants] ([IdAssistant], [CodAssistant], [NameAssistant]) VALUES (1, N'Assis01', N'Asistente01')
INSERT [dbo].[Assistants] ([IdAssistant], [CodAssistant], [NameAssistant]) VALUES (2, N'Assis02', N'Asistente02')
SET IDENTITY_INSERT [dbo].[Assistants] OFF
SET IDENTITY_INSERT [dbo].[AuditDB] ON 

INSERT [dbo].[AuditDB] ([IdAuditDB], [IdUser], [TableNameAuditDB], [DateOperationAuditDB], [OperationTypeAuditDB], [OldValueAuditDB], [NewValueAuditDB]) VALUES (19, 7, N'RpaParameters', CAST(N'2021-04-13 16:52:19.7600000' AS DateTime2), N'Update', N'Prueba Parametro 4', N'2')
INSERT [dbo].[AuditDB] ([IdAuditDB], [IdUser], [TableNameAuditDB], [DateOperationAuditDB], [OperationTypeAuditDB], [OldValueAuditDB], [NewValueAuditDB]) VALUES (18, 7, N'RpaParameters', CAST(N'2021-04-13 16:51:53.4100000' AS DateTime2), N'Update', N'Prueba Parametro 4', N'2')
SET IDENTITY_INSERT [dbo].[AuditDB] OFF
SET IDENTITY_INSERT [dbo].[AuditTickets] ON 

INSERT [dbo].[AuditTickets] ([IdAuditTicket], [IdTicket], [IdUser], [StartDateAuditTicket], [EndDateAuditTicket], [StartStateAuditTicket], [EndStateAuditTicket], [DescriptionAuditTicket]) VALUES (2, N'SIB', 5, CAST(N'2021-02-13 00:00:00.0000000' AS DateTime2), CAST(N'2021-02-13 00:00:00.0000000' AS DateTime2), 1, 2, N'2')
INSERT [dbo].[AuditTickets] ([IdAuditTicket], [IdTicket], [IdUser], [StartDateAuditTicket], [EndDateAuditTicket], [StartStateAuditTicket], [EndStateAuditTicket], [DescriptionAuditTicket]) VALUES (4, N'SIB', 5, CAST(N'2021-02-13 00:00:00.0000000' AS DateTime2), CAST(N'2021-02-13 00:00:00.0000000' AS DateTime2), 1, 2, N'2')
INSERT [dbo].[AuditTickets] ([IdAuditTicket], [IdTicket], [IdUser], [StartDateAuditTicket], [EndDateAuditTicket], [StartStateAuditTicket], [EndStateAuditTicket], [DescriptionAuditTicket]) VALUES (5, N'SIB', 5, CAST(N'2021-04-13 00:00:00.0000000' AS DateTime2), CAST(N'2021-04-13 00:00:00.0000000' AS DateTime2), 1, 2, N'2')
INSERT [dbo].[AuditTickets] ([IdAuditTicket], [IdTicket], [IdUser], [StartDateAuditTicket], [EndDateAuditTicket], [StartStateAuditTicket], [EndStateAuditTicket], [DescriptionAuditTicket]) VALUES (6, N'SIB_01', 5, CAST(N'2021-04-13 00:00:00.0000000' AS DateTime2), CAST(N'2021-04-13 00:00:00.0000000' AS DateTime2), 1, 2, N'1')
SET IDENTITY_INSERT [dbo].[AuditTickets] OFF
SET IDENTITY_INSERT [dbo].[EmailParameters] ON 

INSERT [dbo].[EmailParameters] ([IdEmailParameter], [CodEmailParameter], [TOEmailParameter], [CCEmailParameter], [BCCEmailParameter], [SubjectEmailParameter], [BodyEmailParameter], [IsHTMLEmailParameter], [IdAssistant]) VALUES (11, N'Mail01', N'crrmuno@bancolombia.com.co', N'crrmuno@bancolombia.com.co', N'crrmuno@bancolombia.com.co', N'Actualización', N'Preuba de actualizacion', 1, 1)
SET IDENTITY_INSERT [dbo].[EmailParameters] OFF
SET IDENTITY_INSERT [dbo].[ProcessTime] ON 

INSERT [dbo].[ProcessTime] ([IdProcessTime], [CodProcessTime], [NameProcessTime], [LastExecutionProcessTime], [WaitTimeProcessTime]) VALUES (1, N'Proc01', N'proceso', CAST(N'2020-11-10 10:40:00.0000000' AS DateTime2), 180)
SET IDENTITY_INSERT [dbo].[ProcessTime] OFF
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([IdRole], [CodRole], [NameRole], [DescriptionRole], [IdAssistant], [RolId]) VALUES (1, N'Rol01', N'Finacle', N'Consulta Finacle', 2, 1)
INSERT [dbo].[Roles] ([IdRole], [CodRole], [NameRole], [DescriptionRole], [IdAssistant], [RolId]) VALUES (2, N'Rol02', N'AS400', N'Consulta AS400', 1, 2)
SET IDENTITY_INSERT [dbo].[Roles] OFF
SET IDENTITY_INSERT [dbo].[RpaParameters] ON 

INSERT [dbo].[RpaParameters] ([IdParameter], [CodParameter], [NameParameter], [ValueParameter], [DescriptionParameter], [IsVisibleParameter], [IdAssistant]) VALUES (1, N'Parameter01', N'34', N'Prueba Parametro 4', N'Prueba', 1, 1)
INSERT [dbo].[RpaParameters] ([IdParameter], [CodParameter], [NameParameter], [ValueParameter], [DescriptionParameter], [IsVisibleParameter], [IdAssistant]) VALUES (2, N'Parametro02', N'MesesBackup', N'2', N'Numero de meses que ', 1, 1)
SET IDENTITY_INSERT [dbo].[RpaParameters] OFF
SET IDENTITY_INSERT [dbo].[States] ON 

INSERT [dbo].[States] ([IdState], [CodState], [NameState], [IdAssistant]) VALUES (1, N'State01', N'Prueba2', 1)
INSERT [dbo].[States] ([IdState], [CodState], [NameState], [IdAssistant]) VALUES (2, N'State02', N'Prueba', 2)
SET IDENTITY_INSERT [dbo].[States] OFF
INSERT [dbo].[Tickets] ([IdTicket], [IdState], [CreationDate], [Locked], [FinalDate], [ExecutionDate], [Priority]) VALUES (N'SIB', 1, CAST(N'2021-01-14 00:00:00.0000000' AS DateTime2), 1, CAST(N'2020-12-14 00:00:00.0000000' AS DateTime2), CAST(N'2020-12-14 00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Tickets] ([IdTicket], [IdState], [CreationDate], [Locked], [FinalDate], [ExecutionDate], [Priority]) VALUES (N'SIB_01', 2, CAST(N'2021-01-14 00:00:00.0000000' AS DateTime2), 1, CAST(N'2020-12-14 00:00:00.0000000' AS DateTime2), CAST(N'2020-12-14 00:00:00.0000000' AS DateTime2), 1)
INSERT [dbo].[Tickets] ([IdTicket], [IdState], [CreationDate], [Locked], [FinalDate], [ExecutionDate], [Priority]) VALUES (N'SIB_2', 1, CAST(N'2021-04-13 00:00:00.0000000' AS DateTime2), 1, CAST(N'2021-02-13 00:00:00.0000000' AS DateTime2), CAST(N'2021-02-13 00:00:00.0000000' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([IdUser], [NameUser], [ActiveUser], [ComputerNameUser], [IdType], [FullName]) VALUES (5, N'RPAIND01', 1, N'PB', 2, NULL)
INSERT [dbo].[Users] ([IdUser], [NameUser], [ActiveUser], [ComputerNameUser], [IdType], [FullName]) VALUES (6, N'RPAIND02', 1, N'PB', 2, NULL)
INSERT [dbo].[Users] ([IdUser], [NameUser], [ActiveUser], [ComputerNameUser], [IdType], [FullName]) VALUES (7, N'CRRMUNO', 1, N'PB', 3, N'CRISTIAN')
INSERT [dbo].[Users] ([IdUser], [NameUser], [ActiveUser], [ComputerNameUser], [IdType], [FullName]) VALUES (8, N'ANDRES', 1, N'PB833', 3, N'ANDRES BRICE')
INSERT [dbo].[Users] ([IdUser], [NameUser], [ActiveUser], [ComputerNameUser], [IdType], [FullName]) VALUES (16, N'RPAADH03', 1, N'0BOOO', 2, NULL)
SET IDENTITY_INSERT [dbo].[Users] OFF
INSERT [dbo].[UsersAssistants] ([IdUser], [IdAssistant], [ActiveUserAssistant]) VALUES (5, 1, 1)
INSERT [dbo].[UsersAssistants] ([IdUser], [IdAssistant], [ActiveUserAssistant]) VALUES (5, 2, 1)
INSERT [dbo].[UsersAssistants] ([IdUser], [IdAssistant], [ActiveUserAssistant]) VALUES (6, 1, 1)
INSERT [dbo].[UsersAssistants] ([IdUser], [IdAssistant], [ActiveUserAssistant]) VALUES (7, 1, 1)
INSERT [dbo].[UsersAssistants] ([IdUser], [IdAssistant], [ActiveUserAssistant]) VALUES (7, 2, 1)
INSERT [dbo].[UsersAssistants] ([IdUser], [IdAssistant], [ActiveUserAssistant]) VALUES (8, 1, 0)
INSERT [dbo].[UsersAssistants] ([IdUser], [IdAssistant], [ActiveUserAssistant]) VALUES (8, 2, 1)
INSERT [dbo].[UsersAssistants] ([IdUser], [IdAssistant], [ActiveUserAssistant]) VALUES (16, 1, 1)
INSERT [dbo].[UsersAssistants] ([IdUser], [IdAssistant], [ActiveUserAssistant]) VALUES (16, 2, 1)
INSERT [dbo].[UsersRoles] ([IdUser], [IdRole], [MaxIterationsUserRole], [ActiveUserRole], [RunnerName]) VALUES (5, 1, 0, 0, NULL)
INSERT [dbo].[UsersRoles] ([IdUser], [IdRole], [MaxIterationsUserRole], [ActiveUserRole], [RunnerName]) VALUES (5, 2, 0, 1, NULL)
INSERT [dbo].[UsersRoles] ([IdUser], [IdRole], [MaxIterationsUserRole], [ActiveUserRole], [RunnerName]) VALUES (6, 1, 0, 0, NULL)
INSERT [dbo].[UsersRoles] ([IdUser], [IdRole], [MaxIterationsUserRole], [ActiveUserRole], [RunnerName]) VALUES (6, 2, 0, 1, NULL)
INSERT [dbo].[UsersRoles] ([IdUser], [IdRole], [MaxIterationsUserRole], [ActiveUserRole], [RunnerName]) VALUES (16, 1, 0, 0, NULL)
INSERT [dbo].[UsersRoles] ([IdUser], [IdRole], [MaxIterationsUserRole], [ActiveUserRole], [RunnerName]) VALUES (16, 2, 0, 0, NULL)
SET IDENTITY_INSERT [dbo].[UsersTypes] ON 

INSERT [dbo].[UsersTypes] ([IdType], [CodUserType], [NameUserType], [DescriptionUserType]) VALUES (2, N'Type01', N'RPA', N'Usuario RPA')
INSERT [dbo].[UsersTypes] ([IdType], [CodUserType], [NameUserType], [DescriptionUserType]) VALUES (3, N'Type02', N'Admin', N'Usuario Administrador del proceso')
INSERT [dbo].[UsersTypes] ([IdType], [CodUserType], [NameUserType], [DescriptionUserType]) VALUES (4, N'Type03', N'SuperAdmin', N'Usuario Super Administrador')
SET IDENTITY_INSERT [dbo].[UsersTypes] OFF
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_Assistants_CodAssistant]    Script Date: 16/04/2021 09:44:49 AM ******/
ALTER TABLE [dbo].[Assistants] ADD  CONSTRAINT [UQ_Assistants_CodAssistant] UNIQUE NONCLUSTERED 
(
	[CodAssistant] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_EmailParameters_CodEmailParameters]    Script Date: 16/04/2021 09:44:49 AM ******/
ALTER TABLE [dbo].[EmailParameters] ADD  CONSTRAINT [UQ_EmailParameters_CodEmailParameters] UNIQUE NONCLUSTERED 
(
	[CodEmailParameter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_Roles_CodRol]    Script Date: 16/04/2021 09:44:49 AM ******/
ALTER TABLE [dbo].[Roles] ADD  CONSTRAINT [UQ_Roles_CodRol] UNIQUE NONCLUSTERED 
(
	[CodRole] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_RpaParameters_CodRpaParameters]    Script Date: 16/04/2021 09:44:49 AM ******/
ALTER TABLE [dbo].[RpaParameters] ADD  CONSTRAINT [UQ_RpaParameters_CodRpaParameters] UNIQUE NONCLUSTERED 
(
	[CodParameter] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ_States_CodStates]    Script Date: 16/04/2021 09:44:49 AM ******/
ALTER TABLE [dbo].[States] ADD  CONSTRAINT [UQ_States_CodStates] UNIQUE NONCLUSTERED 
(
	[CodState] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Users__13F185178E05B3F4]    Script Date: 16/04/2021 09:44:49 AM ******/
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [UQ__Users__13F185178E05B3F4] UNIQUE NONCLUSTERED 
(
	[NameUser] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Holidays] ADD  CONSTRAINT [DF_Holidays_active]  DEFAULT ((1)) FOR [ActiveHoliday]
GO
ALTER TABLE [dbo].[TicketsUsers] ADD  CONSTRAINT [DF_TicketsUsers_FechaInsertTicketsUsers]  DEFAULT (getdate()) FOR [DateInsertTicketUser]
GO
ALTER TABLE [dbo].[AuditTickets]  WITH CHECK ADD  CONSTRAINT [FK_AuditTickets_EndState] FOREIGN KEY([EndStateAuditTicket])
REFERENCES [dbo].[States] ([IdState])
GO
ALTER TABLE [dbo].[AuditTickets] CHECK CONSTRAINT [FK_AuditTickets_EndState]
GO
ALTER TABLE [dbo].[AuditTickets]  WITH CHECK ADD  CONSTRAINT [FK_AuditTickets_StartState] FOREIGN KEY([StartStateAuditTicket])
REFERENCES [dbo].[States] ([IdState])
GO
ALTER TABLE [dbo].[AuditTickets] CHECK CONSTRAINT [FK_AuditTickets_StartState]
GO
ALTER TABLE [dbo].[AuditTickets]  WITH CHECK ADD  CONSTRAINT [FK_AuditTickets_Tickets] FOREIGN KEY([IdTicket])
REFERENCES [dbo].[Tickets] ([IdTicket])
GO
ALTER TABLE [dbo].[AuditTickets] CHECK CONSTRAINT [FK_AuditTickets_Tickets]
GO
ALTER TABLE [dbo].[AuditTickets]  WITH CHECK ADD  CONSTRAINT [FK_AuditTickets_Users] FOREIGN KEY([IdUser])
REFERENCES [dbo].[Users] ([IdUser])
GO
ALTER TABLE [dbo].[AuditTickets] CHECK CONSTRAINT [FK_AuditTickets_Users]
GO
ALTER TABLE [dbo].[EmailParameters]  WITH CHECK ADD  CONSTRAINT [FK_EmailParameters_Assistants] FOREIGN KEY([IdAssistant])
REFERENCES [dbo].[Assistants] ([IdAssistant])
GO
ALTER TABLE [dbo].[EmailParameters] CHECK CONSTRAINT [FK_EmailParameters_Assistants]
GO
ALTER TABLE [dbo].[Roles]  WITH CHECK ADD  CONSTRAINT [FK_Roles_Assistants] FOREIGN KEY([IdAssistant])
REFERENCES [dbo].[Assistants] ([IdAssistant])
GO
ALTER TABLE [dbo].[Roles] CHECK CONSTRAINT [FK_Roles_Assistants]
GO
ALTER TABLE [dbo].[States]  WITH CHECK ADD  CONSTRAINT [FK_States_Assistants] FOREIGN KEY([IdAssistant])
REFERENCES [dbo].[Assistants] ([IdAssistant])
GO
ALTER TABLE [dbo].[States] CHECK CONSTRAINT [FK_States_Assistants]
GO
ALTER TABLE [dbo].[Tickets]  WITH CHECK ADD  CONSTRAINT [FK_Tickets_States] FOREIGN KEY([IdState])
REFERENCES [dbo].[States] ([IdState])
GO
ALTER TABLE [dbo].[Tickets] CHECK CONSTRAINT [FK_Tickets_States]
GO
ALTER TABLE [dbo].[TicketsUsers]  WITH CHECK ADD  CONSTRAINT [FK_TicketsUsers_Tickets] FOREIGN KEY([IdTicket])
REFERENCES [dbo].[Tickets] ([IdTicket])
GO
ALTER TABLE [dbo].[TicketsUsers] CHECK CONSTRAINT [FK_TicketsUsers_Tickets]
GO
ALTER TABLE [dbo].[TicketsUsers]  WITH CHECK ADD  CONSTRAINT [FK_TicketsUsers_Users] FOREIGN KEY([IdUser])
REFERENCES [dbo].[Users] ([IdUser])
GO
ALTER TABLE [dbo].[TicketsUsers] CHECK CONSTRAINT [FK_TicketsUsers_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_UsersTypes] FOREIGN KEY([IdType])
REFERENCES [dbo].[UsersTypes] ([IdType])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_UsersTypes]
GO
ALTER TABLE [dbo].[UsersAssistants]  WITH CHECK ADD  CONSTRAINT [FK_UsersAssistants_Assistants] FOREIGN KEY([IdAssistant])
REFERENCES [dbo].[Assistants] ([IdAssistant])
GO
ALTER TABLE [dbo].[UsersAssistants] CHECK CONSTRAINT [FK_UsersAssistants_Assistants]
GO
ALTER TABLE [dbo].[UsersAssistants]  WITH CHECK ADD  CONSTRAINT [FK_UsersAssistants_Users] FOREIGN KEY([IdUser])
REFERENCES [dbo].[Users] ([IdUser])
GO
ALTER TABLE [dbo].[UsersAssistants] CHECK CONSTRAINT [FK_UsersAssistants_Users]
GO
ALTER TABLE [dbo].[UsersRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersRoles_Roles] FOREIGN KEY([IdRole])
REFERENCES [dbo].[Roles] ([IdRole])
GO
ALTER TABLE [dbo].[UsersRoles] CHECK CONSTRAINT [FK_UsersRoles_Roles]
GO
ALTER TABLE [dbo].[UsersRoles]  WITH CHECK ADD  CONSTRAINT [FK_UsersRoles_Users] FOREIGN KEY([IdUser])
REFERENCES [dbo].[Users] ([IdUser])
GO
ALTER TABLE [dbo].[UsersRoles] CHECK CONSTRAINT [FK_UsersRoles_Users]
GO
/****** Object:  StoredProcedure [dbo].[DeleteHistory]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DeleteHistory]  

AS    

BEGIN
	SET NOCOUNT ON;

	DECLARE @DaysDebugDBRecords AS tinyint

	BEGIN TRAN T
	BEGIN TRY
	

		SELECT @DaysDebugDBRecords=ValueParameter FROM RpaParameters WHERE CodParameter = 'Par06'

		DELETE AuditDB WHERE DateOperationAuditDB <= (GETDATE()-@DaysDebugDBRecords)

		--SI SE DESEA SE PUEDE INGRESAR LA LOGICA PARA DEPURAR LA BASE DE DATOS

		COMMIT TRAN T
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN T;
		THROW;
	END CATCH
END



GO
/****** Object:  StoredProcedure [dbo].[InsertUserRoles]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertUserRoles] 
	@NetworkUser   AS VARCHAR(15), 
    @ComputerName  AS VARCHAR(12), 
    @AssistantCode AS VARCHAR(50)
AS 
BEGIN 
	SET nocount ON; 

	BEGIN TRAN t 

    BEGIN TRY
		DECLARE @IdUser      AS TINYINT, 
                @IdRol       AS TINYINT, 
                @IdAssistant AS TINYINT,
				@IdType		 AS TINYINT; 

		SELECT @IdAssistant = IdAssistant 
        FROM   Assistants 
        WHERE  CodAssistant = @AssistantCode; 

		SELECT @IdType = IdType 
        FROM   UsersTypes 
        WHERE  CodUserType = 'Type01'; 

        IF NOT EXISTS (SELECT 1 FROM users WHERE  NameUser = @NetworkUser)
			BEGIN 
				INSERT INTO Users (NameUser, ActiveUser, ComputerNameUser, IdType) VALUES (@NetworkUser, 1, @ComputerName, @IdType); 

				SET @IdUser = Scope_identity(); 

				INSERT INTO UsersAssistants(IdUser, IdAssistant, ActiveUserAssistant) VALUES (@IdUser, @IdAssistant, 1) 
			END 
        ELSE 
			BEGIN 
				SELECT @IdUser = iduser FROM   users WHERE  NameUser = @NetworkUser; 

				  IF NOT EXISTS (SELECT 1 FROM UsersAssistants WHERE  IdAssistant = @IdAssistant AND IdUser= @IdUser)
					BEGIN 
						INSERT INTO UsersAssistants(IdUser, IdAssistant, ActiveUserAssistant) VALUES (@IdUser, @IdAssistant, 1) 
					END
			END

        INSERT INTO UsersRoles (IdUser,IdRole,MaxIterationsUserRole,ActiveUserRole)
        SELECT @IdUser, R.IdRole, 1, 0 FROM Roles R
        WHERE NOT EXISTS (SELECT 1 FROM UsersRoles UR WHERE UR.IdUser = @IdUser AND R.IdRole = UR.IdRole)

        COMMIT TRAN T
	END TRY
    BEGIN CATCH
		ROLLBACK TRAN T;
		THROW;
    END CATCH
END 


GO
/****** Object:  StoredProcedure [dbo].[SelectEmailParameters]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectEmailParameters]
	@CodEmailParameter varchar(50),
	@CodAssistant varchar(50),
	@TOEmailParameter varchar(Max) OUTPUT,
	@CCEmailParameter varchar(Max) OUTPUT,
	@BCCEmailParameter varchar(Max) OUTPUT,
	@SubjectEmailParameter varchar(250) OUTPUT,
	@BodyEmailParameter varchar(Max) OUTPUT,
	@IsHTMLEmailParameter varchar(5) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @IdAssistant AS VARCHAR(255);
	SELECT @IdAssistant =IdAssistant FROM Assistants WHERE CodAssistant=@CodAssistant

	SELECT	@TOEmailParameter=TOEmailParameter, 
			@CCEmailParameter=CCEmailParameter, 
			@BCCEmailParameter=BCCEmailParameter, 
			@SubjectEmailParameter=SubjectEmailParameter, @BodyEmailParameter=BodyEmailParameter,
			@IsHTMLEmailParameter=IIF (IsHTMLEmailParameter=1,'True','False')
	FROM EmailParameters
	WHERE CodEmailParameter = @CodEmailParameter AND IdAssistant=@IdAssistant

END







GO
/****** Object:  StoredProcedure [dbo].[SelectParameters]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SelectParameters]
@CodAsistente AS VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @cols AS VARCHAR(MAX),@query  AS NVARCHAR(MAX)

	select @cols = STUFF((SELECT ',' + QUOTENAME(a.CodParameter) 
						from RpaParameters a
						inner join Assistants b ON a.IdAssistant=b.IdAssistant
						where b.CodAssistant = @CodAsistente
						group by CodParameter
				FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')

	set @query = N'SELECT ' + @cols + N' from 
					(
					select ValueParameter, CodParameter
					from RpaParameters 
				
				) x
				pivot 
				(
					max(ValueParameter)
					for CodParameter in (' + @cols + N')
				) p '

	exec sp_executesql @query;

END








GO
/****** Object:  StoredProcedure [dbo].[SelectProcessTime]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SelectProcessTime]
	@CodProcessTime AS VARCHAR (50),
	@WaitTimeProcessTime AS INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	Select @WaitTimeProcessTime=DATEDIFF(MINUTE,LastExecutionProcessTime,GETDATE()) from ProcessTime 
	WHERE CodProcessTime=@CodProcessTime

END






GO
/****** Object:  StoredProcedure [dbo].[SelectRolesByUser]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectRolesByUser]
	@NameUser varchar(10),
	@CodAssistant AS VARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @cols AS VARCHAR(MAX),@query  AS NVARCHAR(MAX)
		
	select @cols = STUFF((SELECT ',' + QUOTENAME(a.CodRole) 
						from Roles a
						inner join Assistants b on a.IdAssistant = b.IdAssistant
						where b.CodAssistant = @CodAssistant
						group by CodRole
				FOR XML PATH(''), TYPE
				).value('.', 'NVARCHAR(MAX)') 
			,1,1,'')

	set @query = N'SELECT ' + @cols + N' from 
					(
					SELECT CAST(A.ActiveUserRole AS VARCHAR) AS Active, B.CodRole 
					FROM UsersRoles A
					INNER JOIN Roles B ON A.IdRole = B.IdRole
					INNER JOIN Users C ON A.IdUser = C.IdUser
					WHERE C.NameUser = '''+@NameUser+'''
				) x
				pivot 
				(
					Max(Active)
					for CodRole in (' + @cols + N')
				) p 
				UNION ALL
				SELECT ' + @cols + N' from
				(
					SELECT CAST(A.MaxIterationsUserRole AS VARCHAR) AS MaxIterationsUserRole, B.CodRole 
					FROM UsersRoles A
					INNER JOIN Roles B ON A.IdRole = B.IdRole
					INNER JOIN Users C ON A.IdUser = C.IdUser
					WHERE C.NameUser = '''+@NameUser+'''
				) x  
				pivot 
				(
					Max(MaxIterationsUserRole)
					for CodRole in (' + @cols + N')
				) p 
				UNION ALL
				SELECT ' + @cols + ' from
				(
					SELECT A.RunnerName, B.CodRole 
					FROM UsersRoles A
					INNER JOIN Roles B ON A.IdRole = B.IdRole 
					INNER JOIN Users C ON A.IdUser = C.IdUser 
					WHERE C.NameUser = '''+@NameUser+'''
				) x  
				PIVOT 
				(
					MAX(RunnerName)
					FOR CodRole IN (' + @cols + ')
				) p
				UNION ALL
				SELECT ' + @cols + ' from
				(
					SELECT CAST(B.RolId AS VARCHAR) AS RolId, B.CodRole 
					FROM UsersRoles A
					INNER JOIN Roles B ON A.IdRole = B.IdRole 
					INNER JOIN Users C ON A.IdUser = C.IdUser 
					WHERE C.NameUser = '''+@NameUser+'''
				) x  
				PIVOT 
				(
					MAX(RolId)
					FOR CodRole IN (' + @cols + ')
				) p ;'


	exec sp_executesql @query;

END






GO
/****** Object:  StoredProcedure [dbo].[SelectTicket]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SelectTicket]
	@NameUser AS VARCHAR(10),	
	@CodState AS VARCHAR(50),
	@CodAssistant As VARCHAR(50),
	@IdTicket AS INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRAN T
	BEGIN TRY
		
		DECLARE @IdUser AS TINYINT;
		SELECT @IdUser=IdUser FROM Users WHERE NameUser = @NameUser;

		DECLARE @IdAssitant AS TINYINT;
		SELECT @IdAssitant = IdAssistant FROM Assistants WHERE CodAssistant=@CodAssistant
		
		if  (@IdUser in (SELECT IdUser FROM UsersAssistants WHERE IdAssistant=@IdAssitant and ActiveUserAssistant=1))
			BEGIN
				
				--Libera el registro bloqueado que lleva demasiado tiempo sin ser procesado				
				UPDATE Tickets SET Locked = 0 Where IdTicket IN (SELECT IdTicket FROM TicketsUsers WHERE DATEDIFF(MINUTE, DateInsertTicketUser, GETDATE())>=60)
				DELETE A FROM TicketsUsers A INNER JOIN Tickets B ON A.IdTicket = B.IdTicket WHERE B.Locked =0

				--Libera el registro bloqueado por el mismo usuario
				UPDATE Tickets SET Locked = 0 WHERE IdTicket IN (SELECT IdTicket FROM TicketsUsers WHERE IdUser = @IdUser);
				DELETE TicketsUsers where IdTicket in (SELECT IdTicket FROM TicketsUsers WHERE IdUser = @IdUser)

				--Selecciona el ticket a procesar
				SELECT TOP 1 @IdTicket=A.IdTicket 
				FROM Tickets A 
					INNER JOIN States B ON A.IdState = B.IdState 
				WHERE A.Locked = 0 
					AND NOT EXISTS (SELECT IdTicket FROM TicketsUsers WHERE IdTicket = A.IdTicket)
					AND b.IdAssistant = @IdAssitant 
					AND B.CodState in (SELECT Item FROM Split(@CodState, ';'))

				--Bloquea el ticket a procesar
				IF(@IdTicket IS NOT NULL AND @IdTicket <> '')
					BEGIN
						IF(@IdUser IS NOT NULL AND @IdUser > 0)
							BEGIN
								INSERT INTO TicketsUsers(IdTicket,IdUser) VALUES (@IdTicket, @IdUser);
								UPDATE Tickets SET Locked = 1 WHERE IdTicket = @IdTicket;
							END
					END
			END
		
		COMMIT TRAN T
	END TRY
	BEGIN CATCH
		SET @IdTicket = null
		ROLLBACK TRAN T;
		THROW;
	END CATCH

END







GO
/****** Object:  StoredProcedure [dbo].[UpdateProcessTime]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateProcessTime]  
	@CodProcessTime varchar(255) 
AS    

BEGIN
	SET NOCOUNT ON;
	UPDATE ProcessTime SET LastExecutionProcessTime = GETDATE() WHERE CodProcessTime=@CodProcessTime
END









GO
/****** Object:  StoredProcedure [dbo].[UpdateTicket]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateTicket]
	@IdTicket AS INT,
	@CodEndState AS VARCHAR(50),
	@NameUser AS VARCHAR(10),
	@Description AS VARCHAR(MAX),
	@Liberate AS TINYINT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @StartDate AS DATETIME2(2)
	SELECT @StartDate=DateInsertTicketUser FROM TicketsUsers WHERE IdTicket=@IdTicket

	DECLARE @IdStartState AS TINYINT;
	SELECT @IdStartState=IdState FROM Tickets WHERE IdTicket = @IdTicket;

	DECLARE @IdEndState AS TINYINT;
	SELECT @IdEndState=IdState FROM States WHERE CodState = @CodEndState;
		
	DECLARE @IdUser AS TINYINT;
	SELECT @IdUser=IdUser FROM Users WHERE NameUser = @NameUser;

	BEGIN TRAN T
	BEGIN TRY

		INSERT INTO AuditTickets VALUES(@IdTicket, @IdUser,CONVERT(datetime, @StartDate, 20),CONVERT(datetime, GETDATE(), 20), @IdStartState, @IdEndState, @Description);
		
		IF(@Liberate = '1')
			BEGIN
				UPDATE Tickets SET Locked = 0, IdState = @IdEndState WHERE IdTicket = @IdTicket;
				DELETE TicketsUsers WHERE IdTicket = @IdTicket;
			END
		ELSE 
			BEGIN
				UPDATE Tickets SET IdState = @IdEndState WHERE IdTicket = @IdTicket;
			END	
			
		IF(@CodEndState in ('Codigo final 1','Codigo Final 2'))
			BEGIN
				UPDATE Tickets SET FinalDate = GETDATE() WHERE IdTicket = @IdTicket;
			END

		COMMIT TRAN T

	END TRY
	BEGIN CATCH
		ROLLBACK TRAN T;
		THROW;
	END CATCH

END







GO
/****** Object:  Trigger [dbo].[TR_Update_EmailParameters]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TR_Update_EmailParameters]
ON [dbo].[EmailParameters]
AFTER UPDATE
AS 
BEGIN
    SET NOCOUNT ON;
	DECLARE @OldTo AS nvarchar(max);
	DECLARE @NewTo AS nvarchar(max);
	DECLARE @OldBody AS nvarchar(max);
	DECLARE @NewBody AS nvarchar(max);
	DECLARE @IdUser AS INT;
	
	SELECT @IdUser = IdUser FROM Users WHERE NameUser = replace(SYSTEM_USER, 'BANCOLOMBIA\','');
	SELECT @NewTo =  TOEmailParameter FROM EmailParameters;
	SELECT @OldTo = TOEmailParameter FROM deleted;
	SELECT @NewBody =  BodyEmailParameter FROM EmailParameters;
	SELECT @OldBody = BodyEmailParameter FROM deleted;

	IF @NewTo != @OldTo
		BEGIN
			INSERT INTO AuditDB VALUES(@IdUser,'EmailParameters', GETDATE(),'Update',@OldTo,@NewTo)
		END
	IF @NewBody != @OldBody
		BEGIN
			INSERT INTO AuditDB VALUES(@IdUser,'EmailParameters', GETDATE(),'Update',@OldBody,@NewBody)
		END
END
GO
/****** Object:  Trigger [dbo].[TR_Delete_AuditDb]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Batch submitted through debugger: SQLQuery5.sql|7|0|C:\Users\crrmuno\AppData\Local\Temp\~vsF578.sql
CREATE TRIGGER [dbo].[TR_Delete_AuditDb]  
ON  [dbo].[RpaParameters]
FOR UPDATE
AS  
BEGIN  
	SET NOCOUNT ON;  
	IF (UPDATE(ValueParameter) or UPDATE(NameParameter) or UPDATE(DescriptionParameter) or UPDATE(IsVisibleParameter))  
	BEGIN  TRY
		DELETE FROM AuditDB WHERE DateOperationAuditDB <= DATEADD(MONTH,-1,GETDATE())
	END TRY
	BEGIN CATCH
	END CATCH
 END
GO
/****** Object:  Trigger [dbo].[TR_Update_RpaParameters]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [dbo].[TR_Update_RpaParameters]
ON [dbo].[RpaParameters]
AFTER UPDATE
AS 
BEGIN
    SET NOCOUNT ON;
    DECLARE @action as char(1);
	DECLARE @After as nvarchar(max);
	DECLARE @Before as nvarchar(max);
	DECLARE @IdUser as int;

	SELECT @IdUser = IdUser FROM Users WHERE NameUser = replace(SYSTEM_USER, 'BANCOLOMBIA\','');
    SELECT @After = ValueParameter FROM RpaParameters;
	SELECT @Before = ValueParameter FROM deleted;
	BEGIN
		INSERT INTO AuditDB VALUES(@IdUser,'RpaParameters', GETDATE(),'Update',@Before,@After)
    END
END
GO
/****** Object:  DdlTrigger [LockDeleteTable]    Script Date: 16/04/2021 09:44:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [LockDeleteTable]
ON DATABASE
FOR DROP_TABLE
AS
PRINT 'Para borrar esta tabla debes deshabilitar el Trigger'
ROLLBACK
GO
ENABLE TRIGGER [LockDeleteTable] ON DATABASE
GO
USE [master]
GO
ALTER DATABASE [NewModel] SET  READ_WRITE 
GO
