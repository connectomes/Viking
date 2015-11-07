IF OBJECT_ID (N'dbo.ufnLineFromAngleAndDistance', N'FN') IS NOT NULL
    DROP FUNCTION ufnLineFromAngleAndDistance;
GO
CREATE FUNCTION dbo.ufnLineFromAngleAndDistance(@angle float, @distance float, @offset geometry)
RETURNS geometry
AS 
-- Returns a line centered on offset with @angle and total length = @distance
BEGIN
    DECLARE @ret geometry

	DECLARE @P1X float
	DECLARE @P1Y float
	DECLARE @P2X float
	DECLARE @P2Y float
	DECLARE @Radius float
	DECLARE @Tau float
	set @Radius = @distance / 2.0 

	
	--Need to create a line centered on 0,0 so we can translate it to the center of S
	set @P1X = (COS(@Angle - PI()) * @Radius) + @offset.STX
	set @P1Y = (SIN(@Angle - PI()) * @Radius) + @offset.STY
	set @P2X = (COS(@Angle) * @Radius) + @offset.STX
	set @P2Y = (SIN(@Angle) * @Radius) + @offset.STY
		
	set @ret = geometry::STLineFromText( 'LINESTRING ( ' + STR(@P1X)  + ' ' +
												STR(@P1Y) + ', ' +
												STR(@P2X) + ' ' +
												STR(@P2Y) + ')',0)
				  
    RETURN @ret
END