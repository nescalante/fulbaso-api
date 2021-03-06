USE [cf]
GO
/****** Object:  Table [dbo].[FloorTypes]    Script Date: 07/21/2011 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FloorTypes](
	[FloorTypeID] [tinyint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_FloorTypes] PRIMARY KEY CLUSTERED 
(
	[FloorTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CourtTypes]    Script Date: 07/21/2011 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CourtTypes](
	[CourtTypeID] [tinyint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_CourtTypes] PRIMARY KEY CLUSTERED 
(
	[CourtTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Places]    Script Date: 07/21/2011 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Places](
	[PlaceID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Description] [text] NULL,
	[Address] [varchar](200) NULL,
	[LocationID] [int] NULL,
	[Phone] [varchar](200) NULL,
	[HowToArrive] [varchar](500) NULL,
	[Logo] [image] NULL,
	[DateFrom] [date] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Page] [varchar](20) NULL,
 CONSTRAINT [PK_Places] PRIMARY KEY CLUSTERED 
(
	[PlaceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_Places_Page] UNIQUE NONCLUSTERED 
(
	[Page] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Places_Name] ON [dbo].[Places] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Territories]    Script Date: 07/21/2011 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Territories](
	[TerritoryID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Territories] PRIMARY KEY CLUSTERED 
(
	[TerritoryID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PlaceServices]    Script Date: 07/21/2011 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlaceServices](
	[PlaceID] [int] NOT NULL,
	[Service] [tinyint] NOT NULL,
 CONSTRAINT [PK_PlaceServices] PRIMARY KEY CLUSTERED 
(
	[PlaceID] ASC,
	[Service] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clients]    Script Date: 07/21/2011 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Clients](
	[ClientID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](200) NOT NULL,
	[Phone] [varchar](200) NULL,
	[PlaceID] [int] NOT NULL,
 CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
(
	[ClientID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Clients_Name] ON [dbo].[Clients] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Clients_Phone] ON [dbo].[Clients] 
(
	[Phone] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Regions]    Script Date: 07/21/2011 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Regions](
	[RegionID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[TerritoryID] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED 
(
	[RegionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Courts]    Script Date: 07/21/2011 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Courts](
	[CourtID] [int] IDENTITY(1,1) NOT NULL,
	[PlaceID] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Players] [int] NULL,
	[CourtTypeID] [tinyint] NULL,
	[FloorTypeID] [tinyint] NULL,
	[IsIndoor] [bit] NULL,
	[IsLighted] [bit] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Courts] PRIMARY KEY CLUSTERED 
(
	[CourtID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Courts] ON [dbo].[Courts] 
(
	[PlaceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourtConfigurations]    Script Date: 07/21/2011 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourtConfigurations](
	[CourtConfigurationID] [int] NOT NULL,
	[CourtID] [int] NOT NULL,
	[StartHour] [tinyint] NULL,
	[EndHour] [tinyint] NULL,
	[Days] [tinyint] NULL,
	[Price] [money] NOT NULL,
	[Order] [tinyint] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_CourtConfigurations] PRIMARY KEY CLUSTERED 
(
	[CourtConfigurationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CourtBooks]    Script Date: 07/21/2011 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CourtBooks](
	[CourtBookID] [int] IDENTITY(1,1) NOT NULL,
	[CourtID] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[Price] [money] NOT NULL,
	[ReserveRequired] [bit] NOT NULL,
	[Reserve] [money] NULL,
	[Paid] [money] NULL,
	[ClientID] [int] NOT NULL,
 CONSTRAINT [PK_CourtBooks] PRIMARY KEY CLUSTERED 
(
	[CourtBookID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_CourtBooks] ON [dbo].[CourtBooks] 
(
	[StartTime] ASC,
	[CourtID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Locations]    Script Date: 07/21/2011 16:45:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Locations](
	[LocationID] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[RegionID] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Locations] PRIMARY KEY CLUSTERED 
(
	[LocationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[PlaceView]    Script Date: 07/21/2011 16:45:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[PlaceView]
AS
SELECT     TOP (100) PERCENT dbo.Places.PlaceID AS ID, dbo.Places.Name, dbo.Places.Address, dbo.Places.Phone, dbo.Places.Page, dbo.Locations.Description AS Location, 
                      dbo.Regions.Description AS Region,
                          (SELECT     COUNT(*) AS Expr1
                            FROM          dbo.Courts
                            WHERE      (PlaceID = dbo.Places.PlaceID)) AS Courts, dbo.Places.Name + ISNULL(dbo.Locations.Description, '') + ISNULL(dbo.Regions.Description, '') 
                      + ISNULL(dbo.Places.Address, '') AS exp
FROM         dbo.Places LEFT OUTER JOIN
                      dbo.Locations ON dbo.Places.LocationID = dbo.Locations.LocationID LEFT OUTER JOIN
                      dbo.Regions ON dbo.Locations.RegionID = dbo.Regions.RegionID
ORDER BY dbo.Places.Name
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[43] 4[4] 2[30] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Locations"
            Begin Extent = 
               Top = 6
               Left = 236
               Bottom = 131
               Right = 419
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Regions"
            Begin Extent = 
               Top = 145
               Left = 262
               Bottom = 264
               Right = 422
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Places"
            Begin Extent = 
               Top = 56
               Left = 22
               Bottom = 175
               Right = 182
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 10
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1365
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PlaceView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PlaceView'
GO
/****** Object:  ForeignKey [FK_Clients_Places]    Script Date: 07/21/2011 16:45:14 ******/
ALTER TABLE [dbo].[Clients]  WITH CHECK ADD  CONSTRAINT [FK_Clients_Places] FOREIGN KEY([PlaceID])
REFERENCES [dbo].[Places] ([PlaceID])
GO
ALTER TABLE [dbo].[Clients] CHECK CONSTRAINT [FK_Clients_Places]
GO
/****** Object:  ForeignKey [FK_CourtBooks_Clients]    Script Date: 07/21/2011 16:45:14 ******/
ALTER TABLE [dbo].[CourtBooks]  WITH CHECK ADD  CONSTRAINT [FK_CourtBooks_Clients] FOREIGN KEY([ClientID])
REFERENCES [dbo].[Clients] ([ClientID])
GO
ALTER TABLE [dbo].[CourtBooks] CHECK CONSTRAINT [FK_CourtBooks_Clients]
GO
/****** Object:  ForeignKey [FK_CourtBooks_Courts]    Script Date: 07/21/2011 16:45:14 ******/
ALTER TABLE [dbo].[CourtBooks]  WITH CHECK ADD  CONSTRAINT [FK_CourtBooks_Courts] FOREIGN KEY([CourtID])
REFERENCES [dbo].[Courts] ([CourtID])
GO
ALTER TABLE [dbo].[CourtBooks] CHECK CONSTRAINT [FK_CourtBooks_Courts]
GO
/****** Object:  ForeignKey [FK_CourtConfigurations_Courts]    Script Date: 07/21/2011 16:45:14 ******/
ALTER TABLE [dbo].[CourtConfigurations]  WITH CHECK ADD  CONSTRAINT [FK_CourtConfigurations_Courts] FOREIGN KEY([CourtID])
REFERENCES [dbo].[Courts] ([CourtID])
GO
ALTER TABLE [dbo].[CourtConfigurations] CHECK CONSTRAINT [FK_CourtConfigurations_Courts]
GO
/****** Object:  ForeignKey [FK_Courts_CourtTypes]    Script Date: 07/21/2011 16:45:14 ******/
ALTER TABLE [dbo].[Courts]  WITH CHECK ADD  CONSTRAINT [FK_Courts_CourtTypes] FOREIGN KEY([CourtTypeID])
REFERENCES [dbo].[CourtTypes] ([CourtTypeID])
GO
ALTER TABLE [dbo].[Courts] CHECK CONSTRAINT [FK_Courts_CourtTypes]
GO
/****** Object:  ForeignKey [FK_Courts_FloorTypes]    Script Date: 07/21/2011 16:45:14 ******/
ALTER TABLE [dbo].[Courts]  WITH CHECK ADD  CONSTRAINT [FK_Courts_FloorTypes] FOREIGN KEY([FloorTypeID])
REFERENCES [dbo].[FloorTypes] ([FloorTypeID])
GO
ALTER TABLE [dbo].[Courts] CHECK CONSTRAINT [FK_Courts_FloorTypes]
GO
/****** Object:  ForeignKey [FK_Courts_Places]    Script Date: 07/21/2011 16:45:14 ******/
ALTER TABLE [dbo].[Courts]  WITH CHECK ADD  CONSTRAINT [FK_Courts_Places] FOREIGN KEY([PlaceID])
REFERENCES [dbo].[Places] ([PlaceID])
GO
ALTER TABLE [dbo].[Courts] CHECK CONSTRAINT [FK_Courts_Places]
GO
/****** Object:  ForeignKey [FK_Locations_Regions]    Script Date: 07/21/2011 16:45:14 ******/
ALTER TABLE [dbo].[Locations]  WITH CHECK ADD  CONSTRAINT [FK_Locations_Regions] FOREIGN KEY([RegionID])
REFERENCES [dbo].[Regions] ([RegionID])
GO
ALTER TABLE [dbo].[Locations] CHECK CONSTRAINT [FK_Locations_Regions]
GO
/****** Object:  ForeignKey [FK_PlaceServices_Places]    Script Date: 07/21/2011 16:45:14 ******/
ALTER TABLE [dbo].[PlaceServices]  WITH CHECK ADD  CONSTRAINT [FK_PlaceServices_Places] FOREIGN KEY([PlaceID])
REFERENCES [dbo].[Places] ([PlaceID])
GO
ALTER TABLE [dbo].[PlaceServices] CHECK CONSTRAINT [FK_PlaceServices_Places]
GO
/****** Object:  ForeignKey [FK_Regions_Territories]    Script Date: 07/21/2011 16:45:14 ******/
ALTER TABLE [dbo].[Regions]  WITH CHECK ADD  CONSTRAINT [FK_Regions_Territories] FOREIGN KEY([TerritoryID])
REFERENCES [dbo].[Territories] ([TerritoryID])
GO
ALTER TABLE [dbo].[Regions] CHECK CONSTRAINT [FK_Regions_Territories]
GO
