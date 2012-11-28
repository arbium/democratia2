using System;
using Federation.Core;

namespace Federation.Web.ViewModels
{
    public class UserEditViewModel
    {
        public Guid Id { get; set; }
        public string Age { get; set; }
        public string Avatar { get; set; }
        public string BirthCity { get; set; }
        public DateTime? BirthDate { get; set; } 
        public string City { get; set; }
        public string Facebook { get; set; }
        public string FullName { get; set; }
        public string Info { get; set; }
        public string LiveJournal { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string SurName { get; set; }
        public bool IsVerified { get; set; }
        
        public UserEditViewModel()
        {
        }

        public UserEditViewModel(User user)
        {
            if (user != null)
            {
                Avatar = ImageService.GetImageUrl<User>(user.Avatar);
                Facebook = user.Facebook;
                FullName = user.FullName;
                Name = user.FirstName;
                Patronymic = user.Patronymic;
                SurName = user.SurName;
                Info = user.Info;
                LiveJournal = user.LiveJournal;
                IsVerified = user.IsVerified;

                if (user.Address != null)
                    City = user.Address.City.Title;

                BirthDate = user.BirthDate;

                if (user.BirthDate.HasValue)
                {
                    var diff = DateTime.Now.Year - user.BirthDate.Value.Year;
                    Age = DeclinationService.OfNumber((diff + (user.BirthDate.Value <= DateTime.Now.AddYears(-diff) ? 0 : -1)), "год", "года", "лет");
                }

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
            }
        }
    }
}