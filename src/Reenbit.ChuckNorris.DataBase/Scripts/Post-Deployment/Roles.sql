DECLARE @actualRoles TABLE (
	[Name] NVARCHAR(256),
	NormalizedName NVARCHAR(256),
	ConcurrencyStamp NVARCHAR (MAX) NULL
)

INSERT INTO @actualRoles
VALUES 
   	 ('SuperAdmin', 'SUPERADMIN', '7ab63ba4-2fe0-4f8d-bf90-c73a3664f8f1')

MERGE [dbo].[AspNetRoles] AS TARGET
USING @actualRoles AS SOURCE
ON TARGET.[ConcurrencyStamp] = SOURCE.[ConcurrencyStamp]
WHEN MATCHED
	THEN UPDATE
		 SET 
		 	 TARGET.[Name] = SOURCE.[Name],
			 TARGET.[NormalizedName] = SOURCE.[NormalizedName]
WHEN NOT MATCHED BY TARGET
	THEN INSERT ([Name], [NormalizedName], [ConcurrencyStamp])
		 VALUES (SOURCE.[Name], SOURCE.[NormalizedName],SOURCE.[ConcurrencyStamp])
WHEN NOT MATCHED BY SOURCE 
	THEN DELETE
;