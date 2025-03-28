USE [master]
GO
/****** Object:  Database [EastAsiaUOfT]    Script Date: 3/28/2025 4:21:49 PM ******/
CREATE DATABASE [EastAsiaUOfT]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'EastAsiaUOfT', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\EastAsiaUOfT.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'EastAsiaUOfT_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\EastAsiaUOfT_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [EastAsiaUOfT] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [EastAsiaUOfT].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [EastAsiaUOfT] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET ARITHABORT OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [EastAsiaUOfT] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [EastAsiaUOfT] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET  DISABLE_BROKER 
GO
ALTER DATABASE [EastAsiaUOfT] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [EastAsiaUOfT] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [EastAsiaUOfT] SET  MULTI_USER 
GO
ALTER DATABASE [EastAsiaUOfT] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [EastAsiaUOfT] SET DB_CHAINING OFF 
GO
ALTER DATABASE [EastAsiaUOfT] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [EastAsiaUOfT] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [EastAsiaUOfT] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [EastAsiaUOfT] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [EastAsiaUOfT] SET QUERY_STORE = ON
GO
ALTER DATABASE [EastAsiaUOfT] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [EastAsiaUOfT]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[RoleGroupId] [int] NOT NULL,
	[StudentId] [int] NULL,
	[LecturerId] [int] NULL,
	[CreateDay] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK__Account__3214EC074F5D5C45] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Class]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Class](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClassName] [nvarchar](100) NOT NULL,
	[FacultyId] [int] NULL,
	[CreateDay] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK__Class__3214EC078B23491F] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClassSchedule]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClassSchedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClassShiftId] [int] NOT NULL,
	[CourseId] [int] NOT NULL,
	[RoomId] [int] NOT NULL,
	[ClassId] [int] NOT NULL,
	[DayOfWeek] [int] NOT NULL,
	[CreateDay] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
	[StartDay] [date] NULL,
	[EndDay] [date] NULL,
 CONSTRAINT [PK__ClassSch__3214EC07BE111558] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClassShift]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClassShift](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[StartTime] [time](7) NOT NULL,
	[EndTime] [time](7) NOT NULL,
	[CreateDay] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK__ClassShi__3214EC07341B22F7] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Course]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Course](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubjectId] [int] NULL,
	[LecturerId] [int] NULL,
	[Semester] [int] NOT NULL,
	[SchoolYear] [int] NOT NULL,
	[CreateDay] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
	[CourseName] [nvarchar](50) NULL,
 CONSTRAINT [PK__Course__3214EC07372680A6] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Faculty]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Faculty](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FacultyName] [nvarchar](100) NOT NULL,
	[CreateDay] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK__Faculty__3214EC078EC48E7C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Lecturer]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Lecturer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[PhoneNumber] [nvarchar](15) NULL,
	[FacultyId] [int] NULL,
	[CreateDay] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
	[Image] [nvarchar](max) NULL,
	[Gender] [bit] NOT NULL,
	[Cccd] [nvarchar](13) NULL,
	[Address] [nvarchar](max) NULL,
	[BirthDate] [date] NULL,
	[LecturerCode] [nvarchar](20) NULL,
 CONSTRAINT [PK__Lecturer__3214EC07A43C83A4] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleCode] [nvarchar](max) NULL,
	[ParentId] [int] NULL,
	[CreateDay] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleGroup]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NameRoleGroup] [nvarchar](max) NULL,
	[CreateDay] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_RoleGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleInRoleGroup]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleInRoleGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[RoleGroupId] [int] NOT NULL,
 CONSTRAINT [PK_RoleInRoleGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Room]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Room](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Building] [nvarchar](50) NULL,
	[Floor] [nvarchar](50) NULL,
	[Vacuity] [bit] NOT NULL,
	[Status] [bit] NOT NULL,
	[CreateDay] [datetime] NOT NULL,
 CONSTRAINT [PK_Room] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Student]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Student](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[BirthDate] [date] NOT NULL,
	[Gender] [bit] NOT NULL,
	[Address] [nvarchar](255) NULL,
	[Email] [nvarchar](100) NULL,
	[PhoneNumber] [nvarchar](15) NULL,
	[ClassId] [int] NULL,
	[CreateDay] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
	[Image] [nvarchar](max) NULL,
	[CCCD] [nvarchar](13) NOT NULL,
	[StudentCode] [nvarchar](20) NULL,
 CONSTRAINT [PK__Student__3214EC077783B928] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subject]    Script Date: 3/28/2025 4:21:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subject](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubjectName] [nvarchar](100) NOT NULL,
	[Credit] [int] NOT NULL,
	[FacultyId] [int] NULL,
	[CreateDay] [datetime] NOT NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK__Subject__3214EC0729CA485C] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Account] ON 

INSERT [dbo].[Account] ([Id], [Username], [Password], [RoleGroupId], [StudentId], [LecturerId], [CreateDay], [Status]) VALUES (5, N'root', N'AQAAAAIAAYagAAAAEEfJQCPy6d5d9qp+Uv/0ckjsEiu1yOxhGjzC/GtH/doErSvbEICwlWrMu4vIvBxjbg==', 4, NULL, NULL, CAST(N'2025-03-23T01:14:44.587' AS DateTime), 1)
INSERT [dbo].[Account] ([Id], [Username], [Password], [RoleGroupId], [StudentId], [LecturerId], [CreateDay], [Status]) VALUES (6, N'lehieu', N'AQAAAAIAAYagAAAAECCdi68Vae9zEdU6r9dE/81C9fucbwhK6c6dQ0TVjRFeuP0eSpENvIqkh72xhvg7Rg==', 4, NULL, NULL, CAST(N'2025-03-23T01:19:14.270' AS DateTime), 1)
INSERT [dbo].[Account] ([Id], [Username], [Password], [RoleGroupId], [StudentId], [LecturerId], [CreateDay], [Status]) VALUES (15, N'GV20251002', N'AQAAAAIAAYagAAAAEFy2wCrVYSywJAJLYRZGiw3eGH5FLjBxaiVllbRuRiQLArzi4nz1fVyK2O/60f5qCw==', 5, NULL, 1002, CAST(N'2025-03-23T15:43:01.910' AS DateTime), 1)
INSERT [dbo].[Account] ([Id], [Username], [Password], [RoleGroupId], [StudentId], [LecturerId], [CreateDay], [Status]) VALUES (1009, N'SV20252007', N'AQAAAAIAAYagAAAAECOUm+bc2FeAN3oG667Odzd/0bN0jZDuqofwe8O2DjEIHpChJjaPeTZkCCt4aGp0lQ==', 3, 2007, NULL, CAST(N'2025-03-26T10:29:08.930' AS DateTime), 1)
INSERT [dbo].[Account] ([Id], [Username], [Password], [RoleGroupId], [StudentId], [LecturerId], [CreateDay], [Status]) VALUES (1010, N'SV20251006', N'AQAAAAIAAYagAAAAECOUm+bc2FeAN3oG667Odzd/0bN0jZDuqofwe8O2DjEIHpChJjaPeTZkCCt4aGp0lQ==', 3, 1006, NULL, CAST(N'2025-03-26T10:29:08.930' AS DateTime), 1)
INSERT [dbo].[Account] ([Id], [Username], [Password], [RoleGroupId], [StudentId], [LecturerId], [CreateDay], [Status]) VALUES (1011, N'GV20251003', N'AQAAAAIAAYagAAAAEIfZ+ip7Kl0d047PKeKRI1rE4VXi7M/tajjCm3dhWrAM3nQYpQpWSeKvgQfyQhQQDQ==', 5, NULL, 1003, CAST(N'2025-03-26T15:26:41.147' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[Account] OFF
GO
SET IDENTITY_INSERT [dbo].[Class] ON 

INSERT [dbo].[Class] ([Id], [ClassName], [FacultyId], [CreateDay], [Status]) VALUES (1, N'DC.CNTN.12.10.11', 1, CAST(N'2025-03-18T14:21:36.170' AS DateTime), 1)
INSERT [dbo].[Class] ([Id], [ClassName], [FacultyId], [CreateDay], [Status]) VALUES (2, N'DATN.CNTT.12.10.11', 1, CAST(N'2025-03-18T16:32:41.927' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[Class] OFF
GO
SET IDENTITY_INSERT [dbo].[ClassSchedule] ON 

INSERT [dbo].[ClassSchedule] ([Id], [ClassShiftId], [CourseId], [RoomId], [ClassId], [DayOfWeek], [CreateDay], [Status], [StartDay], [EndDay]) VALUES (1007, 1, 3, 1, 1, 3, CAST(N'2025-03-26T15:08:27.430' AS DateTime), 1, CAST(N'2025-03-24' AS Date), CAST(N'2025-03-30' AS Date))
INSERT [dbo].[ClassSchedule] ([Id], [ClassShiftId], [CourseId], [RoomId], [ClassId], [DayOfWeek], [CreateDay], [Status], [StartDay], [EndDay]) VALUES (1009, 1, 3, 1, 1, 2, CAST(N'2025-03-26T15:08:56.693' AS DateTime), 1, CAST(N'2025-03-24' AS Date), CAST(N'2025-03-30' AS Date))
INSERT [dbo].[ClassSchedule] ([Id], [ClassShiftId], [CourseId], [RoomId], [ClassId], [DayOfWeek], [CreateDay], [Status], [StartDay], [EndDay]) VALUES (1013, 2, 1003, 2, 2, 2, CAST(N'2025-03-26T15:13:52.420' AS DateTime), 1, CAST(N'2025-03-24' AS Date), CAST(N'2025-03-30' AS Date))
INSERT [dbo].[ClassSchedule] ([Id], [ClassShiftId], [CourseId], [RoomId], [ClassId], [DayOfWeek], [CreateDay], [Status], [StartDay], [EndDay]) VALUES (1015, 1, 1004, 1, 1, 3, CAST(N'2025-03-26T15:25:53.517' AS DateTime), 1, CAST(N'2025-03-17' AS Date), CAST(N'2025-03-23' AS Date))
SET IDENTITY_INSERT [dbo].[ClassSchedule] OFF
GO
SET IDENTITY_INSERT [dbo].[ClassShift] ON 

INSERT [dbo].[ClassShift] ([Id], [Name], [StartTime], [EndTime], [CreateDay], [Status]) VALUES (1, N'Ca 1', CAST(N'07:00:00' AS Time), CAST(N'09:25:00' AS Time), CAST(N'2025-03-25T11:28:19.093' AS DateTime), 1)
INSERT [dbo].[ClassShift] ([Id], [Name], [StartTime], [EndTime], [CreateDay], [Status]) VALUES (2, N'Ca 2', CAST(N'09:30:00' AS Time), CAST(N'11:55:00' AS Time), CAST(N'2025-03-25T11:32:57.010' AS DateTime), 1)
INSERT [dbo].[ClassShift] ([Id], [Name], [StartTime], [EndTime], [CreateDay], [Status]) VALUES (3, N'Ca 3', CAST(N'13:00:00' AS Time), CAST(N'15:25:00' AS Time), CAST(N'2025-03-25T11:33:27.727' AS DateTime), 1)
INSERT [dbo].[ClassShift] ([Id], [Name], [StartTime], [EndTime], [CreateDay], [Status]) VALUES (4, N'Ca 4', CAST(N'15:30:00' AS Time), CAST(N'17:55:00' AS Time), CAST(N'2025-03-25T11:34:04.933' AS DateTime), 1)
INSERT [dbo].[ClassShift] ([Id], [Name], [StartTime], [EndTime], [CreateDay], [Status]) VALUES (5, N'Ca 5', CAST(N'18:00:00' AS Time), CAST(N'20:25:00' AS Time), CAST(N'2025-03-25T11:40:42.390' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[ClassShift] OFF
GO
SET IDENTITY_INSERT [dbo].[Course] ON 

INSERT [dbo].[Course] ([Id], [SubjectId], [LecturerId], [Semester], [SchoolYear], [CreateDay], [Status], [CourseName]) VALUES (3, 2, 1002, 1, 2025, CAST(N'2025-03-25T15:19:29.330' AS DateTime), 1, N'CSDL_HL_1_2025')
INSERT [dbo].[Course] ([Id], [SubjectId], [LecturerId], [Semester], [SchoolYear], [CreateDay], [Status], [CourseName]) VALUES (1003, 3, 1002, 1, 2025, CAST(N'2025-03-26T15:11:03.503' AS DateTime), 1, N'LTP_HL_1_2025')
INSERT [dbo].[Course] ([Id], [SubjectId], [LecturerId], [Semester], [SchoolYear], [CreateDay], [Status], [CourseName]) VALUES (1004, 4, 1003, 1, 2025, CAST(N'2025-03-26T15:25:14.873' AS DateTime), 1, N'CTDLVGT_GB_1_2025')
SET IDENTITY_INSERT [dbo].[Course] OFF
GO
SET IDENTITY_INSERT [dbo].[Faculty] ON 

INSERT [dbo].[Faculty] ([Id], [FacultyName], [CreateDay], [Status]) VALUES (1, N'Công nghệ thông tin', CAST(N'2025-03-18T14:21:26.280' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[Faculty] OFF
GO
SET IDENTITY_INSERT [dbo].[Lecturer] ON 

INSERT [dbo].[Lecturer] ([Id], [FullName], [Email], [PhoneNumber], [FacultyId], [CreateDay], [Status], [Image], [Gender], [Cccd], [Address], [BirthDate], [LecturerCode]) VALUES (1002, N'Hieu Le', N'lehieu18102k3@gmail.com', N'0397790753', 1, CAST(N'2025-03-20T10:07:07.090' AS DateTime), 1, N'/uploads/OIP (2).jpg', 1, N'0321514', N'AN Khe/Quynh Phu/ Thai Binh', CAST(N'2025-03-01' AS Date), N'GV20251002')
INSERT [dbo].[Lecturer] ([Id], [FullName], [Email], [PhoneNumber], [FacultyId], [CreateDay], [Status], [Image], [Gender], [Cccd], [Address], [BirthDate], [LecturerCode]) VALUES (1003, N'Gia Bảo', N'admin@gmail.com', N'0813751480', 1, CAST(N'2025-03-26T15:24:05.680' AS DateTime), 1, N'/uploads/592c1c3e-2577-49a2-b166-8d43af35f52a.jpg', 1, N'032115612', N'AN Khe/Quynh Phu/ Thai Binh', CAST(N'2003-11-20' AS Date), N'GV20251003')
SET IDENTITY_INSERT [dbo].[Lecturer] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (4, N'Class', NULL, CAST(N'2025-03-20T17:42:58.097' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (5, N'Class.Delete', 4, CAST(N'2025-03-20T17:46:44.023' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (6, N'Class.Create', 4, CAST(N'2025-03-20T17:46:58.343' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (1004, N'Student', NULL, CAST(N'2025-03-21T10:29:47.870' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (1005, N'Admin', NULL, CAST(N'2025-03-21T17:06:47.077' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (1006, N'Lecturer', NULL, CAST(N'2025-03-21T17:08:33.120' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2005, N'Class.Edit', 4, CAST(N'2025-03-25T10:12:32.660' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2008, N'Student.Create', 1004, CAST(N'2025-03-26T10:34:36.017' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2009, N'LecturerRole', NULL, CAST(N'2025-03-28T10:06:06.273' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2010, N'StudentRole', NULL, CAST(N'2025-03-28T10:06:28.777' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2011, N'Student.Edit', 1004, CAST(N'2025-03-28T14:17:20.297' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2014, N'Student.Delete', 1004, CAST(N'2025-03-28T14:19:54.707' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2015, N'Lecturer.Create', 1006, CAST(N'2025-03-28T14:21:08.253' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2016, N'Lecturer.Edit', 1006, CAST(N'2025-03-28T14:21:26.457' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2017, N'Lecturer.Delete', 1006, CAST(N'2025-03-28T14:21:40.483' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2018, N'ClassSchedule', NULL, CAST(N'2025-03-28T14:22:09.140' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2019, N'ClassSchedule.Create', 2018, CAST(N'2025-03-28T14:22:23.270' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2020, N'ClassSchedule.Edit', 2018, CAST(N'2025-03-28T14:22:45.637' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2021, N'ClassSchedule.Delete', 2018, CAST(N'2025-03-28T14:22:58.683' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2022, N'ClassShift', NULL, CAST(N'2025-03-28T14:23:17.843' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2023, N'ClassShift.Create', 2022, CAST(N'2025-03-28T14:23:41.730' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2024, N'ClassShift.Edit', 2022, CAST(N'2025-03-28T14:24:51.407' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2025, N'ClassShift.Delete', 2022, CAST(N'2025-03-28T14:25:04.450' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2026, N'Course', NULL, CAST(N'2025-03-28T14:25:20.003' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2027, N'Course.Create', 2026, CAST(N'2025-03-28T14:25:28.190' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2028, N'Course.Edit', 2026, CAST(N'2025-03-28T14:25:39.940' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2029, N'Course.Delete', 2026, CAST(N'2025-03-28T14:42:59.073' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2030, N'Course.View', 2026, CAST(N'2025-03-28T14:43:13.257' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2031, N'Class.View', 4, CAST(N'2025-03-28T14:43:34.840' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2032, N'Student.View', 1004, CAST(N'2025-03-28T14:43:52.350' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2033, N'Lecturer.View', 1006, CAST(N'2025-03-28T14:44:06.873' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2034, N'ClassSchedule.View', 2018, CAST(N'2025-03-28T14:44:38.313' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2035, N'ClassShift.View', 2022, CAST(N'2025-03-28T14:44:54.253' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2036, N'Faculty', NULL, CAST(N'2025-03-28T14:45:16.173' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2037, N'Faculty.Create', 2036, CAST(N'2025-03-28T14:45:23.900' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2038, N'Faculty.Edit', 2036, CAST(N'2025-03-28T14:45:33.620' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2039, N'Faculty.Delete', 2036, CAST(N'2025-03-28T14:45:44.607' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2040, N'Faculty.View', 2036, CAST(N'2025-03-28T14:45:57.297' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2041, N'RoleGroup', NULL, CAST(N'2025-03-28T14:46:28.477' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2042, N'RoleGroup.Create', 2041, CAST(N'2025-03-28T14:46:38.690' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2043, N'RoleGroup.Edit', 2041, CAST(N'2025-03-28T14:46:47.620' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2044, N'RoleGroup.Delete', 2041, CAST(N'2025-03-28T14:46:58.637' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2045, N'RoleGroup.View', 2041, CAST(N'2025-03-28T14:47:09.670' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2046, N'Room', NULL, CAST(N'2025-03-28T14:47:29.037' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2047, N'Room.Create', 2046, CAST(N'2025-03-28T14:47:37.790' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2048, N'Room.Edit', 2046, CAST(N'2025-03-28T14:47:46.620' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2049, N'Room.Delete', 2046, CAST(N'2025-03-28T14:47:58.143' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2050, N'Room.View', 2046, CAST(N'2025-03-28T14:48:06.480' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2051, N'Subject', NULL, CAST(N'2025-03-28T14:48:15.313' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2052, N'Subject.Create', 2051, CAST(N'2025-03-28T14:48:24.500' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2053, N'Subject.Edit', 2051, CAST(N'2025-03-28T14:48:33.573' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2054, N'Subject.Delete', 2051, CAST(N'2025-03-28T14:48:43.457' AS DateTime), 1)
INSERT [dbo].[Role] ([Id], [RoleCode], [ParentId], [CreateDay], [Status]) VALUES (2055, N'Subject.View', 2051, CAST(N'2025-03-28T14:48:51.773' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[RoleGroup] ON 

INSERT [dbo].[RoleGroup] ([Id], [NameRoleGroup], [CreateDay], [Status]) VALUES (3, N'Student', CAST(N'2025-03-21T15:18:00.613' AS DateTime), 1)
INSERT [dbo].[RoleGroup] ([Id], [NameRoleGroup], [CreateDay], [Status]) VALUES (4, N'Admin', CAST(N'2025-03-21T17:06:56.630' AS DateTime), 1)
INSERT [dbo].[RoleGroup] ([Id], [NameRoleGroup], [CreateDay], [Status]) VALUES (5, N'Lecturer', CAST(N'2025-03-21T17:08:52.677' AS DateTime), 1)
INSERT [dbo].[RoleGroup] ([Id], [NameRoleGroup], [CreateDay], [Status]) VALUES (1004, N'Quản lý nhân sự', CAST(N'2025-03-25T10:12:53.657' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[RoleGroup] OFF
GO
SET IDENTITY_INSERT [dbo].[RoleInRoleGroup] ON 

INSERT [dbo].[RoleInRoleGroup] ([Id], [RoleId], [RoleGroupId]) VALUES (6, 1005, 4)
INSERT [dbo].[RoleInRoleGroup] ([Id], [RoleId], [RoleGroupId]) VALUES (1006, 2009, 5)
INSERT [dbo].[RoleInRoleGroup] ([Id], [RoleId], [RoleGroupId]) VALUES (1007, 2010, 3)
INSERT [dbo].[RoleInRoleGroup] ([Id], [RoleId], [RoleGroupId]) VALUES (1008, 2008, 1004)
INSERT [dbo].[RoleInRoleGroup] ([Id], [RoleId], [RoleGroupId]) VALUES (1009, 1004, 1004)
INSERT [dbo].[RoleInRoleGroup] ([Id], [RoleId], [RoleGroupId]) VALUES (1010, 1006, 1004)
SET IDENTITY_INSERT [dbo].[RoleInRoleGroup] OFF
GO
SET IDENTITY_INSERT [dbo].[Room] ON 

INSERT [dbo].[Room] ([Id], [Name], [Building], [Floor], [Vacuity], [Status], [CreateDay]) VALUES (1, N'DTD 101', N'Đình trọng dật', N'1', 1, 1, CAST(N'2025-03-25T10:53:38.607' AS DateTime))
INSERT [dbo].[Room] ([Id], [Name], [Building], [Floor], [Vacuity], [Status], [CreateDay]) VALUES (2, N'DTD 102', N'Đình trọng dật', N'1', 1, 1, CAST(N'2025-03-25T10:56:11.880' AS DateTime))
SET IDENTITY_INSERT [dbo].[Room] OFF
GO
SET IDENTITY_INSERT [dbo].[Student] ON 

INSERT [dbo].[Student] ([Id], [FullName], [BirthDate], [Gender], [Address], [Email], [PhoneNumber], [ClassId], [CreateDay], [Status], [Image], [CCCD], [StudentCode]) VALUES (1006, N'Hieu Le', CAST(N'2025-03-08' AS Date), 1, N'AN Khe/Quynh Phu/ Thai Binh', N'lehieu18102k3@gmail.com', N'0397790753', 1, CAST(N'2025-03-21T17:01:15.740' AS DateTime), 1, N'/uploads/592c1c3e-2577-49a2-b166-8d43af35f52a.jpg', N'0321514', N'SV20251006')
INSERT [dbo].[Student] ([Id], [FullName], [BirthDate], [Gender], [Address], [Email], [PhoneNumber], [ClassId], [CreateDay], [Status], [Image], [CCCD], [StudentCode]) VALUES (2007, N'Hieu Le', CAST(N'2025-03-08' AS Date), 1, N'AN Khe/Quynh Phu/ Thai Binh', N'lehieu181023@gmail.com', N'0397790754', 1, CAST(N'2025-03-23T15:12:54.730' AS DateTime), 1, N'https://img.freepik.com/premium-vector/default-image-icon-vector-missing-picture-page-website-design-mobile-app-no-photo-available_87543-11093.jpg', N'0321514', N'SV20252007')
SET IDENTITY_INSERT [dbo].[Student] OFF
GO
SET IDENTITY_INSERT [dbo].[Subject] ON 

INSERT [dbo].[Subject] ([Id], [SubjectName], [Credit], [FacultyId], [CreateDay], [Status]) VALUES (2, N'Cơ sở dữ liệu', 3, 1, CAST(N'2025-03-20T15:08:57.383' AS DateTime), 1)
INSERT [dbo].[Subject] ([Id], [SubjectName], [Credit], [FacultyId], [CreateDay], [Status]) VALUES (3, N'Lập trình Php', 3, 1, CAST(N'2025-03-26T15:10:47.220' AS DateTime), 1)
INSERT [dbo].[Subject] ([Id], [SubjectName], [Credit], [FacultyId], [CreateDay], [Status]) VALUES (4, N'Cấu trúc dữ liệu và giải thuật', 3, 1, CAST(N'2025-03-26T15:24:59.700' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[Subject] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Account__536C85E4F81D9A31]    Script Date: 3/28/2025 4:21:49 PM ******/
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [UQ__Account__536C85E4F81D9A31] UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Lecturer__A9D105346CE22659]    Script Date: 3/28/2025 4:21:49 PM ******/
ALTER TABLE [dbo].[Lecturer] ADD  CONSTRAINT [UQ__Lecturer__A9D105346CE22659] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Lecturer__BB9CDAB39FAEF2BE]    Script Date: 3/28/2025 4:21:49 PM ******/
ALTER TABLE [dbo].[Lecturer] ADD UNIQUE NONCLUSTERED 
(
	[LecturerCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Student__1FC886046879061C]    Script Date: 3/28/2025 4:21:49 PM ******/
ALTER TABLE [dbo].[Student] ADD UNIQUE NONCLUSTERED 
(
	[StudentCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Student__A9D10534E0C6A5FE]    Script Date: 3/28/2025 4:21:49 PM ******/
ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [UQ__Student__A9D10534E0C6A5FE] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF__Account__CreateD__74AE54BC]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[Account] ADD  CONSTRAINT [DF__Account__Status__75A278F5]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Class] ADD  CONSTRAINT [DF__Class__CreateDay__4316F928]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[Class] ADD  CONSTRAINT [DF__Class__Status__440B1D61]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[ClassSchedule] ADD  CONSTRAINT [DF__ClassSche__Creat__4E1E9780]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[ClassSchedule] ADD  CONSTRAINT [DF__ClassSche__Statu__4F12BBB9]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[ClassShift] ADD  CONSTRAINT [DF__ClassShif__Creat__41B8C09B]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[ClassShift] ADD  CONSTRAINT [DF__ClassShif__Statu__42ACE4D4]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Course] ADD  CONSTRAINT [DF__Course__CreateDa__59063A47]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[Course] ADD  CONSTRAINT [DF__Course__Status__59FA5E80]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Faculty] ADD  CONSTRAINT [DF__Faculty__CreateD__3E52440B]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[Faculty] ADD  CONSTRAINT [DF__Faculty__Status__3F466844]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Lecturer] ADD  CONSTRAINT [DF__Lecturer__Create__4E88ABD4]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[Lecturer] ADD  CONSTRAINT [DF__Lecturer__Status__4F7CD00D]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_CreateDay]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[Role] ADD  CONSTRAINT [DF_Role_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[RoleGroup] ADD  CONSTRAINT [DF_RoleGroup_CreateDay]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[RoleGroup] ADD  CONSTRAINT [DF_RoleGroup_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Room] ADD  CONSTRAINT [DF_Room_Vacuity]  DEFAULT ((1)) FOR [Vacuity]
GO
ALTER TABLE [dbo].[Room] ADD  CONSTRAINT [DF_Room_Status]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Room] ADD  CONSTRAINT [DF_Room_CreateDay]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [DF__Student__CreateD__48CFD27E]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[Student] ADD  CONSTRAINT [DF__Student__Status__49C3F6B7]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Subject] ADD  CONSTRAINT [DF__Subject__CreateD__534D60F1]  DEFAULT (getdate()) FOR [CreateDay]
GO
ALTER TABLE [dbo].[Subject] ADD  CONSTRAINT [DF__Subject__Status__5441852A]  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK__Account__Lecture__73BA3083] FOREIGN KEY([LecturerId])
REFERENCES [dbo].[Lecturer] ([Id])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK__Account__Lecture__73BA3083]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK__Account__Student__72C60C4A] FOREIGN KEY([StudentId])
REFERENCES [dbo].[Student] ([Id])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK__Account__Student__72C60C4A]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_Account_RoleGroup] FOREIGN KEY([RoleGroupId])
REFERENCES [dbo].[RoleGroup] ([Id])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_Account_RoleGroup]
GO
ALTER TABLE [dbo].[Class]  WITH CHECK ADD  CONSTRAINT [FK__Class__FacultyId__4222D4EF] FOREIGN KEY([FacultyId])
REFERENCES [dbo].[Faculty] ([Id])
GO
ALTER TABLE [dbo].[Class] CHECK CONSTRAINT [FK__Class__FacultyId__4222D4EF]
GO
ALTER TABLE [dbo].[ClassSchedule]  WITH CHECK ADD  CONSTRAINT [FK_ClassSchedule_Class] FOREIGN KEY([ClassId])
REFERENCES [dbo].[Class] ([Id])
GO
ALTER TABLE [dbo].[ClassSchedule] CHECK CONSTRAINT [FK_ClassSchedule_Class]
GO
ALTER TABLE [dbo].[ClassSchedule]  WITH CHECK ADD  CONSTRAINT [FK_ClassSchedule_ClassShift] FOREIGN KEY([ClassShiftId])
REFERENCES [dbo].[ClassShift] ([Id])
GO
ALTER TABLE [dbo].[ClassSchedule] CHECK CONSTRAINT [FK_ClassSchedule_ClassShift]
GO
ALTER TABLE [dbo].[ClassSchedule]  WITH CHECK ADD  CONSTRAINT [FK_ClassSchedule_Course] FOREIGN KEY([CourseId])
REFERENCES [dbo].[Course] ([Id])
GO
ALTER TABLE [dbo].[ClassSchedule] CHECK CONSTRAINT [FK_ClassSchedule_Course]
GO
ALTER TABLE [dbo].[ClassSchedule]  WITH CHECK ADD  CONSTRAINT [FK_ClassSchedule_Room] FOREIGN KEY([RoomId])
REFERENCES [dbo].[Room] ([Id])
GO
ALTER TABLE [dbo].[ClassSchedule] CHECK CONSTRAINT [FK_ClassSchedule_Room]
GO
ALTER TABLE [dbo].[Course]  WITH CHECK ADD  CONSTRAINT [FK__Course__Lecturer__5812160E] FOREIGN KEY([LecturerId])
REFERENCES [dbo].[Lecturer] ([Id])
GO
ALTER TABLE [dbo].[Course] CHECK CONSTRAINT [FK__Course__Lecturer__5812160E]
GO
ALTER TABLE [dbo].[Course]  WITH CHECK ADD  CONSTRAINT [FK__Course__SubjectI__571DF1D5] FOREIGN KEY([SubjectId])
REFERENCES [dbo].[Subject] ([Id])
GO
ALTER TABLE [dbo].[Course] CHECK CONSTRAINT [FK__Course__SubjectI__571DF1D5]
GO
ALTER TABLE [dbo].[Lecturer]  WITH CHECK ADD  CONSTRAINT [FK__Lecturer__Facult__4D94879B] FOREIGN KEY([FacultyId])
REFERENCES [dbo].[Faculty] ([Id])
GO
ALTER TABLE [dbo].[Lecturer] CHECK CONSTRAINT [FK__Lecturer__Facult__4D94879B]
GO
ALTER TABLE [dbo].[RoleInRoleGroup]  WITH CHECK ADD  CONSTRAINT [FK_RoleInRoleGroup_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[RoleInRoleGroup] CHECK CONSTRAINT [FK_RoleInRoleGroup_Role]
GO
ALTER TABLE [dbo].[RoleInRoleGroup]  WITH CHECK ADD  CONSTRAINT [FK_RoleInRoleGroup_RoleGroup] FOREIGN KEY([RoleGroupId])
REFERENCES [dbo].[RoleGroup] ([Id])
GO
ALTER TABLE [dbo].[RoleInRoleGroup] CHECK CONSTRAINT [FK_RoleInRoleGroup_RoleGroup]
GO
ALTER TABLE [dbo].[Student]  WITH CHECK ADD  CONSTRAINT [FK__Student__ClassId__47DBAE45] FOREIGN KEY([ClassId])
REFERENCES [dbo].[Class] ([Id])
GO
ALTER TABLE [dbo].[Student] CHECK CONSTRAINT [FK__Student__ClassId__47DBAE45]
GO
ALTER TABLE [dbo].[Subject]  WITH CHECK ADD  CONSTRAINT [FK__Subject__Faculty__52593CB8] FOREIGN KEY([FacultyId])
REFERENCES [dbo].[Faculty] ([Id])
GO
ALTER TABLE [dbo].[Subject] CHECK CONSTRAINT [FK__Subject__Faculty__52593CB8]
GO
ALTER TABLE [dbo].[ClassSchedule]  WITH CHECK ADD  CONSTRAINT [CK__ClassSche__DayOf__4D2A7347] CHECK  (([DayOfWeek]>=(1) AND [DayOfWeek]<=(7)))
GO
ALTER TABLE [dbo].[ClassSchedule] CHECK CONSTRAINT [CK__ClassSche__DayOf__4D2A7347]
GO
USE [master]
GO
ALTER DATABASE [EastAsiaUOfT] SET  READ_WRITE 
GO
