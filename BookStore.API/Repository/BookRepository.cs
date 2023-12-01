
using BookStore.API.Data;
using BookStore.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;

namespace BookStore.API.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly BookStoreContext _context;

        private readonly IMapper _mapper; //automapper DI

        public BookRepository(BookStoreContext context, IMapper mapper)  // DI to get automapper
        {

            _context = context;
            _mapper = mapper;
        }
        public async Task<List<BookModel>> GetAllBooksAsync() {

            //var records = await  _context.Books.Select(x => new BookModel() //doesnt know bookModel so convert bookModel -> book (BookStoreContext)
            //{ 
            //    Id= x.Id,   
            //    Title = x.Title,                                      //manual mapping          
            //    Description = x.Description,                         
            //}).ToListAsync();

            //return records;


            var records = await _context.Books.ToListAsync();   
            return _mapper.Map<List<BookModel>>(records);               //using automapper
        }

        public async Task<BookModel> GetBookByIdAsync(int bookId)
        {

            //var records  = await _context.Books.Where(x => x.Id == bookId).Select(x => new BookModel() //doesnt know bookModel so convert bookModel -> book (BookStoreContext)
            //{   
            //    Id = x.Id,
            //    Title = x.Title,                                         
            //    Description = x.Description,                            //manual mapping          
            //}).FirstOrDefaultAsync();

            //return records;

            var book = await _context.Books.FindAsync(bookId);       
            return _mapper.Map<BookModel>(book);                        //using automapper

        }


        public async Task<int> AddNewBookAsync(BookModel bookModel)
        {       //bookModel -> book (_context knows BookModel -> book only )
            var book = new Books()                  
            {
                Title = bookModel.Title,
                Description = bookModel.Description
            };


            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return book.Id;
        }

        public async Task UpdateBookAsync(int bookId, BookModel bookModel)
        { //HTTPPUT: Create API to update the item in the database 

            //var book = await _context.Books.FindAsync(bookId); //hitting db here
            //if (book != null) { 
            //    book.Title = bookModel.Title;   
            //    book.Description = bookModel.Description;   

            //    await _context.SaveChangesAsync(); //hitting db here
            //}

            //HTTPPUT: Update item in one database call

            var book = new Books()    
            {   
                Id =bookId,            //
                Title = bookModel.Title,
                Description = bookModel.Description
            };


            _context.Books.Update(book); //diffrence
            await _context.SaveChangesAsync();

        }
        //public async Task 
        public async Task UpdateBookPatchAsync(int bookId, JsonPatchDocument bookModel)
        {//HTTPPATCH: Create API to partially update an item 
            var book = await _context.Books.FindAsync(bookId);
            if (book != null) 
            {
                bookModel.ApplyTo(book);
                await _context.SaveChangesAsync(); 
            }
        }


        public async Task DeleteBookAsync(int bookId)
        {       //bookModel -> book (_context knows BookModel -> book only )


            //var book = _context.Books.Where(x => x.Title == "" ).FirstOrDefaultAsync();

            var book = new Books() { Id = bookId };
            _context.Books.Remove(book);

            await _context.SaveChangesAsync();

        }
    }
}
