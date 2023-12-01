
namespace BookStore.API.Model
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public int TotalPrice { get; set; }
        public Guid UserId { get; set; }
    }
}
