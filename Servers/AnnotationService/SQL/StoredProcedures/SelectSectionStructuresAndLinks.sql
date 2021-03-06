USE [Test]
GO
/****** Object:  StoredProcedure [dbo].[SelectSectionStructuresAndLinks]    Script Date: 3/24/2016 3:02:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

		ALTER PROCEDURE [dbo].[SelectSectionStructuresAndLinks]
			-- Add the parameters for the stored procedure here
			@Z float,
			@QueryDate datetime
		AS
		BEGIN 
				SET NOCOUNT ON;

				IF OBJECT_ID('tempdb..#SectionLocations') IS NOT NULL DROP TABLE #SectionLocations
				select distinct ParentID into #SectionLocations from Location where Z = @Z order by ParentID

				IF @QueryDate IS NOT NULL
					select s.* from Structure s 
						JOIN #SectionLocations l ON (l.ParentID = s.ID)
						where s.LastModified >= @QueryDate
				ELSE
					select s.* from Structure s JOIN #SectionLocations l ON (l.ParentID = s.ID)

				Select * from StructureLink L
				where (L.TargetID in (Select ParentID from #SectionLocations))
					OR (L.SourceID in (Select ParentID from #SectionLocations)) 

				DROP TABLE #SectionLocations
		END 
		