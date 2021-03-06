USE [master]
GO
/****** Object:  Database [TaskMgr]    Script Date: 2/6/2018 10:15:27 AM ******/
CREATE DATABASE [TaskMgr]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TaskMgr', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\TaskMgr.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'TaskMgr_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\TaskMgr_log.ldf' , SIZE = 2816KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [TaskMgr] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TaskMgr].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TaskMgr] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TaskMgr] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TaskMgr] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TaskMgr] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TaskMgr] SET ARITHABORT OFF 
GO
ALTER DATABASE [TaskMgr] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TaskMgr] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TaskMgr] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TaskMgr] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TaskMgr] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TaskMgr] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TaskMgr] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TaskMgr] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TaskMgr] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TaskMgr] SET  DISABLE_BROKER 
GO
ALTER DATABASE [TaskMgr] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TaskMgr] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TaskMgr] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TaskMgr] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TaskMgr] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TaskMgr] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TaskMgr] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TaskMgr] SET RECOVERY FULL 
GO
ALTER DATABASE [TaskMgr] SET  MULTI_USER 
GO
ALTER DATABASE [TaskMgr] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TaskMgr] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TaskMgr] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TaskMgr] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [TaskMgr] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'TaskMgr', N'ON'
GO
USE [TaskMgr]
GO
/****** Object:  User [TaskMgrWebAppUser]    Script Date: 2/6/2018 10:15:27 AM ******/
CREATE USER [TaskMgrWebAppUser] FOR LOGIN [TaskMgrWebAppUser] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [TaskMgrWebAppUser]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [TaskMgrWebAppUser]
GO
/****** Object:  Table [dbo].[KeyValues]    Script Date: 2/6/2018 10:15:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KeyValues](
	[Key] [nvarchar](20) NOT NULL,
	[DTVal] [datetime] NULL,
	[IntVal] [int] NULL,
	[StrVal] [nvarchar](200) NULL,
	[FloatVal] [float] NULL,
 CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Queues]    Script Date: 2/6/2018 10:15:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Queues](
	[QueueId] [int] IDENTITY(1,1) NOT NULL,
	[ScheduleId] [int] NOT NULL,
	[ScheduledStart] [datetime] NOT NULL,
	[Completed] [datetime] NULL,
	[Status] [nvarchar](15) NULL,
	[Suspended] [bit] NOT NULL,
	[Cancelled] [bit] NOT NULL,
 CONSTRAINT [PK_Queue] PRIMARY KEY CLUSTERED 
(
	[QueueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QueueSteps]    Script Date: 2/6/2018 10:15:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QueueSteps](
	[QueueStepId] [int] IDENTITY(1,1) NOT NULL,
	[QueueId] [int] NOT NULL,
	[Seq] [int] NOT NULL,
	[StepId] [int] NULL,
	[Added] [datetime] NULL,
	[ExecutionStarted] [datetime] NULL,
	[ExecutionCompleted] [datetime] NULL,
	[ReturnValues] [nvarchar](max) NULL,
	[FailureInfo] [nvarchar](max) NULL,
	[Status] [nvarchar](15) NULL,
	[LastExecutionSuspended] [datetime] NULL,
	[PostExecutionDecision] [nvarchar](15) NULL,
 CONSTRAINT [PK_QueueSteps] PRIMARY KEY CLUSTERED 
(
	[QueueStepId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Schedules]    Script Date: 2/6/2018 10:15:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Schedules](
	[ScheduleId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[TaskId] [int] NOT NULL,
	[Freq] [nvarchar](10) NOT NULL,
	[Start] [datetime] NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[Disabled] [bit] NOT NULL,
	[NextQueue] [datetime] NULL,
	[EndDt] [datetime] NULL,
	[PeriodInterval] [int] NULL,
	[IsCompleted] [bit] NOT NULL,
 CONSTRAINT [PK_Schedules] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Steps]    Script Date: 2/6/2018 10:15:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Steps](
	[StepId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Class] [nvarchar](100) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
 CONSTRAINT [PK_Steps] PRIMARY KEY CLUSTERED 
(
	[StepId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tasks]    Script Date: 2/6/2018 10:15:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tasks](
	[TaskId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NULL,
	[StartedEmails] [nvarchar](1000) NULL,
	[CompletedEmails] [nvarchar](1000) NULL,
	[FailureEmails] [nvarchar](1000) NULL,
	[EmailsOnStepStart] [bit] NOT NULL,
	[EmailsOnStepComplete] [bit] NOT NULL,
	[IsValid] [bit] NOT NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[TaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TaskSteps]    Script Date: 2/6/2018 10:15:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TaskSteps](
	[TaskStepId] [int] IDENTITY(1,1) NOT NULL,
	[TaskId] [int] NOT NULL,
	[Seq] [int] NOT NULL,
	[StepId] [int] NULL,
	[PostExecutionDecision] [nvarchar](15) NULL,
	[GotoSeq] [int] NULL,
 CONSTRAINT [PK_TaskSteps] PRIMARY KEY CLUSTERED 
(
	[TaskStepId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UniqueScheduleNames]    Script Date: 2/6/2018 10:15:27 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UniqueScheduleNames] ON [dbo].[Schedules]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UniqueStepNames]    Script Date: 2/6/2018 10:15:27 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UniqueStepNames] ON [dbo].[Steps]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UniqueTaskNames]    Script Date: 2/6/2018 10:15:27 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UniqueTaskNames] ON [dbo].[Tasks]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Queues]  WITH CHECK ADD  CONSTRAINT [FK_Queue_Schedules] FOREIGN KEY([ScheduleId])
REFERENCES [dbo].[Schedules] ([ScheduleId])
GO
ALTER TABLE [dbo].[Queues] CHECK CONSTRAINT [FK_Queue_Schedules]
GO
ALTER TABLE [dbo].[QueueSteps]  WITH CHECK ADD  CONSTRAINT [FK_QueueSteps_Queue] FOREIGN KEY([QueueId])
REFERENCES [dbo].[Queues] ([QueueId])
GO
ALTER TABLE [dbo].[QueueSteps] CHECK CONSTRAINT [FK_QueueSteps_Queue]
GO
ALTER TABLE [dbo].[QueueSteps]  WITH CHECK ADD  CONSTRAINT [FK_QueueSteps_Steps] FOREIGN KEY([StepId])
REFERENCES [dbo].[Steps] ([StepId])
GO
ALTER TABLE [dbo].[QueueSteps] CHECK CONSTRAINT [FK_QueueSteps_Steps]
GO
ALTER TABLE [dbo].[Schedules]  WITH CHECK ADD  CONSTRAINT [FK_Schedules_Tasks] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Tasks] ([TaskId])
GO
ALTER TABLE [dbo].[Schedules] CHECK CONSTRAINT [FK_Schedules_Tasks]
GO
ALTER TABLE [dbo].[TaskSteps]  WITH CHECK ADD  CONSTRAINT [FK_TaskSteps_Steps] FOREIGN KEY([StepId])
REFERENCES [dbo].[Steps] ([StepId])
GO
ALTER TABLE [dbo].[TaskSteps] CHECK CONSTRAINT [FK_TaskSteps_Steps]
GO
ALTER TABLE [dbo].[TaskSteps]  WITH CHECK ADD  CONSTRAINT [FK_TaskSteps_Tasks] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Tasks] ([TaskId])
GO
ALTER TABLE [dbo].[TaskSteps] CHECK CONSTRAINT [FK_TaskSteps_Tasks]
GO
USE [master]
GO
ALTER DATABASE [TaskMgr] SET  READ_WRITE 
GO
