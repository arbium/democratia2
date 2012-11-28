using System;
using System.Data.Objects;

namespace Federation.Core
{
    public interface IDataService
    {
        IDataBox<Address> AddressSet { get; }
        IDataBox<Album> AlbumSet { get; }
        IDataBox<AlbumItem> AlbumItemSet { get; }
        IDataBox<Attach> AttachSet { get; }
        IDataBox<Badge> BadgeSet { get; }
        IDataBox<BaseUser> BaseUserSet { get; }
        IDataBox<BlockedRecord> BlockedRecordSet { get; }
        IDataBox<Bulletin> BulletinSet { get; }
        IDataBox<SurveyBulletin> SurveyBulletinSet { get; }
        IDataBox<Candidate> CandidateSet { get; }
        IDataBox<City> CitySet { get; }
        IDataBox<Coauthor> CoauthorSet { get; }
        IDataBox<Comment> CommentSet { get; }
        IDataBox<Content> ContentSet { get; }
        IDataBox<Expert> ExpertSet { get; }
        IDataBox<ExpertVote> ExpertVoteSet { get; }
        IDataBox<GroupMember> GroupMemberSet { get; }
        IDataBox<Group> GroupSet { get; }
        IDataBox<GroupAd> GroupAdSet { get; }
        IDataBox<GroupCategory> GroupCategorySet { get; }
        IDataBox<Invite> InviteSet { get; }
        IDataBox<Like> LikeSet { get; }
        IDataBox<Message> MessageSet { get; }
        IDataBox<MailRecord> MailRecordSet { get; }
        IDataBox<Option> OptionSet { get; }
        IDataBox<Region> RegionSet { get; }
        IDataBox<Tag> TagSet { get; }
        IDataBox<SocialAccount> SocialAccountSet { get; }
        IDataBox<ErrorText> ErrorTextSet { get; }
        IDataBox<ProfileChangeRequest> ProfileChangeRequestSet { get; }
        IDataBox<ScheduleJob> ScheduleJobSet { get; }
        IDataBox<ScheduleExecutionInfo> ScheduleExecutionInfoSet { get; }
        IDataBox<SmsInfo> SmsInfoSet { get; }
        IDataBox<PayPalVerification> PayPalVerificationSet { get; }
        IDataBox<UserAuthentificationLog> UserAuthentificationLogSet { get; }
        IDataBox<UserAuthorizationLog> UserAuthorizationLogSet { get; }
        IDataBox<UserPollVoteLog> UserPollVoteLogSet { get; }
        IDataBox<UserRegistrationLog> UserRegistrationLogSet { get; }

        ObjectResult<Comment> GetCommentsByFullText(string query, int count);
        ObjectResult<Post> GetPostsByFullText(string query, int count);
        ObjectResult<Petition> GetPetitionsByFullText(string query, int count);
        ObjectResult<Poll> GetPollsByFullText(string query, int count);
        ObjectResult<Survey> GetSurveysByFullText(string query, int count);
        ObjectResult<Group> GetGroupsByFullText(string query, int count);

        ObjectResult<Content> GetOrderedByRatingGroupContent(Guid groupId);

        void BeginWork();
        void EndWork();

        void SaveChanges();
        void LoadProperty<TEntity>(TEntity entity, System.Linq.Expressions.Expression<Func<TEntity, object>> selector);
        void LoadProperty(object entity, string navigationProperty);
    }
}