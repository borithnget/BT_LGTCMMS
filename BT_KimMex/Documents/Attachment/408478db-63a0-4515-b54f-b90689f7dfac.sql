USE [ascfcamb_sambath_data]
GO
/****** Object:  Table [dbo].[tbl_committee_incentive]    Script Date: 10/2/2020 10:56:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_committee_incentive](
	[committee_incentive_id] [int] IDENTITY(1,1) NOT NULL,
	[committee_incentive_type] [nvarchar](50) NULL,
	[user_id] [int] NULL,
	[total_amount] [money] NULL,
	[remark] [nvarchar](max) NULL,
	[active] [bit] NULL,
	[created_at] [datetime] NULL,
	[updated_at] [datetime] NULL,
	[created_by] [int] NULL,
	[updated_by] [int] NULL,
 CONSTRAINT [PK_tbl_committee_incentive] PRIMARY KEY CLUSTERED 
(
	[committee_incentive_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_mission_cost_setting]    Script Date: 10/2/2020 10:56:12 AM ******/

INSERT [dbo].[tbl_mission_cost_setting] ([setting_id], [setting_type], [mission_number], [mission_percentage], [national_leader_number], [national_leader_percentage], [national_vice_leader_number], [national_vice_leader_percentage], [national_member_number], [national_member_percentage], [regional_leader_number], [regional_leader_percentage], [national_administrator_number], [national_administrator_percentage], [meeting_number], [meeting_percentage], [province_leader_number], [province_percentage], [province_vice_leader_number], [province_vice_leader_percentage], [district_leader_number], [district_leader_percentage], [district_vice_leader_number], [district_vice_leader_percentage], [commune_leader_number], [commune_leader_percentage], [commune_vice_leader_number], [commune_vice_leader_percentage], [village_number], [village_percentage], [group_leader1_number], [group_leader1_percentage], [group_leader2_number], [group_leader2_percentage], [group_leader3_number], [group_leader3_percentage], [province_admin_number], [province_admin_percentage], [district_admin_number], [district_admin_percentage], [commune_admin_number], [commune_admin_percentage], [village_admin_number], [village_admin_percentage], [created_at], [updated_at], [status]) VALUES (2, N'delivery baby', 0, 0, 0, 0, 0, 0, 0, 0, 1, 3, 0, 0, 0, 0, 1, 3, 6, 2, 1, 2, 2, 1.5, 1, 1.5, 2, 1, 1, 1.5, 1, 15, 1, 5, 1, 2.5, 1, 7.5, 0, 0, 0, 0, 0, 0, NULL, NULL, 1)
INSERT [dbo].[tbl_mission_cost_setting] ([setting_id], [setting_type], [mission_number], [mission_percentage], [national_leader_number], [national_leader_percentage], [national_vice_leader_number], [national_vice_leader_percentage], [national_member_number], [national_member_percentage], [regional_leader_number], [regional_leader_percentage], [national_administrator_number], [national_administrator_percentage], [meeting_number], [meeting_percentage], [province_leader_number], [province_percentage], [province_vice_leader_number], [province_vice_leader_percentage], [district_leader_number], [district_leader_percentage], [district_vice_leader_number], [district_vice_leader_percentage], [commune_leader_number], [commune_leader_percentage], [commune_vice_leader_number], [commune_vice_leader_percentage], [village_number], [village_percentage], [group_leader1_number], [group_leader1_percentage], [group_leader2_number], [group_leader2_percentage], [group_leader3_number], [group_leader3_percentage], [province_admin_number], [province_admin_percentage], [district_admin_number], [district_admin_percentage], [commune_admin_number], [commune_admin_percentage], [village_admin_number], [village_admin_percentage], [created_at], [updated_at], [status]) VALUES (3, N'wedding', 0, 0, 0, 0, 0, 0, 0, 0, 1, 3, 0, 0, 0, 0, 1, 3, 6, 2, 1, 2, 2, 1.5, 1, 1.5, 2, 1, 1, 1.5, 1, 15, 1, 5, 1, 2.5, 1, 7.5, 0, 0, 0, 0, 0, 0, NULL, NULL, 1)

