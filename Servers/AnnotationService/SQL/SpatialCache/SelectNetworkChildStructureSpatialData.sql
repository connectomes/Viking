USE [Test]
GO
/****** Object:  StoredProcedure [dbo].[SelectNetworkChildStructures]    Script Date: 2/7/2018 3:19:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

		    CREATE PROCEDURE [dbo].[SelectNetworkChildStructureSpatialData]
						-- Add the parameters for the stored procedure here
						@IDs integer_list READONLY,
						@Hops int
			AS
			BEGIN
				select S.* from StructureSpatialCache S 
					inner join NetworkChildStructureIDs(@IDs, @Hops) N ON N.ID = S.ID
			END