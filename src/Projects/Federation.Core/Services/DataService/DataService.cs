using System;
using System.Data.Objects;

namespace Federation.Core
{
    public static class DataService
    {
        public static class PerThread
        {            
            private static IDataService _dataContextPerRequest = ServiceLocator.GetInstance<IDataService>("PerThread");

            public static ObjectResult<Comment> GetCommentsByFullText(string query, int count)
            {
                return _dataContextPerRequest.GetCommentsByFullText(query, count);
            }

            public static ObjectResult<Post> GetPostsByFullText(string query, int count)
            {
                return _dataContextPerRequest.GetPostsByFullText(query, count);
            }

            public static ObjectResult<Petition> GetPetitionsByFullText(string query, int count)
            {
                return _dataContextPerRequest.GetPetitionsByFullText(query, count);
            }

            public static ObjectResult<Poll> GetPollsByFullText(string query, int count)
            {
                return _dataContextPerRequest.GetPollsByFullText(query, count);
            }

            public static ObjectResult<Survey> GetSurveysByFullText(string query, int count)
            {
                return _dataContextPerRequest.GetSurveysByFullText(query, count);
            }

            public static ObjectResult<Group> GetGroupsByFullText(string query, int count)
            {
                return _dataContextPerRequest.GetGroupsByFullText(query, count);
            }

            public static ObjectResult<Content> GetOrderedByRatingGroupContent(Guid groupId)
            {
                return _dataContextPerRequest.GetOrderedByRatingGroupContent(groupId);
            }

            public static IDataBox<Address> AddressSet
            {
                get { return _dataContextPerRequest.AddressSet; }
            }

            public static IDataBox<Album> AlbumSet
            {
                get { return _dataContextPerRequest.AlbumSet; }
            }

            public static IDataBox<AlbumItem> AlbumItemSet
            {
                get { return _dataContextPerRequest.AlbumItemSet; }
            }

            public static IDataBox<Attach> AttachSet
            {
                get { return _dataContextPerRequest.AttachSet; }
            }

            public static IDataBox<Badge> BadgeSet
            {
                get { return _dataContextPerRequest.BadgeSet; }
            }

            public static IDataBox<BaseUser> BaseUserSet
            {
                get { return _dataContextPerRequest.BaseUserSet; }
            }

            public static IDataBox<BlockedRecord> BlockedRecordSet
            {
                get { return _dataContextPerRequest.BlockedRecordSet; }
            }

            public static IDataBox<Bulletin> BulletinSet
            {
                get { return _dataContextPerRequest.BulletinSet; }
            }

            public static IDataBox<SurveyBulletin> SurveyBulletinSet
            {
                get { return _dataContextPerRequest.SurveyBulletinSet; }
            }

            public static IDataBox<Candidate> CandidateSet
            {
                get { return _dataContextPerRequest.CandidateSet; }
            }

            public static IDataBox<City> CitySet
            {
                get { return _dataContextPerRequest.CitySet; }
            }

            public static IDataBox<Coauthor> CoauthorSet
            {
                get { return _dataContextPerRequest.CoauthorSet; }
            }

            public static IDataBox<Comment> CommentSet
            {
                get { return _dataContextPerRequest.CommentSet; }
            }

            public static IDataBox<Content> ContentSet
            {
                get { return _dataContextPerRequest.ContentSet; }
            }

            public static IDataBox<Expert> ExpertSet
            {
                get { return _dataContextPerRequest.ExpertSet; }
            }

            public static IDataBox<ExpertVote> ExpertVoteSet
            {
                get { return _dataContextPerRequest.ExpertVoteSet; }
            }

            public static IDataBox<GroupMember> GroupMemberSet
            {
                get { return _dataContextPerRequest.GroupMemberSet; }
            }

            public static IDataBox<Group> GroupSet
            {
                get { return _dataContextPerRequest.GroupSet; }
            }

            public static IDataBox<GroupAd> GroupAdSet
            {
                get { return _dataContextPerRequest.GroupAdSet; }
            }

            public static IDataBox<GroupCategory> GroupCategorySet
            {
                get { return _dataContextPerRequest.GroupCategorySet; }
            }

            public static IDataBox<Invite> InviteSet
            {
                get { return _dataContextPerRequest.InviteSet; }
            }

            public static IDataBox<Like> LikeSet
            {
                get { return _dataContextPerRequest.LikeSet; }
            }


            public static IDataBox<Message> MessageSet
            {
                get { return _dataContextPerRequest.MessageSet; }
            }

            public static IDataBox<MailRecord> MailRecordSet
            {
                get { return _dataContextPerRequest.MailRecordSet; }
            }

            public static IDataBox<Option> OptionSet
            {
                get { return _dataContextPerRequest.OptionSet; }
            }

            public static IDataBox<Region> RegionSet
            {
                get { return _dataContextPerRequest.RegionSet; }
            }

            public static IDataBox<SocialAccount> SocialAccountSet
            {
                get { return _dataContextPerRequest.SocialAccountSet; }
            }

            public static IDataBox<Tag> TagSet
            {
                get { return _dataContextPerRequest.TagSet; }
            }

            public static IDataBox<ErrorText> ErrorTextSet
            {
                get { return _dataContextPerRequest.ErrorTextSet; }
            }

            public static IDataBox<ProfileChangeRequest> ProfileChangeRequestSet
            {
                get { return _dataContextPerRequest.ProfileChangeRequestSet; }
            }

            public static IDataBox<ScheduleJob> ScheduleJobSet
            {
                get { return _dataContextPerRequest.ScheduleJobSet; }
            }

            public static IDataBox<ScheduleExecutionInfo> ScheduleExecutionInfoSet
            {
                get { return _dataContextPerRequest.ScheduleExecutionInfoSet; }
            }

            public static IDataBox<SmsInfo> SmsInfoSet
            {
                get { return _dataContextPerRequest.SmsInfoSet; }
            }

            public static IDataBox<PayPalVerification> PayPalVerificationSet
            {
                get { return _dataContextPerRequest.PayPalVerificationSet; }
            }

            public static IDataBox<UserAuthentificationLog> UserAuthentificationLogSet
            {
                get { return _dataContextPerRequest.UserAuthentificationLogSet; }
            }

            public static IDataBox<UserAuthorizationLog> UserAuthorizationLogSet
            {
                get { return _dataContextPerRequest.UserAuthorizationLogSet; }
            
            }

            public static IDataBox<UserPollVoteLog> UserPollVoteLogSet
            {
                get { return _dataContextPerRequest.UserPollVoteLogSet; }
            
            }

            public static IDataBox<UserRegistrationLog> UserRegistrationLogSet
            {
                get { return _dataContextPerRequest.UserRegistrationLogSet; }
            }

            public static void BeginWork()
            {
                _dataContextPerRequest.BeginWork();
            }

            public static void EndWork()
            {
                _dataContextPerRequest.EndWork();
            }

            public static void SaveChanges()
            {
                _dataContextPerRequest.SaveChanges();
            }

            public static void LoadProperty<TEntity>(TEntity entity, System.Linq.Expressions.Expression<Func<TEntity, object>> selector)
            {
                _dataContextPerRequest.LoadProperty<TEntity>(entity, selector);
            }

            public static void LoadProperty(object entity, string navigationProperty)
            {
                _dataContextPerRequest.LoadProperty(entity, navigationProperty);
            }

        }
    }
}
