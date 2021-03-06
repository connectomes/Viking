USE [Test]
GO
/****** Object:  StoredProcedure [dbo].[SelectSectionLocationsAndLinks]    Script Date: 9/9/2015 10:56:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SelectSectionLocationsAndLinksInBounds]
	-- Add the parameters for the stored procedure here
	@Z float,
	@BBox geometry,
	@QueryDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#LocationsInBounds') IS NOT NULL DROP TABLE #LocationsInBounds

	select ID into #LocationsInBounds FROM Location where Z = @Z
	 
	IF @QueryDate IS NOT NULL
		Select * from BoundedLocations(@BBox) where Z = @Z AND LastModified >= @QueryDate
	ELSE
		Select * from BoundedLocations(@BBox) where Z = @Z
		
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
	
	 
END
