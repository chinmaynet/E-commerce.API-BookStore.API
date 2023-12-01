using BookStore.API.Model;
using Microsoft.AspNetCore.JsonPatch;

namespace BookStore.API.Repository
{
    public interface IBookRepository
    {
        Task<List<BookModel>> GetAllBooksAsync();
        Task<BookModel> GetBookByIdAsync(int bookId);
        Task<int> AddNewBookAsync(BookModel bookModel);

        Task UpdateBookAsync(int bookId, BookModel bookModel);

        Task UpdateBookPatchAsync(int bookId, JsonPatchDocument bookModel);
        //Task UpdateBookPatchAsync(int id, Azure.JsonPatchDocument bookModel);

        Task DeleteBookAsync(int bookId);
    }
}
