namespace BookStore.API.Model
{
    public class ProductCartDto
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public string color { get; set; }
        public string catagory { get; set; }
        public string description { get; set; }
        public int quantity { get; set; }

        //public List<ProductImage> ProductImages { get; set; }
        public List<string> ImagePaths { get; set; }
    }
}
