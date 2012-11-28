using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Objects;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Federation.Core
{
    public class DataPerThreadMap
    {
        private static Dictionary<int, EntityModelContainer> _dictionary = new Dictionary<int, EntityModelContainer>();
    }

    // Само создание и удаление контекста происходит в global.asax. Сделать это через евенты проблематично. 
    // Делать if(HttpContext.Current.Session.Contains(PerRequestHttpContextKey)) в каждом свойстве накладно
    [Export("PerThread", typeof(IDataService))]
    public class EFDataSevicePerThreadImpl : IDataService
    {
        [ThreadStatic]
        private static EntityModelContainer _context = null;

        private EntityModelContainer Context
        {
            get
            {
                if (_context == null)
                    BeginWork();

                return _context;
            }
        }

        public ObjectResult<Comment> GetCommentsByFullText(string query, int count)
        {
            return Context.GetCommentsByFullText(query, count);
        }

        public ObjectResult<Post> GetPostsByFullText(string query, int count)
        {
            return Context.GetPostsByFullText(query, count);
        }

        public ObjectResult<Petition> GetPetitionsByFullText(string query, int count)
        {
            return Context.GetPetitionsByFullText(query, count);
        }

        public ObjectResult<Poll> GetPollsByFullText(string query, int count)
        {
            return Context.GetPollsByFullText(query, count);
        }

        public ObjectResult<Survey> GetSurveysByFullText(string query, int count)
        {
            return Context.GetSurveysByFullText(query, count);
        }

        public ObjectResult<Group> GetGroupsByFullText(string query, int count)
        {
            return Context.GetGroupsByFullText(query, count);
        }

        public ObjectResult<Content> GetOrderedByRatingGroupContent(Guid groupId)
        {
            return Context.GetOrderedByRatingGroupContent(groupId);
        }

        public IDataBox<Address> AddressSet
        {
            get
            {
                return new DataBox<Address>(Context.AddressSet);
            }
        }

        public IDataBox<Album> AlbumSet
        {
            get
            {
                return new DataBox<Album>(Context.AlbumSet);
            }
        }

        public IDataBox<AlbumItem> AlbumItemSet
        {
            get
            {
                return new DataBox<AlbumItem>(Context.AlbumItemSet);
            }
        }

        public IDataBox<Attach> AttachSet
        {
            get
            {
                return new DataBox<Attach>(Context.AttachSet);
            }
        }

        public IDataBox<Badge> BadgeSet
        {
            get
            {
                return new DataBox<Badge>(Context.BadgeSet);
            }
        }

        public IDataBox<BaseUser> BaseUserSet
        {
            get
            {
                return new DataBox<BaseUser>(Context.BaseUserSet);
            }
        }

        public IDataBox<BlockedRecord> BlockedRecordSet
        {
            get
            {
                return new DataBox<BlockedRecord>(Context.BlockedRecordSet);
            }
        }

        public IDataBox<Bulletin> BulletinSet
        {
            get
            {
                return new DataBox<Bulletin>(Context.BulletinSet);
            }
        }

        public IDataBox<SurveyBulletin> SurveyBulletinSet
        {
            get
            {
                return new DataBox<SurveyBulletin>(Context.SurveyBulletinSet);
            }
        }

        public IDataBox<Candidate> CandidateSet
        {
            get
            {
                return new DataBox<Candidate>(Context.CandidateSet);
            }
        }

        public IDataBox<City> CitySet
        {
            get
            {
                return new DataBox<City>(Context.CitySet);
            }
        }

        public IDataBox<Coauthor> CoauthorSet
        {
            get
            {
                return new DataBox<Coauthor>(Context.CoauthorSet);
            }
        }

        public IDataBox<Comment> CommentSet
        {
            get
            {
                return new DataBox<Comment>(Context.CommentSet);
            }
        }

        public IDataBox<Content> ContentSet
        {
            get
            {
                return new DataBox<Content>(Context.ContentSet);
            }
        }

        public IDataBox<Expert> ExpertSet
        {
            get
            {
                return new DataBox<Expert>(Context.ExpertSet);
            }
        }

        public IDataBox<ExpertVote> ExpertVoteSet
        {
            get
            {
                return new DataBox<ExpertVote>(Context.ExpertVoteSet);
            }
        }

        public IDataBox<GroupMember> GroupMemberSet
        {
            get
            {
                return new DataBox<GroupMember>(Context.GroupMemberSet);
            }
        }

        public IDataBox<Group> GroupSet
        {
            get
            {
                return new DataBox<Group>(Context.GroupSet);
            }
        }

        public IDataBox<GroupAd> GroupAdSet
        {
            get
            {
                return new DataBox<GroupAd>(Context.GroupAdSet);
            }
        }

        public IDataBox<GroupCategory> GroupCategorySet
        {
            get
            {
                return new DataBox<GroupCategory>(Context.GroupCategorySet);
            }
        }

        public IDataBox<Invite> InviteSet
        {
            get
            {
                return new DataBox<Invite>(Context.InviteSet);
            }
        }

        public IDataBox<Like> LikeSet
        {
            get
            {
                return new DataBox<Like>(Context.LikeSet);
            }
        }

        public IDataBox<MailRecord> MailRecordSet
        {
            get
            {
                return new DataBox<MailRecord>(Context.MailRecordSet);
            }
        }

        public IDataBox<Message> MessageSet
        {
            get
            {
                return new DataBox<Message>(Context.MessageSet);
            }
        }

        public IDataBox<Option> OptionSet
        {
            get
            {
                return new DataBox<Option>(Context.OptionSet);
            }
        }

        public IDataBox<Region> RegionSet
        {
            get
            {
                return new DataBox<Region>(Context.RegionSet);
            }
        }

        public IDataBox<Tag> TagSet
        {
            get
            {
                return new DataBox<Tag>(Context.TagSet);
            }
        }

        public IDataBox<SmsInfo> SmsInfoSet
        {
            get
            {
                return new DataBox<SmsInfo>(Context.SmsInfoSet);
            }
        }

        public IDataBox<ErrorText> ErrorTextSet
        {
            get
            {
                return new DataBox<ErrorText>(Context.ErrorTextSet);
            }
        }

        public IDataBox<ProfileChangeRequest> ProfileChangeRequestSet
        {
            get
            {
                return new DataBox<ProfileChangeRequest>(Context.ProfileChangeRequestSet);
            }
        }


        public IDataBox<ScheduleJob> ScheduleJobSet
        {
            get
            {
                return new DataBox<ScheduleJob>(Context.ScheduleJobSet);
            }
        }

        public IDataBox<ScheduleExecutionInfo> ScheduleExecutionInfoSet
        {
            get
            {
                return new DataBox<ScheduleExecutionInfo>(Context.ScheduleExecutionInfoSet);
            }
        }

        public IDataBox<SocialAccount> SocialAccountSet
        {
            get {
                return new DataBox<SocialAccount>(Context.SocialAccountSet);
            }
        }

        public IDataBox<PayPalVerification> PayPalVerificationSet
        {
            get
            {
                return new DataBox<PayPalVerification>(Context.PayPalVerificationSet);
            }
        }


        public IDataBox<UserAuthentificationLog> UserAuthentificationLogSet
        {
            get 
            {
                return new DataBox<UserAuthentificationLog>(Context.UserAuthentificationLogSet);
            }
        }

        public IDataBox<UserAuthorizationLog> UserAuthorizationLogSet
        {
            get 
            {
                return new DataBox<UserAuthorizationLog>(Context.UserAuthorizationLogSet);
            }

        }

        public IDataBox<UserPollVoteLog> UserPollVoteLogSet
        {
            get 
            {
                return new DataBox<UserPollVoteLog>(Context.UserPollVoteLogSet);
            }

        }

        public IDataBox<UserRegistrationLog> UserRegistrationLogSet
        {
            get
            {
                return new DataBox<UserRegistrationLog>(Context.UserRegistrationLogSet);
            }

        }

        public void BeginWork()
        {
            _context = new EntityModelContainer();
        }

        public void EndWork()
        {        
            try
            {
                Context.SaveChanges();
            }
            catch (Exception exception)
            {
                Logger.Write("Message: " + exception.Message + "\r\n Data: \r\n" + exception.Data + "\r\n Trace:\r\n" + exception.StackTrace, "Exceptions", 0, 32667, TraceEventType.Error);//TODO: подкрутить вывод в лог
            }
        }

        public void SaveChanges()
        {
            try
            {
                Context.SaveChanges();
            }
            catch(Exception e)
            {
                Context.Dispose();
                throw e;
            }
        }

        public void LoadProperty<TEntity>(TEntity entity, Expression<Func<TEntity, object>> selector)
        {
            Context.LoadProperty<TEntity>(entity, selector);
        }

        public void LoadProperty(object entity, string navigationProperty)
        {
            Context.LoadProperty(entity, navigationProperty);
        }
    }
}