CREATE VIEW [dbo].[PostsWithOwnersView]
	AS SELECT  
	p.Id ,
	p.Title ,
	p.Content,
	p.OwnerId ,
	u.Name as OwnerName

	FROM dbo.Posts p INNER JOin dbo.Users u ON p.OwnerId = u.Id
