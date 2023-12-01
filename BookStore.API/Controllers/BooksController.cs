using Azure;
using BookStore.API.Model;
using BookStore.API.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace BookStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        public BooksController(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAllBooks()
        {//HTTPGET: Create API to get all items from database 
            var result = await _bookRepository.GetAllBooksAsync();  
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetBookById([FromRoute]int id)
        {//HTTPGET: Create API to get one item from database
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null) {
                return NotFound();
            }
            return Ok(book);
        }


        [HttpPost] 
        [Route("")] 
        public async Task<IActionResult> AddNewBook([FromBody]BookModel bookModel)
        {//HTTPPOST: Create API to add new item in the database 
            var id = await _bookRepository.AddNewBookAsync(bookModel);
            return CreatedAtAction(nameof(GetBookById),new { id = id, Controller = "books"},id);
           
        }

        [HttpPut] //updates all of one single record
        [Route("{id}")]
        public async Task<IActionResult> UpdateBook( [FromBody] BookModel bookModel, [FromRoute] int id)
        {//HTTPPUT: Create API to update the item in the database 
            await _bookRepository.UpdateBookAsync(id ,bookModel);
            return Ok();
        }



        [HttpPatch("{id}")]//any specific property of record
      
        public async Task<IActionResult> UpdateBookPatch([FromBody] Microsoft.AspNetCore.JsonPatch.JsonPatchDocument bookModel, [FromRoute] int id)
        {// HTTPPATCH: Create API to partially update an item
            await _bookRepository.UpdateBookPatchAsync(id, bookModel);
            return Ok();

        } //patch  //https://localhost:44376/api/books/8       //[{  "op":"replace","path":"title","value":"Update title again 1" }]

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteBook([FromRoute] int id) { 
        
            await _bookRepository.DeleteBookAsync(id);
            return Ok();
        }
    } 
}
