USE [Test]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnLastStructureModification]    Script Date: 10/18/2017 1:31:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	Return the last modified date for a structure. 
-- =============================================
ALTER FUNCTION [dbo].[ufnLastStructureMorphologyModification]
(
	-- Add the parameters for the function here
	@ID bigint
)
RETURNS DateTime
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar DateTime

	-- Add the T-SQL statements to compute the return value here
	select @ResultVar = max(Q.LastModified) from (
		select L.LastModified as LastModified from Location L where L.ParentID = @ID
		union
		select LLA.Created as LastModified from Location L 
			inner join LocationLink LLA ON LLA.A = L.ID
			where L.ParentID = @ID
		union
		select S.LastModified as LastModified from Structure S where S.ID = @ID
		) Q
		
	RETURN @ResultVar
END
