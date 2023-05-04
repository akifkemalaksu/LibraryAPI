using LibraryAPI.Caching.Interfaces;
using LibraryAPI.Contexts;
using LibraryAPI.Dtos;
using LibraryAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private const string BOOKS_LIST_FOR_CACHING = "books";
        private readonly ICustomCache _customCache;
        private readonly LibraryContext _libraryContext;

        public BooksController(LibraryContext libraryContext, ICustomCache customCache)
        {
            _libraryContext = libraryContext;
            _customCache = customCache;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var books = _customCache.Get<List<Book>>(BOOKS_LIST_FOR_CACHING);
            if (books is null)
            {
                books = _libraryContext.Books.ToList();
                _customCache.Set("books", books, DateTime.Now.AddSeconds(10));
            }
            return Ok(books);
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id) => Ok(_libraryContext.Books.FirstOrDefault(b => b.Id == id));

        [HttpPost]
        public IActionResult Post(AddBookDto addBook)
        {
            var book = new Book
            {
                Name = addBook.Name,
                PageSize = addBook.PageSize,
                PublisherId = addBook.PublisherId,
                Writers = addBook.Writers is not null ? _libraryContext.Writers.Where(w => addBook.Writers.Contains(w.Id)).ToList() : null
            };
            _libraryContext.Add(book);
            _libraryContext.SaveChanges();
            _customCache.Remove(BOOKS_LIST_FOR_CACHING);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var book = _libraryContext.Books.FirstOrDefault(b => b.Id == id);
            if (book is null) return NotFound();

            _libraryContext.Remove(book);
            _libraryContext.SaveChanges();
            _customCache.Remove(BOOKS_LIST_FOR_CACHING);
            return Ok();
        }

        [HttpPut]
        public IActionResult Update(UpdateBookDto updateBook)
        {
            var book = _libraryContext.Books.FirstOrDefault(b => b.Id == updateBook.Id);
            if (book is null) return NotFound();

            book.Name = updateBook.Name;
            _libraryContext.Update(book);
            _libraryContext.SaveChanges();
            _customCache.Remove(BOOKS_LIST_FOR_CACHING);
            return Ok();
        }
    }
}
