USE [Test]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnLastStructureModificationRecursive]    Script Date: 10/18/2017 1:38:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	Return the last modified date for a structure. 
-- =============================================
CREATE FUNCTION [dbo].[ufnLastStructureMorphologyModificationRecursive]
(
	-- Add the parameters for the function here
	@ID bigint
)
RETURNS DateTime
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar DateTime

	select @ResultVar = max(dbo.ufnLastStructureModification(S.ID)) from Structure S where S.ID = @ID or S.ParentID = @ID
	 
	RETURN @ResultVar
END
