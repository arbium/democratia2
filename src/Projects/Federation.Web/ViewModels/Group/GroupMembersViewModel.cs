using System;
using System.Collections.Generic;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class GroupMembersViewModel
    {        
        public Guid GroupId { get; set; }
        public string GroupUrl { get; set; }
        public string GroupName { get; set; }
        public GroupPrivacy GroupPrivacy { get; set; }        
        public int MaxModers { get; set; }
        public int RequestsCount { get; set; }
        public int NoPhotoCount { get; set; }
        public bool IsMemberModeration { get; set; }
        public bool IsBlank { get; set; }
        public int MembersTotalCount { get; set; }
        public string Query { get; set; }

        public readonly byte ShowCount = 10;

        public readonly IList<GroupMembers_MemberViewModel> Members = new List<GroupMembers_MemberViewModel>();
        public readonly Group_ModersViewModel Moderators = new Group_ModersViewModel();

        public GroupMembersViewModel()
        {
        }

        public GroupMembersViewModel(Group group, string query = null)
        {            
            if (group != null)
            {
                GroupId = group.Id;
                GroupUrl = group.Url;
                GroupName = group.Name;
                GroupPrivacy = (GroupPrivacy)group.Privacy;
                MaxModers = group.ModeratorsCount;
                IsMemberModeration = group.PrivacyEnum.HasFlag(GroupPrivacy.MemberModeration);
                IsBlank = group.State == (byte)GroupState.Blank;

                
                MembersTotalCount = DataService.PerThread.GroupMemberSet.Count(x => x.GroupId == group.Id && (x.State == (byte)GroupMemberState.Approved || x.State == (byte)GroupMemberState.Moderator));

                var groupMembers = group.GroupMembers;

                RequestsCount = groupMembers.Count(x => x.State == (byte)GroupMemberState.NotApproved);                
                Moderators = new Group_ModersViewModel(group);
                NoPhotoCount = groupMembers.Count(x => x.State == (byte)GroupMemberState.Approved && x.User.Avatar == null);

                if (string.IsNullOrWhiteSpace(query))
                {
                    Members = groupMembers.Where(x => x.State == (byte)GroupMemberState.Approved && x.User.Avatar != null)
                        .OrderBy(x => Guid.NewGuid()).Take(ShowCount).Select(x => new GroupMembers_MemberViewModel(x)).ToList();
                }
                else
                {
                    Query = query;

                    var fullnamed = groupMembers
                        .Where(x => !string.IsNullOrEmpty(x.User.SurName) && !string.IsNullOrEmpty(x.User.FirstName) && 
                            query.Contains(x.User.SurName) && query.Contains(x.User.FirstName));
                    var lastnamed = groupMembers.Where(x => !string.IsNullOrEmpty(x.User.SurName) && query.Contains(x.User.SurName));
                    var firstnamed = groupMembers.Where(x => !string.IsNullOrEmpty(x.User.FirstName) && query.Contains(x.User.FirstName));

                    Members = fullnamed.Union(lastnamed.Union(firstnamed)).Distinct().Take(ShowCount).Select(x => new GroupMembers_MemberViewModel(x)).ToList();
                }
            }
        }
    }

    public class GroupMembers_MemberViewModel
    {
        public Guid Id { get; set; }
        public Guid GroupMemberId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        
        public GroupMembers_MemberViewModel()
        {
        }

        public GroupMembers_MemberViewModel(GroupMember member)
        {
            if (member != null)
            {
                Id = member.UserId;
                GroupMemberId = member.Id;
                FullName = member.User.FullName;
                Avatar = ImageService.GetImageUrl<User>(member.User.Avatar);
            }
        }
    }
}