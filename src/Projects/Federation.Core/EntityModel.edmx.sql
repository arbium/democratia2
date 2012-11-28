
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 11/09/2012 20:21:05
-- Generated from EDMX file: C:\Projects\Federation\src\Projects\Federation.Core\EntityModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Federation];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_GroupMemberGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupMemberSet] DROP CONSTRAINT [FK_GroupMemberGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_UserGroupMember]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupMemberSet] DROP CONSTRAINT [FK_UserGroupMember];
GO
IF OBJECT_ID(N'[dbo].[FK_ExpertGroupMember]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExpertSet] DROP CONSTRAINT [FK_ExpertGroupMember];
GO
IF OBJECT_ID(N'[dbo].[FK_TagExpert_Tag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TagExpert] DROP CONSTRAINT [FK_TagExpert_Tag];
GO
IF OBJECT_ID(N'[dbo].[FK_TagExpert_Expert]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TagExpert] DROP CONSTRAINT [FK_TagExpert_Expert];
GO
IF OBJECT_ID(N'[dbo].[FK_ExpertExpertVote]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExpertVoteSet] DROP CONSTRAINT [FK_ExpertExpertVote];
GO
IF OBJECT_ID(N'[dbo].[FK_ExpertVoteTag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExpertVoteSet] DROP CONSTRAINT [FK_ExpertVoteTag];
GO
IF OBJECT_ID(N'[dbo].[FK_ExpertVoteGroupMember]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExpertVoteSet] DROP CONSTRAINT [FK_ExpertVoteGroupMember];
GO
IF OBJECT_ID(N'[dbo].[FK_CandidateGroupMember]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CandidateSet] DROP CONSTRAINT [FK_CandidateGroupMember];
GO
IF OBJECT_ID(N'[dbo].[FK_ElectionBulletinCandidate_ElectionBulletin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ElectionBulletinCandidate] DROP CONSTRAINT [FK_ElectionBulletinCandidate_ElectionBulletin];
GO
IF OBJECT_ID(N'[dbo].[FK_ElectionBulletinCandidate_Candidate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ElectionBulletinCandidate] DROP CONSTRAINT [FK_ElectionBulletinCandidate_Candidate];
GO
IF OBJECT_ID(N'[dbo].[FK_ExpertGrantorPollBulletin_PollBulletin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExpertGrantorPollBulletin] DROP CONSTRAINT [FK_ExpertGrantorPollBulletin_PollBulletin];
GO
IF OBJECT_ID(N'[dbo].[FK_ExpertGrantorPollBulletin_PollBulletin1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExpertGrantorPollBulletin] DROP CONSTRAINT [FK_ExpertGrantorPollBulletin_PollBulletin1];
GO
IF OBJECT_ID(N'[dbo].[FK_AddressCity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AddressSet] DROP CONSTRAINT [FK_AddressCity];
GO
IF OBJECT_ID(N'[dbo].[FK_BirthAddressUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BaseUserSet_User] DROP CONSTRAINT [FK_BirthAddressUser];
GO
IF OBJECT_ID(N'[dbo].[FK_CommentUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CommentSet] DROP CONSTRAINT [FK_CommentUser];
GO
IF OBJECT_ID(N'[dbo].[FK_ContentComment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CommentSet] DROP CONSTRAINT [FK_ContentComment];
GO
IF OBJECT_ID(N'[dbo].[FK_ContentTag_Content]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentTag] DROP CONSTRAINT [FK_ContentTag_Content];
GO
IF OBJECT_ID(N'[dbo].[FK_ContentTag_Tag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentTag] DROP CONSTRAINT [FK_ContentTag_Tag];
GO
IF OBJECT_ID(N'[dbo].[FK_ContentUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentSet] DROP CONSTRAINT [FK_ContentUser];
GO
IF OBJECT_ID(N'[dbo].[FK_PollBulletinPoll]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BulletinSet_PollBulletin] DROP CONSTRAINT [FK_PollBulletinPoll];
GO
IF OBJECT_ID(N'[dbo].[FK_ElectionBulletinElection]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BulletinSet_ElectionBulletin] DROP CONSTRAINT [FK_ElectionBulletinElection];
GO
IF OBJECT_ID(N'[dbo].[FK_CandidateElection]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CandidateSet] DROP CONSTRAINT [FK_CandidateElection];
GO
IF OBJECT_ID(N'[dbo].[FK_RegionCity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CitySet] DROP CONSTRAINT [FK_RegionCity];
GO
IF OBJECT_ID(N'[dbo].[FK_AdministratorBlockedRecord]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BlockedRecordSet] DROP CONSTRAINT [FK_AdministratorBlockedRecord];
GO
IF OBJECT_ID(N'[dbo].[FK_SignerPetition_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SignerPetition] DROP CONSTRAINT [FK_SignerPetition_User];
GO
IF OBJECT_ID(N'[dbo].[FK_SignerPetition_Petition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SignerPetition] DROP CONSTRAINT [FK_SignerPetition_Petition];
GO
IF OBJECT_ID(N'[dbo].[FK_ResidentialAddressUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BaseUserSet_User] DROP CONSTRAINT [FK_ResidentialAddressUser];
GO
IF OBJECT_ID(N'[dbo].[FK_BlockedRecordBaseUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BlockedRecordSet] DROP CONSTRAINT [FK_BlockedRecordBaseUser];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupInheritance]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupSet] DROP CONSTRAINT [FK_GroupInheritance];
GO
IF OBJECT_ID(N'[dbo].[FK_InviteBaseUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[InviteSet] DROP CONSTRAINT [FK_InviteBaseUser];
GO
IF OBJECT_ID(N'[dbo].[FK_InviteBaseUser1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BaseUserSet] DROP CONSTRAINT [FK_InviteBaseUser1];
GO
IF OBJECT_ID(N'[dbo].[FK_ChildComments]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CommentSet] DROP CONSTRAINT [FK_ChildComments];
GO
IF OBJECT_ID(N'[dbo].[FK_CoauthorUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CoauthorSet] DROP CONSTRAINT [FK_CoauthorUser];
GO
IF OBJECT_ID(N'[dbo].[FK_CoauthorPetition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CoauthorSet] DROP CONSTRAINT [FK_CoauthorPetition];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupUser_Group]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubscriptionGroupUser] DROP CONSTRAINT [FK_GroupUser_Group];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupUser_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubscriptionGroupUser] DROP CONSTRAINT [FK_GroupUser_User];
GO
IF OBJECT_ID(N'[dbo].[FK_SubscribeUserUser_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubscriptionUserUser] DROP CONSTRAINT [FK_SubscribeUserUser_User];
GO
IF OBJECT_ID(N'[dbo].[FK_SubscribeUserUser_User1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubscriptionUserUser] DROP CONSTRAINT [FK_SubscribeUserUser_User1];
GO
IF OBJECT_ID(N'[dbo].[FK_LikeUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LikeSet] DROP CONSTRAINT [FK_LikeUser];
GO
IF OBJECT_ID(N'[dbo].[FK_MessageBaseUserAuthor]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MessageSet] DROP CONSTRAINT [FK_MessageBaseUserAuthor];
GO
IF OBJECT_ID(N'[dbo].[FK_MessageBaseUserRecipient]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MessageSet] DROP CONSTRAINT [FK_MessageBaseUserRecipient];
GO
IF OBJECT_ID(N'[dbo].[FK_MailRecordUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MailRecordSet] DROP CONSTRAINT [FK_MailRecordUser];
GO
IF OBJECT_ID(N'[dbo].[FK_BulletinGroupMember]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BulletinSet] DROP CONSTRAINT [FK_BulletinGroupMember];
GO
IF OBJECT_ID(N'[dbo].[FK_BadgeUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BadgeSet] DROP CONSTRAINT [FK_BadgeUser];
GO
IF OBJECT_ID(N'[dbo].[FK_BadgeGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BadgeSet] DROP CONSTRAINT [FK_BadgeGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_CommentComment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CommentSet] DROP CONSTRAINT [FK_CommentComment];
GO
IF OBJECT_ID(N'[dbo].[FK_CandidatePetition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentSet_Petition] DROP CONSTRAINT [FK_CandidatePetition];
GO
IF OBJECT_ID(N'[dbo].[FK_AttachContent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AttachSet] DROP CONSTRAINT [FK_AttachContent];
GO
IF OBJECT_ID(N'[dbo].[FK_ContentGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentSet] DROP CONSTRAINT [FK_ContentGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_AttachGroup_Attach]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AttachGroup] DROP CONSTRAINT [FK_AttachGroup_Attach];
GO
IF OBJECT_ID(N'[dbo].[FK_AttachGroup_Group]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AttachGroup] DROP CONSTRAINT [FK_AttachGroup_Group];
GO
IF OBJECT_ID(N'[dbo].[FK_AttachUser_Attach]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AttachUser] DROP CONSTRAINT [FK_AttachUser_Attach];
GO
IF OBJECT_ID(N'[dbo].[FK_AttachUser_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AttachUser] DROP CONSTRAINT [FK_AttachUser_User];
GO
IF OBJECT_ID(N'[dbo].[FK_AlbumGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AlbumSet] DROP CONSTRAINT [FK_AlbumGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_AlbumUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AlbumSet] DROP CONSTRAINT [FK_AlbumUser];
GO
IF OBJECT_ID(N'[dbo].[FK_AlbumItemAlbum]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AlbumItemSet] DROP CONSTRAINT [FK_AlbumItemAlbum];
GO
IF OBJECT_ID(N'[dbo].[FK_LikeContent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LikeSet] DROP CONSTRAINT [FK_LikeContent];
GO
IF OBJECT_ID(N'[dbo].[FK_LikeAlbumItem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LikeSet] DROP CONSTRAINT [FK_LikeAlbumItem];
GO
IF OBJECT_ID(N'[dbo].[FK_ProfileChangeRequestUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProfileChangeRequestSet] DROP CONSTRAINT [FK_ProfileChangeRequestUser];
GO
IF OBJECT_ID(N'[dbo].[FK_SocialAccountUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SocialAccountSet] DROP CONSTRAINT [FK_SocialAccountUser];
GO
IF OBJECT_ID(N'[dbo].[FK_ScheduleJobScheduleExecutionInfo]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ScheduleExecutionInfoSet] DROP CONSTRAINT [FK_ScheduleJobScheduleExecutionInfo];
GO
IF OBJECT_ID(N'[dbo].[FK_SurveyOption]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OptionSet] DROP CONSTRAINT [FK_SurveyOption];
GO
IF OBJECT_ID(N'[dbo].[FK_SurveySurveyBulletins]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SurveyBulletinSet] DROP CONSTRAINT [FK_SurveySurveyBulletins];
GO
IF OBJECT_ID(N'[dbo].[FK_SurveyBulletinUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SurveyBulletinSet] DROP CONSTRAINT [FK_SurveyBulletinUser];
GO
IF OBJECT_ID(N'[dbo].[FK_SurveyBulletinOption_SurveyBulletin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SurveyBulletinOption] DROP CONSTRAINT [FK_SurveyBulletinOption_SurveyBulletin];
GO
IF OBJECT_ID(N'[dbo].[FK_SurveyBulletinOption_Option]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SurveyBulletinOption] DROP CONSTRAINT [FK_SurveyBulletinOption_Option];
GO
IF OBJECT_ID(N'[dbo].[FK_SmsInfoBaseUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SmsInfoSet] DROP CONSTRAINT [FK_SmsInfoBaseUser];
GO
IF OBJECT_ID(N'[dbo].[FK_TagGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TagSet] DROP CONSTRAINT [FK_TagGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupCategoryGroup_GroupCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupCategoryGroup] DROP CONSTRAINT [FK_GroupCategoryGroup_GroupCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupCategoryGroup_Group]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupCategoryGroup] DROP CONSTRAINT [FK_GroupCategoryGroup_Group];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupRatingGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupRatingSet] DROP CONSTRAINT [FK_GroupRatingGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupFriendlyGroups]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupSet] DROP CONSTRAINT [FK_GroupFriendlyGroups];
GO
IF OBJECT_ID(N'[dbo].[FK_GroupAdGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[GroupAdSet] DROP CONSTRAINT [FK_GroupAdGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_LikeComment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LikeSet] DROP CONSTRAINT [FK_LikeComment];
GO
IF OBJECT_ID(N'[dbo].[FK_UserBlackedUser_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserBlackedUser] DROP CONSTRAINT [FK_UserBlackedUser_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserBlackedUser_User1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserBlackedUser] DROP CONSTRAINT [FK_UserBlackedUser_User1];
GO
IF OBJECT_ID(N'[dbo].[FK_UserPayPalVerification]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PayPalVerificationSet] DROP CONSTRAINT [FK_UserPayPalVerification];
GO
IF OBJECT_ID(N'[dbo].[FK_SubscriptionSettingsUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SubscriptionSettingsSet] DROP CONSTRAINT [FK_SubscriptionSettingsUser];
GO
IF OBJECT_ID(N'[dbo].[FK_User_inherits_BaseUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BaseUserSet_User] DROP CONSTRAINT [FK_User_inherits_BaseUser];
GO
IF OBJECT_ID(N'[dbo].[FK_ElectionBulletin_inherits_Bulletin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BulletinSet_ElectionBulletin] DROP CONSTRAINT [FK_ElectionBulletin_inherits_Bulletin];
GO
IF OBJECT_ID(N'[dbo].[FK_PollBulletin_inherits_Bulletin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BulletinSet_PollBulletin] DROP CONSTRAINT [FK_PollBulletin_inherits_Bulletin];
GO
IF OBJECT_ID(N'[dbo].[FK_Voting_inherits_Content]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentSet_Voting] DROP CONSTRAINT [FK_Voting_inherits_Content];
GO
IF OBJECT_ID(N'[dbo].[FK_Poll_inherits_Voting]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentSet_Poll] DROP CONSTRAINT [FK_Poll_inherits_Voting];
GO
IF OBJECT_ID(N'[dbo].[FK_Election_inherits_Voting]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentSet_Election] DROP CONSTRAINT [FK_Election_inherits_Voting];
GO
IF OBJECT_ID(N'[dbo].[FK_Administrator_inherits_BaseUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[BaseUserSet_Administrator] DROP CONSTRAINT [FK_Administrator_inherits_BaseUser];
GO
IF OBJECT_ID(N'[dbo].[FK_Petition_inherits_Voting]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentSet_Petition] DROP CONSTRAINT [FK_Petition_inherits_Voting];
GO
IF OBJECT_ID(N'[dbo].[FK_Survey_inherits_Voting]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentSet_Survey] DROP CONSTRAINT [FK_Survey_inherits_Voting];
GO
IF OBJECT_ID(N'[dbo].[FK_Post_inherits_Content]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ContentSet_Post] DROP CONSTRAINT [FK_Post_inherits_Content];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[BaseUserSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BaseUserSet];
GO
IF OBJECT_ID(N'[dbo].[GroupSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GroupSet];
GO
IF OBJECT_ID(N'[dbo].[GroupMemberSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GroupMemberSet];
GO
IF OBJECT_ID(N'[dbo].[TagSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TagSet];
GO
IF OBJECT_ID(N'[dbo].[ExpertSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExpertSet];
GO
IF OBJECT_ID(N'[dbo].[ExpertVoteSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExpertVoteSet];
GO
IF OBJECT_ID(N'[dbo].[CandidateSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CandidateSet];
GO
IF OBJECT_ID(N'[dbo].[AddressSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AddressSet];
GO
IF OBJECT_ID(N'[dbo].[CitySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CitySet];
GO
IF OBJECT_ID(N'[dbo].[CommentSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CommentSet];
GO
IF OBJECT_ID(N'[dbo].[MessageSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MessageSet];
GO
IF OBJECT_ID(N'[dbo].[ContentSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContentSet];
GO
IF OBJECT_ID(N'[dbo].[BulletinSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BulletinSet];
GO
IF OBJECT_ID(N'[dbo].[RegionSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegionSet];
GO
IF OBJECT_ID(N'[dbo].[BlockedRecordSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BlockedRecordSet];
GO
IF OBJECT_ID(N'[dbo].[InviteSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[InviteSet];
GO
IF OBJECT_ID(N'[dbo].[CoauthorSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CoauthorSet];
GO
IF OBJECT_ID(N'[dbo].[LikeSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LikeSet];
GO
IF OBJECT_ID(N'[dbo].[MailRecordSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MailRecordSet];
GO
IF OBJECT_ID(N'[dbo].[BadgeSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BadgeSet];
GO
IF OBJECT_ID(N'[dbo].[ErrorTextSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ErrorTextSet];
GO
IF OBJECT_ID(N'[dbo].[AttachSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AttachSet];
GO
IF OBJECT_ID(N'[dbo].[ScheduleJobSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ScheduleJobSet];
GO
IF OBJECT_ID(N'[dbo].[AlbumSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AlbumSet];
GO
IF OBJECT_ID(N'[dbo].[AlbumItemSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AlbumItemSet];
GO
IF OBJECT_ID(N'[dbo].[ProfileChangeRequestSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProfileChangeRequestSet];
GO
IF OBJECT_ID(N'[dbo].[SocialAccountSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SocialAccountSet];
GO
IF OBJECT_ID(N'[dbo].[ScheduleExecutionInfoSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ScheduleExecutionInfoSet];
GO
IF OBJECT_ID(N'[dbo].[OptionSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OptionSet];
GO
IF OBJECT_ID(N'[dbo].[SurveyBulletinSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SurveyBulletinSet];
GO
IF OBJECT_ID(N'[dbo].[SmsInfoSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SmsInfoSet];
GO
IF OBJECT_ID(N'[dbo].[GroupAdSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GroupAdSet];
GO
IF OBJECT_ID(N'[dbo].[GroupCategorySet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GroupCategorySet];
GO
IF OBJECT_ID(N'[dbo].[GroupRatingSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GroupRatingSet];
GO
IF OBJECT_ID(N'[dbo].[PayPalVerificationSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PayPalVerificationSet];
GO
IF OBJECT_ID(N'[dbo].[UserAuthentificationLogSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserAuthentificationLogSet];
GO
IF OBJECT_ID(N'[dbo].[UserAuthorizationLogSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserAuthorizationLogSet];
GO
IF OBJECT_ID(N'[dbo].[UserPollVoteLogSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserPollVoteLogSet];
GO
IF OBJECT_ID(N'[dbo].[UserRegistrationLogSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserRegistrationLogSet];
GO
IF OBJECT_ID(N'[dbo].[SubscriptionSettingsSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SubscriptionSettingsSet];
GO
IF OBJECT_ID(N'[dbo].[BaseUserSet_User]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BaseUserSet_User];
GO
IF OBJECT_ID(N'[dbo].[BulletinSet_ElectionBulletin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BulletinSet_ElectionBulletin];
GO
IF OBJECT_ID(N'[dbo].[BulletinSet_PollBulletin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BulletinSet_PollBulletin];
GO
IF OBJECT_ID(N'[dbo].[ContentSet_Voting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContentSet_Voting];
GO
IF OBJECT_ID(N'[dbo].[ContentSet_Poll]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContentSet_Poll];
GO
IF OBJECT_ID(N'[dbo].[ContentSet_Election]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContentSet_Election];
GO
IF OBJECT_ID(N'[dbo].[BaseUserSet_Administrator]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BaseUserSet_Administrator];
GO
IF OBJECT_ID(N'[dbo].[ContentSet_Petition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContentSet_Petition];
GO
IF OBJECT_ID(N'[dbo].[ContentSet_Survey]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContentSet_Survey];
GO
IF OBJECT_ID(N'[dbo].[ContentSet_Post]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContentSet_Post];
GO
IF OBJECT_ID(N'[dbo].[TagExpert]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TagExpert];
GO
IF OBJECT_ID(N'[dbo].[ElectionBulletinCandidate]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ElectionBulletinCandidate];
GO
IF OBJECT_ID(N'[dbo].[ExpertGrantorPollBulletin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExpertGrantorPollBulletin];
GO
IF OBJECT_ID(N'[dbo].[ContentTag]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContentTag];
GO
IF OBJECT_ID(N'[dbo].[SignerPetition]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SignerPetition];
GO
IF OBJECT_ID(N'[dbo].[SubscriptionGroupUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SubscriptionGroupUser];
GO
IF OBJECT_ID(N'[dbo].[SubscriptionUserUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SubscriptionUserUser];
GO
IF OBJECT_ID(N'[dbo].[AttachGroup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AttachGroup];
GO
IF OBJECT_ID(N'[dbo].[AttachUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AttachUser];
GO
IF OBJECT_ID(N'[dbo].[SurveyBulletinOption]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SurveyBulletinOption];
GO
IF OBJECT_ID(N'[dbo].[GroupCategoryGroup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GroupCategoryGroup];
GO
IF OBJECT_ID(N'[dbo].[UserBlackedUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserBlackedUser];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'BaseUserSet'
CREATE TABLE [dbo].[BaseUserSet] (
    [Id] uniqueidentifier  NOT NULL,
    [InviteTicket_Id] uniqueidentifier  NULL
);
GO

-- Creating table 'GroupSet'
CREATE TABLE [dbo].[GroupSet] (
    [Id] uniqueidentifier  NOT NULL,
    [ModeratorsCount] smallint  NOT NULL,
    [ElectionFrequency] smallint  NOT NULL,
    [Privacy] smallint  NOT NULL,
    [State] tinyint  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Summary] nvarchar(max)  NULL,
    [Logo] nvarchar(max)  NULL,
    [CreationDate] datetime  NOT NULL,
    [Label] nvarchar(max)  NULL,
    [ParentGroupId] uniqueidentifier  NULL,
    [Type] tinyint  NOT NULL,
    [PollQuorum] tinyint  NOT NULL,
    [ElectionQuorum] tinyint  NOT NULL,
    [GroupFriendlyGroups_FriendlyGroup_Id] uniqueidentifier  NULL
);
GO

-- Creating table 'GroupMemberSet'
CREATE TABLE [dbo].[GroupMemberSet] (
    [Id] uniqueidentifier  NOT NULL,
    [GroupId] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [EntryDate] datetime  NOT NULL,
    [State] tinyint  NOT NULL,
    [ExitDate] datetime  NULL
);
GO

-- Creating table 'TagSet'
CREATE TABLE [dbo].[TagSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [IsRecommended] bit  NOT NULL,
    [GroupId] uniqueidentifier  NULL,
    [TopicState] tinyint  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Weight] int  NOT NULL,
    [LowerTitle] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ExpertSet'
CREATE TABLE [dbo].[ExpertSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Info] nvarchar(max)  NULL,
    [GroupMember_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ExpertVoteSet'
CREATE TABLE [dbo].[ExpertVoteSet] (
    [Id] uniqueidentifier  NOT NULL,
    [ExpertId] uniqueidentifier  NOT NULL,
    [TagId] uniqueidentifier  NOT NULL,
    [GroupMemberId] uniqueidentifier  NOT NULL,
    [Date] datetime  NOT NULL
);
GO

-- Creating table 'CandidateSet'
CREATE TABLE [dbo].[CandidateSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ElectionId] uniqueidentifier  NOT NULL,
    [GroupMember_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AddressSet'
CREATE TABLE [dbo].[AddressSet] (
    [Id] uniqueidentifier  NOT NULL,
    [PostalCode] nvarchar(max)  NULL,
    [Street] nvarchar(max)  NOT NULL,
    [House] nvarchar(max)  NOT NULL,
    [Apartment] nvarchar(max)  NULL,
    [CityId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'CitySet'
CREATE TABLE [dbo].[CitySet] (
    [Id] uniqueidentifier  NOT NULL,
    [RegionId] uniqueidentifier  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Type] tinyint  NOT NULL
);
GO

-- Creating table 'CommentSet'
CREATE TABLE [dbo].[CommentSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [ContentId] uniqueidentifier  NOT NULL,
    [DateTime] datetime  NOT NULL,
    [IsHidden] bit  NOT NULL,
    [ParentCommentId] uniqueidentifier  NULL,
    [ReplyToId] uniqueidentifier  NULL
);
GO

-- Creating table 'MessageSet'
CREATE TABLE [dbo].[MessageSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [Date] datetime  NOT NULL,
    [AuthorId] uniqueidentifier  NULL,
    [RecipientId] uniqueidentifier  NOT NULL,
    [IsRead] bit  NOT NULL,
    [Type] tinyint  NOT NULL,
    [IsDeletedByRecipient] bit  NOT NULL,
    [IsDeletedByAuthor] bit  NOT NULL
);
GO

-- Creating table 'ContentSet'
CREATE TABLE [dbo].[ContentSet] (
    [Id] uniqueidentifier  NOT NULL,
    [AuthorId] uniqueidentifier  NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [PublishDate] datetime  NULL,
    [State] tinyint  NOT NULL,
    [IsDiscussionClosed] bit  NOT NULL,
    [GroupId] uniqueidentifier  NULL,
    [IsHidden] bit  NOT NULL
);
GO

-- Creating table 'BulletinSet'
CREATE TABLE [dbo].[BulletinSet] (
    [Id] uniqueidentifier  NOT NULL,
    [OwnerId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RegionSet'
CREATE TABLE [dbo].[RegionSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Title] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'BlockedRecordSet'
CREATE TABLE [dbo].[BlockedRecordSet] (
    [Id] uniqueidentifier  NOT NULL,
    [BlockDate] datetime  NOT NULL,
    [BlockExpireDate] datetime  NOT NULL,
    [AdministratorId] uniqueidentifier  NOT NULL,
    [Reason] nvarchar(max)  NOT NULL,
    [BaseUser_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'InviteSet'
CREATE TABLE [dbo].[InviteSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Surname] nvarchar(max)  NOT NULL,
    [Patronymic] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NULL,
    [Facebook] nvarchar(max)  NULL,
    [LiveJournal] nvarchar(max)  NULL,
    [Key] nvarchar(max)  NULL,
    [State] tinyint  NOT NULL,
    [BaseUserId] uniqueidentifier  NULL,
    [UserInfo] nvarchar(max)  NULL,
    [Comment] nvarchar(max)  NULL,
    [CreationDate] datetime  NOT NULL,
    [Phone] nvarchar(max)  NULL
);
GO

-- Creating table 'CoauthorSet'
CREATE TABLE [dbo].[CoauthorSet] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [PetitionId] uniqueidentifier  NOT NULL,
    [IsAccepted] bit  NULL
);
GO

-- Creating table 'LikeSet'
CREATE TABLE [dbo].[LikeSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Value] bit  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [ContentId] uniqueidentifier  NULL,
    [AlbumItemId] uniqueidentifier  NULL,
    [CommentId] uniqueidentifier  NULL
);
GO

-- Creating table 'MailRecordSet'
CREATE TABLE [dbo].[MailRecordSet] (
    [Id] uniqueidentifier  NOT NULL,
    [IsDelivered] bit  NOT NULL,
    [Date] datetime  NOT NULL,
    [User_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'BadgeSet'
CREATE TABLE [dbo].[BadgeSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Type] int  NULL,
    [UserId] uniqueidentifier  NULL,
    [GroupId] uniqueidentifier  NULL,
    [IsAcquired] bit  NOT NULL,
    [Value] int  NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Image] nvarchar(max)  NOT NULL,
    [AcquireDate] datetime  NULL
);
GO

-- Creating table 'ErrorTextSet'
CREATE TABLE [dbo].[ErrorTextSet] (
    [Key] varchar(30)  NOT NULL,
    [Text] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'AttachSet'
CREATE TABLE [dbo].[AttachSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Content_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ScheduleJobSet'
CREATE TABLE [dbo].[ScheduleJobSet] (
    [Id] uniqueidentifier  NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [ExecutionDate] datetime  NOT NULL,
    [Task] varbinary(max)  NOT NULL,
    [State] tinyint  NOT NULL,
    [DoRepeat] bit  NOT NULL,
    [TryRecover] bit  NOT NULL,
    [RepeatPeriod] int  NULL,
    [NextExecutionDate] datetime  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [IsUnique] bit  NOT NULL
);
GO

-- Creating table 'AlbumSet'
CREATE TABLE [dbo].[AlbumSet] (
    [Id] uniqueidentifier  NOT NULL,
    [GroupId] uniqueidentifier  NULL,
    [UserId] uniqueidentifier  NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [ChangeDate] datetime  NOT NULL,
    [IsOpen] bit  NOT NULL
);
GO

-- Creating table 'AlbumItemSet'
CREATE TABLE [dbo].[AlbumItemSet] (
    [Id] uniqueidentifier  NOT NULL,
    [AlbumId] uniqueidentifier  NOT NULL,
    [Title] nvarchar(max)  NULL,
    [Description] nvarchar(max)  NULL,
    [Type] tinyint  NOT NULL,
    [Src] nvarchar(max)  NOT NULL,
    [CreationDate] datetime  NOT NULL
);
GO

-- Creating table 'ProfileChangeRequestSet'
CREATE TABLE [dbo].[ProfileChangeRequestSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Firstname] nvarchar(max)  NOT NULL,
    [Surname] nvarchar(max)  NOT NULL,
    [Patronymic] nvarchar(max)  NOT NULL,
    [PhoneNumber] nvarchar(max)  NOT NULL,
    [Facebook] nvarchar(max)  NULL,
    [LiveJournal] nvarchar(max)  NULL,
    [Comment] nvarchar(max)  NULL,
    [AdminResponse] nvarchar(max)  NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [Date] datetime  NOT NULL
);
GO

-- Creating table 'SocialAccountSet'
CREATE TABLE [dbo].[SocialAccountSet] (
    [Id] uniqueidentifier  NOT NULL,
    [SocialId] nvarchar(max)  NOT NULL,
    [DirectUrl] nvarchar(max)  NULL,
    [SocialType] tinyint  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ScheduleExecutionInfoSet'
CREATE TABLE [dbo].[ScheduleExecutionInfoSet] (
    [Id] uniqueidentifier  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [Info] nvarchar(max)  NULL,
    [State] tinyint  NOT NULL,
    [ScheduleJobId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'OptionSet'
CREATE TABLE [dbo].[OptionSet] (
    [Id] uniqueidentifier  NOT NULL,
    [SurveyId] uniqueidentifier  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Position] tinyint  NOT NULL
);
GO

-- Creating table 'SurveyBulletinSet'
CREATE TABLE [dbo].[SurveyBulletinSet] (
    [SurveyId] uniqueidentifier  NOT NULL,
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'SmsInfoSet'
CREATE TABLE [dbo].[SmsInfoSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Phone] nvarchar(20)  NOT NULL,
    [CreationDate] datetime  NOT NULL,
    [ChangeDate] datetime  NOT NULL,
    [Cost] float  NOT NULL,
    [BaseUserId] uniqueidentifier  NULL,
    [Message] nvarchar(800)  NULL,
    [ResponseError] tinyint  NULL,
    [SendError] tinyint  NULL,
    [State] tinyint  NOT NULL,
    [PartsCount] tinyint  NOT NULL
);
GO

-- Creating table 'GroupAdSet'
CREATE TABLE [dbo].[GroupAdSet] (
    [Id] uniqueidentifier  NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [Group_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'GroupCategorySet'
CREATE TABLE [dbo].[GroupCategorySet] (
    [Id] uniqueidentifier  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'GroupRatingSet'
CREATE TABLE [dbo].[GroupRatingSet] (
    [Id] uniqueidentifier  NOT NULL,
    [OverallRating] int  NOT NULL,
    [OldOverallRating] int  NOT NULL,
    [MembersRating] int  NOT NULL,
    [ContentRating] int  NOT NULL,
    [ExpertsRating] int  NOT NULL,
    [Group_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'PayPalVerificationSet'
CREATE TABLE [dbo].[PayPalVerificationSet] (
    [Id] uniqueidentifier  NOT NULL,
    [VerificationDate] datetime  NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'UserAuthentificationLogSet'
CREATE TABLE [dbo].[UserAuthentificationLogSet] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NULL,
    [IpAddress] nvarchar(max)  NOT NULL,
    [Status] bit  NOT NULL,
    [DateTime] datetime  NOT NULL,
    [UserName] nvarchar(max)  NULL
);
GO

-- Creating table 'UserAuthorizationLogSet'
CREATE TABLE [dbo].[UserAuthorizationLogSet] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [IpAddress] nvarchar(max)  NOT NULL,
    [Status] bit  NOT NULL,
    [DateTime] datetime  NOT NULL
);
GO

-- Creating table 'UserPollVoteLogSet'
CREATE TABLE [dbo].[UserPollVoteLogSet] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [GroupMemberId] uniqueidentifier  NOT NULL,
    [PollId] uniqueidentifier  NOT NULL,
    [Result] nvarchar(max)  NOT NULL,
    [DateTime] datetime  NOT NULL
);
GO

-- Creating table 'UserRegistrationLogSet'
CREATE TABLE [dbo].[UserRegistrationLogSet] (
    [Id] uniqueidentifier  NOT NULL,
    [UserId] uniqueidentifier  NOT NULL,
    [IpAddress] nvarchar(max)  NOT NULL,
    [DateTime] datetime  NOT NULL
);
GO

-- Creating table 'SubscriptionSettingsSet'
CREATE TABLE [dbo].[SubscriptionSettingsSet] (
    [Id] uniqueidentifier  NOT NULL,
    [IsSubscribed] bit  NOT NULL,
    [EncryptedSubscriptionEmail] nvarchar(max)  NULL,
    [DigestPeriodicity] tinyint  NOT NULL,
    [User_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'BaseUserSet_User'
CREATE TABLE [dbo].[BaseUserSet_User] (
    [FirstName] nvarchar(max)  NULL,
    [SurName] nvarchar(max)  NULL,
    [Patronymic] nvarchar(max)  NULL,
    [BirthDate] datetime  NULL,
    [Avatar] nvarchar(max)  NULL,
    [Info] nvarchar(max)  NULL,
    [BirthAddressId] uniqueidentifier  NULL,
    [IsVerified] bit  NOT NULL,
    [AddressId] uniqueidentifier  NULL,
    [Login] nvarchar(max)  NULL,
    [Password] uniqueidentifier  NOT NULL,
    [EncryptedEmail] nvarchar(max)  NOT NULL,
    [Salt] uniqueidentifier  NOT NULL,
    [IsEmailVerified] bit  NOT NULL,
    [Facebook] nvarchar(max)  NULL,
    [LiveJournal] nvarchar(max)  NULL,
    [Sex] bit  NULL,
    [EncryptedPhoneNumber] nvarchar(max)  NULL,
    [RegistrationDate] datetime  NOT NULL,
    [IsPhoneVerified] bit  NOT NULL,
    [UTCOffset] smallint  NOT NULL,
    [Label] nvarchar(max)  NULL,
    [LiveJournalSindication] bit  NOT NULL,
    [LiveJournalSindicateAsDraft] bit  NOT NULL,
    [LiveJournalSynchDate] datetime  NULL,
    [LastActivity] datetime  NOT NULL,
    [IsOutdated] bit  NOT NULL,
    [IsForeigner] bit  NOT NULL,
    [QuestProgress] smallint  NOT NULL,
    [IsQuestRejected] bit  NOT NULL,
    [IsPayPalVerified] bit  NOT NULL,
    [IsTicketVerified] bit  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'BulletinSet_ElectionBulletin'
CREATE TABLE [dbo].[BulletinSet_ElectionBulletin] (
    [ElectionId] uniqueidentifier  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'BulletinSet_PollBulletin'
CREATE TABLE [dbo].[BulletinSet_PollBulletin] (
    [Result] tinyint  NOT NULL,
    [Weight] int  NOT NULL,
    [Comment] nvarchar(max)  NULL,
    [PollId] uniqueidentifier  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ContentSet_Voting'
CREATE TABLE [dbo].[ContentSet_Voting] (
    [Duration] smallint  NULL,
    [IsFinished] bit  NOT NULL,
    [HasOpenProtocol] bit  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ContentSet_Poll'
CREATE TABLE [dbo].[ContentSet_Poll] (
    [Result] tinyint  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ContentSet_Election'
CREATE TABLE [dbo].[ContentSet_Election] (
    [Quorum] int  NOT NULL,
    [AgitationDuration] smallint  NOT NULL,
    [Stage] tinyint  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'BaseUserSet_Administrator'
CREATE TABLE [dbo].[BaseUserSet_Administrator] (
    [Login] nvarchar(max)  NOT NULL,
    [Password] uniqueidentifier  NOT NULL,
    [Salt] uniqueidentifier  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ContentSet_Petition'
CREATE TABLE [dbo].[ContentSet_Petition] (
    [IsPrivate] bit  NOT NULL,
    [Id] uniqueidentifier  NOT NULL,
    [Candidate_Id] uniqueidentifier  NULL
);
GO

-- Creating table 'ContentSet_Survey'
CREATE TABLE [dbo].[ContentSet_Survey] (
    [VariantsCount] tinyint  NOT NULL,
    [IsPrivate] bit  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ContentSet_Post'
CREATE TABLE [dbo].[ContentSet_Post] (
    [IsExternal] bit  NOT NULL,
    [Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'TagExpert'
CREATE TABLE [dbo].[TagExpert] (
    [Tags_Id] uniqueidentifier  NOT NULL,
    [Experts_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ElectionBulletinCandidate'
CREATE TABLE [dbo].[ElectionBulletinCandidate] (
    [Electorate_Id] uniqueidentifier  NOT NULL,
    [Result_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ExpertGrantorPollBulletin'
CREATE TABLE [dbo].[ExpertGrantorPollBulletin] (
    [GrantorBulletins_Id] uniqueidentifier  NOT NULL,
    [ExpertBulletins_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ContentTag'
CREATE TABLE [dbo].[ContentTag] (
    [Contents_Id] uniqueidentifier  NOT NULL,
    [Tags_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'SignerPetition'
CREATE TABLE [dbo].[SignerPetition] (
    [Signers_Id] uniqueidentifier  NOT NULL,
    [SignedPetitions_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'SubscriptionGroupUser'
CREATE TABLE [dbo].[SubscriptionGroupUser] (
    [SubscriptionGroups_Id] uniqueidentifier  NOT NULL,
    [Subscribers_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'SubscriptionUserUser'
CREATE TABLE [dbo].[SubscriptionUserUser] (
    [SubscriptionUsers_Id] uniqueidentifier  NOT NULL,
    [Subscribers_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AttachGroup'
CREATE TABLE [dbo].[AttachGroup] (
    [Attachs_Id] uniqueidentifier  NOT NULL,
    [Groups_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AttachUser'
CREATE TABLE [dbo].[AttachUser] (
    [Attachs_Id] uniqueidentifier  NOT NULL,
    [Users_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'SurveyBulletinOption'
CREATE TABLE [dbo].[SurveyBulletinOption] (
    [SurveyBulletins_Id] uniqueidentifier  NOT NULL,
    [Result_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'GroupCategoryGroup'
CREATE TABLE [dbo].[GroupCategoryGroup] (
    [Categories_Id] uniqueidentifier  NOT NULL,
    [Groups_Id] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'UserBlackedUser'
CREATE TABLE [dbo].[UserBlackedUser] (
    [BlackListsOwners_Id] uniqueidentifier  NOT NULL,
    [BlackList_Id] uniqueidentifier  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'BaseUserSet'
ALTER TABLE [dbo].[BaseUserSet]
ADD CONSTRAINT [PK_BaseUserSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GroupSet'
ALTER TABLE [dbo].[GroupSet]
ADD CONSTRAINT [PK_GroupSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GroupMemberSet'
ALTER TABLE [dbo].[GroupMemberSet]
ADD CONSTRAINT [PK_GroupMemberSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TagSet'
ALTER TABLE [dbo].[TagSet]
ADD CONSTRAINT [PK_TagSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExpertSet'
ALTER TABLE [dbo].[ExpertSet]
ADD CONSTRAINT [PK_ExpertSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExpertVoteSet'
ALTER TABLE [dbo].[ExpertVoteSet]
ADD CONSTRAINT [PK_ExpertVoteSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CandidateSet'
ALTER TABLE [dbo].[CandidateSet]
ADD CONSTRAINT [PK_CandidateSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AddressSet'
ALTER TABLE [dbo].[AddressSet]
ADD CONSTRAINT [PK_AddressSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CitySet'
ALTER TABLE [dbo].[CitySet]
ADD CONSTRAINT [PK_CitySet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CommentSet'
ALTER TABLE [dbo].[CommentSet]
ADD CONSTRAINT [PK_CommentSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MessageSet'
ALTER TABLE [dbo].[MessageSet]
ADD CONSTRAINT [PK_MessageSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ContentSet'
ALTER TABLE [dbo].[ContentSet]
ADD CONSTRAINT [PK_ContentSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BulletinSet'
ALTER TABLE [dbo].[BulletinSet]
ADD CONSTRAINT [PK_BulletinSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RegionSet'
ALTER TABLE [dbo].[RegionSet]
ADD CONSTRAINT [PK_RegionSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BlockedRecordSet'
ALTER TABLE [dbo].[BlockedRecordSet]
ADD CONSTRAINT [PK_BlockedRecordSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'InviteSet'
ALTER TABLE [dbo].[InviteSet]
ADD CONSTRAINT [PK_InviteSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CoauthorSet'
ALTER TABLE [dbo].[CoauthorSet]
ADD CONSTRAINT [PK_CoauthorSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LikeSet'
ALTER TABLE [dbo].[LikeSet]
ADD CONSTRAINT [PK_LikeSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MailRecordSet'
ALTER TABLE [dbo].[MailRecordSet]
ADD CONSTRAINT [PK_MailRecordSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BadgeSet'
ALTER TABLE [dbo].[BadgeSet]
ADD CONSTRAINT [PK_BadgeSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Key] in table 'ErrorTextSet'
ALTER TABLE [dbo].[ErrorTextSet]
ADD CONSTRAINT [PK_ErrorTextSet]
    PRIMARY KEY CLUSTERED ([Key] ASC);
GO

-- Creating primary key on [Id] in table 'AttachSet'
ALTER TABLE [dbo].[AttachSet]
ADD CONSTRAINT [PK_AttachSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ScheduleJobSet'
ALTER TABLE [dbo].[ScheduleJobSet]
ADD CONSTRAINT [PK_ScheduleJobSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AlbumSet'
ALTER TABLE [dbo].[AlbumSet]
ADD CONSTRAINT [PK_AlbumSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AlbumItemSet'
ALTER TABLE [dbo].[AlbumItemSet]
ADD CONSTRAINT [PK_AlbumItemSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProfileChangeRequestSet'
ALTER TABLE [dbo].[ProfileChangeRequestSet]
ADD CONSTRAINT [PK_ProfileChangeRequestSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SocialAccountSet'
ALTER TABLE [dbo].[SocialAccountSet]
ADD CONSTRAINT [PK_SocialAccountSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ScheduleExecutionInfoSet'
ALTER TABLE [dbo].[ScheduleExecutionInfoSet]
ADD CONSTRAINT [PK_ScheduleExecutionInfoSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OptionSet'
ALTER TABLE [dbo].[OptionSet]
ADD CONSTRAINT [PK_OptionSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SurveyBulletinSet'
ALTER TABLE [dbo].[SurveyBulletinSet]
ADD CONSTRAINT [PK_SurveyBulletinSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SmsInfoSet'
ALTER TABLE [dbo].[SmsInfoSet]
ADD CONSTRAINT [PK_SmsInfoSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GroupAdSet'
ALTER TABLE [dbo].[GroupAdSet]
ADD CONSTRAINT [PK_GroupAdSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GroupCategorySet'
ALTER TABLE [dbo].[GroupCategorySet]
ADD CONSTRAINT [PK_GroupCategorySet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GroupRatingSet'
ALTER TABLE [dbo].[GroupRatingSet]
ADD CONSTRAINT [PK_GroupRatingSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PayPalVerificationSet'
ALTER TABLE [dbo].[PayPalVerificationSet]
ADD CONSTRAINT [PK_PayPalVerificationSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserAuthentificationLogSet'
ALTER TABLE [dbo].[UserAuthentificationLogSet]
ADD CONSTRAINT [PK_UserAuthentificationLogSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserAuthorizationLogSet'
ALTER TABLE [dbo].[UserAuthorizationLogSet]
ADD CONSTRAINT [PK_UserAuthorizationLogSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserPollVoteLogSet'
ALTER TABLE [dbo].[UserPollVoteLogSet]
ADD CONSTRAINT [PK_UserPollVoteLogSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserRegistrationLogSet'
ALTER TABLE [dbo].[UserRegistrationLogSet]
ADD CONSTRAINT [PK_UserRegistrationLogSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SubscriptionSettingsSet'
ALTER TABLE [dbo].[SubscriptionSettingsSet]
ADD CONSTRAINT [PK_SubscriptionSettingsSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BaseUserSet_User'
ALTER TABLE [dbo].[BaseUserSet_User]
ADD CONSTRAINT [PK_BaseUserSet_User]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BulletinSet_ElectionBulletin'
ALTER TABLE [dbo].[BulletinSet_ElectionBulletin]
ADD CONSTRAINT [PK_BulletinSet_ElectionBulletin]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BulletinSet_PollBulletin'
ALTER TABLE [dbo].[BulletinSet_PollBulletin]
ADD CONSTRAINT [PK_BulletinSet_PollBulletin]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ContentSet_Voting'
ALTER TABLE [dbo].[ContentSet_Voting]
ADD CONSTRAINT [PK_ContentSet_Voting]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ContentSet_Poll'
ALTER TABLE [dbo].[ContentSet_Poll]
ADD CONSTRAINT [PK_ContentSet_Poll]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ContentSet_Election'
ALTER TABLE [dbo].[ContentSet_Election]
ADD CONSTRAINT [PK_ContentSet_Election]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'BaseUserSet_Administrator'
ALTER TABLE [dbo].[BaseUserSet_Administrator]
ADD CONSTRAINT [PK_BaseUserSet_Administrator]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ContentSet_Petition'
ALTER TABLE [dbo].[ContentSet_Petition]
ADD CONSTRAINT [PK_ContentSet_Petition]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ContentSet_Survey'
ALTER TABLE [dbo].[ContentSet_Survey]
ADD CONSTRAINT [PK_ContentSet_Survey]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ContentSet_Post'
ALTER TABLE [dbo].[ContentSet_Post]
ADD CONSTRAINT [PK_ContentSet_Post]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Tags_Id], [Experts_Id] in table 'TagExpert'
ALTER TABLE [dbo].[TagExpert]
ADD CONSTRAINT [PK_TagExpert]
    PRIMARY KEY NONCLUSTERED ([Tags_Id], [Experts_Id] ASC);
GO

-- Creating primary key on [Electorate_Id], [Result_Id] in table 'ElectionBulletinCandidate'
ALTER TABLE [dbo].[ElectionBulletinCandidate]
ADD CONSTRAINT [PK_ElectionBulletinCandidate]
    PRIMARY KEY NONCLUSTERED ([Electorate_Id], [Result_Id] ASC);
GO

-- Creating primary key on [GrantorBulletins_Id], [ExpertBulletins_Id] in table 'ExpertGrantorPollBulletin'
ALTER TABLE [dbo].[ExpertGrantorPollBulletin]
ADD CONSTRAINT [PK_ExpertGrantorPollBulletin]
    PRIMARY KEY NONCLUSTERED ([GrantorBulletins_Id], [ExpertBulletins_Id] ASC);
GO

-- Creating primary key on [Contents_Id], [Tags_Id] in table 'ContentTag'
ALTER TABLE [dbo].[ContentTag]
ADD CONSTRAINT [PK_ContentTag]
    PRIMARY KEY NONCLUSTERED ([Contents_Id], [Tags_Id] ASC);
GO

-- Creating primary key on [Signers_Id], [SignedPetitions_Id] in table 'SignerPetition'
ALTER TABLE [dbo].[SignerPetition]
ADD CONSTRAINT [PK_SignerPetition]
    PRIMARY KEY NONCLUSTERED ([Signers_Id], [SignedPetitions_Id] ASC);
GO

-- Creating primary key on [SubscriptionGroups_Id], [Subscribers_Id] in table 'SubscriptionGroupUser'
ALTER TABLE [dbo].[SubscriptionGroupUser]
ADD CONSTRAINT [PK_SubscriptionGroupUser]
    PRIMARY KEY NONCLUSTERED ([SubscriptionGroups_Id], [Subscribers_Id] ASC);
GO

-- Creating primary key on [SubscriptionUsers_Id], [Subscribers_Id] in table 'SubscriptionUserUser'
ALTER TABLE [dbo].[SubscriptionUserUser]
ADD CONSTRAINT [PK_SubscriptionUserUser]
    PRIMARY KEY NONCLUSTERED ([SubscriptionUsers_Id], [Subscribers_Id] ASC);
GO

-- Creating primary key on [Attachs_Id], [Groups_Id] in table 'AttachGroup'
ALTER TABLE [dbo].[AttachGroup]
ADD CONSTRAINT [PK_AttachGroup]
    PRIMARY KEY NONCLUSTERED ([Attachs_Id], [Groups_Id] ASC);
GO

-- Creating primary key on [Attachs_Id], [Users_Id] in table 'AttachUser'
ALTER TABLE [dbo].[AttachUser]
ADD CONSTRAINT [PK_AttachUser]
    PRIMARY KEY NONCLUSTERED ([Attachs_Id], [Users_Id] ASC);
GO

-- Creating primary key on [SurveyBulletins_Id], [Result_Id] in table 'SurveyBulletinOption'
ALTER TABLE [dbo].[SurveyBulletinOption]
ADD CONSTRAINT [PK_SurveyBulletinOption]
    PRIMARY KEY NONCLUSTERED ([SurveyBulletins_Id], [Result_Id] ASC);
GO

-- Creating primary key on [Categories_Id], [Groups_Id] in table 'GroupCategoryGroup'
ALTER TABLE [dbo].[GroupCategoryGroup]
ADD CONSTRAINT [PK_GroupCategoryGroup]
    PRIMARY KEY NONCLUSTERED ([Categories_Id], [Groups_Id] ASC);
GO

-- Creating primary key on [BlackListsOwners_Id], [BlackList_Id] in table 'UserBlackedUser'
ALTER TABLE [dbo].[UserBlackedUser]
ADD CONSTRAINT [PK_UserBlackedUser]
    PRIMARY KEY NONCLUSTERED ([BlackListsOwners_Id], [BlackList_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [GroupId] in table 'GroupMemberSet'
ALTER TABLE [dbo].[GroupMemberSet]
ADD CONSTRAINT [FK_GroupMemberGroup]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupMemberGroup'
CREATE INDEX [IX_FK_GroupMemberGroup]
ON [dbo].[GroupMemberSet]
    ([GroupId]);
GO

-- Creating foreign key on [UserId] in table 'GroupMemberSet'
ALTER TABLE [dbo].[GroupMemberSet]
ADD CONSTRAINT [FK_UserGroupMember]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserGroupMember'
CREATE INDEX [IX_FK_UserGroupMember]
ON [dbo].[GroupMemberSet]
    ([UserId]);
GO

-- Creating foreign key on [GroupMember_Id] in table 'ExpertSet'
ALTER TABLE [dbo].[ExpertSet]
ADD CONSTRAINT [FK_ExpertGroupMember]
    FOREIGN KEY ([GroupMember_Id])
    REFERENCES [dbo].[GroupMemberSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExpertGroupMember'
CREATE INDEX [IX_FK_ExpertGroupMember]
ON [dbo].[ExpertSet]
    ([GroupMember_Id]);
GO

-- Creating foreign key on [Tags_Id] in table 'TagExpert'
ALTER TABLE [dbo].[TagExpert]
ADD CONSTRAINT [FK_TagExpert_Tag]
    FOREIGN KEY ([Tags_Id])
    REFERENCES [dbo].[TagSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Experts_Id] in table 'TagExpert'
ALTER TABLE [dbo].[TagExpert]
ADD CONSTRAINT [FK_TagExpert_Expert]
    FOREIGN KEY ([Experts_Id])
    REFERENCES [dbo].[ExpertSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TagExpert_Expert'
CREATE INDEX [IX_FK_TagExpert_Expert]
ON [dbo].[TagExpert]
    ([Experts_Id]);
GO

-- Creating foreign key on [ExpertId] in table 'ExpertVoteSet'
ALTER TABLE [dbo].[ExpertVoteSet]
ADD CONSTRAINT [FK_ExpertExpertVote]
    FOREIGN KEY ([ExpertId])
    REFERENCES [dbo].[ExpertSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExpertExpertVote'
CREATE INDEX [IX_FK_ExpertExpertVote]
ON [dbo].[ExpertVoteSet]
    ([ExpertId]);
GO

-- Creating foreign key on [TagId] in table 'ExpertVoteSet'
ALTER TABLE [dbo].[ExpertVoteSet]
ADD CONSTRAINT [FK_ExpertVoteTag]
    FOREIGN KEY ([TagId])
    REFERENCES [dbo].[TagSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExpertVoteTag'
CREATE INDEX [IX_FK_ExpertVoteTag]
ON [dbo].[ExpertVoteSet]
    ([TagId]);
GO

-- Creating foreign key on [GroupMemberId] in table 'ExpertVoteSet'
ALTER TABLE [dbo].[ExpertVoteSet]
ADD CONSTRAINT [FK_ExpertVoteGroupMember]
    FOREIGN KEY ([GroupMemberId])
    REFERENCES [dbo].[GroupMemberSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExpertVoteGroupMember'
CREATE INDEX [IX_FK_ExpertVoteGroupMember]
ON [dbo].[ExpertVoteSet]
    ([GroupMemberId]);
GO

-- Creating foreign key on [GroupMember_Id] in table 'CandidateSet'
ALTER TABLE [dbo].[CandidateSet]
ADD CONSTRAINT [FK_CandidateGroupMember]
    FOREIGN KEY ([GroupMember_Id])
    REFERENCES [dbo].[GroupMemberSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CandidateGroupMember'
CREATE INDEX [IX_FK_CandidateGroupMember]
ON [dbo].[CandidateSet]
    ([GroupMember_Id]);
GO

-- Creating foreign key on [Electorate_Id] in table 'ElectionBulletinCandidate'
ALTER TABLE [dbo].[ElectionBulletinCandidate]
ADD CONSTRAINT [FK_ElectionBulletinCandidate_ElectionBulletin]
    FOREIGN KEY ([Electorate_Id])
    REFERENCES [dbo].[BulletinSet_ElectionBulletin]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Result_Id] in table 'ElectionBulletinCandidate'
ALTER TABLE [dbo].[ElectionBulletinCandidate]
ADD CONSTRAINT [FK_ElectionBulletinCandidate_Candidate]
    FOREIGN KEY ([Result_Id])
    REFERENCES [dbo].[CandidateSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ElectionBulletinCandidate_Candidate'
CREATE INDEX [IX_FK_ElectionBulletinCandidate_Candidate]
ON [dbo].[ElectionBulletinCandidate]
    ([Result_Id]);
GO

-- Creating foreign key on [GrantorBulletins_Id] in table 'ExpertGrantorPollBulletin'
ALTER TABLE [dbo].[ExpertGrantorPollBulletin]
ADD CONSTRAINT [FK_ExpertGrantorPollBulletin_PollBulletin]
    FOREIGN KEY ([GrantorBulletins_Id])
    REFERENCES [dbo].[BulletinSet_PollBulletin]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [ExpertBulletins_Id] in table 'ExpertGrantorPollBulletin'
ALTER TABLE [dbo].[ExpertGrantorPollBulletin]
ADD CONSTRAINT [FK_ExpertGrantorPollBulletin_PollBulletin1]
    FOREIGN KEY ([ExpertBulletins_Id])
    REFERENCES [dbo].[BulletinSet_PollBulletin]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExpertGrantorPollBulletin_PollBulletin1'
CREATE INDEX [IX_FK_ExpertGrantorPollBulletin_PollBulletin1]
ON [dbo].[ExpertGrantorPollBulletin]
    ([ExpertBulletins_Id]);
GO

-- Creating foreign key on [CityId] in table 'AddressSet'
ALTER TABLE [dbo].[AddressSet]
ADD CONSTRAINT [FK_AddressCity]
    FOREIGN KEY ([CityId])
    REFERENCES [dbo].[CitySet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AddressCity'
CREATE INDEX [IX_FK_AddressCity]
ON [dbo].[AddressSet]
    ([CityId]);
GO

-- Creating foreign key on [BirthAddressId] in table 'BaseUserSet_User'
ALTER TABLE [dbo].[BaseUserSet_User]
ADD CONSTRAINT [FK_BirthAddressUser]
    FOREIGN KEY ([BirthAddressId])
    REFERENCES [dbo].[AddressSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BirthAddressUser'
CREATE INDEX [IX_FK_BirthAddressUser]
ON [dbo].[BaseUserSet_User]
    ([BirthAddressId]);
GO

-- Creating foreign key on [UserId] in table 'CommentSet'
ALTER TABLE [dbo].[CommentSet]
ADD CONSTRAINT [FK_CommentUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CommentUser'
CREATE INDEX [IX_FK_CommentUser]
ON [dbo].[CommentSet]
    ([UserId]);
GO

-- Creating foreign key on [ContentId] in table 'CommentSet'
ALTER TABLE [dbo].[CommentSet]
ADD CONSTRAINT [FK_ContentComment]
    FOREIGN KEY ([ContentId])
    REFERENCES [dbo].[ContentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ContentComment'
CREATE INDEX [IX_FK_ContentComment]
ON [dbo].[CommentSet]
    ([ContentId]);
GO

-- Creating foreign key on [Contents_Id] in table 'ContentTag'
ALTER TABLE [dbo].[ContentTag]
ADD CONSTRAINT [FK_ContentTag_Content]
    FOREIGN KEY ([Contents_Id])
    REFERENCES [dbo].[ContentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Tags_Id] in table 'ContentTag'
ALTER TABLE [dbo].[ContentTag]
ADD CONSTRAINT [FK_ContentTag_Tag]
    FOREIGN KEY ([Tags_Id])
    REFERENCES [dbo].[TagSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ContentTag_Tag'
CREATE INDEX [IX_FK_ContentTag_Tag]
ON [dbo].[ContentTag]
    ([Tags_Id]);
GO

-- Creating foreign key on [AuthorId] in table 'ContentSet'
ALTER TABLE [dbo].[ContentSet]
ADD CONSTRAINT [FK_ContentUser]
    FOREIGN KEY ([AuthorId])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ContentUser'
CREATE INDEX [IX_FK_ContentUser]
ON [dbo].[ContentSet]
    ([AuthorId]);
GO

-- Creating foreign key on [PollId] in table 'BulletinSet_PollBulletin'
ALTER TABLE [dbo].[BulletinSet_PollBulletin]
ADD CONSTRAINT [FK_PollBulletinPoll]
    FOREIGN KEY ([PollId])
    REFERENCES [dbo].[ContentSet_Poll]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PollBulletinPoll'
CREATE INDEX [IX_FK_PollBulletinPoll]
ON [dbo].[BulletinSet_PollBulletin]
    ([PollId]);
GO

-- Creating foreign key on [ElectionId] in table 'BulletinSet_ElectionBulletin'
ALTER TABLE [dbo].[BulletinSet_ElectionBulletin]
ADD CONSTRAINT [FK_ElectionBulletinElection]
    FOREIGN KEY ([ElectionId])
    REFERENCES [dbo].[ContentSet_Election]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ElectionBulletinElection'
CREATE INDEX [IX_FK_ElectionBulletinElection]
ON [dbo].[BulletinSet_ElectionBulletin]
    ([ElectionId]);
GO

-- Creating foreign key on [ElectionId] in table 'CandidateSet'
ALTER TABLE [dbo].[CandidateSet]
ADD CONSTRAINT [FK_CandidateElection]
    FOREIGN KEY ([ElectionId])
    REFERENCES [dbo].[ContentSet_Election]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CandidateElection'
CREATE INDEX [IX_FK_CandidateElection]
ON [dbo].[CandidateSet]
    ([ElectionId]);
GO

-- Creating foreign key on [RegionId] in table 'CitySet'
ALTER TABLE [dbo].[CitySet]
ADD CONSTRAINT [FK_RegionCity]
    FOREIGN KEY ([RegionId])
    REFERENCES [dbo].[RegionSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RegionCity'
CREATE INDEX [IX_FK_RegionCity]
ON [dbo].[CitySet]
    ([RegionId]);
GO

-- Creating foreign key on [AdministratorId] in table 'BlockedRecordSet'
ALTER TABLE [dbo].[BlockedRecordSet]
ADD CONSTRAINT [FK_AdministratorBlockedRecord]
    FOREIGN KEY ([AdministratorId])
    REFERENCES [dbo].[BaseUserSet_Administrator]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AdministratorBlockedRecord'
CREATE INDEX [IX_FK_AdministratorBlockedRecord]
ON [dbo].[BlockedRecordSet]
    ([AdministratorId]);
GO

-- Creating foreign key on [Signers_Id] in table 'SignerPetition'
ALTER TABLE [dbo].[SignerPetition]
ADD CONSTRAINT [FK_SignerPetition_User]
    FOREIGN KEY ([Signers_Id])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [SignedPetitions_Id] in table 'SignerPetition'
ALTER TABLE [dbo].[SignerPetition]
ADD CONSTRAINT [FK_SignerPetition_Petition]
    FOREIGN KEY ([SignedPetitions_Id])
    REFERENCES [dbo].[ContentSet_Petition]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SignerPetition_Petition'
CREATE INDEX [IX_FK_SignerPetition_Petition]
ON [dbo].[SignerPetition]
    ([SignedPetitions_Id]);
GO

-- Creating foreign key on [AddressId] in table 'BaseUserSet_User'
ALTER TABLE [dbo].[BaseUserSet_User]
ADD CONSTRAINT [FK_ResidentialAddressUser]
    FOREIGN KEY ([AddressId])
    REFERENCES [dbo].[AddressSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ResidentialAddressUser'
CREATE INDEX [IX_FK_ResidentialAddressUser]
ON [dbo].[BaseUserSet_User]
    ([AddressId]);
GO

-- Creating foreign key on [BaseUser_Id] in table 'BlockedRecordSet'
ALTER TABLE [dbo].[BlockedRecordSet]
ADD CONSTRAINT [FK_BlockedRecordBaseUser]
    FOREIGN KEY ([BaseUser_Id])
    REFERENCES [dbo].[BaseUserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BlockedRecordBaseUser'
CREATE INDEX [IX_FK_BlockedRecordBaseUser]
ON [dbo].[BlockedRecordSet]
    ([BaseUser_Id]);
GO

-- Creating foreign key on [ParentGroupId] in table 'GroupSet'
ALTER TABLE [dbo].[GroupSet]
ADD CONSTRAINT [FK_GroupInheritance]
    FOREIGN KEY ([ParentGroupId])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupInheritance'
CREATE INDEX [IX_FK_GroupInheritance]
ON [dbo].[GroupSet]
    ([ParentGroupId]);
GO

-- Creating foreign key on [BaseUserId] in table 'InviteSet'
ALTER TABLE [dbo].[InviteSet]
ADD CONSTRAINT [FK_InviteBaseUser]
    FOREIGN KEY ([BaseUserId])
    REFERENCES [dbo].[BaseUserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_InviteBaseUser'
CREATE INDEX [IX_FK_InviteBaseUser]
ON [dbo].[InviteSet]
    ([BaseUserId]);
GO

-- Creating foreign key on [InviteTicket_Id] in table 'BaseUserSet'
ALTER TABLE [dbo].[BaseUserSet]
ADD CONSTRAINT [FK_InviteBaseUser1]
    FOREIGN KEY ([InviteTicket_Id])
    REFERENCES [dbo].[InviteSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_InviteBaseUser1'
CREATE INDEX [IX_FK_InviteBaseUser1]
ON [dbo].[BaseUserSet]
    ([InviteTicket_Id]);
GO

-- Creating foreign key on [ParentCommentId] in table 'CommentSet'
ALTER TABLE [dbo].[CommentSet]
ADD CONSTRAINT [FK_ChildComments]
    FOREIGN KEY ([ParentCommentId])
    REFERENCES [dbo].[CommentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ChildComments'
CREATE INDEX [IX_FK_ChildComments]
ON [dbo].[CommentSet]
    ([ParentCommentId]);
GO

-- Creating foreign key on [UserId] in table 'CoauthorSet'
ALTER TABLE [dbo].[CoauthorSet]
ADD CONSTRAINT [FK_CoauthorUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CoauthorUser'
CREATE INDEX [IX_FK_CoauthorUser]
ON [dbo].[CoauthorSet]
    ([UserId]);
GO

-- Creating foreign key on [PetitionId] in table 'CoauthorSet'
ALTER TABLE [dbo].[CoauthorSet]
ADD CONSTRAINT [FK_CoauthorPetition]
    FOREIGN KEY ([PetitionId])
    REFERENCES [dbo].[ContentSet_Petition]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CoauthorPetition'
CREATE INDEX [IX_FK_CoauthorPetition]
ON [dbo].[CoauthorSet]
    ([PetitionId]);
GO

-- Creating foreign key on [SubscriptionGroups_Id] in table 'SubscriptionGroupUser'
ALTER TABLE [dbo].[SubscriptionGroupUser]
ADD CONSTRAINT [FK_GroupUser_Group]
    FOREIGN KEY ([SubscriptionGroups_Id])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Subscribers_Id] in table 'SubscriptionGroupUser'
ALTER TABLE [dbo].[SubscriptionGroupUser]
ADD CONSTRAINT [FK_GroupUser_User]
    FOREIGN KEY ([Subscribers_Id])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupUser_User'
CREATE INDEX [IX_FK_GroupUser_User]
ON [dbo].[SubscriptionGroupUser]
    ([Subscribers_Id]);
GO

-- Creating foreign key on [SubscriptionUsers_Id] in table 'SubscriptionUserUser'
ALTER TABLE [dbo].[SubscriptionUserUser]
ADD CONSTRAINT [FK_SubscribeUserUser_User]
    FOREIGN KEY ([SubscriptionUsers_Id])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Subscribers_Id] in table 'SubscriptionUserUser'
ALTER TABLE [dbo].[SubscriptionUserUser]
ADD CONSTRAINT [FK_SubscribeUserUser_User1]
    FOREIGN KEY ([Subscribers_Id])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SubscribeUserUser_User1'
CREATE INDEX [IX_FK_SubscribeUserUser_User1]
ON [dbo].[SubscriptionUserUser]
    ([Subscribers_Id]);
GO

-- Creating foreign key on [UserId] in table 'LikeSet'
ALTER TABLE [dbo].[LikeSet]
ADD CONSTRAINT [FK_LikeUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LikeUser'
CREATE INDEX [IX_FK_LikeUser]
ON [dbo].[LikeSet]
    ([UserId]);
GO

-- Creating foreign key on [AuthorId] in table 'MessageSet'
ALTER TABLE [dbo].[MessageSet]
ADD CONSTRAINT [FK_MessageBaseUserAuthor]
    FOREIGN KEY ([AuthorId])
    REFERENCES [dbo].[BaseUserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MessageBaseUserAuthor'
CREATE INDEX [IX_FK_MessageBaseUserAuthor]
ON [dbo].[MessageSet]
    ([AuthorId]);
GO

-- Creating foreign key on [RecipientId] in table 'MessageSet'
ALTER TABLE [dbo].[MessageSet]
ADD CONSTRAINT [FK_MessageBaseUserRecipient]
    FOREIGN KEY ([RecipientId])
    REFERENCES [dbo].[BaseUserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MessageBaseUserRecipient'
CREATE INDEX [IX_FK_MessageBaseUserRecipient]
ON [dbo].[MessageSet]
    ([RecipientId]);
GO

-- Creating foreign key on [User_Id] in table 'MailRecordSet'
ALTER TABLE [dbo].[MailRecordSet]
ADD CONSTRAINT [FK_MailRecordUser]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MailRecordUser'
CREATE INDEX [IX_FK_MailRecordUser]
ON [dbo].[MailRecordSet]
    ([User_Id]);
GO

-- Creating foreign key on [OwnerId] in table 'BulletinSet'
ALTER TABLE [dbo].[BulletinSet]
ADD CONSTRAINT [FK_BulletinGroupMember]
    FOREIGN KEY ([OwnerId])
    REFERENCES [dbo].[GroupMemberSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BulletinGroupMember'
CREATE INDEX [IX_FK_BulletinGroupMember]
ON [dbo].[BulletinSet]
    ([OwnerId]);
GO

-- Creating foreign key on [UserId] in table 'BadgeSet'
ALTER TABLE [dbo].[BadgeSet]
ADD CONSTRAINT [FK_BadgeUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BadgeUser'
CREATE INDEX [IX_FK_BadgeUser]
ON [dbo].[BadgeSet]
    ([UserId]);
GO

-- Creating foreign key on [GroupId] in table 'BadgeSet'
ALTER TABLE [dbo].[BadgeSet]
ADD CONSTRAINT [FK_BadgeGroup]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BadgeGroup'
CREATE INDEX [IX_FK_BadgeGroup]
ON [dbo].[BadgeSet]
    ([GroupId]);
GO

-- Creating foreign key on [ReplyToId] in table 'CommentSet'
ALTER TABLE [dbo].[CommentSet]
ADD CONSTRAINT [FK_CommentComment]
    FOREIGN KEY ([ReplyToId])
    REFERENCES [dbo].[CommentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CommentComment'
CREATE INDEX [IX_FK_CommentComment]
ON [dbo].[CommentSet]
    ([ReplyToId]);
GO

-- Creating foreign key on [Candidate_Id] in table 'ContentSet_Petition'
ALTER TABLE [dbo].[ContentSet_Petition]
ADD CONSTRAINT [FK_CandidatePetition]
    FOREIGN KEY ([Candidate_Id])
    REFERENCES [dbo].[CandidateSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CandidatePetition'
CREATE INDEX [IX_FK_CandidatePetition]
ON [dbo].[ContentSet_Petition]
    ([Candidate_Id]);
GO

-- Creating foreign key on [Content_Id] in table 'AttachSet'
ALTER TABLE [dbo].[AttachSet]
ADD CONSTRAINT [FK_AttachContent]
    FOREIGN KEY ([Content_Id])
    REFERENCES [dbo].[ContentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AttachContent'
CREATE INDEX [IX_FK_AttachContent]
ON [dbo].[AttachSet]
    ([Content_Id]);
GO

-- Creating foreign key on [GroupId] in table 'ContentSet'
ALTER TABLE [dbo].[ContentSet]
ADD CONSTRAINT [FK_ContentGroup]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ContentGroup'
CREATE INDEX [IX_FK_ContentGroup]
ON [dbo].[ContentSet]
    ([GroupId]);
GO

-- Creating foreign key on [Attachs_Id] in table 'AttachGroup'
ALTER TABLE [dbo].[AttachGroup]
ADD CONSTRAINT [FK_AttachGroup_Attach]
    FOREIGN KEY ([Attachs_Id])
    REFERENCES [dbo].[AttachSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Groups_Id] in table 'AttachGroup'
ALTER TABLE [dbo].[AttachGroup]
ADD CONSTRAINT [FK_AttachGroup_Group]
    FOREIGN KEY ([Groups_Id])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AttachGroup_Group'
CREATE INDEX [IX_FK_AttachGroup_Group]
ON [dbo].[AttachGroup]
    ([Groups_Id]);
GO

-- Creating foreign key on [Attachs_Id] in table 'AttachUser'
ALTER TABLE [dbo].[AttachUser]
ADD CONSTRAINT [FK_AttachUser_Attach]
    FOREIGN KEY ([Attachs_Id])
    REFERENCES [dbo].[AttachSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Users_Id] in table 'AttachUser'
ALTER TABLE [dbo].[AttachUser]
ADD CONSTRAINT [FK_AttachUser_User]
    FOREIGN KEY ([Users_Id])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AttachUser_User'
CREATE INDEX [IX_FK_AttachUser_User]
ON [dbo].[AttachUser]
    ([Users_Id]);
GO

-- Creating foreign key on [GroupId] in table 'AlbumSet'
ALTER TABLE [dbo].[AlbumSet]
ADD CONSTRAINT [FK_AlbumGroup]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AlbumGroup'
CREATE INDEX [IX_FK_AlbumGroup]
ON [dbo].[AlbumSet]
    ([GroupId]);
GO

-- Creating foreign key on [UserId] in table 'AlbumSet'
ALTER TABLE [dbo].[AlbumSet]
ADD CONSTRAINT [FK_AlbumUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AlbumUser'
CREATE INDEX [IX_FK_AlbumUser]
ON [dbo].[AlbumSet]
    ([UserId]);
GO

-- Creating foreign key on [AlbumId] in table 'AlbumItemSet'
ALTER TABLE [dbo].[AlbumItemSet]
ADD CONSTRAINT [FK_AlbumItemAlbum]
    FOREIGN KEY ([AlbumId])
    REFERENCES [dbo].[AlbumSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AlbumItemAlbum'
CREATE INDEX [IX_FK_AlbumItemAlbum]
ON [dbo].[AlbumItemSet]
    ([AlbumId]);
GO

-- Creating foreign key on [ContentId] in table 'LikeSet'
ALTER TABLE [dbo].[LikeSet]
ADD CONSTRAINT [FK_LikeContent]
    FOREIGN KEY ([ContentId])
    REFERENCES [dbo].[ContentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LikeContent'
CREATE INDEX [IX_FK_LikeContent]
ON [dbo].[LikeSet]
    ([ContentId]);
GO

-- Creating foreign key on [AlbumItemId] in table 'LikeSet'
ALTER TABLE [dbo].[LikeSet]
ADD CONSTRAINT [FK_LikeAlbumItem]
    FOREIGN KEY ([AlbumItemId])
    REFERENCES [dbo].[AlbumItemSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LikeAlbumItem'
CREATE INDEX [IX_FK_LikeAlbumItem]
ON [dbo].[LikeSet]
    ([AlbumItemId]);
GO

-- Creating foreign key on [UserId] in table 'ProfileChangeRequestSet'
ALTER TABLE [dbo].[ProfileChangeRequestSet]
ADD CONSTRAINT [FK_ProfileChangeRequestUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProfileChangeRequestUser'
CREATE INDEX [IX_FK_ProfileChangeRequestUser]
ON [dbo].[ProfileChangeRequestSet]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'SocialAccountSet'
ALTER TABLE [dbo].[SocialAccountSet]
ADD CONSTRAINT [FK_SocialAccountUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SocialAccountUser'
CREATE INDEX [IX_FK_SocialAccountUser]
ON [dbo].[SocialAccountSet]
    ([UserId]);
GO

-- Creating foreign key on [ScheduleJobId] in table 'ScheduleExecutionInfoSet'
ALTER TABLE [dbo].[ScheduleExecutionInfoSet]
ADD CONSTRAINT [FK_ScheduleJobScheduleExecutionInfo]
    FOREIGN KEY ([ScheduleJobId])
    REFERENCES [dbo].[ScheduleJobSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ScheduleJobScheduleExecutionInfo'
CREATE INDEX [IX_FK_ScheduleJobScheduleExecutionInfo]
ON [dbo].[ScheduleExecutionInfoSet]
    ([ScheduleJobId]);
GO

-- Creating foreign key on [SurveyId] in table 'OptionSet'
ALTER TABLE [dbo].[OptionSet]
ADD CONSTRAINT [FK_SurveyOption]
    FOREIGN KEY ([SurveyId])
    REFERENCES [dbo].[ContentSet_Survey]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SurveyOption'
CREATE INDEX [IX_FK_SurveyOption]
ON [dbo].[OptionSet]
    ([SurveyId]);
GO

-- Creating foreign key on [SurveyId] in table 'SurveyBulletinSet'
ALTER TABLE [dbo].[SurveyBulletinSet]
ADD CONSTRAINT [FK_SurveySurveyBulletins]
    FOREIGN KEY ([SurveyId])
    REFERENCES [dbo].[ContentSet_Survey]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SurveySurveyBulletins'
CREATE INDEX [IX_FK_SurveySurveyBulletins]
ON [dbo].[SurveyBulletinSet]
    ([SurveyId]);
GO

-- Creating foreign key on [UserId] in table 'SurveyBulletinSet'
ALTER TABLE [dbo].[SurveyBulletinSet]
ADD CONSTRAINT [FK_SurveyBulletinUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SurveyBulletinUser'
CREATE INDEX [IX_FK_SurveyBulletinUser]
ON [dbo].[SurveyBulletinSet]
    ([UserId]);
GO

-- Creating foreign key on [SurveyBulletins_Id] in table 'SurveyBulletinOption'
ALTER TABLE [dbo].[SurveyBulletinOption]
ADD CONSTRAINT [FK_SurveyBulletinOption_SurveyBulletin]
    FOREIGN KEY ([SurveyBulletins_Id])
    REFERENCES [dbo].[SurveyBulletinSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Result_Id] in table 'SurveyBulletinOption'
ALTER TABLE [dbo].[SurveyBulletinOption]
ADD CONSTRAINT [FK_SurveyBulletinOption_Option]
    FOREIGN KEY ([Result_Id])
    REFERENCES [dbo].[OptionSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SurveyBulletinOption_Option'
CREATE INDEX [IX_FK_SurveyBulletinOption_Option]
ON [dbo].[SurveyBulletinOption]
    ([Result_Id]);
GO

-- Creating foreign key on [BaseUserId] in table 'SmsInfoSet'
ALTER TABLE [dbo].[SmsInfoSet]
ADD CONSTRAINT [FK_SmsInfoBaseUser]
    FOREIGN KEY ([BaseUserId])
    REFERENCES [dbo].[BaseUserSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SmsInfoBaseUser'
CREATE INDEX [IX_FK_SmsInfoBaseUser]
ON [dbo].[SmsInfoSet]
    ([BaseUserId]);
GO

-- Creating foreign key on [GroupId] in table 'TagSet'
ALTER TABLE [dbo].[TagSet]
ADD CONSTRAINT [FK_TagGroup]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TagGroup'
CREATE INDEX [IX_FK_TagGroup]
ON [dbo].[TagSet]
    ([GroupId]);
GO

-- Creating foreign key on [Categories_Id] in table 'GroupCategoryGroup'
ALTER TABLE [dbo].[GroupCategoryGroup]
ADD CONSTRAINT [FK_GroupCategoryGroup_GroupCategory]
    FOREIGN KEY ([Categories_Id])
    REFERENCES [dbo].[GroupCategorySet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Groups_Id] in table 'GroupCategoryGroup'
ALTER TABLE [dbo].[GroupCategoryGroup]
ADD CONSTRAINT [FK_GroupCategoryGroup_Group]
    FOREIGN KEY ([Groups_Id])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupCategoryGroup_Group'
CREATE INDEX [IX_FK_GroupCategoryGroup_Group]
ON [dbo].[GroupCategoryGroup]
    ([Groups_Id]);
GO

-- Creating foreign key on [Group_Id] in table 'GroupRatingSet'
ALTER TABLE [dbo].[GroupRatingSet]
ADD CONSTRAINT [FK_GroupRatingGroup]
    FOREIGN KEY ([Group_Id])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupRatingGroup'
CREATE INDEX [IX_FK_GroupRatingGroup]
ON [dbo].[GroupRatingSet]
    ([Group_Id]);
GO

-- Creating foreign key on [GroupFriendlyGroups_FriendlyGroup_Id] in table 'GroupSet'
ALTER TABLE [dbo].[GroupSet]
ADD CONSTRAINT [FK_GroupFriendlyGroups]
    FOREIGN KEY ([GroupFriendlyGroups_FriendlyGroup_Id])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupFriendlyGroups'
CREATE INDEX [IX_FK_GroupFriendlyGroups]
ON [dbo].[GroupSet]
    ([GroupFriendlyGroups_FriendlyGroup_Id]);
GO

-- Creating foreign key on [Group_Id] in table 'GroupAdSet'
ALTER TABLE [dbo].[GroupAdSet]
ADD CONSTRAINT [FK_GroupAdGroup]
    FOREIGN KEY ([Group_Id])
    REFERENCES [dbo].[GroupSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupAdGroup'
CREATE INDEX [IX_FK_GroupAdGroup]
ON [dbo].[GroupAdSet]
    ([Group_Id]);
GO

-- Creating foreign key on [CommentId] in table 'LikeSet'
ALTER TABLE [dbo].[LikeSet]
ADD CONSTRAINT [FK_LikeComment]
    FOREIGN KEY ([CommentId])
    REFERENCES [dbo].[CommentSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LikeComment'
CREATE INDEX [IX_FK_LikeComment]
ON [dbo].[LikeSet]
    ([CommentId]);
GO

-- Creating foreign key on [BlackListsOwners_Id] in table 'UserBlackedUser'
ALTER TABLE [dbo].[UserBlackedUser]
ADD CONSTRAINT [FK_UserBlackedUser_User]
    FOREIGN KEY ([BlackListsOwners_Id])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [BlackList_Id] in table 'UserBlackedUser'
ALTER TABLE [dbo].[UserBlackedUser]
ADD CONSTRAINT [FK_UserBlackedUser_User1]
    FOREIGN KEY ([BlackList_Id])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserBlackedUser_User1'
CREATE INDEX [IX_FK_UserBlackedUser_User1]
ON [dbo].[UserBlackedUser]
    ([BlackList_Id]);
GO

-- Creating foreign key on [UserId] in table 'PayPalVerificationSet'
ALTER TABLE [dbo].[PayPalVerificationSet]
ADD CONSTRAINT [FK_UserPayPalVerification]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserPayPalVerification'
CREATE INDEX [IX_FK_UserPayPalVerification]
ON [dbo].[PayPalVerificationSet]
    ([UserId]);
GO

-- Creating foreign key on [User_Id] in table 'SubscriptionSettingsSet'
ALTER TABLE [dbo].[SubscriptionSettingsSet]
ADD CONSTRAINT [FK_SubscriptionSettingsUser]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[BaseUserSet_User]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SubscriptionSettingsUser'
CREATE INDEX [IX_FK_SubscriptionSettingsUser]
ON [dbo].[SubscriptionSettingsSet]
    ([User_Id]);
GO

-- Creating foreign key on [Id] in table 'BaseUserSet_User'
ALTER TABLE [dbo].[BaseUserSet_User]
ADD CONSTRAINT [FK_User_inherits_BaseUser]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[BaseUserSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'BulletinSet_ElectionBulletin'
ALTER TABLE [dbo].[BulletinSet_ElectionBulletin]
ADD CONSTRAINT [FK_ElectionBulletin_inherits_Bulletin]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[BulletinSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'BulletinSet_PollBulletin'
ALTER TABLE [dbo].[BulletinSet_PollBulletin]
ADD CONSTRAINT [FK_PollBulletin_inherits_Bulletin]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[BulletinSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'ContentSet_Voting'
ALTER TABLE [dbo].[ContentSet_Voting]
ADD CONSTRAINT [FK_Voting_inherits_Content]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[ContentSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'ContentSet_Poll'
ALTER TABLE [dbo].[ContentSet_Poll]
ADD CONSTRAINT [FK_Poll_inherits_Voting]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[ContentSet_Voting]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'ContentSet_Election'
ALTER TABLE [dbo].[ContentSet_Election]
ADD CONSTRAINT [FK_Election_inherits_Voting]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[ContentSet_Voting]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'BaseUserSet_Administrator'
ALTER TABLE [dbo].[BaseUserSet_Administrator]
ADD CONSTRAINT [FK_Administrator_inherits_BaseUser]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[BaseUserSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'ContentSet_Petition'
ALTER TABLE [dbo].[ContentSet_Petition]
ADD CONSTRAINT [FK_Petition_inherits_Voting]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[ContentSet_Voting]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'ContentSet_Survey'
ALTER TABLE [dbo].[ContentSet_Survey]
ADD CONSTRAINT [FK_Survey_inherits_Voting]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[ContentSet_Voting]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'ContentSet_Post'
ALTER TABLE [dbo].[ContentSet_Post]
ADD CONSTRAINT [FK_Post_inherits_Content]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[ContentSet]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------