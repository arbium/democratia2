using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class Group_ModersViewModel
    {
        public IList<Group_Moders_ItemViewModel> ModeratorList { get; set; }
        public int EmptySlots { get; set; }
        public int FreeSlots { get; set; }
        public bool CanBeModerator { get; set; }
        public Guid MemberId { get; set; }


        public Group_ModersViewModel(Group group = null)
        {
            ModeratorList = new List<Group_Moders_ItemViewModel>();
            if (group != null)
            {
                var moderators = DataService.PerThread.GroupMemberSet.Where(x => x.GroupId == group.Id && x.State == (byte)GroupMemberState.Moderator).ToList();

                foreach (var moderator in moderators)
                    ModeratorList.Add(new Group_Moders_ItemViewModel(moderator.User));

                EmptySlots = group.ModeratorsCount - ModeratorList.Count;
                EmptySlots = EmptySlots < 0 ? 0 : EmptySlots;

                FreeSlots = 3 - ModeratorList.Count;
                FreeSlots = FreeSlots < 0 ? 0 : FreeSlots;

                CanBeModerator = false;
                if (FreeSlots > 0 && UserContext.Current != null)
                {
                    var gm = GroupService.UserInGroup(UserContext.Current.Id, group);
                    if (gm != null)
                    {
                        MemberId = gm.Id;
                        if (group.State == (byte)GroupState.Archive)
                            CanBeModerator = true;
                    }
                }
                
            }
        }
    }

    public class Group_Moders_ItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public string Avatar { get; set; }

        public Group_Moders_ItemViewModel(User user = null)
        {
            if (user != null)
            {
                Id = user.Id;
                Name = user.FirstName;
                Surname = user.SurName;
                if (user.Address != null)
                    City = user.Address.City.Title;
                Avatar = ImageService.GetImageUrl<User>(user.Avatar);
            }
        }
    }
}