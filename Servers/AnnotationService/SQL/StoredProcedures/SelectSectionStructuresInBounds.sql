USE [Test]
GO
/****** Object:  StoredProcedure [dbo].[SelectSectionStructuresInBounds]    Script Date: 3/24/2016 5:18:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

			ALTER PROCEDURE [dbo].[SelectSectionStructuresInMosaicBounds]
				-- Add the parameters for the stored procedure here
				@Z float,
				@BBox geometry,
				@MinRadius float,
				@QueryDate datetime
			AS
			BEGIN 
					-- SET NOCOUNT ON added to prevent extra result sets from
					-- interfering with SELECT statements.
					SET NOCOUNT ON;

					--IF OBJECT_ID('tempdb..#SectionLocationsInBounds') IS NOT NULL DROP TABLE #SectionLocationsInBounds
					--select * into #SectionLocationsInBounds from Location where (@bbox.STIntersects(MosaicShape) = 1) and Z = @Z AND Radius >= @MinRadius order by ParentID

					IF @QueryDate IS NOT NULL
						select S.* from Structure S
							inner join (Select distinct ParentID from Location 
								where (@bbox.STIntersects(MosaicShape) = 1) and Z = @Z AND Radius >= @MinRadius) L 
									ON L.ParentID = S.ID
						WHERE S.LastModified >= @QueryDate
					ELSE
						select S.* from Structure S
							inner join (Select distinct ParentID from Location 
								where (@bbox.STIntersects(MosaicShape) = 1) and Z = @Z AND Radius >= @MinRadius) L 
									ON L.ParentID = S.ID
			END
			