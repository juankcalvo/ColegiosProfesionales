USE [master]
GO
/****** Object:  Database [ColegiosProfesionales]    Script Date: 6/21/2024 10:50:18 AM ******/
CREATE DATABASE [ColegiosProfesionales]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ColegiosProfecionales', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\ColegiosProfecionales.mdf' , SIZE = 139264KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ColegiosProfecionales_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\ColegiosProfecionales_log.ldf' , SIZE = 532480KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [ColegiosProfesionales] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ColegiosProfesionales].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ColegiosProfesionales] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET ARITHABORT OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ColegiosProfesionales] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ColegiosProfesionales] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ColegiosProfesionales] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ColegiosProfesionales] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET RECOVERY FULL 
GO
ALTER DATABASE [ColegiosProfesionales] SET  MULTI_USER 
GO
ALTER DATABASE [ColegiosProfesionales] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ColegiosProfesionales] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ColegiosProfesionales] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ColegiosProfesionales] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ColegiosProfesionales] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ColegiosProfesionales] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'ColegiosProfesionales', N'ON'
GO
ALTER DATABASE [ColegiosProfesionales] SET QUERY_STORE = ON
GO
ALTER DATABASE [ColegiosProfesionales] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [ColegiosProfesionales]
GO
/****** Object:  Table [dbo].[Colegiado]    Script Date: 6/21/2024 10:50:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Colegiado](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ColegioId] [int] NULL,
	[FotoColegiadoId] [int] NULL,
	[CondicionColegiadoId] [int] NOT NULL,
	[Identificacion] [nvarchar](100) NULL,
	[Nombre] [nvarchar](200) NULL,
	[NumeroCarne] [nvarchar](50) NULL,
	[Telefono] [varchar](20) NULL,
	[CorreoElectronico] [nvarchar](120) NULL,
	[Direccion] [nvarchar](max) NULL,
	[LugarTrabajo] [nvarchar](max) NULL,
	[Especialidad] [varchar](250) NULL,
	[FechaIncorporacion] [date] NULL,
	[TipoSuspencion] [varchar](70) NULL,
	[FechaReferencia] [smalldatetime] NULL,
	[Fuente] [nvarchar](250) NULL,
	[Estado] [bit] NOT NULL,
	[Consultar] [bit] NOT NULL,
	[UniversidadId] [int] NULL,
	[EsNacional] [bit] NULL,
	[Procesado] [bit] NULL,
	[IdColegiado] [varchar](50) NULL,
	[CitasInscripcion] [varchar](50) NULL,
 CONSTRAINT [PK_Colegiado] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Colegio]    Script Date: 6/21/2024 10:50:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Colegio](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](250) NOT NULL,
	[Profesion] [varchar](100) NULL,
	[Web] [nvarchar](200) NULL,
	[Fecha] [smalldatetime] NOT NULL,
	[Estado] [bit] NOT NULL,
	[Modulo] [bit] NOT NULL,
	[Titulo] [nvarchar](50) NULL,
 CONSTRAINT [PK_Colegio] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EstadoColegiado]    Script Date: 6/21/2024 10:50:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EstadoColegiado](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [nvarchar](250) NOT NULL,
	[Estado] [int] NOT NULL,
 CONSTRAINT [PK_EstadoColegiado] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FotoColegiado]    Script Date: 6/21/2024 10:50:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FotoColegiado](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ColegiadoId] [int] NULL,
	[FotoColegiado] [varbinary](max) NULL,
	[Fecha] [datetime] NULL,
 CONSTRAINT [PK_FotoColegiado] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Colegiado]    Script Date: 6/21/2024 10:50:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Colegiado] ON [dbo].[Colegiado]
(
	[Identificacion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Colegiado_1]    Script Date: 6/21/2024 10:50:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Colegiado_1] ON [dbo].[Colegiado]
(
	[NumeroCarne] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Colegiado_2]    Script Date: 6/21/2024 10:50:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Colegiado_2] ON [dbo].[Colegiado]
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Colegiado_3]    Script Date: 6/21/2024 10:50:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Colegiado_3] ON [dbo].[Colegiado]
(
	[Estado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Colegiado_4]    Script Date: 6/21/2024 10:50:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Colegiado_4] ON [dbo].[Colegiado]
(
	[Consultar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Colegiado_5]    Script Date: 6/21/2024 10:50:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Colegiado_5] ON [dbo].[Colegiado]
(
	[EsNacional] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Colegiado_6]    Script Date: 6/21/2024 10:50:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_Colegiado_6] ON [dbo].[Colegiado]
(
	[Identificacion] ASC,
	[ColegioId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_EstadoColegiado]    Script Date: 6/21/2024 10:50:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_EstadoColegiado] ON [dbo].[EstadoColegiado]
(
	[Descripcion] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Colegiado] ADD  CONSTRAINT [DF_Colegiado_Estado]  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[Colegiado] ADD  CONSTRAINT [DF_Colegiado_Consultar]  DEFAULT ((1)) FOR [Consultar]
GO
ALTER TABLE [dbo].[Colegio] ADD  CONSTRAINT [DF_Colegio_Fecha]  DEFAULT (getdate()) FOR [Fecha]
GO
ALTER TABLE [dbo].[Colegio] ADD  CONSTRAINT [DF_Colegio_Estado]  DEFAULT ((1)) FOR [Estado]
GO
ALTER TABLE [dbo].[Colegio] ADD  CONSTRAINT [DF_Colegio_Modulo]  DEFAULT ((0)) FOR [Modulo]
GO
ALTER TABLE [dbo].[EstadoColegiado] ADD  CONSTRAINT [DF_EstadoColegiado_Descripcion]  DEFAULT ((1)) FOR [Descripcion]
GO
ALTER TABLE [dbo].[Colegiado]  WITH NOCHECK ADD  CONSTRAINT [FK_Colegiado_Colegio] FOREIGN KEY([ColegioId])
REFERENCES [dbo].[Colegio] ([Id])
GO
ALTER TABLE [dbo].[Colegiado] CHECK CONSTRAINT [FK_Colegiado_Colegio]
GO
ALTER TABLE [dbo].[Colegiado]  WITH NOCHECK ADD  CONSTRAINT [FK_Colegiado_EstadoColegiado] FOREIGN KEY([CondicionColegiadoId])
REFERENCES [dbo].[EstadoColegiado] ([Id])
GO
ALTER TABLE [dbo].[Colegiado] CHECK CONSTRAINT [FK_Colegiado_EstadoColegiado]
GO
ALTER TABLE [dbo].[Colegiado]  WITH NOCHECK ADD  CONSTRAINT [FK_Colegiado_FotoColegiado] FOREIGN KEY([FotoColegiadoId])
REFERENCES [dbo].[FotoColegiado] ([Id])
GO
ALTER TABLE [dbo].[Colegiado] CHECK CONSTRAINT [FK_Colegiado_FotoColegiado]
GO
USE [master]
GO
ALTER DATABASE [ColegiosProfesionales] SET  READ_WRITE 
GO
