USE [master]
GO
/****** Object:  Database [LuxuryFlightV11]    Script Date: 6/30/2025 4:45:08 PM ******/
CREATE DATABASE [LuxuryFlightV11]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'LuxuryFlightV11', FILENAME = N'D:\SQLServer\Database\LuxuryFlightV11.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'LuxuryFlightV11_log', FILENAME = N'D:\SQLServer\Database\LuxuryFlightV11_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [LuxuryFlightV11] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [LuxuryFlightV11].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [LuxuryFlightV11] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET ARITHABORT OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [LuxuryFlightV11] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [LuxuryFlightV11] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [LuxuryFlightV11] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET  ENABLE_BROKER 
GO
ALTER DATABASE [LuxuryFlightV11] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [LuxuryFlightV11] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [LuxuryFlightV11] SET  MULTI_USER 
GO
ALTER DATABASE [LuxuryFlightV11] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [LuxuryFlightV11] SET DB_CHAINING OFF 
GO
ALTER DATABASE [LuxuryFlightV11] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [LuxuryFlightV11] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [LuxuryFlightV11] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [LuxuryFlightV11] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [LuxuryFlightV11] SET QUERY_STORE = ON
GO
ALTER DATABASE [LuxuryFlightV11] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [LuxuryFlightV11]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[account_id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](50) NOT NULL,
	[password] [varchar](250) NOT NULL,
	[role_id] [int] NOT NULL,
	[status_id] [int] NOT NULL,
	[refresh_token] [nvarchar](250) NULL,
	[refresh_token_expiry_time] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[account_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Admin]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[admin_id] [int] IDENTITY(1,1) NOT NULL,
	[fullname] [nvarchar](50) NOT NULL,
	[address] [nvarchar](255) NOT NULL,
	[phone_number] [varchar](17) NOT NULL,
	[email] [varchar](50) NOT NULL,
	[account_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[admin_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Airport]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Airport](
	[airport_id] [int] IDENTITY(1,1) NOT NULL,
	[airport_code] [varchar](5) NOT NULL,
	[airport_name] [nvarchar](50) NOT NULL,
	[city] [nvarchar](255) NOT NULL,
	[manager_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[airport_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AirportPrice]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AirportPrice](
	[airport_price_id] [int] IDENTITY(1,1) NOT NULL,
	[airport_from_id] [int] NOT NULL,
	[airport_to_id] [int] NOT NULL,
	[base_price] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[airport_price_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClassSeat]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClassSeat](
	[class_id] [int] IDENTITY(1,1) NOT NULL,
	[class_name] [varchar](50) NOT NULL,
	[price] [decimal](10, 2) NOT NULL,
	[description] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[class_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Complaint]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Complaint](
	[complaint_id] [int] IDENTITY(1,1) NOT NULL,
	[status_id] [int] NOT NULL,
	[supporter_id] [int] NOT NULL,
	[customer_id] [int] NOT NULL,
	[create_at] [datetime] NULL,
	[description] [nvarchar](255) NOT NULL,
	[file_type] [nvarchar](50) NULL,
	[file_url] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[complaint_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[customer_id] [int] IDENTITY(1,1) NOT NULL,
	[fullname] [nvarchar](50) NOT NULL,
	[address] [nvarchar](255) NOT NULL,
	[phone_number] [varchar](17) NOT NULL,
	[email] [varchar](50) NOT NULL,
	[account_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[customer_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Discount]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Discount](
	[discount_id] [int] IDENTITY(1,1) NOT NULL,
	[discount_code] [nvarchar](50) NOT NULL,
	[discount_percent] [decimal](5, 2) NOT NULL,
	[discount_title] [nvarchar](255) NOT NULL,
	[discount_infor] [nvarchar](max) NULL,
	[status] [tinyint] NULL,
	[customer_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[discount_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Feedback]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Feedback](
	[feedback_id] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](255) NOT NULL,
	[rate] [int] NOT NULL,
	[content] [nvarchar](max) NOT NULL,
	[create_at] [date] NULL,
	[account_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[feedback_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Flight]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Flight](
	[flight_id] [int] IDENTITY(1,1) NOT NULL,
	[flight_code] [varchar](50) NOT NULL,
	[tax] [decimal](4, 2) NOT NULL,
	[status_id] [int] NOT NULL,
	[departure_time] [datetime] NOT NULL,
	[arrival_time] [datetime] NOT NULL,
	[plane_id] [int] NOT NULL,
	[manager_id] [int] NOT NULL,
	[customer_id] [int] NOT NULL,
	[departure_airport_id] [int] NOT NULL,
	[arrival_airport_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[flight_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Flight_Seat]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Flight_Seat](
	[flight_id] [int] NOT NULL,
	[seat_id] [int] NOT NULL,
	[isSat] [bit] NULL,
	[ticket_id] [int] NULL,
 CONSTRAINT [PK_Flight_Seat] PRIMARY KEY CLUSTERED 
(
	[flight_id] ASC,
	[seat_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FlightService]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FlightService](
	[flight_id] [int] NOT NULL,
	[service_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[flight_id] ASC,
	[service_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Items]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Items](
	[item_id] [int] IDENTITY(1,1) NOT NULL,
	[item_name] [nvarchar](50) NOT NULL,
	[detail] [nvarchar](100) NULL,
	[price] [int] NOT NULL,
	[status_id] [int] NULL,
	[image] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[item_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Manager]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Manager](
	[manager_id] [int] IDENTITY(1,1) NOT NULL,
	[fullname] [nvarchar](50) NOT NULL,
	[address] [nvarchar](255) NOT NULL,
	[phone_number] [varchar](17) NOT NULL,
	[email] [varchar](50) NOT NULL,
	[account_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[manager_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[News]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[News](
	[new_id] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](255) NOT NULL,
	[image] [nvarchar](255) NULL,
	[content] [nvarchar](max) NOT NULL,
	[category] [nvarchar](50) NOT NULL,
	[author] [nvarchar](50) NOT NULL,
	[account_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[new_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Notification]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Notification](
	[notification_id] [int] IDENTITY(1,1) NOT NULL,
	[account_id] [int] NOT NULL,
	[title] [nvarchar](255) NOT NULL,
	[content] [nvarchar](255) NOT NULL,
	[create_at] [datetime] NULL,
	[status_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[notification_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[payment_id] [int] IDENTITY(1,1) NOT NULL,
	[price] [int] NOT NULL,
	[payment_method] [varchar](50) NOT NULL,
	[ticket_id] [int] NOT NULL,
	[payment_date] [date] NOT NULL,
	[status_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[payment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Plane]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Plane](
	[plane_id] [int] IDENTITY(1,1) NOT NULL,
	[plane_code] [varchar](50) NOT NULL,
	[model] [varchar](50) NOT NULL,
	[status_id] [int] NOT NULL,
	[manufacture] [varchar](100) NOT NULL,
	[year_of_manufacture] [int] NOT NULL,
	[manager_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[plane_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[role_id] [int] IDENTITY(1,1) NOT NULL,
	[role_name] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Seat]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Seat](
	[seat_id] [int] IDENTITY(1,1) NOT NULL,
	[seat_number] [nvarchar](10) NOT NULL,
	[class_id] [int] NOT NULL,
	[status_id] [int] NOT NULL,
	[plane_id] [int] NULL,
 CONSTRAINT [PK__Seat__906DED9C118FEE73] PRIMARY KEY CLUSTERED 
(
	[seat_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Service]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Service](
	[service_id] [int] IDENTITY(1,1) NOT NULL,
	[service_name] [nvarchar](50) NOT NULL,
	[detail] [nvarchar](100) NULL,
	[manager_id] [int] NOT NULL,
	[status_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[service_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Service_Item]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Service_Item](
	[service_id] [int] NOT NULL,
	[item_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[service_id] ASC,
	[item_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Status]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status](
	[status_id] [int] IDENTITY(1,1) NOT NULL,
	[status_name] [nvarchar](50) NOT NULL,
	[status_type] [char](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[status_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Supporter]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Supporter](
	[supporter_id] [int] IDENTITY(1,1) NOT NULL,
	[fullname] [nvarchar](50) NOT NULL,
	[address] [nvarchar](255) NOT NULL,
	[phone_number] [varchar](17) NOT NULL,
	[email] [varchar](50) NOT NULL,
	[account_id] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[supporter_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ticket]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ticket](
	[ticket_id] [int] IDENTITY(1,1) NOT NULL,
	[ticket_number] [nvarchar](20) NOT NULL,
	[flight_id] [int] NOT NULL,
	[status_id] [int] NOT NULL,
	[booking_date] [date] NOT NULL,
	[customer_id] [int] NULL,
	[total_price] [decimal](18, 0) NOT NULL,
	[gender] [nvarchar](10) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[date_of_birth] [date] NOT NULL,
	[full_name] [nvarchar](100) NOT NULL,
	[class_seat_id] [int] NOT NULL,
	[contact_full_name] [nvarchar](100) NULL,
	[contact_phone] [nvarchar](10) NULL,
	[contact_email] [nvarchar](100) NULL,
	[contact_address] [nvarchar](255) NULL,
 CONSTRAINT [PK__Ticket__D596F96BCC0A6FCE] PRIMARY KEY CLUSTERED 
(
	[ticket_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Ticket_Item]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Ticket_Item](
	[ticket_id] [int] NOT NULL,
	[item_id] [int] NOT NULL,
	[quantity] [int] NULL,
 CONSTRAINT [PK_Ticket_Item] PRIMARY KEY CLUSTERED 
(
	[ticket_id] ASC,
	[item_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User_Logs]    Script Date: 6/30/2025 4:45:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User_Logs](
	[log_id] [int] IDENTITY(1,1) NOT NULL,
	[account_id] [int] NOT NULL,
	[detail] [nvarchar](50) NULL,
	[access_time] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[log_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Account] ON 

INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (1, N'ad', N'123', 1, 1, N'8af48e1b-76ed-4300-aa47-f9ef0eabd899', CAST(N'2025-06-30T05:15:32.3885368' AS DateTime2))
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (2, N'sp', N'123', 2, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (3, N'cs', N'123', 3, 1, N'4d14ae4d-343f-4f58-b90b-a87875ad17b1', CAST(N'2025-06-30T14:06:16.8374861' AS DateTime2))
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (4, N'mn', N'Nhat6464@', 4, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (5, N'manager1', N'123', 4, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (6, N'manager2', N'123', 4, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (7, N'manager3', N'123', 4, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (8, N'support1', N'123', 2, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (9, N'support2', N'123', 2, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (10, N'customer1', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (11, N'customer2', N'123', 3, 2, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (12, N'customer3', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (13, N'customer4', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (14, N'customer5', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (15, N'customer6', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (16, N'customer7', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (17, N'customer8', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (18, N'customer9', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (19, N'customer10', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (20, N'customer11', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (21, N'customer12', N'123', 3, 2, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (22, N'customer13', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (23, N'customer14', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (24, N'customer15', N'123', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (25, N'minh123', N'8e7855a30a2e8dd1d71b4d58ed9de467c379a12d9f47fed887af7bc8e9946b2d', 4, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (26, N'nhat123', N'8e7855a30a2e8dd1d71b4d58ed9de467c379a12d9f47fed887af7bc8e9946b2d', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (27, N'csvu', N'0f85ac1095ec51a63131338b6a3be6c5662e38a670a06aa7b409c9088faf5fb6', 1, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (28, N'csvu1', N'cfb4f6ae3ebc611aeb1d85c4926991d4982195dc938afe5cd497246d33429b9b', 3, 1, NULL, NULL)
INSERT [dbo].[Account] ([account_id], [username], [password], [role_id], [status_id], [refresh_token], [refresh_token_expiry_time]) VALUES (29, N'mn1', N'0865299150', 4, 1, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Account] OFF
GO
SET IDENTITY_INSERT [dbo].[Admin] ON 

INSERT [dbo].[Admin] ([admin_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (1, N'Đào Thanh Nhất', N'Đầu đường xó chợ', N'0901234567', N'minhdao3444@gmail.com', 1)
SET IDENTITY_INSERT [dbo].[Admin] OFF
GO
SET IDENTITY_INSERT [dbo].[Airport] ON 

INSERT [dbo].[Airport] ([airport_id], [airport_code], [airport_name], [city], [manager_id]) VALUES (1, N'HAN', N'Noi Bai International Airport', N'Hanoi', 1)
INSERT [dbo].[Airport] ([airport_id], [airport_code], [airport_name], [city], [manager_id]) VALUES (2, N'SGN', N'Tan Son Nhat International Airport', N'Ho Chi Minh City', 1)
INSERT [dbo].[Airport] ([airport_id], [airport_code], [airport_name], [city], [manager_id]) VALUES (3, N'DNG', N'Da Nang International Airport', N'Da Nang', 1)
INSERT [dbo].[Airport] ([airport_id], [airport_code], [airport_name], [city], [manager_id]) VALUES (4, N'PQA', N'Phu Quoc International Airport', N'Phu Quoc', 1)
INSERT [dbo].[Airport] ([airport_id], [airport_code], [airport_name], [city], [manager_id]) VALUES (5, N'CRA', N'Cam Ranh International Airport', N'Nha Trang', 1)
SET IDENTITY_INSERT [dbo].[Airport] OFF
GO
SET IDENTITY_INSERT [dbo].[AirportPrice] ON 

INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (1, 1, 3, CAST(2000000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (2, 3, 1, CAST(2000000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (3, 1, 2, CAST(1500000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (4, 2, 1, CAST(1500000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (5, 2, 3, CAST(1800000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (6, 3, 2, CAST(1800000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (7, 1, 4, CAST(2500000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (8, 4, 1, CAST(2500000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (9, 2, 4, CAST(2200000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (10, 4, 2, CAST(2200000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (11, 3, 4, CAST(2300000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (12, 4, 3, CAST(2300000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (13, 1, 5, CAST(2300000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (14, 2, 5, CAST(1900000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (15, 3, 5, CAST(2000000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (16, 4, 5, CAST(2100000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (17, 5, 4, CAST(2100000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (18, 5, 1, CAST(2300000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (19, 5, 2, CAST(1900000.00 AS Decimal(10, 2)))
INSERT [dbo].[AirportPrice] ([airport_price_id], [airport_from_id], [airport_to_id], [base_price]) VALUES (20, 5, 3, CAST(2000000.00 AS Decimal(10, 2)))
SET IDENTITY_INSERT [dbo].[AirportPrice] OFF
GO
SET IDENTITY_INSERT [dbo].[ClassSeat] ON 

INSERT [dbo].[ClassSeat] ([class_id], [class_name], [price], [description]) VALUES (1, N'SkyElite', CAST(500000.00 AS Decimal(10, 2)), N'Highest level of luxury with top-tier amenities.')
INSERT [dbo].[ClassSeat] ([class_id], [class_name], [price], [description]) VALUES (2, N'SkyPrime', CAST(300000.00 AS Decimal(10, 2)), N'Premium class with superior comfort and service.')
INSERT [dbo].[ClassSeat] ([class_id], [class_name], [price], [description]) VALUES (3, N'Economy', CAST(100000.00 AS Decimal(10, 2)), N'Affordable and standard travel class.')
SET IDENTITY_INSERT [dbo].[ClassSeat] OFF
GO
SET IDENTITY_INSERT [dbo].[Customer] ON 

INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (1, N'Đức Thịnh', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 3)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (2, N'Đức Trần', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 10)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (3, N'Thịnh Nguyễn', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 11)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (4, N'Nam Cường', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 12)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (5, N'Cường Bùi', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 13)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (6, N'Nam Hà', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 14)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (7, N'Văn Liêm', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 15)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (8, N'Cường Bùi', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 16)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (9, N'Nam Hà', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 17)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (10, N'Văn Liêm', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 18)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (11, N'Lê Lan', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 19)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (12, N'Hà Bắc', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 20)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (13, N'Chi Nguyễn', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 21)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (14, N'Trang Hà', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 22)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (15, N'Vân Loan', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 23)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (16, N'Hạnh Nguyễn', N'Góc Nhỏ Cuối Phố', N'0923456789', N'cs@example.com', 24)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (17, N'Nhat Minh Dao', N'Triệu Sơn', N'0961442230', N'leanh65345@gmail.com', 25)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (18, N'Nhat Minh Dao', N'Triệu Sơn', N'0961442230', N'nguyenbuinhatls31@gmail.com', 26)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (19, N'Vuu', N'Lang Son', N'0121212', N'vuvien73@gmail.com', 27)
INSERT [dbo].[Customer] ([customer_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (20, N'Tran Quang Khai', N'Lang son', N'0123456789', N'nhucute4953@gmail.com', 28)
SET IDENTITY_INSERT [dbo].[Customer] OFF
GO
SET IDENTITY_INSERT [dbo].[Discount] ON 

INSERT [dbo].[Discount] ([discount_id], [discount_code], [discount_percent], [discount_title], [discount_infor], [status], [customer_id]) VALUES (1, N'SUMMER2025', CAST(0.15 AS Decimal(5, 2)), N'Giảm giá mùa hè', N'Áp dụng cho tất cả đơn hàng trên 500k.', 1, 1)
INSERT [dbo].[Discount] ([discount_id], [discount_code], [discount_percent], [discount_title], [discount_infor], [status], [customer_id]) VALUES (2, N'NEWYEAR2025', CAST(0.20 AS Decimal(5, 2)), N'Giảm giá năm mới', N'Áp dụng cho thành viên VIP.', 1, 1)
SET IDENTITY_INSERT [dbo].[Discount] OFF
GO
SET IDENTITY_INSERT [dbo].[Feedback] ON 

INSERT [dbo].[Feedback] ([feedback_id], [title], [rate], [content], [create_at], [account_id]) VALUES (1, N'Nice trip', 5, N'Chuyến bay rẻ và tiện lợi kk', CAST(N'2025-04-13' AS Date), 3)
INSERT [dbo].[Feedback] ([feedback_id], [title], [rate], [content], [create_at], [account_id]) VALUES (2, N'Good trip', 4, N'Chuyến bay rẻ', CAST(N'2025-02-27' AS Date), 10)
INSERT [dbo].[Feedback] ([feedback_id], [title], [rate], [content], [create_at], [account_id]) VALUES (3, N'Convenient trip', 5, N'Chuyến bay tiện lợi', CAST(N'2025-02-28' AS Date), 11)
INSERT [dbo].[Feedback] ([feedback_id], [title], [rate], [content], [create_at], [account_id]) VALUES (4, N'Good', 5, N'Chuyến bay tuyệt vời', CAST(N'2025-03-03' AS Date), 12)
INSERT [dbo].[Feedback] ([feedback_id], [title], [rate], [content], [create_at], [account_id]) VALUES (5, N'Nice', 5, N'Dịch vụ tận tâm', CAST(N'2025-03-05' AS Date), 13)
INSERT [dbo].[Feedback] ([feedback_id], [title], [rate], [content], [create_at], [account_id]) VALUES (6, N'Happy', 5, N'Tôi rất hài lòng', CAST(N'2025-03-06' AS Date), 14)
INSERT [dbo].[Feedback] ([feedback_id], [title], [rate], [content], [create_at], [account_id]) VALUES (7, N'Happy trip', 5, N'Chuyến bay rất tuyệt', CAST(N'2025-03-06' AS Date), 15)
SET IDENTITY_INSERT [dbo].[Feedback] OFF
GO
SET IDENTITY_INSERT [dbo].[Flight] ON 

INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (1, N'1000845601', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-09T07:30:00.000' AS DateTime), CAST(N'2025-03-09T09:30:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (2, N'1000845602', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-10T13:00:00.000' AS DateTime), CAST(N'2025-03-10T15:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (3, N'1000845603', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-11T09:30:00.000' AS DateTime), CAST(N'2025-03-11T11:30:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (4, N'1000845604', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-12T13:00:00.000' AS DateTime), CAST(N'2025-03-12T15:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (5, N'1000845605', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-13T10:30:00.000' AS DateTime), CAST(N'2025-03-13T12:30:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (6, N'1000845606', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-14T14:00:00.000' AS DateTime), CAST(N'2025-03-14T16:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (7, N'1000845607', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-15T15:30:00.000' AS DateTime), CAST(N'2025-03-15T17:30:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (8, N'1000845608', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-16T16:00:00.000' AS DateTime), CAST(N'2025-03-16T18:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (9, N'1000845609', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-17T08:30:00.000' AS DateTime), CAST(N'2025-03-17T10:30:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (10, N'1000845610', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-18T09:00:00.000' AS DateTime), CAST(N'2025-03-18T11:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (11, N'1000845611', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-19T07:30:00.000' AS DateTime), CAST(N'2025-03-19T09:30:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (12, N'1000845612', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-20T13:00:00.000' AS DateTime), CAST(N'2025-03-20T15:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (13, N'1000845613', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-21T09:00:00.000' AS DateTime), CAST(N'2025-03-21T11:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (14, N'1000845614', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-22T07:30:00.000' AS DateTime), CAST(N'2025-03-22T09:30:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (15, N'1000845615', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-23T13:00:00.000' AS DateTime), CAST(N'2025-03-23T15:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (16, N'1000845616', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-24T09:00:00.000' AS DateTime), CAST(N'2025-03-24T11:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (17, N'1000845617', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-25T07:30:00.000' AS DateTime), CAST(N'2025-03-25T09:30:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (18, N'1000845618', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-26T13:00:00.000' AS DateTime), CAST(N'2025-03-26T15:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (19, N'1000845619', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-27T09:00:00.000' AS DateTime), CAST(N'2025-03-27T11:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (20, N'1000845620', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-28T07:30:00.000' AS DateTime), CAST(N'2025-03-28T09:30:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (21, N'1000845621', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-29T13:00:00.000' AS DateTime), CAST(N'2025-03-29T15:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (22, N'1000845622', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-30T09:00:00.000' AS DateTime), CAST(N'2025-03-30T11:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (23, N'1000845623', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-31T07:30:00.000' AS DateTime), CAST(N'2025-03-31T09:30:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (24, N'1000845624', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-04-01T13:00:00.000' AS DateTime), CAST(N'2025-04-01T15:00:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (25, N'1000845625', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-09T07:30:00.000' AS DateTime), CAST(N'2025-03-09T09:30:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (26, N'1000845626', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-10T13:00:00.000' AS DateTime), CAST(N'2025-03-10T15:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (27, N'1000845627', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-11T09:30:00.000' AS DateTime), CAST(N'2025-03-11T11:30:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (28, N'1000845628', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-12T13:00:00.000' AS DateTime), CAST(N'2025-03-12T15:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (29, N'1000845629', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-13T10:30:00.000' AS DateTime), CAST(N'2025-03-13T12:30:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (30, N'1000845630', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-14T14:00:00.000' AS DateTime), CAST(N'2025-03-14T16:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (31, N'1000845631', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-15T15:30:00.000' AS DateTime), CAST(N'2025-03-15T17:30:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (32, N'1000845632', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-16T16:00:00.000' AS DateTime), CAST(N'2025-03-16T18:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (33, N'1000845633', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-17T08:30:00.000' AS DateTime), CAST(N'2025-03-17T10:30:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (34, N'1000845634', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-18T09:00:00.000' AS DateTime), CAST(N'2025-03-18T11:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (35, N'1000845635', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-19T07:30:00.000' AS DateTime), CAST(N'2025-03-19T09:30:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (36, N'1000845636', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-20T13:00:00.000' AS DateTime), CAST(N'2025-03-20T15:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (37, N'1000845637', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-21T09:00:00.000' AS DateTime), CAST(N'2025-03-21T11:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (38, N'1000845638', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-22T07:30:00.000' AS DateTime), CAST(N'2025-03-22T09:30:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (39, N'1000845639', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-23T13:00:00.000' AS DateTime), CAST(N'2025-03-23T15:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (40, N'1000845640', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-24T09:00:00.000' AS DateTime), CAST(N'2025-03-24T11:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (41, N'1000845641', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-25T07:30:00.000' AS DateTime), CAST(N'2025-03-25T09:30:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (42, N'1000845642', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-26T15:00:00.000' AS DateTime), CAST(N'2025-03-26T17:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (43, N'1000845643', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-27T11:00:00.000' AS DateTime), CAST(N'2025-03-27T12:30:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (44, N'1000845644', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-28T07:30:00.000' AS DateTime), CAST(N'2025-03-28T09:30:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (45, N'1000845645', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-29T13:00:00.000' AS DateTime), CAST(N'2025-03-29T15:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (46, N'1000845646', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-30T09:00:00.000' AS DateTime), CAST(N'2025-03-30T11:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (47, N'1000845647', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-31T07:30:00.000' AS DateTime), CAST(N'2025-03-31T09:30:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (48, N'1000845648', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-04-01T13:00:00.000' AS DateTime), CAST(N'2025-04-01T15:00:00.000' AS DateTime), 2, 1, 1, 1, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (49, N'1000845649', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-09T07:30:00.000' AS DateTime), CAST(N'2025-03-09T09:30:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (50, N'1000845650', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-10T13:00:00.000' AS DateTime), CAST(N'2025-03-10T15:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (51, N'1000845651', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-11T09:30:00.000' AS DateTime), CAST(N'2025-03-11T11:30:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (52, N'1000845652', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-12T13:00:00.000' AS DateTime), CAST(N'2025-03-12T15:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (53, N'1000845653', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-13T10:30:00.000' AS DateTime), CAST(N'2025-03-13T12:30:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (54, N'1000845654', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-14T14:00:00.000' AS DateTime), CAST(N'2025-03-14T16:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (55, N'1000845655', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-15T15:30:00.000' AS DateTime), CAST(N'2025-03-15T17:30:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (56, N'1000845656', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-16T16:00:00.000' AS DateTime), CAST(N'2025-03-16T18:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (57, N'1000845657', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-17T08:30:00.000' AS DateTime), CAST(N'2025-03-17T10:30:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (58, N'1000845658', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-18T09:00:00.000' AS DateTime), CAST(N'2025-03-18T11:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (59, N'1000845659', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-19T07:30:00.000' AS DateTime), CAST(N'2025-03-19T09:30:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (60, N'1000845660', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-20T13:00:00.000' AS DateTime), CAST(N'2025-03-20T15:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (61, N'1000845661', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-21T09:00:00.000' AS DateTime), CAST(N'2025-03-21T11:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (62, N'1000845662', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-22T07:30:00.000' AS DateTime), CAST(N'2025-03-22T09:30:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (63, N'1000845663', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-23T13:00:00.000' AS DateTime), CAST(N'2025-03-23T15:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (64, N'1000845664', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-24T09:00:00.000' AS DateTime), CAST(N'2025-03-24T11:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (65, N'1000845665', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-25T07:30:00.000' AS DateTime), CAST(N'2025-03-25T09:30:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (66, N'1000845666', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-26T17:30:00.000' AS DateTime), CAST(N'2025-03-26T19:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (67, N'1000845667', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-27T15:00:00.000' AS DateTime), CAST(N'2025-03-27T17:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (68, N'1000845668', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-28T07:30:00.000' AS DateTime), CAST(N'2025-03-28T09:30:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (69, N'1000845669', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-29T13:00:00.000' AS DateTime), CAST(N'2025-03-29T15:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (70, N'1000845670', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-30T09:00:00.000' AS DateTime), CAST(N'2025-03-30T11:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (71, N'1000845671', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-31T07:30:00.000' AS DateTime), CAST(N'2025-03-31T09:30:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (72, N'1000845672', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-04-01T13:00:00.000' AS DateTime), CAST(N'2025-04-01T15:00:00.000' AS DateTime), 3, 1, 1, 1, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (73, N'1000845673', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-09T07:30:00.000' AS DateTime), CAST(N'2025-03-09T09:30:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (74, N'1000845674', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-10T13:00:00.000' AS DateTime), CAST(N'2025-03-10T15:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (75, N'1000845675', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-11T09:30:00.000' AS DateTime), CAST(N'2025-03-11T11:30:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (76, N'1000845676', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-12T13:00:00.000' AS DateTime), CAST(N'2025-03-12T15:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (77, N'1000845677', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-13T10:30:00.000' AS DateTime), CAST(N'2025-03-13T12:30:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (78, N'1000845678', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-14T14:00:00.000' AS DateTime), CAST(N'2025-03-14T16:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (79, N'1000845679', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-15T15:30:00.000' AS DateTime), CAST(N'2025-03-15T17:30:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (80, N'1000845680', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-16T16:00:00.000' AS DateTime), CAST(N'2025-03-16T18:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (81, N'1000845681', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-17T08:30:00.000' AS DateTime), CAST(N'2025-03-17T10:30:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (82, N'1000845682', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-18T09:00:00.000' AS DateTime), CAST(N'2025-03-18T11:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (83, N'1000845683', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-19T07:30:00.000' AS DateTime), CAST(N'2025-03-19T09:30:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (84, N'1000845684', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-20T13:00:00.000' AS DateTime), CAST(N'2025-03-20T15:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (85, N'1000845685', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-21T09:00:00.000' AS DateTime), CAST(N'2025-03-21T11:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (86, N'1000845686', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-22T07:30:00.000' AS DateTime), CAST(N'2025-03-22T09:30:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (87, N'1000845687', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-23T13:00:00.000' AS DateTime), CAST(N'2025-03-23T15:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (88, N'1000845688', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-24T09:00:00.000' AS DateTime), CAST(N'2025-03-24T11:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (89, N'1000845689', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-25T07:30:00.000' AS DateTime), CAST(N'2025-03-25T09:30:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (90, N'1000845690', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-26T20:00:00.000' AS DateTime), CAST(N'2025-03-26T15:21:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (91, N'1000845691', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-27T09:00:00.000' AS DateTime), CAST(N'2025-03-27T11:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (92, N'1000845692', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-28T07:30:00.000' AS DateTime), CAST(N'2025-03-28T09:30:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (93, N'1000845693', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-29T13:00:00.000' AS DateTime), CAST(N'2025-03-29T15:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (94, N'1000845694', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-30T09:00:00.000' AS DateTime), CAST(N'2025-03-30T11:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (95, N'1000845695', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-31T07:30:00.000' AS DateTime), CAST(N'2025-03-31T09:30:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (96, N'1000845696', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-04-01T13:00:00.000' AS DateTime), CAST(N'2025-04-01T15:00:00.000' AS DateTime), 4, 1, 1, 1, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (97, N'1000845697', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-09T07:30:00.000' AS DateTime), CAST(N'2025-03-09T09:30:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (98, N'1000845698', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-10T13:00:00.000' AS DateTime), CAST(N'2025-03-10T15:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (99, N'1000845699', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-11T09:30:00.000' AS DateTime), CAST(N'2025-03-11T11:30:00.000' AS DateTime), 5, 1, 1, 2, 1)
GO
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (100, N'1000845700', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-12T13:00:00.000' AS DateTime), CAST(N'2025-03-12T15:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (101, N'1000845701', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-13T10:30:00.000' AS DateTime), CAST(N'2025-03-13T12:30:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (102, N'1000845702', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-14T14:00:00.000' AS DateTime), CAST(N'2025-03-14T16:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (103, N'1000845703', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-15T15:30:00.000' AS DateTime), CAST(N'2025-03-15T17:30:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (104, N'1000845704', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-16T16:00:00.000' AS DateTime), CAST(N'2025-03-16T18:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (105, N'1000845705', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-17T08:30:00.000' AS DateTime), CAST(N'2025-03-17T10:30:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (106, N'1000845706', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-18T09:00:00.000' AS DateTime), CAST(N'2025-03-18T11:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (107, N'1000845707', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-19T07:30:00.000' AS DateTime), CAST(N'2025-03-19T09:30:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (108, N'1000845708', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-20T13:00:00.000' AS DateTime), CAST(N'2025-03-20T15:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (109, N'1000845709', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-21T09:00:00.000' AS DateTime), CAST(N'2025-03-21T11:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (110, N'1000845710', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-22T07:30:00.000' AS DateTime), CAST(N'2025-03-22T09:30:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (111, N'1000845711', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-23T13:00:00.000' AS DateTime), CAST(N'2025-03-23T15:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (112, N'1000845712', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-24T09:00:00.000' AS DateTime), CAST(N'2025-03-24T11:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (113, N'1000845713', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-25T07:30:00.000' AS DateTime), CAST(N'2025-03-25T09:30:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (114, N'1000845714', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-26T13:00:00.000' AS DateTime), CAST(N'2025-03-26T15:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (115, N'1000845715', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-27T09:00:00.000' AS DateTime), CAST(N'2025-03-27T11:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (116, N'1000845716', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-28T07:30:00.000' AS DateTime), CAST(N'2025-03-28T09:30:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (117, N'1000845717', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-29T13:00:00.000' AS DateTime), CAST(N'2025-03-29T15:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (118, N'1000845718', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-30T09:00:00.000' AS DateTime), CAST(N'2025-03-30T11:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (119, N'1000845719', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-31T07:30:00.000' AS DateTime), CAST(N'2025-03-31T09:30:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (120, N'1000845720', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-04-01T13:00:00.000' AS DateTime), CAST(N'2025-04-01T15:00:00.000' AS DateTime), 5, 1, 1, 2, 1)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (121, N'1000845721', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-09T07:30:00.000' AS DateTime), CAST(N'2025-03-09T09:30:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (122, N'1000845722', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-10T13:00:00.000' AS DateTime), CAST(N'2025-03-10T15:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (123, N'1000845723', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-11T09:30:00.000' AS DateTime), CAST(N'2025-03-11T11:30:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (124, N'1000845724', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-12T13:00:00.000' AS DateTime), CAST(N'2025-03-12T15:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (125, N'1000845725', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-13T10:30:00.000' AS DateTime), CAST(N'2025-03-13T12:30:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (126, N'1000845726', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-14T14:00:00.000' AS DateTime), CAST(N'2025-03-14T16:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (127, N'1000845727', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-15T15:30:00.000' AS DateTime), CAST(N'2025-03-15T17:30:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (128, N'1000845728', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-16T16:00:00.000' AS DateTime), CAST(N'2025-03-16T18:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (129, N'1000845729', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-17T08:30:00.000' AS DateTime), CAST(N'2025-03-17T10:30:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (130, N'1000845730', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-18T09:00:00.000' AS DateTime), CAST(N'2025-03-18T11:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (131, N'1000845731', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-19T07:30:00.000' AS DateTime), CAST(N'2025-03-19T09:30:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (132, N'1000845732', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-20T13:00:00.000' AS DateTime), CAST(N'2025-03-20T15:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (133, N'1000845733', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-21T09:00:00.000' AS DateTime), CAST(N'2025-03-21T11:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (134, N'1000845734', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-22T07:30:00.000' AS DateTime), CAST(N'2025-03-22T09:30:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (135, N'1000845735', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-23T13:00:00.000' AS DateTime), CAST(N'2025-03-23T15:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (136, N'1000845736', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-24T09:00:00.000' AS DateTime), CAST(N'2025-03-24T11:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (137, N'1000845737', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-25T07:30:00.000' AS DateTime), CAST(N'2025-03-25T09:30:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (138, N'1000845738', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-26T13:00:00.000' AS DateTime), CAST(N'2025-03-26T15:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (139, N'1000845739', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-27T09:00:00.000' AS DateTime), CAST(N'2025-03-27T11:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (140, N'1000845740', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-28T07:30:00.000' AS DateTime), CAST(N'2025-03-28T09:30:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (141, N'1000845741', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-29T13:00:00.000' AS DateTime), CAST(N'2025-03-29T15:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (142, N'1000845742', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-30T09:00:00.000' AS DateTime), CAST(N'2025-03-30T11:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (143, N'1000845743', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-31T07:30:00.000' AS DateTime), CAST(N'2025-03-31T09:30:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (144, N'1000845744', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-04-01T13:00:00.000' AS DateTime), CAST(N'2025-04-01T15:00:00.000' AS DateTime), 6, 1, 1, 2, 3)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (145, N'1000845745', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-09T07:30:00.000' AS DateTime), CAST(N'2025-03-09T09:30:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (146, N'1000845746', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-10T13:00:00.000' AS DateTime), CAST(N'2025-03-10T15:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (147, N'1000845747', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-11T09:30:00.000' AS DateTime), CAST(N'2025-03-11T11:30:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (148, N'1000845748', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-12T13:00:00.000' AS DateTime), CAST(N'2025-03-12T15:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (149, N'1000845749', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-13T10:30:00.000' AS DateTime), CAST(N'2025-03-13T12:30:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (150, N'1000845750', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-14T14:00:00.000' AS DateTime), CAST(N'2025-03-14T16:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (151, N'1000845751', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-15T15:30:00.000' AS DateTime), CAST(N'2025-03-15T17:30:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (152, N'1000845752', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-16T16:00:00.000' AS DateTime), CAST(N'2025-03-16T18:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (153, N'1000845753', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-17T08:30:00.000' AS DateTime), CAST(N'2025-03-17T10:30:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (154, N'1000845754', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-18T09:00:00.000' AS DateTime), CAST(N'2025-03-18T11:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (155, N'1000845755', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-19T07:30:00.000' AS DateTime), CAST(N'2025-03-19T09:30:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (156, N'1000845756', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-20T13:00:00.000' AS DateTime), CAST(N'2025-03-20T15:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (157, N'1000845757', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-21T09:00:00.000' AS DateTime), CAST(N'2025-03-21T11:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (158, N'1000845758', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-22T07:30:00.000' AS DateTime), CAST(N'2025-03-22T09:30:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (159, N'1000845759', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-23T13:00:00.000' AS DateTime), CAST(N'2025-03-23T15:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (160, N'1000845760', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-24T09:00:00.000' AS DateTime), CAST(N'2025-03-24T11:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (161, N'1000845761', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-25T07:30:00.000' AS DateTime), CAST(N'2025-03-25T09:30:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (162, N'1000845762', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-26T13:00:00.000' AS DateTime), CAST(N'2025-03-26T15:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (163, N'1000845763', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-27T09:00:00.000' AS DateTime), CAST(N'2025-03-27T11:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (164, N'1000845764', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-28T07:30:00.000' AS DateTime), CAST(N'2025-03-28T09:30:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (165, N'1000845765', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-29T13:00:00.000' AS DateTime), CAST(N'2025-03-29T15:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (166, N'1000845766', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-30T09:00:00.000' AS DateTime), CAST(N'2025-03-30T11:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (167, N'1000845767', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-31T07:30:00.000' AS DateTime), CAST(N'2025-03-31T09:30:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (168, N'1000845768', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-04-01T13:00:00.000' AS DateTime), CAST(N'2025-04-01T15:00:00.000' AS DateTime), 7, 1, 1, 2, 4)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (169, N'1000845769', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-09T07:30:00.000' AS DateTime), CAST(N'2025-03-09T09:30:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (170, N'1000845770', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-10T13:00:00.000' AS DateTime), CAST(N'2025-03-10T15:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (171, N'1000845771', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-11T09:30:00.000' AS DateTime), CAST(N'2025-03-11T11:30:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (172, N'1000845772', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-12T13:00:00.000' AS DateTime), CAST(N'2025-03-12T15:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (173, N'1000845773', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-13T10:30:00.000' AS DateTime), CAST(N'2025-03-13T12:30:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (174, N'1000845774', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-14T14:00:00.000' AS DateTime), CAST(N'2025-03-14T16:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (175, N'1000845775', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-15T15:30:00.000' AS DateTime), CAST(N'2025-03-15T17:30:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (176, N'1000845776', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-16T16:00:00.000' AS DateTime), CAST(N'2025-03-16T18:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (177, N'1000845777', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-17T08:30:00.000' AS DateTime), CAST(N'2025-03-17T10:30:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (178, N'1000845778', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-18T09:00:00.000' AS DateTime), CAST(N'2025-03-18T11:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (179, N'1000845779', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-19T07:30:00.000' AS DateTime), CAST(N'2025-03-19T09:30:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (180, N'1000845780', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-20T13:00:00.000' AS DateTime), CAST(N'2025-03-20T15:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (181, N'1000845781', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-21T09:00:00.000' AS DateTime), CAST(N'2025-03-21T11:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (182, N'1000845782', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-22T07:30:00.000' AS DateTime), CAST(N'2025-03-22T09:30:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (183, N'1000845783', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-23T13:00:00.000' AS DateTime), CAST(N'2025-03-23T15:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (184, N'1000845784', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-24T09:00:00.000' AS DateTime), CAST(N'2025-03-24T11:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (185, N'1000845785', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-25T07:30:00.000' AS DateTime), CAST(N'2025-03-25T09:30:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (186, N'1000845786', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-26T13:00:00.000' AS DateTime), CAST(N'2025-03-26T15:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (187, N'1000845787', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-27T09:00:00.000' AS DateTime), CAST(N'2025-03-27T11:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (188, N'1000845788', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-28T07:30:00.000' AS DateTime), CAST(N'2025-03-28T09:30:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (189, N'1000845789', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-29T13:00:00.000' AS DateTime), CAST(N'2025-03-29T15:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (190, N'1000845790', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-30T09:00:00.000' AS DateTime), CAST(N'2025-03-30T11:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (191, N'1000845791', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-31T07:30:00.000' AS DateTime), CAST(N'2025-03-31T09:30:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (192, N'1000845792', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-04-01T13:00:00.000' AS DateTime), CAST(N'2025-04-01T15:00:00.000' AS DateTime), 8, 1, 1, 2, 5)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (193, N'0538503635', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-28T21:30:00.000' AS DateTime), CAST(N'2025-03-28T23:30:00.000' AS DateTime), 1, 1, 1, 1, 2)
INSERT [dbo].[Flight] ([flight_id], [flight_code], [tax], [status_id], [departure_time], [arrival_time], [plane_id], [manager_id], [customer_id], [departure_airport_id], [arrival_airport_id]) VALUES (194, N'0495353135', CAST(0.10 AS Decimal(4, 2)), 1, CAST(N'2025-03-28T13:20:00.000' AS DateTime), CAST(N'2025-03-28T14:20:00.000' AS DateTime), 1, 1, 1, 1, 2)
SET IDENTITY_INSERT [dbo].[Flight] OFF
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (1, 1, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (2, 2, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (3, 3, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (4, 3, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (4, 4, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (4, 7, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (5, 3, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (5, 5, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (6, 6, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (7, 7, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (8, 8, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (9, 9, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (10, 10, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (11, 1, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (12, 1, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (12, 2, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (12, 3, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (13, 3, 1, 5)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (14, 4, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (15, 5, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (16, 6, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (17, 7, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (18, 1, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (18, 2, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (18, 3, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (18, 4, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (18, 5, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (18, 6, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (18, 7, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (18, 8, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (19, 1, 1, 17)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (19, 2, 1, 18)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (19, 3, 1, 19)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (19, 4, 1, 20)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (19, 5, 1, 21)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (19, 6, 1, 22)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (19, 7, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (19, 9, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 10, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 1030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 2030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3001, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (20, 3030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (21, 1, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (22, 2, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (23, 3, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (24, 4, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (25, 11, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (26, 12, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (27, 13, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (28, 12, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (28, 14, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (28, 17, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (29, 12, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (29, 15, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (29, 17, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (30, 16, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (31, 17, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (32, 18, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (33, 19, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (34, 20, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (35, 11, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (36, 12, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (37, 13, 1, 6)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (38, 14, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (39, 15, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (40, 16, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (41, 17, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (42, 11, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (42, 12, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (42, 13, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (42, 14, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (42, 15, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (42, 16, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (42, 17, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (42, 18, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (43, 11, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (43, 12, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (43, 13, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (43, 14, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (43, 15, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (43, 16, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (43, 17, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (43, 19, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 20, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1029, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 1030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 2030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (44, 3030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (46, 11, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (47, 12, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (47, 13, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (48, 14, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (49, 21, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (50, 22, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (51, 23, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (52, 22, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (52, 24, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (52, 27, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (53, 22, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (53, 25, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (54, 26, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (55, 27, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (56, 28, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (57, 29, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (58, 30, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (59, 21, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (60, 22, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (61, 23, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (62, 24, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (63, 25, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (64, 26, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (65, 27, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (66, 21, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (66, 22, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (66, 23, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (66, 24, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (66, 25, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (66, 26, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (66, 27, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (66, 28, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (67, 21, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (67, 22, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (67, 23, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (67, 24, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (67, 25, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (67, 26, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (67, 27, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (67, 29, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 30, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 1030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 2030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (68, 3030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (69, 21, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (70, 22, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (71, 23, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (72, 24, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (73, 31, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (74, 32, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (75, 33, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (76, 32, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (76, 34, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (76, 37, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (77, 32, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (77, 35, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (77, 37, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (78, 36, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (79, 37, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (80, 38, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (81, 39, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (82, 40, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (83, 31, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (84, 32, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (85, 33, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (86, 34, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (87, 35, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (88, 36, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (89, 37, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (90, 31, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (90, 32, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (90, 33, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (90, 34, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (90, 35, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (90, 36, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (90, 37, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (90, 38, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (91, 31, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (91, 32, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (91, 33, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (91, 34, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (91, 35, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (91, 36, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (91, 37, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (91, 39, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 40, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 1030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 2030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3006, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (92, 3030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (93, 31, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (94, 32, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (95, 33, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (96, 34, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (97, 41, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (98, 42, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (99, 43, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (100, 42, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (100, 44, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (101, 42, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (101, 45, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (101, 47, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (102, 46, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (103, 47, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (104, 48, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (105, 49, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (106, 50, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (107, 41, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (108, 41, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (108, 42, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (108, 43, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (109, 43, 1, 4)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (110, 44, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (111, 45, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (112, 46, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (113, 47, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (114, 41, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (114, 42, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (114, 43, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (114, 44, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (114, 45, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (114, 46, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (114, 47, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (114, 48, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (115, 41, 1, 7)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (115, 42, 1, 8)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (115, 43, 1, 9)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (115, 44, 1, 11)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (115, 45, 1, 12)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (115, 46, 1, 13)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (115, 47, 1, 14)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (115, 49, 1, 15)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 50, 1, 10)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 1030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2003, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 2030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (116, 3030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (117, 41, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (118, 42, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (119, 43, 1, 44)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (120, 44, 1, 43)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (121, 51, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (122, 52, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (123, 53, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (124, 52, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (124, 54, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (125, 52, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (125, 55, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (125, 57, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (126, 56, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (127, 57, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (128, 58, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (129, 59, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (130, 60, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (131, 51, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (132, 52, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (133, 53, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (134, 54, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (135, 55, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (136, 56, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (137, 57, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (138, 51, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (138, 52, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (138, 53, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (138, 54, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (138, 55, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (138, 56, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (138, 57, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (138, 58, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (139, 51, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (139, 52, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (139, 53, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (139, 54, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (139, 55, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (139, 56, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (139, 57, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (139, 59, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 60, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1002, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 1030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 2030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (140, 3030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (141, 51, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (142, 52, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (143, 53, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (144, 54, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (145, 61, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (146, 62, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (147, 63, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (148, 64, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (149, 65, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (150, 66, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (151, 67, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (152, 68, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (153, 69, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (154, 70, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (155, 61, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (156, 62, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (157, 63, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (158, 64, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (159, 65, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (160, 66, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (161, 67, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (162, 61, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (162, 62, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (162, 63, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (162, 64, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (162, 65, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (162, 66, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (162, 67, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (162, 68, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (163, 61, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (163, 62, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (163, 63, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (163, 64, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (163, 65, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (163, 66, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (163, 67, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (163, 69, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 70, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 1030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 2030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3014, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (164, 3030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (165, 61, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (166, 62, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (167, 63, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (168, 64, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (169, 71, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (170, 72, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (171, 73, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (172, 74, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (173, 75, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (174, 76, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (175, 77, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (176, 78, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (177, 79, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (178, 80, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (179, 71, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (180, 72, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (181, 73, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (182, 74, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (183, 75, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (184, 76, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (185, 77, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (186, 71, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (186, 72, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (186, 73, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (186, 74, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (186, 75, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (186, 76, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (186, 77, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (186, 78, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (187, 71, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (187, 72, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (187, 73, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (187, 74, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (187, 75, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (187, 76, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (187, 77, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (187, 79, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 80, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 1030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2016, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 2030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (188, 3030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (189, 71, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (190, 72, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (191, 73, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (192, 74, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 4, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 5, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 6, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 7, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 8, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 9, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 10, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 1030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2012, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 2030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (193, 3030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 4, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 5, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 6, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 7, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 8, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 9, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 10, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 1030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2012, 0, NULL)
GO
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 2030, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3001, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3002, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3003, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3004, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3005, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3006, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3007, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3008, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3009, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3010, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3011, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3012, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3013, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3014, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3015, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3016, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3017, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3018, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3019, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3020, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3021, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3022, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3023, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3024, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3025, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3026, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3027, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3028, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3029, 0, NULL)
INSERT [dbo].[Flight_Seat] ([flight_id], [seat_id], [isSat], [ticket_id]) VALUES (194, 3030, 0, NULL)
GO
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (1, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (1, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (1, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (2, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (2, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (2, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (3, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (3, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (3, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (4, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (4, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (4, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (5, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (5, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (5, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (6, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (6, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (6, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (7, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (7, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (7, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (8, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (8, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (8, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (9, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (9, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (9, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (10, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (10, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (10, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (11, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (11, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (11, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (12, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (12, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (12, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (13, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (13, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (13, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (14, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (14, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (14, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (15, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (15, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (15, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (16, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (16, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (16, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (17, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (17, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (17, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (18, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (18, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (18, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (19, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (19, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (19, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (20, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (20, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (20, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (21, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (21, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (21, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (22, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (22, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (22, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (23, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (23, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (23, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (24, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (24, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (24, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (25, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (25, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (25, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (26, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (26, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (26, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (27, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (27, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (27, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (28, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (28, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (28, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (29, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (29, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (29, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (30, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (30, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (30, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (31, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (31, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (31, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (32, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (32, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (32, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (33, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (33, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (33, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (34, 1)
GO
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (34, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (34, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (35, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (35, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (35, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (36, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (36, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (36, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (37, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (37, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (37, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (38, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (38, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (38, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (39, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (39, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (39, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (40, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (40, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (40, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (41, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (41, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (41, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (42, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (42, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (42, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (43, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (43, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (43, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (44, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (44, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (44, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (45, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (45, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (45, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (46, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (46, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (46, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (47, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (47, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (47, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (48, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (48, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (48, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (49, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (49, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (49, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (50, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (50, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (50, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (51, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (51, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (51, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (52, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (52, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (52, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (53, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (53, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (53, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (54, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (54, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (54, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (55, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (55, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (55, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (56, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (56, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (56, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (57, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (57, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (57, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (58, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (58, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (58, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (59, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (59, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (59, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (60, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (60, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (60, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (61, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (61, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (61, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (62, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (62, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (62, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (63, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (63, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (63, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (64, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (64, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (64, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (65, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (65, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (65, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (66, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (66, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (66, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (67, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (67, 2)
GO
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (67, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (68, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (68, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (68, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (69, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (69, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (69, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (70, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (70, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (70, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (71, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (71, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (71, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (72, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (72, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (72, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (73, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (73, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (73, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (74, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (74, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (74, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (75, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (75, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (75, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (76, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (76, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (76, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (77, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (77, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (77, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (78, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (78, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (78, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (79, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (79, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (79, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (80, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (80, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (80, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (81, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (81, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (81, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (82, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (82, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (82, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (83, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (83, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (83, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (84, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (84, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (84, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (85, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (85, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (85, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (86, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (86, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (86, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (87, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (87, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (87, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (88, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (88, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (88, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (89, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (89, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (89, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (90, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (90, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (90, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (91, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (91, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (91, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (92, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (92, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (92, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (93, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (93, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (93, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (94, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (94, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (94, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (95, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (95, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (95, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (96, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (96, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (96, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (97, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (97, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (97, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (98, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (98, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (98, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (99, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (99, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (99, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (100, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (100, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (100, 3)
GO
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (101, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (101, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (101, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (102, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (102, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (102, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (103, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (103, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (103, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (104, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (104, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (104, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (105, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (105, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (105, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (106, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (106, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (106, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (107, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (107, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (107, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (108, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (108, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (108, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (109, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (109, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (109, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (110, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (110, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (110, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (111, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (111, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (111, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (112, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (112, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (112, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (113, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (113, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (113, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (114, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (114, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (114, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (115, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (115, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (115, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (116, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (116, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (116, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (117, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (117, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (117, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (118, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (118, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (118, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (119, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (119, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (119, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (120, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (120, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (120, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (121, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (121, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (121, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (122, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (122, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (122, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (123, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (123, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (123, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (124, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (124, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (124, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (125, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (125, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (125, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (126, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (126, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (126, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (127, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (127, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (127, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (128, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (128, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (128, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (129, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (129, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (129, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (130, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (130, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (130, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (131, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (131, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (131, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (132, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (132, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (132, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (133, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (133, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (133, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (134, 1)
GO
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (134, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (134, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (135, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (135, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (135, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (136, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (136, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (136, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (137, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (137, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (137, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (138, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (138, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (138, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (139, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (139, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (139, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (140, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (140, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (140, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (141, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (141, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (141, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (142, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (142, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (142, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (143, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (143, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (143, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (144, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (144, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (144, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (145, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (145, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (145, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (146, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (146, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (146, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (147, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (147, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (147, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (148, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (148, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (148, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (149, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (149, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (149, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (150, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (150, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (150, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (151, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (151, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (151, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (152, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (152, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (152, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (153, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (153, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (153, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (154, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (154, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (154, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (155, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (155, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (155, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (156, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (156, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (156, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (157, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (157, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (157, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (158, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (158, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (158, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (159, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (159, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (159, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (160, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (160, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (160, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (161, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (161, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (161, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (162, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (162, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (162, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (163, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (163, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (163, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (164, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (164, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (164, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (165, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (165, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (165, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (166, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (166, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (166, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (167, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (167, 2)
GO
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (167, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (168, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (168, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (168, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (169, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (169, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (169, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (170, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (170, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (170, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (171, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (171, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (171, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (172, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (172, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (172, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (173, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (173, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (173, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (174, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (174, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (174, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (175, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (175, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (175, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (176, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (176, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (176, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (177, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (177, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (177, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (178, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (178, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (178, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (179, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (179, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (179, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (180, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (180, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (180, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (181, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (181, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (181, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (182, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (182, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (182, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (183, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (183, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (183, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (184, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (184, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (184, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (185, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (185, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (185, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (186, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (186, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (186, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (187, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (187, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (187, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (188, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (188, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (188, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (189, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (189, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (189, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (190, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (190, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (190, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (191, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (191, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (191, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (192, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (192, 2)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (192, 3)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (193, 1)
INSERT [dbo].[FlightService] ([flight_id], [service_id]) VALUES (194, 1)
GO
SET IDENTITY_INSERT [dbo].[Items] ON 

INSERT [dbo].[Items] ([item_id], [item_name], [detail], [price], [status_id], [image]) VALUES (1, N'Set cơm Nhật', N'Bữa ăn Nhật Bản với sushi, sashimi, tempura', 200000, 1, N'anh-bien.jpg')
INSERT [dbo].[Items] ([item_id], [item_name], [detail], [price], [status_id], [image]) VALUES (2, N'Set cơm Âu', N'Bữa ăn kiểu Âu gồm beefsteak, rượu vang, tráng miệng', 300000, 1, N'anh-bien.jpg')
INSERT [dbo].[Items] ([item_id], [item_name], [detail], [price], [status_id], [image]) VALUES (3, N'Hành lý 10kg', N'Tăng thêm 10kg hành lý ký gửi', 200000, 1, N'anh-bien.jpg')
INSERT [dbo].[Items] ([item_id], [item_name], [detail], [price], [status_id], [image]) VALUES (4, N'Hành lý 20kg', N'Tăng thêm 20kg hành lý ký gửi', 350000, 1, N'anh-bien.jpg')
INSERT [dbo].[Items] ([item_id], [item_name], [detail], [price], [status_id], [image]) VALUES (5, N'Dịch vụ xe VIP', N'Xe sang đưa đón tận nơi', 800000, 1, N'anh-bien.jpg')
INSERT [dbo].[Items] ([item_id], [item_name], [detail], [price], [status_id], [image]) VALUES (6, N'Dịch vụ xe phổ thông', N'Xe du lịch phổ thông đón sân bay', 400000, 1, N'anh-bien.jpg')
SET IDENTITY_INSERT [dbo].[Items] OFF
GO
SET IDENTITY_INSERT [dbo].[Manager] ON 

INSERT [dbo].[Manager] ([manager_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (1, N'Pham Thi D', N'101 Broad St', N'0934567890', N'manager1@example.com', 4)
SET IDENTITY_INSERT [dbo].[Manager] OFF
GO
SET IDENTITY_INSERT [dbo].[News] ON 

INSERT [dbo].[News] ([new_id], [title], [image], [content], [category], [author], [account_id]) VALUES (1, N'Mở bán vé máy bay Tết 2025 - Đặt vé sớm hơn, tiết kiệm nhiều hơn', N've-may-bay-tet-2025.jpg', N'Theo thông báo mới nhất từ việc mở bán vé máy bay của nhiều hãng hàng không, hiện Vietnam Airlines, Bamboo Airways, Vietjet Air và Viettravel đã chính thức mở bán vé Tết 2025. Việc lên kế hoạch và đặt vé máy bay Tết từ sớm là điều rất cần thiết. Việc đặt vé sớm không chỉ giúp bạn đảm bảo một hành trình suôn sẻ mà còn giúp tiết kiệm chi phí đáng kể.

LÝ DO BẠN NÊN ĐẶT VÉ NGAY HÔM NAY:

Giá vé tốt hơn khi bạn đặt sớm, tránh tình trạng giá tăng cao vào sát Tết.
Nhiều lựa chọn chuyến bay, khung giờ phù hợp với lịch trình của bạn và gia đình.
Chỗ ngồi thoải mái, vị trí mong muốn sẽ còn sẵn khi bạn đặt trước.
Tránh tình trạng hết vé do nhu cầu đi lại tăng cao nên vé máy bay có thể trở nên khan hiếm trong dịp Tết
Hiện nay, mức vé Tết giữa các chặng vẫn đang duy trì mức giá ổn định, tham khảo như: :

Giá vé bay trên các chặng bay như TP. Hồ Chí Minh đi Đà Nẵng, Buôn Ma Thuột, Pleiku, Nha Trang, Đà Lạt… chỉ từ 890.000 VNĐ
Đối với các chặng bay từ TP. Hồ Chí Minh đi Hà Nội, Vinh, Thanh Hóa, Hải Phòng….. giá vé chỉ từ 1.790.000 VNĐ
(**)Mức giá trên chưa bao gồm thuế, phí

Càng cận Tết, giá vé sẽ càng tăng cao! Vì vậy, quý khách hàng đừng quên lên lịch đặt vé Tết càng sớm càng tốt để tránh trường hợp cháy vé, giá vé "cao ngất ngưởng" vào dịp cận Tết

Mọi thông tin đặt vé tết từ hôm nay, vui lòng liên hệ Etrip4u để được hỗ trợ tốt nhất.', N'News', N'LuxuryFlight Staff', 5)
INSERT [dbo].[News] ([new_id], [title], [image], [content], [category], [author], [account_id]) VALUES (2, N'Cảnh giác với thủ đoạn giả mạo Cục Hàng không Việt Nam thông báo hủy chuyến bay để lừa đảo', N'luadao.jpg', N'Bằng thủ đoạn mạo danh Cục Hàng không Việt Nam và thông báo chuyến bay bị hủy, các đối tượng lừa đảo sẽ yêu cầu người dân truy cập vào website giả mạo để đặt lại vé, nhằm thu thập dữ liệu cá nhân và lừa đảo chiếm đoạt tài sản...

Ngày 25/02/2025, Công an TP. Đà Nẵng đưa thông tin cảnh báo đến người dân về những chiêu lừa đảo mới và khuyến cáo nguyên tắc “3 không” để phòng tránh. Đơn cử, trên không gian mạng đã xuất hiện hình thức lừa đảo trực tuyến mới giả mạo Cục Hàng không Việt Nam thông báo chuyến bay bị hủy.

Trước tình hình này, Phòng An ninh mạng và phòng,  chống tội phạm sử dụng công nghệ cao – Công an TP.Đà Nẵng khuyến cáo người dân cần nâng cao cảnh giác hơn nữa khi sử dụng các nền tảng mạng xã hội. Người dân cần tuân thủ theo nguyên tắc “3 KHÔNG”: Không cung cấp thông tin cá nhân, tài khoản ngân hàng, số thẻ tín dụng qua điện thoại, tin nhắn hay email 
- Không truy cập đường link thanh toán từ tin nhắn hoặc email không rõ nguồn gốc - Không tải về những app không rõ nguồn gốc để tránh bị đánh cắp thông tin cá nhân. Khi thực hiện thanh toán hóa đơn trực tuyến, người dân cần truy cập trực tiếp vào website hoặc ứng dụng chính thức của đơn vị cung cấp dịch vụ.
LuxuryFlight trân trọng khuyến nghị hành khách luôn nâng cao cảnh giác với các thủ đoạn lừa đảo công nghệ cao. Nếu nhận thấy có dấu hiệu đáng ngờ hoặc phát hiện bị lừa đảo, hành khách cần ngay lập tức trình báo cho cơ quan Công an gần nhất để được hỗ trợ. ', N'Notice', N'LuxuryFlight Staff', 6)
INSERT [dbo].[News] ([new_id], [title], [image], [content], [category], [author], [account_id]) VALUES (3, N'Thông báo nâng cấp hệ thống tháng 3/2025', N'maintain.jpg', N'Hệ thống sẽ được nâng cấp vào ngày 15/3/2025. Chi tiết thời gian và ảnh hưởng sẽ được đề cập sau. Quý khách vui lòng sắp xếp lịch di chuyển tránh thời gian nâng cấp hệ thống để tránh những rủi ro không đáng có. Trân trọng cảm ơn!', N'Notice', N'LuxuryFlight Staff', 6)
INSERT [dbo].[News] ([new_id], [title], [image], [content], [category], [author], [account_id]) VALUES (4, N'Chương trình khuyến mãi đặc biệt Xuân 2025', N'tet-promotion.png', N'Nhiều ưu đãi hấp dẫn dành cho khách hàng nhân dịp xuân 2025.', N'Promotion', N'LuxuryFlight Staff', 7)
INSERT [dbo].[News] ([new_id], [title], [image], [content], [category], [author], [account_id]) VALUES (18, N'Máy bay Hàn Quốc xuất kích theo sát chiến đấu cơ Nga', N'img_20150310_192348.jpg', N'Dẫn lại thông tin từ Hội đồng Tham mưu trưởng Liên quân Hàn Quốc (JCS), hãng tin này cho biết vụ việc xảy ra vào sáng 15-3, buộc quân đội Hàn Quốc điều máy bay chiến đấu theo sát tình huống.

JCS tường thuật các máy bay quân sự Nga lần lượt tiến vào KADIZ lúc 9 giờ 20 phút (giờ địa phương) trước khi rời đi theo hướng Đông và Bắc.  Tuy nhiên, JCS xác nhận không có chiến đấu cơ nào vi phạm không phận Hàn Quốc.

Quân đội Hàn Quốc phát hiện những máy bay Nga nói trên từ trước khi chúng tiến vào KADIZ và nhanh chóng triển khai lực lượng không quân giám sát, phản ứng kịp thời. ', N'News', N'LuxuryFlight Staff', 4)
INSERT [dbo].[News] ([new_id], [title], [image], [content], [category], [author], [account_id]) VALUES (19, N'Tin tuc chinh tri', N'WIN_20240604_12_01_29_Pro.jpg', N'La tin tuc ve chinh tri', N'News', N'LuxuryFlight Staff', 5)
SET IDENTITY_INSERT [dbo].[News] OFF
GO
SET IDENTITY_INSERT [dbo].[Plane] ON 

INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (1, N'VN123', N'Boeing 747 - 27/03/1977', 1, N'Boeing', 2018, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (2, N'VN456', N'Boeing 747 - 01/08/1990', 1, N'Boeing', 2017, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (3, N'VN789', N'Airbus A32 - 15/05/2005', 1, N'Airbus', 2016, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (4, N'VN767', N'Boeing 747 - 11/05/2003', 1, N'Airbus', 2015, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (5, N'VN421', N'Airbus A32 - 01/03/2001', 1, N'Airbus', 2014, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (6, N'VN643', N'Boeing 747 - 15/06/2000', 1, N'Airbus', 2012, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (7, N'VN214', N'Airbus A32 - 22/08/2006', 1, N'Airbus', 2011, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (8, N'VN421', N'Boeing 747 - 31/05/2003', 1, N'Airbus', 2016, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (9, N'VN644', N'Airbus A32 - 14/07/1999', 1, N'Airbus', 2015, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (10, N'VN245', N'Airbus A32 - 16/04/2007', 1, N'Airbus', 2018, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (11, N'VN231', N'Airbus A32 - 18/08/2002', 1, N'Airbus', 2019, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (12, N'VN222', N'Boeing 747 - 21/01/2003', 1, N'Airbus', 2020, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (13, N'VN189', N'Boeing 747 - 31/03/2004', 1, N'Airbus', 2014, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (14, N'VN289', N'Airbus A32 - 27/09/2004', 1, N'Airbus', 2011, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (15, N'VN198', N'Airbus A32 - 19/03/2001', 1, N'Airbus', 2012, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (16, N'VN177', N'Boeing 747 - 27/02/1998', 1, N'Airbus', 2014, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (17, N'VN354', N'Airbus A32 - 12/05/1997', 1, N'Airbus', 2015, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (18, N'VN322', N'Airbus A32 - 18/10/2006', 1, N'Airbus', 2016, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (19, N'VN367', N'Boeing 747 - 17/04/1999', 1, N'Airbus', 2017, 1)
INSERT [dbo].[Plane] ([plane_id], [plane_code], [model], [status_id], [manufacture], [year_of_manufacture], [manager_id]) VALUES (20, N'VN564', N'Airbus A32 - 19/05/1996', 1, N'Airbus', 2018, 1)
SET IDENTITY_INSERT [dbo].[Plane] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([role_id], [role_name]) VALUES (1, N'Admin')
INSERT [dbo].[Role] ([role_id], [role_name]) VALUES (2, N'Supporter')
INSERT [dbo].[Role] ([role_id], [role_name]) VALUES (3, N'Customer')
INSERT [dbo].[Role] ([role_id], [role_name]) VALUES (4, N'Manager')
INSERT [dbo].[Role] ([role_id], [role_name]) VALUES (5, N'Guest')
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[Seat] ON 

INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1, N'SE1', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2, N'SE2', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3, N'SE3', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (4, N'SP1', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (5, N'SP2', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (6, N'SP3', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (7, N'EC1', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (8, N'EC2', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (9, N'EC3', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (10, N'EC4', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (11, N'SE1', 1, 1, 2)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (12, N'SE2', 1, 1, 2)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (13, N'SE3', 1, 1, 2)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (14, N'SP1', 2, 1, 2)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (15, N'SP2', 2, 1, 2)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (16, N'SP3', 2, 1, 2)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (17, N'EC1', 3, 1, 2)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (18, N'EC2', 3, 1, 2)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (19, N'EC3', 3, 1, 2)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (20, N'EC4', 3, 1, 2)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (21, N'SE1', 1, 1, 3)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (22, N'SE2', 1, 1, 3)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (23, N'SE3', 1, 1, 3)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (24, N'SP1', 2, 1, 3)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (25, N'SP2', 2, 1, 3)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (26, N'SP3', 2, 1, 3)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (27, N'EC1', 3, 1, 3)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (28, N'EC2', 3, 1, 3)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (29, N'EC3', 3, 1, 3)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (30, N'EC4', 3, 1, 3)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (31, N'SE1', 1, 1, 4)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (32, N'SE2', 1, 1, 4)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (33, N'SE3', 1, 1, 4)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (34, N'SP1', 2, 1, 4)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (35, N'SP2', 2, 1, 4)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (36, N'SP3', 2, 1, 4)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (37, N'EC1', 3, 1, 4)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (38, N'EC2', 3, 1, 4)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (39, N'EC3', 3, 1, 4)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (40, N'EC4', 3, 1, 4)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (41, N'SE1', 1, 1, 5)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (42, N'SE2', 1, 1, 5)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (43, N'SE3', 1, 1, 5)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (44, N'SP1', 2, 1, 5)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (45, N'SP2', 2, 1, 5)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (46, N'SP3', 2, 1, 5)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (47, N'EC1', 3, 1, 5)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (48, N'EC2', 3, 1, 5)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (49, N'EC3', 3, 1, 5)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (50, N'EC4', 3, 1, 5)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (51, N'SE1', 1, 1, 6)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (52, N'SE2', 1, 1, 6)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (53, N'SE3', 1, 1, 6)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (54, N'SP1', 2, 1, 6)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (55, N'SP2', 2, 1, 6)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (56, N'SP3', 2, 1, 6)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (57, N'EC1', 3, 1, 6)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (58, N'EC2', 3, 1, 6)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (59, N'EC3', 3, 1, 6)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (60, N'EC4', 3, 1, 6)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (61, N'SE1', 1, 1, 7)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (62, N'SE2', 1, 1, 7)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (63, N'SE3', 1, 1, 7)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (64, N'SP1', 2, 1, 7)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (65, N'SP2', 2, 1, 7)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (66, N'SP3', 2, 1, 7)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (67, N'EC1', 3, 1, 7)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (68, N'EC2', 3, 1, 7)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (69, N'EC3', 3, 1, 7)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (70, N'EC4', 3, 1, 7)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (71, N'SE1', 1, 1, 8)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (72, N'SE2', 1, 1, 8)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (73, N'SE3', 1, 1, 8)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (74, N'SP1', 2, 1, 8)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (75, N'SP2', 2, 1, 8)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (76, N'SP3', 2, 1, 8)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (77, N'EC1', 3, 1, 8)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (78, N'EC2', 3, 1, 8)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (79, N'EC3', 3, 1, 8)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (80, N'EC4', 3, 1, 8)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1001, N'F01', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1002, N'F02', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1003, N'F03', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1004, N'F04', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1005, N'F05', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1006, N'F06', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1007, N'F07', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1008, N'F08', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1009, N'F09', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1010, N'F10', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1011, N'F11', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1012, N'F12', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1013, N'F13', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1014, N'F14', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1015, N'F15', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1016, N'F16', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1017, N'F17', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1018, N'F18', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1019, N'F19', 1, 1, 1)
GO
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1020, N'F20', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1021, N'F21', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1022, N'F22', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1023, N'F23', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1024, N'F24', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1025, N'F25', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1026, N'F26', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1027, N'F27', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1028, N'F28', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1029, N'F29', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (1030, N'F30', 1, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2001, N'B01', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2002, N'B02', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2003, N'B03', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2004, N'B04', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2005, N'B05', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2006, N'B06', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2007, N'B07', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2008, N'B08', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2009, N'B09', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2010, N'B10', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2011, N'B11', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2012, N'B12', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2013, N'B13', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2014, N'B14', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2015, N'B15', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2016, N'B16', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2017, N'B17', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2018, N'B18', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2019, N'B19', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2020, N'B20', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2021, N'B21', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2022, N'B22', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2023, N'B23', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2024, N'B24', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2025, N'B25', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2026, N'B26', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2027, N'B27', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2028, N'B28', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2029, N'B29', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (2030, N'B30', 2, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3001, N'E01', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3002, N'E02', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3003, N'E03', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3004, N'E04', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3005, N'E05', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3006, N'E06', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3007, N'E07', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3008, N'E08', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3009, N'E09', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3010, N'E10', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3011, N'E11', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3012, N'E12', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3013, N'E13', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3014, N'E14', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3015, N'E15', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3016, N'E16', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3017, N'E17', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3018, N'E18', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3019, N'E19', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3020, N'E20', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3021, N'E21', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3022, N'E22', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3023, N'E23', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3024, N'E24', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3025, N'E25', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3026, N'E26', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3027, N'E27', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3028, N'E28', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3029, N'E29', 3, 1, 1)
INSERT [dbo].[Seat] ([seat_id], [seat_number], [class_id], [status_id], [plane_id]) VALUES (3030, N'E30', 3, 1, 1)
SET IDENTITY_INSERT [dbo].[Seat] OFF
GO
SET IDENTITY_INSERT [dbo].[Service] ON 

INSERT [dbo].[Service] ([service_id], [service_name], [detail], [manager_id], [status_id]) VALUES (1, N'Đặt đồ ăn', N'Bữa ăn cao cấp với các món đặc sắc', 1, 1)
INSERT [dbo].[Service] ([service_id], [service_name], [detail], [manager_id], [status_id]) VALUES (2, N'Hành lý thêm', N'Ký gửi hành lý', 1, 1)
INSERT [dbo].[Service] ([service_id], [service_name], [detail], [manager_id], [status_id]) VALUES (3, N'Đưa đón sân bay', N'Dịch vụ xe du lịch đưa đón tận nơi', 1, 1)
SET IDENTITY_INSERT [dbo].[Service] OFF
GO
INSERT [dbo].[Service_Item] ([service_id], [item_id]) VALUES (1, 1)
INSERT [dbo].[Service_Item] ([service_id], [item_id]) VALUES (1, 2)
INSERT [dbo].[Service_Item] ([service_id], [item_id]) VALUES (2, 3)
INSERT [dbo].[Service_Item] ([service_id], [item_id]) VALUES (2, 4)
INSERT [dbo].[Service_Item] ([service_id], [item_id]) VALUES (3, 5)
INSERT [dbo].[Service_Item] ([service_id], [item_id]) VALUES (3, 6)
GO
SET IDENTITY_INSERT [dbo].[Status] ON 

INSERT [dbo].[Status] ([status_id], [status_name], [status_type]) VALUES (1, N'Active', N'1                                                 ')
INSERT [dbo].[Status] ([status_id], [status_name], [status_type]) VALUES (2, N'Inactive', N'2                                                 ')
SET IDENTITY_INSERT [dbo].[Status] OFF
GO
SET IDENTITY_INSERT [dbo].[Supporter] ON 

INSERT [dbo].[Supporter] ([supporter_id], [fullname], [address], [phone_number], [email], [account_id]) VALUES (1, N'Le Thi B', N'456 Side St', N'0912345678', N'supporter1@example.com', 2)
SET IDENTITY_INSERT [dbo].[Supporter] OFF
GO
SET IDENTITY_INSERT [dbo].[Ticket] ON 

INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (2, N'120102120', 108, 1, CAST(N'2024-09-11' AS Date), NULL, CAST(12000 AS Decimal(18, 0)), N'male', N'djs', CAST(N'2024-08-07' AS Date), N'Vy', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (4, N'37723549', 109, 1, CAST(N'2025-03-21' AS Date), NULL, CAST(2150000 AS Decimal(18, 0)), N'male', N'Vu', CAST(N'2025-03-07' AS Date), N'Vien', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (5, N'13882868', 13, 1, CAST(N'2025-03-21' AS Date), NULL, CAST(2150000 AS Decimal(18, 0)), N'male', N'sdsdsd', CAST(N'2025-03-15' AS Date), N'Vưeuwe', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (6, N'89568091', 37, 1, CAST(N'2025-03-21' AS Date), 1, CAST(2700000 AS Decimal(18, 0)), N'male', N'Vu', CAST(N'2025-03-30' AS Date), N'Vien', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (7, N'38409968', 115, 1, CAST(N'2025-03-27' AS Date), NULL, CAST(2150000 AS Decimal(18, 0)), N'male', N'Vien', CAST(N'2025-03-13' AS Date), N'Thanh Vu', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (8, N'28929811', 115, 1, CAST(N'2025-03-27' AS Date), NULL, CAST(2150000 AS Decimal(18, 0)), N'male', N'ksdkskd', CAST(N'2025-03-21' AS Date), N'Thuy Nga', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (9, N'51661522', 115, 1, CAST(N'2025-03-27' AS Date), NULL, CAST(37500 AS Decimal(18, 0)), N'male', N'Vien', CAST(N'2025-03-13' AS Date), N'Thanh VV', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (10, N'16724748', 116, 1, CAST(N'2025-03-27' AS Date), NULL, CAST(1750000 AS Decimal(18, 0)), N'male', N'Vu', CAST(N'2007-03-22' AS Date), N'Vien', 3, N'Vien Thanh Vu', N'0121221', N'vuvthe173552@fpt.edu.vn', N'Lng son')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (11, N'82474428', 115, 1, CAST(N'2025-03-27' AS Date), 19, CAST(1950000 AS Decimal(18, 0)), N'male', N'Vu', CAST(N'2006-02-26' AS Date), N'Vien', 2, N'Vien Thanh Vu', N'012121221', N'vuvien73@gmail.com', N'Lang Son')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (12, N'94580054', 115, 1, CAST(N'2025-03-27' AS Date), 19, CAST(1950000 AS Decimal(18, 0)), N'male', N'Vu', CAST(N'2006-03-05' AS Date), N'Vien', 2, N'Vien Thanh Vu', N'01212122', N'vuvien73@gmail.com', N'lang son')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (13, N'61989475', 115, 1, CAST(N'2025-03-27' AS Date), 19, CAST(2350000 AS Decimal(18, 0)), N'male', N'Vu', CAST(N'2006-03-13' AS Date), N'Vien', 2, N'Vien thanh Vu', N'0121122', N'vuvien73@gmail.com', N'Lang Son')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (14, N'33242104', 115, 1, CAST(N'2025-03-27' AS Date), 19, CAST(2016666 AS Decimal(18, 0)), N'male', N'Nguyễn', CAST(N'2003-05-31' AS Date), N'', 3, N'dd', N'09', N'vuvien73@gmail.com', N'gg')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (15, N'79275496', 115, 1, CAST(N'2025-03-27' AS Date), 19, CAST(1141666 AS Decimal(18, 0)), N'male', N'Nhat', CAST(N'2015-03-22' AS Date), N'Minh', 3, N'dd', N'09', N'vuvien73@gmail.com', N'gg')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (16, N'52724291', 115, 1, CAST(N'2025-03-27' AS Date), 19, CAST(304166 AS Decimal(18, 0)), N'male', N'ddd', CAST(N'2016-03-22' AS Date), N'sss', 3, N'dd', N'09', N'vuvien73@gmail.com', N'gg')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (17, N'38851475', 19, 1, CAST(N'2025-03-27' AS Date), 19, CAST(2216666 AS Decimal(18, 0)), N'male', N'Nguyễn', CAST(N'2003-03-11' AS Date), N'Bùi Nhất', 1, N'fff', N'09', N'vuvien73@gmail.com', N'hn')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (18, N'57075383', 19, 1, CAST(N'2025-03-27' AS Date), 19, CAST(2216666 AS Decimal(18, 0)), N'male', N'dd', CAST(N'2003-02-02' AS Date), N'sss', 1, N'fff', N'09', N'vuvien73@gmail.com', N'hn')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (19, N'62625449', 19, 1, CAST(N'2025-03-27' AS Date), 19, CAST(1141666 AS Decimal(18, 0)), N'male', N'dd', CAST(N'2000-02-02' AS Date), N'ss', 1, N'fff', N'09', N'vuvien73@gmail.com', N'hn')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (20, N'29775130', 19, 1, CAST(N'2025-03-28' AS Date), 19, CAST(1950000 AS Decimal(18, 0)), N'male', N'Nguyễn', CAST(N'2003-02-02' AS Date), N'Bùi Nhất', 2, N'dd', N'0921234212', N'vuvien73@gmail.com', N'dd')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (21, N'55572781', 19, 1, CAST(N'2025-03-28' AS Date), 19, CAST(1950000 AS Decimal(18, 0)), N'male', N'dd', CAST(N'2003-02-02' AS Date), N'sss', 2, N'dd', N'0921234212', N'vuvien73@gmail.com', N'dd')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (22, N'53184621', 19, 1, CAST(N'2025-03-28' AS Date), 19, CAST(37500 AS Decimal(18, 0)), N'male', N'ddd', CAST(N'2025-02-02' AS Date), N'dd', 2, N'dd', N'0921234212', N'vuvien73@gmail.com', N'dd')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (23, N'76523981', 107, 1, CAST(N'2024-11-03' AS Date), NULL, CAST(1620000 AS Decimal(18, 0)), N'male', N'Hung', CAST(N'1991-05-22' AS Date), N'Nguyen Hung', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (24, N'79421653', 113, 1, CAST(N'2024-11-10' AS Date), 5, CAST(1580000 AS Decimal(18, 0)), N'female', N'Huong', CAST(N'1994-08-17' AS Date), N'Tran Huong', 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (25, N'82547631', 110, 1, CAST(N'2024-11-22' AS Date), NULL, CAST(1690000 AS Decimal(18, 0)), N'male', N'Phong', CAST(N'1986-09-15' AS Date), N'Le Phong', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (26, N'87465231', 115, 1, CAST(N'2024-12-01' AS Date), 4, CAST(1720000 AS Decimal(18, 0)), N'female', N'Lan', CAST(N'1993-03-28' AS Date), N'Pham Lan', 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (27, N'89651234', 108, 1, CAST(N'2024-12-12' AS Date), NULL, CAST(1650000 AS Decimal(18, 0)), N'male', N'Quang', CAST(N'1990-07-09' AS Date), N'Hoang Quang', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (28, N'94563217', 121, 1, CAST(N'2024-12-18' AS Date), 2, CAST(1950000 AS Decimal(18, 0)), N'female', N'Thuy', CAST(N'1989-11-14' AS Date), N'Vo Thuy', 3, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (29, N'97123458', 118, 1, CAST(N'2024-12-29' AS Date), NULL, CAST(2180000 AS Decimal(18, 0)), N'male', N'Trung', CAST(N'1995-01-30' AS Date), N'Nguyen Trung', 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (30, N'10246573', 109, 1, CAST(N'2025-01-02' AS Date), 3, CAST(1590000 AS Decimal(18, 0)), N'female', N'Hanh', CAST(N'1997-06-12' AS Date), N'Le Hanh', 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (31, N'10789432', 114, 1, CAST(N'2025-01-07' AS Date), NULL, CAST(1650000 AS Decimal(18, 0)), N'male', N'Binh', CAST(N'1988-04-23' AS Date), N'Tran Binh', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (32, N'11023645', 112, 1, CAST(N'2025-01-12' AS Date), 1, CAST(1720000 AS Decimal(18, 0)), N'female', N'Thu', CAST(N'1992-09-05' AS Date), N'Pham Thu', 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (33, N'11472583', 106, 1, CAST(N'2025-01-20' AS Date), NULL, CAST(1680000 AS Decimal(18, 0)), N'male', N'Vinh', CAST(N'1985-11-18' AS Date), N'Nguyen Vinh', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (34, N'11895632', 117, 1, CAST(N'2025-01-25' AS Date), 5, CAST(1940000 AS Decimal(18, 0)), N'female', N'Thanh', CAST(N'1993-10-07' AS Date), N'Vo Thanh', 3, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (35, N'12034567', 111, 1, CAST(N'2025-01-30' AS Date), NULL, CAST(1760000 AS Decimal(18, 0)), N'male', N'Son', CAST(N'1989-02-15' AS Date), N'Le Son', 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (36, N'12365481', 110, 1, CAST(N'2025-02-03' AS Date), 4, CAST(1650000 AS Decimal(18, 0)), N'male', N'Long', CAST(N'1994-05-20' AS Date), N'Pham Long', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (37, N'12587423', 116, 1, CAST(N'2025-02-09' AS Date), NULL, CAST(1720000 AS Decimal(18, 0)), N'female', N'Nhung', CAST(N'1996-08-12' AS Date), N'Tran Nhung', 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (38, N'12784563', 113, 1, CAST(N'2025-02-17' AS Date), 2, CAST(1680000 AS Decimal(18, 0)), N'male', N'Duc', CAST(N'1991-04-25' AS Date), N'Nguyen Duc', 1, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (39, N'12985674', 120, 1, CAST(N'2025-02-22' AS Date), NULL, CAST(1940000 AS Decimal(18, 0)), N'female', N'Quynh', CAST(N'1997-03-19' AS Date), N'Hoang Quynh', 3, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (40, N'13246578', 115, 1, CAST(N'2025-02-27' AS Date), 3, CAST(1760000 AS Decimal(18, 0)), N'male', N'Minh', CAST(N'1990-12-08' AS Date), N'Le Minh', 2, NULL, NULL, NULL, NULL)
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (42, N'00016576', 120, 1, CAST(N'2025-04-11' AS Date), NULL, CAST(0 AS Decimal(18, 0)), N'male', N'Viể', CAST(N'2003-04-24' AS Date), N'qwqw', 2, N'1221', N'0123456789', N'vuvien73@gmail.com', N'Lang son')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (43, N'84966060', 120, 1, CAST(N'2025-04-12' AS Date), NULL, CAST(0 AS Decimal(18, 0)), N'male', N'Viể', CAST(N'2003-04-16' AS Date), N'qwqw', 2, N'sdsd', N'0123456789', N'vuvien73@gmail.com', N'Lang son')
INSERT [dbo].[Ticket] ([ticket_id], [ticket_number], [flight_id], [status_id], [booking_date], [customer_id], [total_price], [gender], [name], [date_of_birth], [full_name], [class_seat_id], [contact_full_name], [contact_phone], [contact_email], [contact_address]) VALUES (44, N'92200222', 119, 1, CAST(N'2025-04-12' AS Date), NULL, CAST(2350000 AS Decimal(18, 0)), N'male', N'sd', CAST(N'2003-05-01' AS Date), N'qwqw', 1, N'ew', N'0123456789', N'vuvien73@gmail.com', N'Lang son')
SET IDENTITY_INSERT [dbo].[Ticket] OFF
GO
/****** Object:  Index [UQ__Admin__46A222CC12A283A6]    Script Date: 6/30/2025 4:45:09 PM ******/
ALTER TABLE [dbo].[Admin] ADD UNIQUE NONCLUSTERED 
(
	[account_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Customer__46A222CC5A9E3C48]    Script Date: 6/30/2025 4:45:09 PM ******/
ALTER TABLE [dbo].[Customer] ADD UNIQUE NONCLUSTERED 
(
	[account_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Discount__75C1F0060ACA8870]    Script Date: 6/30/2025 4:45:09 PM ******/
ALTER TABLE [dbo].[Discount] ADD UNIQUE NONCLUSTERED 
(
	[discount_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Manager__46A222CCC5AC0D3A]    Script Date: 6/30/2025 4:45:09 PM ******/
ALTER TABLE [dbo].[Manager] ADD UNIQUE NONCLUSTERED 
(
	[account_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Supporte__46A222CCDE9423FB]    Script Date: 6/30/2025 4:45:09 PM ******/
ALTER TABLE [dbo].[Supporter] ADD UNIQUE NONCLUSTERED 
(
	[account_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Ticket__413613D2A41FFAC9]    Script Date: 6/30/2025 4:45:09 PM ******/
ALTER TABLE [dbo].[Ticket] ADD  CONSTRAINT [UQ__Ticket__413613D2A41FFAC9] UNIQUE NONCLUSTERED 
(
	[ticket_number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Complaint] ADD  DEFAULT (getdate()) FOR [create_at]
GO
ALTER TABLE [dbo].[Discount] ADD  DEFAULT ((1)) FOR [status]
GO
ALTER TABLE [dbo].[Items] ADD  DEFAULT ('anh-bien.jpg') FOR [image]
GO
ALTER TABLE [dbo].[Notification] ADD  DEFAULT (getdate()) FOR [create_at]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD FOREIGN KEY([role_id])
REFERENCES [dbo].[Role] ([role_id])
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD FOREIGN KEY([status_id])
REFERENCES [dbo].[Status] ([status_id])
GO
ALTER TABLE [dbo].[Admin]  WITH CHECK ADD FOREIGN KEY([account_id])
REFERENCES [dbo].[Account] ([account_id])
GO
ALTER TABLE [dbo].[Airport]  WITH CHECK ADD FOREIGN KEY([manager_id])
REFERENCES [dbo].[Manager] ([manager_id])
GO
ALTER TABLE [dbo].[AirportPrice]  WITH CHECK ADD FOREIGN KEY([airport_from_id])
REFERENCES [dbo].[Airport] ([airport_id])
GO
ALTER TABLE [dbo].[AirportPrice]  WITH CHECK ADD FOREIGN KEY([airport_to_id])
REFERENCES [dbo].[Airport] ([airport_id])
GO
ALTER TABLE [dbo].[Complaint]  WITH CHECK ADD FOREIGN KEY([customer_id])
REFERENCES [dbo].[Customer] ([customer_id])
GO
ALTER TABLE [dbo].[Complaint]  WITH CHECK ADD FOREIGN KEY([status_id])
REFERENCES [dbo].[Status] ([status_id])
GO
ALTER TABLE [dbo].[Complaint]  WITH CHECK ADD FOREIGN KEY([supporter_id])
REFERENCES [dbo].[Supporter] ([supporter_id])
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD FOREIGN KEY([account_id])
REFERENCES [dbo].[Account] ([account_id])
GO
ALTER TABLE [dbo].[Discount]  WITH CHECK ADD FOREIGN KEY([customer_id])
REFERENCES [dbo].[Customer] ([customer_id])
GO
ALTER TABLE [dbo].[Feedback]  WITH CHECK ADD FOREIGN KEY([account_id])
REFERENCES [dbo].[Account] ([account_id])
GO
ALTER TABLE [dbo].[Flight]  WITH CHECK ADD FOREIGN KEY([arrival_airport_id])
REFERENCES [dbo].[Airport] ([airport_id])
GO
ALTER TABLE [dbo].[Flight]  WITH CHECK ADD FOREIGN KEY([customer_id])
REFERENCES [dbo].[Customer] ([customer_id])
GO
ALTER TABLE [dbo].[Flight]  WITH CHECK ADD FOREIGN KEY([departure_airport_id])
REFERENCES [dbo].[Airport] ([airport_id])
GO
ALTER TABLE [dbo].[Flight]  WITH CHECK ADD FOREIGN KEY([manager_id])
REFERENCES [dbo].[Manager] ([manager_id])
GO
ALTER TABLE [dbo].[Flight]  WITH CHECK ADD FOREIGN KEY([plane_id])
REFERENCES [dbo].[Plane] ([plane_id])
GO
ALTER TABLE [dbo].[Flight]  WITH CHECK ADD FOREIGN KEY([status_id])
REFERENCES [dbo].[Status] ([status_id])
GO
ALTER TABLE [dbo].[Flight_Seat]  WITH CHECK ADD  CONSTRAINT [FK_Flight_Seat_Flight] FOREIGN KEY([flight_id])
REFERENCES [dbo].[Flight] ([flight_id])
GO
ALTER TABLE [dbo].[Flight_Seat] CHECK CONSTRAINT [FK_Flight_Seat_Flight]
GO
ALTER TABLE [dbo].[Flight_Seat]  WITH CHECK ADD  CONSTRAINT [FK_Flight_Seat_Seat] FOREIGN KEY([seat_id])
REFERENCES [dbo].[Seat] ([seat_id])
GO
ALTER TABLE [dbo].[Flight_Seat] CHECK CONSTRAINT [FK_Flight_Seat_Seat]
GO
ALTER TABLE [dbo].[Flight_Seat]  WITH CHECK ADD  CONSTRAINT [FK_Flight_Seat_Ticket] FOREIGN KEY([ticket_id])
REFERENCES [dbo].[Ticket] ([ticket_id])
GO
ALTER TABLE [dbo].[Flight_Seat] CHECK CONSTRAINT [FK_Flight_Seat_Ticket]
GO
ALTER TABLE [dbo].[FlightService]  WITH CHECK ADD FOREIGN KEY([flight_id])
REFERENCES [dbo].[Flight] ([flight_id])
GO
ALTER TABLE [dbo].[FlightService]  WITH CHECK ADD FOREIGN KEY([service_id])
REFERENCES [dbo].[Service] ([service_id])
GO
ALTER TABLE [dbo].[Items]  WITH CHECK ADD  CONSTRAINT [FK_Items_Status] FOREIGN KEY([status_id])
REFERENCES [dbo].[Status] ([status_id])
GO
ALTER TABLE [dbo].[Items] CHECK CONSTRAINT [FK_Items_Status]
GO
ALTER TABLE [dbo].[Manager]  WITH CHECK ADD FOREIGN KEY([account_id])
REFERENCES [dbo].[Account] ([account_id])
GO
ALTER TABLE [dbo].[News]  WITH CHECK ADD FOREIGN KEY([account_id])
REFERENCES [dbo].[Account] ([account_id])
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD FOREIGN KEY([account_id])
REFERENCES [dbo].[Account] ([account_id])
GO
ALTER TABLE [dbo].[Notification]  WITH CHECK ADD FOREIGN KEY([status_id])
REFERENCES [dbo].[Status] ([status_id])
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD FOREIGN KEY([status_id])
REFERENCES [dbo].[Status] ([status_id])
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK__Payment__ticket___1EA48E88] FOREIGN KEY([ticket_id])
REFERENCES [dbo].[Ticket] ([ticket_id])
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK__Payment__ticket___1EA48E88]
GO
ALTER TABLE [dbo].[Plane]  WITH CHECK ADD FOREIGN KEY([manager_id])
REFERENCES [dbo].[Manager] ([manager_id])
GO
ALTER TABLE [dbo].[Plane]  WITH CHECK ADD FOREIGN KEY([status_id])
REFERENCES [dbo].[Status] ([status_id])
GO
ALTER TABLE [dbo].[Seat]  WITH CHECK ADD  CONSTRAINT [FK__Seat__class_id__2180FB33] FOREIGN KEY([class_id])
REFERENCES [dbo].[ClassSeat] ([class_id])
GO
ALTER TABLE [dbo].[Seat] CHECK CONSTRAINT [FK__Seat__class_id__2180FB33]
GO
ALTER TABLE [dbo].[Seat]  WITH CHECK ADD  CONSTRAINT [FK__Seat__status_id__22751F6C] FOREIGN KEY([status_id])
REFERENCES [dbo].[Status] ([status_id])
GO
ALTER TABLE [dbo].[Seat] CHECK CONSTRAINT [FK__Seat__status_id__22751F6C]
GO
ALTER TABLE [dbo].[Seat]  WITH CHECK ADD  CONSTRAINT [FK_Seat_Plane] FOREIGN KEY([plane_id])
REFERENCES [dbo].[Plane] ([plane_id])
GO
ALTER TABLE [dbo].[Seat] CHECK CONSTRAINT [FK_Seat_Plane]
GO
ALTER TABLE [dbo].[Service]  WITH CHECK ADD FOREIGN KEY([manager_id])
REFERENCES [dbo].[Manager] ([manager_id])
GO
ALTER TABLE [dbo].[Service]  WITH CHECK ADD  CONSTRAINT [FK_Service_Status] FOREIGN KEY([status_id])
REFERENCES [dbo].[Status] ([status_id])
GO
ALTER TABLE [dbo].[Service] CHECK CONSTRAINT [FK_Service_Status]
GO
ALTER TABLE [dbo].[Service_Item]  WITH CHECK ADD FOREIGN KEY([item_id])
REFERENCES [dbo].[Items] ([item_id])
GO
ALTER TABLE [dbo].[Service_Item]  WITH CHECK ADD FOREIGN KEY([service_id])
REFERENCES [dbo].[Service] ([service_id])
GO
ALTER TABLE [dbo].[Supporter]  WITH CHECK ADD FOREIGN KEY([account_id])
REFERENCES [dbo].[Account] ([account_id])
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK__Ticket__customer__2B0A656D] FOREIGN KEY([customer_id])
REFERENCES [dbo].[Customer] ([customer_id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK__Ticket__customer__2B0A656D]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK__Ticket__flight_i__2BFE89A6] FOREIGN KEY([flight_id])
REFERENCES [dbo].[Flight] ([flight_id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK__Ticket__flight_i__2BFE89A6]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK__Ticket__status_i__2CF2ADDF] FOREIGN KEY([status_id])
REFERENCES [dbo].[Status] ([status_id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK__Ticket__status_i__2CF2ADDF]
GO
ALTER TABLE [dbo].[Ticket]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_ClassSeat] FOREIGN KEY([class_seat_id])
REFERENCES [dbo].[ClassSeat] ([class_id])
GO
ALTER TABLE [dbo].[Ticket] CHECK CONSTRAINT [FK_Ticket_ClassSeat]
GO
ALTER TABLE [dbo].[Ticket_Item]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Item_Items] FOREIGN KEY([item_id])
REFERENCES [dbo].[Items] ([item_id])
GO
ALTER TABLE [dbo].[Ticket_Item] CHECK CONSTRAINT [FK_Ticket_Item_Items]
GO
ALTER TABLE [dbo].[Ticket_Item]  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Item_Ticket] FOREIGN KEY([ticket_id])
REFERENCES [dbo].[Ticket] ([ticket_id])
GO
ALTER TABLE [dbo].[Ticket_Item] CHECK CONSTRAINT [FK_Ticket_Item_Ticket]
GO
ALTER TABLE [dbo].[User_Logs]  WITH CHECK ADD FOREIGN KEY([account_id])
REFERENCES [dbo].[Account] ([account_id])
GO
USE [master]
GO
ALTER DATABASE [LuxuryFlightV11] SET  READ_WRITE 
GO
