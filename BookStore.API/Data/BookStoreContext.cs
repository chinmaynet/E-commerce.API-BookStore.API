//Db context is mandatory to use for Entity F/W core  and cpnnection string
//Setup the DbContext Class and Database Connection String
using BookStore.API.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace BookStore.API.Data
{
    public class BookStoreContext : DbContext //DbContext entity f/w core class
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options)  : base(options)
        { 
           
        }
        public DbSet<Books> Books { get; set; }     //code first approach //table named books in db

        // public DbSet<Books> Books { get; set; }  //2nd //another table
        
        public DbSet<user> Users { get; set; }

        public DbSet<role> Role { get; set; }

        public DbSet<userRole> UserRole { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductCart> ProductCart { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=.;Database=BookStroreAPI;Integrated Security=True;TrustServerCertificate=True"); //to estabilish Communication with db cs is mandatory  //connection string
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) //composite keys productId, userId, color
        {
            modelBuilder.Entity<ProductCart>()
                .HasKey(pc => new { pc.productId, pc.userId, pc.color });


            base.OnModelCreating(modelBuilder);


        }
       
    }
}
