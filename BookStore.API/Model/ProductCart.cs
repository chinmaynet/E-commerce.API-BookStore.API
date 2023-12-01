using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Model
{
    public class ProductCart
    {
        [Key]
        public Guid productId { get; set; }
        //public string name { get; set; }
        //public int price { get; set; }
        public string color { get; set; }
        //public string catagory { get; set; }
        //public string description { get; set; }

        ////public ICollection<ProductImage> ProductImages { get; set; }

        public int quantity { get; set; }

        public Guid userId { get; set; }

    }
}
