USE [master]
GO
/****** Object:  Database [ScraperLinkedInDB]    Script Date: 9/13/2019 5:26:00 PM ******/
CREATE DATABASE [ScraperLinkedInDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ScraperLinkedInDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\ScraperLinkedInDB.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ScraperLinkedInDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\ScraperLinkedInDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [ScraperLinkedInDB] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ScraperLinkedInDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ScraperLinkedInDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ScraperLinkedInDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ScraperLinkedInDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ScraperLinkedInDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET ALLOW_SNAPSHOT_ISOLATION ON 
GO
ALTER DATABASE [ScraperLinkedInDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ScraperLinkedInDB] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [ScraperLinkedInDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET RECOVERY FULL 
GO
ALTER DATABASE [ScraperLinkedInDB] SET  MULTI_USER 
GO
ALTER DATABASE [ScraperLinkedInDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ScraperLinkedInDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ScraperLinkedInDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ScraperLinkedInDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ScraperLinkedInDB] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'ScraperLinkedInDB', N'ON'
GO
ALTER DATABASE [ScraperLinkedInDB] SET QUERY_STORE = OFF
GO
USE [ScraperLinkedInDB]
GO
/****** Object:  Table [dbo].[Companies]    Script Date: 9/13/2019 5:26:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrganizationName] [nvarchar](250) NULL,
	[OrganizationURL] [nvarchar](500) NULL,
	[Founders] [nvarchar](250) NULL,
	[HeadquartersLocation] [nvarchar](250) NULL,
	[Website] [nvarchar](150) NULL,
	[LinkedInURL] [nvarchar](500) NULL,
	[Facebook] [nvarchar](150) NULL,
	[Twitter] [nvarchar](150) NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[AmountEmployees] [nvarchar](50) NULL,
	[LogoUrl] [nvarchar](500) NULL,
	[Specialties] [nvarchar](max) NULL,
	[Industry] [nvarchar](max) NULL,
	[ExecutionStatusID] [int] NOT NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DebugLogs]    Script Date: 9/13/2019 5:26:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DebugLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Remarks] [nvarchar](250) NULL,
	[Logs] [nvarchar](max) NULL,
	[CreatedOn] [datetime] NULL,
 CONSTRAINT [PK_DebugLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = ON, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExecutionStatuses]    Script Date: 9/13/2019 5:26:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExecutionStatuses](
	[ExecutionStatusID] [int] NOT NULL,
	[StatusName] [nvarchar](75) NOT NULL,
 CONSTRAINT [PK_ExecutionStatuses] PRIMARY KEY CLUSTERED 
(
	[ExecutionStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Profiles]    Script Date: 9/13/2019 5:26:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Profiles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](125) NULL,
	[LastName] [nvarchar](125) NULL,
	[FullName] [nvarchar](250) NULL,
	[Job] [nvarchar](250) NULL,
	[ProfileUrl] [nvarchar](500) NULL,
	[AllSkills] [nvarchar](max) NULL,
	[CompanyID] [int] NOT NULL,
	[ExecutionStatusID] [int] NOT NULL,
	[ProfileStatusID] [int] NOT NULL,
	[DataСreation] [date] NOT NULL,
 CONSTRAINT [PK_Profiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProfileStatuses]    Script Date: 9/13/2019 5:26:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProfileStatuses](
	[ProfileStatusID] [int] NOT NULL,
	[StatusName] [nvarchar](75) NOT NULL,
 CONSTRAINT [PK_ProfileStatuses] PRIMARY KEY CLUSTERED 
(
	[ProfileStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SuitableProfiles]    Script Date: 9/13/2019 5:26:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SuitableProfiles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](125) NULL,
	[LastName] [nvarchar](125) NULL,
	[Job] [nvarchar](250) NULL,
	[PersonLinkedIn] [nvarchar](500) NULL,
	[Company] [nvarchar](250) NULL,
	[Website] [nvarchar](150) NULL,
	[CompanyLogoUrl] [nvarchar](500) NULL,
	[CrunchUrl] [nvarchar](500) NULL,
	[Email] [nvarchar](50) NULL,
	[EmailStatus] [nvarchar](10) NULL,
	[City] [nvarchar](75) NULL,
	[State] [nvarchar](75) NULL,
	[Country] [nvarchar](75) NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[AmountEmployees] [nvarchar](50) NULL,
	[Industry] [nvarchar](max) NULL,
	[Twitter] [nvarchar](150) NULL,
	[Facebook] [nvarchar](150) NULL,
	[TechStack] [nvarchar](max) NULL,
 CONSTRAINT [PK_SuitableProfiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Companies] ADD  CONSTRAINT [DF_Companies_Status]  DEFAULT ((0)) FOR [ExecutionStatusID]
GO
ALTER TABLE [dbo].[DebugLogs] ADD  CONSTRAINT [DF_DebugLogs_Remarks]  DEFAULT ('') FOR [Remarks]
GO
ALTER TABLE [dbo].[DebugLogs] ADD  CONSTRAINT [DF_DebugLogs_Logs]  DEFAULT ('') FOR [Logs]
GO
ALTER TABLE [dbo].[DebugLogs] ADD  CONSTRAINT [DF_DebugLogs_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[Profiles] ADD  CONSTRAINT [DF_Profiles_Status]  DEFAULT ((0)) FOR [ExecutionStatusID]
GO
ALTER TABLE [dbo].[Profiles] ADD  CONSTRAINT [DF_Profiles_ProfileStatusID]  DEFAULT ((0)) FOR [ProfileStatusID]
GO
ALTER TABLE [dbo].[Profiles] ADD  CONSTRAINT [DF_Profiles_ScrapData]  DEFAULT (getdate()) FOR [DataСreation]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_FirstName]  DEFAULT ('...') FOR [FirstName]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_LastName]  DEFAULT ('...') FOR [LastName]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_Job]  DEFAULT ('...') FOR [Job]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_PersonLinkedIn]  DEFAULT ('...') FOR [PersonLinkedIn]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_Company]  DEFAULT ('...') FOR [Company]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_Website]  DEFAULT ('...') FOR [Website]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_CompanyLogoUrl]  DEFAULT ('...') FOR [CompanyLogoUrl]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_CrunchUrl]  DEFAULT ('...') FOR [CrunchUrl]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_Email]  DEFAULT ('...') FOR [Email]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_EmailStatus]  DEFAULT ('...') FOR [EmailStatus]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_City]  DEFAULT ('...') FOR [City]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_State]  DEFAULT ('...') FOR [State]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_Country]  DEFAULT ('...') FOR [Country]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_PhoneNumber]  DEFAULT ('...') FOR [PhoneNumber]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_AmountEmployees]  DEFAULT ('...') FOR [AmountEmployees]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_Industry]  DEFAULT ('...') FOR [Industry]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_Twitter]  DEFAULT ('...') FOR [Twitter]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_Facebook]  DEFAULT ('...') FOR [Facebook]
GO
ALTER TABLE [dbo].[SuitableProfiles] ADD  CONSTRAINT [DF_SuitableProfiles_TechStack]  DEFAULT ('...') FOR [TechStack]
GO
ALTER TABLE [dbo].[Companies]  WITH CHECK ADD  CONSTRAINT [FK_Companies_ExecutionStatuses] FOREIGN KEY([ExecutionStatusID])
REFERENCES [dbo].[ExecutionStatuses] ([ExecutionStatusID])
GO
ALTER TABLE [dbo].[Companies] CHECK CONSTRAINT [FK_Companies_ExecutionStatuses]
GO
ALTER TABLE [dbo].[Profiles]  WITH CHECK ADD  CONSTRAINT [FK_Profiles_Companies] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[Companies] ([Id])
GO
ALTER TABLE [dbo].[Profiles] CHECK CONSTRAINT [FK_Profiles_Companies]
GO
ALTER TABLE [dbo].[Profiles]  WITH CHECK ADD  CONSTRAINT [FK_Profiles_ExecutionStatuses] FOREIGN KEY([ExecutionStatusID])
REFERENCES [dbo].[ExecutionStatuses] ([ExecutionStatusID])
GO
ALTER TABLE [dbo].[Profiles] CHECK CONSTRAINT [FK_Profiles_ExecutionStatuses]
GO
ALTER TABLE [dbo].[Profiles]  WITH CHECK ADD  CONSTRAINT [FK_Profiles_ProfileStatuses] FOREIGN KEY([ProfileStatusID])
REFERENCES [dbo].[ProfileStatuses] ([ProfileStatusID])
GO
ALTER TABLE [dbo].[Profiles] CHECK CONSTRAINT [FK_Profiles_ProfileStatuses]
GO
USE [master]
GO
ALTER DATABASE [ScraperLinkedInDB] SET  READ_WRITE 
GO
