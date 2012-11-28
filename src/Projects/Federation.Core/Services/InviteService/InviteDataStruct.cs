
namespace Federation.Core
{
    public class InviteDataStruct
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }

        public string Email { get; set; }
        public string FacebookId { get; set; }
        public string LiveJournalId { get; set; }
        public string UserInfo { get; set; }

        public InviteState? State { get; set; }
    }
}