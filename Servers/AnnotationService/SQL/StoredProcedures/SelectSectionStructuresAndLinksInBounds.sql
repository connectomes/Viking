USE [Test]
GO
/****** Object:  StoredProcedure [dbo].[SelectSectionStructuresAndLinksInBounds]    Script Date: 3/24/2016 3:27:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

			ALTER PROCEDURE [dbo].[SelectSectionStructuresAndLinksInMosaicBounds]
			-- Add the parameters for the stored procedure here
			@Z float,
			@BBox geometry,
			@MinRadius float,
			@QueryDate datetime
			AS
			BEGIN 
					SET NOCOUNT ON;

					--IF OBJECT_ID('tempdb..#SectionLocationsInBounds') IS NOT NULL DROP TABLE #SectionLocationsInBounds
					IF OBJECT_ID('tempdb..#SectionStructuresInBounds') IS NOT NULL DROP TABLE #SectionStructuresInBounds
					select S.* into #SectionStructuresInBounds from Structure S
						inner join (Select distinct ParentID from Location where (@bbox.STIntersects(MosaicShape) = 1 and Z = @Z AND Radius >= @MinRadius)) L ON L.ParentID = S.ID
						
					IF @QueryDate IS NOT NULL
						select * from #SectionStructuresInBounds where LastModified >= @QueryDate
					ELSE
						select * from #SectionStructuresInBounds
			  
					Select * from StructureLink L
					where (L.TargetID in (Select ID from #SectionStructuresInBounds))
						OR (L.SourceID in (Select ID from #SectionStructuresInBounds)) 

					DROP TABLE #SectionStructuresInBounds
			END 
		