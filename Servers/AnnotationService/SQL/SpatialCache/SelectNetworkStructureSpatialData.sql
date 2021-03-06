USE [Test]
GO
/****** Object:  StoredProcedure [dbo].[SelectNetworkStructures]    Script Date: 2/7/2018 3:21:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

		    CREATE PROCEDURE [dbo].[SelectNetworkStructureSpatialData]
				-- Add the parameters for the stored procedure here
				@IDs integer_list READONLY,
				@Hops int
			AS
			BEGIN
				select S.* from StructureSpatialCache S 
					inner join NetworkStructureIDs(@IDs, @Hops) N ON N.ID = S.ID
			END