using System.Collections.Concurrent;

namespace Federation.Core.Tests
{
    public static class DataManager
    {
        private static BlockingCollection<Address> _addressSet = new BlockingCollection<Address>();
        private static BlockingCollection<Attach> _attachSet = new BlockingCollection<Attach>();
        private static BlockingCollection<Badge> _badgeSet = new BlockingCollection<Badge>();
        private static BlockingCollection<BaseUser> _baseUserSet = new BlockingCollection<BaseUser>();
        private static BlockingCollection<BlockedRecord> _blockedRecordSet = new BlockingCollection<BlockedRecord>();
        private static BlockingCollection<Bulletin> _bulletinSet = new BlockingCollection<Bulletin>();
        private static BlockingCollection<SurveyBulletin> _surveyBulletinSet = new BlockingCollection<SurveyBulletin>();
        private static BlockingCollection<Candidate> _candidateSet = new BlockingCollection<Candidate>();
        private static BlockingCollection<City> _citySet = new BlockingCollection<City>();
        private static BlockingCollection<Coauthor> _coauthorSet = new BlockingCollection<Coauthor>();
        private static BlockingCollection<Comment> _commentSet = new BlockingCollection<Comment>();
        private static BlockingCollection<Content> _contentSet = new BlockingCollection<Content>();
        private static BlockingCollection<Expert> _expertSet = new BlockingCollection<Expert>();
        private static BlockingCollection<ExpertVote> _expertVoteSet = new BlockingCollection<ExpertVote>();
        private static BlockingCollection<GroupMember> _groupMemberSet = new BlockingCollection<GroupMember>();
        private static BlockingCollection<Group> _groupSet = new BlockingCollection<Group>();
        private static BlockingCollection<Invite> _inviteSet = new BlockingCollection<Invite>();
        private static BlockingCollection<Like> _likeSet = new BlockingCollection<Like>();
        private static BlockingCollection<Message> _messageSet = new BlockingCollection<Message>();
        private static BlockingCollection<MailRecord> _mailRecordSet = new BlockingCollection<MailRecord>();
        private static BlockingCollection<Option> _optionSet = new BlockingCollection<Option>();
        private static BlockingCollection<Region> _regionSet = new BlockingCollection<Region>();
        private static BlockingCollection<Tag> _tagSet = new BlockingCollection<Tag>();
        private static BlockingCollection<ErrorText> _errorTextSet = new BlockingCollection<ErrorText>();
        private static BlockingCollection<ScheduleJob> _scheduleJobSet = new BlockingCollection<ScheduleJob>();
        private static BlockingCollection<ScheduleExecutionInfo> _scheduleExecutionInfoSet = new BlockingCollection<ScheduleExecutionInfo>();
        private static BlockingCollection<SocialAccount> _socialAccountSet = new BlockingCollection<SocialAccount>();
        private static BlockingCollection<SmsInfo> _smsInfoSet = new BlockingCollection<SmsInfo>();

        public static BlockingCollection<Address> AddressSet { get { return _addressSet; } }
        public static BlockingCollection<Attach> AttachSet { get { return _attachSet; } }
        public static BlockingCollection<Badge> BadgeSet { get { return _badgeSet; } }
        public static BlockingCollection<BaseUser> BaseUserSet { get { return _baseUserSet; } }
        public static BlockingCollection<BlockedRecord> BlockedRecordSet { get { return _blockedRecordSet; } }
        public static BlockingCollection<Bulletin> BulletinSet { get { return _bulletinSet; } }
        public static BlockingCollection<SurveyBulletin> SurveyBulletinSet { get { return _surveyBulletinSet; } }
        public static BlockingCollection<Candidate> CandidateSet { get { return _candidateSet; } }
        public static BlockingCollection<City> CitySet { get { return _citySet; } }
        public static BlockingCollection<Coauthor> CoauthorSet { get { return _coauthorSet; } }
        public static BlockingCollection<Comment> CommentSet { get { return _commentSet; } }
        public static BlockingCollection<Content> ContentSet { get { return _contentSet; } }
        public static BlockingCollection<Expert> ExpertSet { get { return _expertSet; } }
        public static BlockingCollection<ExpertVote> ExpertVoteSet { get { return _expertVoteSet; } }
        public static BlockingCollection<GroupMember> GroupMemberSet { get { return _groupMemberSet; } }
        public static BlockingCollection<Group> GroupSet { get { return _groupSet; } }
        public static BlockingCollection<Invite> InviteSet { get { return _inviteSet; } }
        public static BlockingCollection<Like> LikeSet { get { return _likeSet; } }
        public static BlockingCollection<Message> MessageSet { get { return _messageSet; } }
        public static BlockingCollection<MailRecord> MailRecordSet { get { return _mailRecordSet; } }
        public static BlockingCollection<Option> OptionSet { get { return _optionSet; } }
        public static BlockingCollection<Region> RegionSet { get { return _regionSet; } }
        public static BlockingCollection<Tag> TagSet { get { return _tagSet; } }
        public static BlockingCollection<ErrorText> ErrorTextSet { get { return _errorTextSet; } }
        public static BlockingCollection<ScheduleJob> ScheduleJobSet { get { return _scheduleJobSet; } }
        public static BlockingCollection<SmsInfo> SmsInfoSet { get { return _smsInfoSet; } }

        public static BlockingCollection<ScheduleExecutionInfo> ScheduleExecutionInfoSet { get { return _scheduleExecutionInfoSet; } }
        public static BlockingCollection<SocialAccount> SocialAccountSet { get { return _socialAccountSet; } }
    }
}
