
CREATE TABLE dbo.Snippets (
    Id         NVARCHAR(450) NOT NULL PRIMARY KEY,
    Code       NVARCHAR(MAX) NOT NULL,
    Language   NVARCHAR(MAX) NOT NULL,
    CreatedAt  DATETIME2(7)  NOT NULL
);
GO


CREATE PROCEDURE dbo.BuscarSnippet
    @Id NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Code
    FROM dbo.Snippets
    WHERE Id = @Id;
END
GO

