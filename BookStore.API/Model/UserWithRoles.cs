namespace BookStore.API.Model
{
    public class UserWithRoles //(model)
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public string UserPassword { get; set; }

        public string UserPhone { get; set; }

        public string ActivityStatus { get; set; }
        //public List<string> Roles { get; set; }
        public string Role { get; set; }
    }
}
