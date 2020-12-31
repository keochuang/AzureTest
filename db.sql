USE [AzureProgrammerTest]
GO

/****** Object:  Table [dbo].[Orders]    Script Date: 2020/12/29 8:19:17 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Orders](
	[OrderNumber] [int] IDENTITY(1,1) NOT NULL,
	[BuyerName] [varchar](200) NOT NULL,
	[BillingZipCode] [varchar](200) NOT NULL,
	[PurchaseOrderNumber] [varchar](200) NOT NULL,
	[OrderAmount] [float] NOT NULL,
 CONSTRAINT [PK_Order1] PRIMARY KEY CLUSTERED 
(
	[OrderNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


