using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Model
{
    public class BookModel
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Please enter Title property")]        //Model Validations
        [StringLength(15)]
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
