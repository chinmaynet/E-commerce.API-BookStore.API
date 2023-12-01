using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.API.Model
{
    public class Product
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public string color { get; set; }
        public string catagory { get; set; }
        public string description { get; set; }

        //public string imageURL { get; set; }
        [NotMapped]
        public List<IFormFile> ImageFiles { get; set; }

       
        public ICollection<ProductImage> ProductImages { get; set; }
        //[NotMapped]
        //public List<string> imagePathss { get; set; }
    }
}
