-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE SplitStructure 
	@LocationIDOfSplitStructure bigint,
	@SplitStructureID bigint OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    IF OBJECT_ID('tempdb..#LocationLinkPool') IS NOT NULL DROP TABLE #LocationLinkPool
	IF OBJECT_ID('tempdb..#LocationsInKeepSubGraph') IS NOT NULL DROP TABLE #LocationsInKeepSubGraph
	IF OBJECT_ID('tempdb..#LocationsInSplitSubGraph') IS NOT NULL DROP TABLE #LocationsInSplitSubGraph
	IF OBJECT_ID('tempdb..#ChildStructureLocations') IS NOT NULL DROP TABLE #ChildStructureLocations
	IF OBJECT_ID('tempdb..#StructureLocations') IS NOT NULL DROP TABLE #StructureLocations
	IF OBJECT_ID('tempdb..#DistanceToEachStructure') IS NOT NULL DROP TABLE #DistanceToEachStructure
	IF OBJECT_ID('tempdb..#DistanceToNearestStructure') IS NOT NULL DROP TABLE #DistanceToNearestStructure
	IF OBJECT_ID('tempdb..#ParentIDForChildStructure') IS NOT NULL DROP TABLE #ParentIDForChildStructure
	
	set @SplitStructureID = 0
	DECLARE @KeepStructureID bigint
	
	set @KeepStructureID = (select ParentID from Location where ID = @LocationIDOfSplitStructure)
		
	SELECT A,B into #LocationLinkPool from dbo.StructureLocationLinks(@KeepStructureID) order by A

	--select * from #LocationLinkPool where A = @LocationIDOfSplitStructure OR B = @LocationIDOfSplitStructure

	CREATE TABLE #LocationsInSplitSubGraph(ID bigint)
	insert into #LocationsInSplitSubGraph (ID) values (@LocationIDOfSplitStructure)
	  
	--Loop over the pool adding to the subgraph until we cannot find any more locations
	DECLARE @RowsAddedToSubgraph bigint
	set @RowsAddedToSubgraph = 1
	While @RowsAddedToSubgraph > 0
	BEGIN
	--insert into #GAggregate (SParentID, Shape) Select SParentID, TMosaicShape from #StructureLinks where TMosaicShape is NOT NULL

		insert into #LocationsInSplitSubGraph (ID) 
			Select B as ID from #LocationLinkPool where A in (select ID from #LocationsInSplitSubGraph)
			union 
			Select A as ID from #LocationLinkPool where B in (select ID from #LocationsInSplitSubGraph)

		set @RowsAddedToSubgraph = @@ROWCOUNT

		--select distinct(ID) from #LocationsInSplitSubGraph

		--Remove links we have already added
		delete LLP from #LocationLinkPool LLP
		join #LocationsInSplitSubGraph SA ON SA.ID = LLP.A
		join #LocationsInSplitSubGraph SB ON SB.ID = LLP.B
	END

	select ID into #LocationsInKeepSubGraph from Location where ParentID = @KeepStructureID AND ID not in (select ID from #LocationsInSplitSubGraph)

	IF ((select COUNT(ID) from #LocationsInKeepSubGraph) = 0)
		THROW 50000, N'The split structure is connected to the entire keep cell.  Break a location link to create two subgraphs and try again', 1;

	--We have built the list of annotations to be used for the old and new structure.  Create a new structure for the split and capture the ID
	INSERT INTO Structure (TypeID, Notes, Verified, Tags, Confidence, ParentID, Created, Label, Username, LastModified)
		SELECT TypeID, Notes, Verified, Tags, Confidence, ParentID, Created, Label, Username, LastModified from Structure S where
			S.ID = @KeepStructureID
	set @SplitStructureID = SCOPE_IDENTITY()

	select VolumeShape, Z, KL.ID, @KeepStructureID as ParentID into  #StructureLocations
	FROM Location L 
	JOIN #LocationsInKeepSubGraph KL ON KL.ID = L.ID
	UNION ALL
	select VolumeShape, Z, SL.ID, @SplitStructureID as ParentID FROM Location L 
	JOIN #LocationsInSplitSubGraph SL ON SL.ID = L.ID

	select ParentID as StructureID, geometry::ConvexHullAggregate(VolumeShape) as Shape, AVG(Z) as Z 
		into #ChildStructureLocations from Location
		where ParentID in (select ID from Structure where ParentID = @KeepStructureID)
		group by ParentID

	--Find the nearest location in either the keep or split structure
	select CSL.StructureID as StructureID, SL.ParentID as NewParentID, MIN(SL.VolumeShape.STDistance(CSL.Shape)) as Distance
		INTO #DistanceToEachStructure from #ChildStructureLocations CSL
		join #StructureLocations SL ON SL.Z = CSL.Z
		Group By CSL.StructureID, SL.ParentID 
		order by CSL.StructureID

	select SL.StructureID as StructureID, MIN(SL.Distance) as Distance 
	INTO #DistanceToNearestStructure from #DistanceToEachStructure SL
	group by SL.StructureID 

	select SD.StructureID as StructureID, SD.NewParentID as NewParentID, SD.Distance as Distance
	into #ParentIDForChildStructure from #DistanceToEachStructure SD
	join #DistanceToNearestStructure SN ON SN.StructureID = SD.StructureID AND SN.Distance = SD.Distance

	update Location set ParentID = @SplitStructureID
	FROM Location L
		INNER JOIN #LocationsInSplitSubGraph LS ON LS.ID = L.ID

	update Structure set ParentID = PCS.NewParentID 
	FROM Structure S
		JOIN #ParentIDForChildStructure PCS ON S.ID = PCS.StructureID

	IF OBJECT_ID('tempdb..#LocationLinkPool') IS NOT NULL DROP TABLE #LocationLinkPool
	IF OBJECT_ID('tempdb..#LocationsInKeepSubGraph') IS NOT NULL DROP TABLE #LocationsInKeepSubGraph
	IF OBJECT_ID('tempdb..#LocationsInSplitSubGraph') IS NOT NULL DROP TABLE #LocationsInSplitSubGraph
	IF OBJECT_ID('tempdb..#ChildStructureLocations') IS NOT NULL DROP TABLE #ChildStructureLocations
	IF OBJECT_ID('tempdb..#StructureLocations') IS NOT NULL DROP TABLE #StructureLocations
	IF OBJECT_ID('tempdb..#DistanceToEachStructure') IS NOT NULL DROP TABLE #DistanceToEachStructure
	IF OBJECT_ID('tempdb..#DistanceToNearestStructure') IS NOT NULL DROP TABLE #DistanceToNearestStructure
	IF OBJECT_ID('tempdb..#ParentIDForChildStructure') IS NOT NULL DROP TABLE #ParentIDForChildStructure 
	END
GO
