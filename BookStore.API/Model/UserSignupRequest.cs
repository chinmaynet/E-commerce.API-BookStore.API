namespace BookStore.API.Model
{
    public class UserSignupRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserPhone { get; set; }
        public string Role { get; set; }
    }
}
