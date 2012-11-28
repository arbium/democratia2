using System;
using System.Linq;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserProfileViewModel
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Age { get; set; }
        public string BirthCity { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Info { get; set; }
        public int CommentsCount { get; set; }
        public bool IsTicketVerified { get; set; }
        public TimeSpan UTCOffset { get; set; }

        public int DraftsCount { get; set; }

        public readonly User_ContactsViewModel Contacts = new User_ContactsViewModel();
        public readonly _BadgesViewModel Badges = new _BadgesViewModel();
        
        public UserProfileViewModel()
        {
        }

        public UserProfileViewModel(User user)
        {
            if (user == null)
                return;

                FullName = user.FullName;
                UserId = user.Id;
                Info = TextHelper.CleanTags(user.Info);
                CommentsCount = user.Comments.Count(x => !x.IsHidden);

                IsTicketVerified = user.IsTicketVerified;

                if (user.BirthAddress != null)
                {
                    switch (user.BirthAddress.City.Type)
                    {
                        case (byte)CityType.City:
                            BirthCity = "г. ";
                            break;

                        case (byte)CityType.Settlement:
                            BirthCity = "п. ";
                            break;

                        case (byte)CityType.Village:
                            BirthCity = "с. ";
                            break;
                    }

                    BirthCity += user.BirthAddress.City.Title;
                }

                BirthDate = user.BirthDate;
                UTCOffset = new TimeSpan(0, user.UTCOffset, 0);

                if (user.BirthDate.HasValue)
                {
                    var diff = DateTime.Now.Year - user.BirthDate.Value.Year;
                    Age = DeclinationService.OfNumber((diff + (user.BirthDate.Value <= DateTime.Now.AddYears(-diff) ? 0 : -1)), "год", "года", "лет");
                }

                Contacts = new User_ContactsViewModel(user);
                DraftsCount = user.Contents.Count(x => x.State == (byte)ContentState.Draft);
                Badges = new _BadgesViewModel(user);
        }
    }
}