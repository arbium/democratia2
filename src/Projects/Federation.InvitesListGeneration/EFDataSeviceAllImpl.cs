//using System;
//using System.ComponentModel.Composition;
//using System.Web;

//namespace Federation.Core
//{
//    //Само создание и удаление контекста происходит в global.asax. Сделать это через евенты проблематично. 
//    //Делать if(HttpContext.Current.Session.Contains(PerRequestHttpContextKey)) в каждом свойстве накладно
//    [Export("PerSession", typeof(IDataService))]
//    [Export("PerRequest", typeof(IDataService))]
//    public class EFDataSeviceAllImpl : IDataService
//    {
//        public EntityModelContainer context = new EntityModelContainer();

//        public IDataBox<Address> AddressSet
//        {
//            get
//            {
//                return new DataBox<Address>(context.AddressSet);
//            }
//        }

//        public IDataBox<BaseUser> BaseUserSet
//        {
//            get
//            {
//                return new DataBox<BaseUser>(context.BaseUserSet);
//            }
//        }

//        public IDataBox<BlockedRecord> BlockedRecordSet
//        {
//            get
//            {
//                return new DataBox<BlockedRecord>(context.BlockedRecordSet);
//            }
//        }

//        public IDataBox<Blog> BlogSet
//        {
//            get
//            {
//                return new DataBox<Blog>(context.BlogSet);
//            }
//        }

//        public IDataBox<Bulletin> BulletinSet
//        {
//            get
//            {
//                return new DataBox<Bulletin>(context.BulletinSet);
//            }
//        }

//        public IDataBox<Candidate> CandidateSet
//        {
//            get
//            {
//                return new DataBox<Candidate>(context.CandidateSet);
//            }
//        }

//        public IDataBox<City> CitySet
//        {
//            get
//            {
//                return new DataBox<City>(context.CitySet);
//            }
//        }

//        public IDataBox<Coauthor> CoauthorSet
//        {
//            get
//            {
//                return new DataBox<Coauthor>(context.CoauthorSet);
//            }
//        }

//        public IDataBox<Comment> CommentSet
//        {
//            get
//            {
//                return new DataBox<Comment>(context.CommentSet);
//            }
//        }

//        public IDataBox<Content> ContentSet
//        {
//            get
//            {
//                return new DataBox<Content>(context.ContentSet);
//            }
//        }

//        public IDataBox<Expert> ExpertSet
//        {
//            get
//            {
//                return new DataBox<Expert>(context.ExpertSet);
//            }
//        }

//        public IDataBox<ExpertVote> ExpertVoteSet
//        {
//            get
//            {
//                return new DataBox<ExpertVote>(context.ExpertVoteSet);
//            }
//        }

//        public IDataBox<GroupMember> GroupMemberSet
//        {
//            get
//            {
//                return new DataBox<GroupMember>(context.GroupMemberSet);
//            }
//        }

//        public IDataBox<Group> GroupSet
//        {
//            get
//            {
//                return new DataBox<Group>(context.GroupSet);
//            }
//        }

//        public IDataBox<Invite> InviteSet
//        {
//            get
//            {
//                return new DataBox<Invite>(context.InviteSet);
//            }
//        }

//        public IDataBox<Like> LikeSet
//        {
//            get
//            {
//                return new DataBox<Like>(context.LikeSet);
//            }
//        }

//        public IDataBox<Message> MessageSet
//        {
//            get
//            {
//                return new DataBox<Message>(context.MessageSet);
//            }
//        }

//        public IDataBox<Region> RegionSet
//        {
//            get
//            {
//                return new DataBox<Region>(context.RegionSet);
//            }
//        }

//        public IDataBox<Tag> TagSet
//        {
//            get
//            {
//                return new DataBox<Tag>(context.TagSet);
//            }
//        }

//        public Guid BeginWork()
//        {
//            throw new NotImplementedException();
//        }

//        public void EndWork(Guid guid)
//        {
//            throw new NotImplementedException();
//        }

//        public void SaveChanges()
//        {
//            context.SaveChanges();
//        }
//    }
//}
