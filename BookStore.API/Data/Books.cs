//Setup the DbContext Class and Database Connection String
namespace BookStore.API.Data
{
    public class Books
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

    }
}
