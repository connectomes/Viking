USE [Test]
GO
/****** Object:  StoredProcedure [dbo].[SelectSectionLocationsAndLinksInBounds]    Script Date: 3/24/2016 3:42:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

				 ALTER PROCEDURE [dbo].[SelectSectionLocationsAndLinksInBounds]
			-- Add the parameters for the stored procedure here
			@Z float,
			@BBox geometry,
			@Radius float,
			@QueryDate datetime
		AS
		BEGIN
			-- SET NOCOUNT ON added to prevent extra result sets from
			-- interfering with SELECT statements.
			SET NOCOUNT ON;

			IF OBJECT_ID('tempdb..#LocationsInBounds') IS NOT NULL DROP TABLE #LocationsInBounds

			--Selecting all columns once into LocationsInBounds and then selecting the temp table is a huge time saver.  3-4 seconds instead of 20.

			select * into #LocationsInBounds FROM Location where Z = @Z AND (@BBox.STIntersects(MosaicShape) = 1) AND Radius >= @Radius order by ID
	 
			IF @QueryDate IS NOT NULL
				Select * from #LocationsInBounds where LastModified >= @QueryDate
			ELSE
				Select * from #LocationsInBounds
	 
			IF @QueryDate IS NOT NULL
				-- Insert statements for procedure here
				Select * from LocationLink
				 WHERE ((A in 
				(select ID from #LocationsInBounds))
				  OR
				  (B in 
				(select ID from #LocationsInBounds)))
				 AND Created >= @QueryDate
			ELSE
				-- Insert statements for procedure here
				Select * from LocationLink
				 WHERE ((A in (select ID from #LocationsInBounds))
						OR	
						(B in (select ID from #LocationsInBounds)))
	
			DROP TABLE #LocationsInBounds
		END 
		