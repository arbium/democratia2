USE [Federation];
GO

--IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE OBJECT_ID = OBJECT_ID('BaseUserSet_User'))
--	DROP FULLTEXT INDEX ON [BaseUserSet_User];
--GO

IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE OBJECT_ID = OBJECT_ID('CommentSet'))
	DROP FULLTEXT INDEX ON [CommentSet];
GO

IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE OBJECT_ID = OBJECT_ID('ContentSet'))
	DROP FULLTEXT INDEX ON [ContentSet];
GO

IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE OBJECT_ID = OBJECT_ID('GroupSet'))
	DROP FULLTEXT INDEX ON [GroupSet];
GO

--IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE OBJECT_ID = OBJECT_ID('TagSet'))
--	DROP FULLTEXT INDEX ON [TagSet];
--GO

IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE NAME = 'FullTextCatalog')
	DROP FULLTEXT CATALOG [FullTextCatalog];
GO

CREATE FULLTEXT CATALOG [FullTextCatalog];
GO

--CREATE FULLTEXT INDEX ON [BaseUserSet_User] (
--	[FirstName]
--		Language 1049,
--	[Info]
--		Language 1049,
--	[Patronymic]
--		Language 1049,
--	[SurName]
--		Language 1049
--)
--KEY INDEX [PK_BaseUserSet_User] ON [FullTextCatalog]
--WITH CHANGE_TRACKING AUTO
--GO

CREATE FULLTEXT INDEX ON [CommentSet] (
	[Text]
		Language 1049
)
KEY INDEX [PK_CommentSet] ON [FullTextCatalog]
WITH CHANGE_TRACKING AUTO
GO

CREATE FULLTEXT INDEX ON [ContentSet] (
	[Text]
		Language 1049,
	[Title]
		Language 1049
)
KEY INDEX [PK_ContentSet] ON [FullTextCatalog]
WITH CHANGE_TRACKING AUTO
GO

CREATE FULLTEXT INDEX ON [GroupSet] (
	[Label]
		Language 1049,
	[Name]
		Language 1049,
	[Summary]
		Language 1049
)
KEY INDEX [PK_GroupSet] ON [FullTextCatalog]
WITH CHANGE_TRACKING AUTO
GO

--CREATE FULLTEXT INDEX ON [TagSet] (
--	[Title]
--		Language 1049
--)
--KEY INDEX [PK_TagSet] ON [FullTextCatalog]
--WITH CHANGE_TRACKING AUTO
--GO

--IF OBJECT_ID('[GetUsersByFullText]', 'P') IS NOT NULL
--    DROP PROCEDURE [GetUsersByFullText];
--GO

IF OBJECT_ID('[GetCommentsByFullText]', 'P') IS NOT NULL
    DROP PROCEDURE [GetCommentsByFullText];
GO

IF OBJECT_ID('[GetPetitionsByFullText]', 'P') IS NOT NULL
    DROP PROCEDURE [GetPetitionsByFullText];
GO

IF OBJECT_ID('[GetPollsByFullText]', 'P') IS NOT NULL
    DROP PROCEDURE [GetPollsByFullText];
GO

IF OBJECT_ID('[GetSurveysByFullText]', 'P') IS NOT NULL
    DROP PROCEDURE [GetSurveysByFullText];
GO

IF OBJECT_ID('[GetPostsByFullText]', 'P') IS NOT NULL
    DROP PROCEDURE [GetPostsByFullText];
GO

IF OBJECT_ID('[GetGroupsByFullText]', 'P') IS NOT NULL
    DROP PROCEDURE [GetGroupsByFullText];
GO

--IF OBJECT_ID('[GetTagsByFullText]', 'P') IS NOT NULL
--    DROP PROCEDURE [GetTagsByFullText];
--GO

--CREATE PROCEDURE [GetUsersByFullText]
--(
--	@search_string varchar(250),
--	@search_count int
--)
--AS
--BEGIN
--	SELECT TOP (@search_count) [BaseUserSet_User].*, [BaseUserSet].* FROM 
--	FREETEXTTABLE ([BaseUserSet_User], ([FirstName],[Info],[Patronymic],[SurName]), @search_string) AS [SEARCH_RESULT]
--	LEFT JOIN [BaseUserSet_User] ON [BaseUserSet_User].[Id] = [SEARCH_RESULT].[KEY]
--	LEFT JOIN [BaseUserSet] ON [BaseUserSet].[Id] = [SEARCH_RESULT].[KEY]
--	ORDER BY [SEARCH_RESULT].[RANK] DESC
--END
--GO

CREATE PROCEDURE [GetCommentsByFullText]
(
	@search_string varchar(250),
	@search_count int
)
AS
BEGIN
	SELECT TOP (@search_count) [CommentSet].* FROM 
	FREETEXTTABLE ([CommentSet], [Text], @search_string) AS [SEARCH_RESULT]
	LEFT JOIN [CommentSet] ON [CommentSet].[Id] = [SEARCH_RESULT].[KEY]
	ORDER BY [SEARCH_RESULT].[RANK] DESC
END
GO

CREATE PROCEDURE [GetPostsByFullText]
(
	@search_string varchar(250),
	@search_count int
)
AS
BEGIN
	SELECT TOP (@search_count) [ContentSet].*, [ContentSet_Post].* FROM 
	FREETEXTTABLE ([ContentSet], ([Text],[Title]), @search_string) AS [SEARCH_RESULT]
	LEFT JOIN [ContentSet] ON [ContentSet].[Id] = [SEARCH_RESULT].[KEY]
	LEFT JOIN [ContentSet_Post] ON [ContentSet_Post].[Id] = [SEARCH_RESULT].[KEY]	
	WHERE (SELECT COUNT(*) FROM [ContentSet_Post] WHERE [ContentSet_Post].[Id] = [SEARCH_RESULT].[KEY]) != 0
	ORDER BY [SEARCH_RESULT].[RANK] DESC
END
GO

CREATE PROCEDURE [GetPetitionsByFullText]
(
	@search_string varchar(250),
	@search_count int
)
AS
BEGIN
	SELECT TOP (@search_count) [ContentSet].*, [ContentSet_Voting].*, [ContentSet_Petition].* FROM 
	FREETEXTTABLE ([ContentSet], ([Text],[Title]), @search_string) AS [SEARCH_RESULT]	
	LEFT JOIN [ContentSet] ON [ContentSet].[Id] = [SEARCH_RESULT].[KEY]
	LEFT JOIN [ContentSet_Voting] ON [ContentSet_Voting].[Id] = [SEARCH_RESULT].[KEY]
	LEFT JOIN [ContentSet_Petition] ON [ContentSet_Petition].[Id] = [SEARCH_RESULT].[KEY]
	WHERE (SELECT COUNT(*) FROM [ContentSet_Petition] WHERE [ContentSet_Petition].[Id] = [SEARCH_RESULT].[KEY]) != 0
	ORDER BY [SEARCH_RESULT].[RANK] DESC
END
GO

CREATE PROCEDURE [GetPollsByFullText]
(
	@search_string varchar(250),
	@search_count int
)
AS
BEGIN
	SELECT TOP (@search_count) [ContentSet].*, [ContentSet_Voting].*, [ContentSet_Poll].* FROM 
	FREETEXTTABLE ([ContentSet], ([Text],[Title]), @search_string) AS [SEARCH_RESULT]	
	LEFT JOIN [ContentSet] ON [ContentSet].[Id] = [SEARCH_RESULT].[KEY]
	LEFT JOIN [ContentSet_Voting] ON [ContentSet_Voting].[Id] = [SEARCH_RESULT].[KEY]
	LEFT JOIN [ContentSet_Poll] ON [ContentSet_Poll].[Id] = [SEARCH_RESULT].[KEY]
	WHERE (SELECT COUNT(*) FROM [ContentSet_Poll] WHERE [ContentSet_Poll].[Id] = [SEARCH_RESULT].[KEY]) != 0
	ORDER BY [SEARCH_RESULT].[RANK] DESC
END
GO

CREATE PROCEDURE [GetSurveysByFullText]
(
	@search_string varchar(250),
	@search_count int
)
AS
BEGIN
	SELECT TOP (@search_count) [ContentSet].*, [ContentSet_Voting].*, [ContentSet_Survey].* FROM 
	FREETEXTTABLE ([ContentSet], ([Text],[Title]), @search_string) AS [SEARCH_RESULT]	
	LEFT JOIN [ContentSet] ON [ContentSet].[Id] = [SEARCH_RESULT].[KEY]
	LEFT JOIN [ContentSet_Voting] ON [ContentSet_Voting].[Id] = [SEARCH_RESULT].[KEY]
	LEFT JOIN [ContentSet_Survey] ON [ContentSet_Survey].[Id] = [SEARCH_RESULT].[KEY]
	WHERE (SELECT COUNT(*) FROM [ContentSet_Survey] WHERE [ContentSet_Survey].[Id] = [SEARCH_RESULT].[KEY]) != 0
	ORDER BY [SEARCH_RESULT].[RANK] DESC
END
GO

CREATE PROCEDURE [GetGroupsByFullText]
(
	@search_string varchar(250),
	@search_count int
)
AS
BEGIN
	SELECT TOP (@search_count) [GroupSet].* FROM 
	FREETEXTTABLE ([GroupSet], ([Label],[Name],[Summary]), @search_string) AS [SEARCH_RESULT]
	LEFT JOIN [GroupSet] ON [GroupSet].[Id] = [SEARCH_RESULT].[KEY]
	ORDER BY [SEARCH_RESULT].[RANK] DESC
END
GO

--CREATE PROCEDURE [GetTagsByFullText]
--(
--	@search_string varchar(250),
--	@search_count int
--)
--AS
--BEGIN
--	SELECT TOP (@search_count) [TagSet].* FROM 
--	FREETEXTTABLE ([TagSet], [Title], @search_string) AS [SEARCH_RESULT]
--	LEFT JOIN [TagSet] ON [TagSet].[Id] = [SEARCH_RESULT].[KEY]
--	ORDER BY [SEARCH_RESULT].[RANK] DESC
--END
--GO