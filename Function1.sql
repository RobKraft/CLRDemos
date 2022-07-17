CREATE FUNCTION [dbo].[Function1]
(
	@param1 int,
	@param2 int
)
RETURNS INT
AS EXTERNAL NAME SomeAssembly.SomeType.SomeMethod
