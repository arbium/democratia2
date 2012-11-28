USE [Federation];
GO

IF OBJECT_ID('[GetOrderedByRatingGroupContent]', 'P') IS NOT NULL
    DROP PROCEDURE [GetOrderedByRatingGroupContent];
GO

CREATE PROCEDURE [GetOrderedByRatingGroupContent] (@group_id uniqueidentifier)
AS
BEGIN
	SELECT tmp1.Id, tmp1.PublishDate, tmp1.AuthorId, tmp1.Title, tmp1.[Text], tmp1.CreationDate, tmp1.[State], tmp1.IsDiscussionClosed, tmp1.GroupId FROM
	
	(SELECT content.*, DATEDIFF(d, content.PublishDate, getdate()) AS DayDiff, COUNT(comment.Id) AS CommentsCount
	FROM [ContentSet] AS content
	JOIN [CommentSet] AS comment ON comment.ContentId = content.Id
	WHERE GroupId = @group_id
	GROUP BY content.Id, content.PublishDate, content.AuthorId, content.Title, content.[Text], content.CreationDate, content.[State], content.IsDiscussionClosed, content.GroupId) AS tmp1

	JOIN
	(SELECT content.Id, COUNT([like].Id) AS MinusesCount
	FROM [ContentSet] AS content
	JOIN [LikeSet] AS [like] ON [like].ContentId = content.Id
	WHERE GroupId = @group_id AND [like].Value = 0
	GROUP BY content.Id) AS tmp2 ON tmp1.Id = tmp2.Id

	JOIN
	(SELECT content.Id, COUNT([like].Id) AS PlusesCount
	FROM [ContentSet] AS content
	JOIN [LikeSet] AS [like] ON [like].ContentId = content.Id
	WHERE GroupId = @group_id AND [like].Value = 1
	GROUP BY content.Id) AS tmp3 ON tmp1.Id = tmp3.Id

	ORDER BY (SQRT(PlusesCount * (PlusesCount + MinusesCount)) + CommentsCount / 10) / SQRT(DayDiff + 0.01) / SQRT(MinusesCount + 1) DESC
END
GO