USE [Rabbit]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnStructureVolume]    Script Date: 2/2/2018 12:53:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

	 
	 ALTER FUNCTION [dbo].[ufnStructureVolume]
	(
		-- Add the parameters for the function here
		@StructureID bigint
	)
	RETURNS float
	AS
	BEGIN
		declare @Area float
		declare @AreaScalar float
		--Measures the area of the PSD
		set @AreaScalar = dbo.XYScale() * dbo.ZScale()

		select top 1 @Area = sum(MosaicShape.STArea()) * @AreaScalar from Location 
		where ParentID = @StructureID and MosaicShape.STDimension() = 2
		group by ParentID
	  
		-- Return the result of the function
		RETURN @Area

	END
	
	