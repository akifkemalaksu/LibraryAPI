using LibraryAPI.Caching.Interfaces;
using LibraryAPI.Contexts;
using LibraryAPI.Dtos;
using LibraryAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WritersController : ControllerBase
    {
        private const string WRITERS_LIST_FOR_CACHING = "writers";

        private readonly LibraryContext _libraryContext;
        private readonly ICustomCache _customCache;

        public WritersController(LibraryContext libraryContext, ICustomCache customCache)
        {
            _libraryContext = libraryContext;
            _customCache = customCache;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var writers = _customCache.Get<List<Writer>>(WRITERS_LIST_FOR_CACHING);
            if (writers is null)
            {
                writers = _libraryContext.Writers.ToList();
                _customCache.Set(WRITERS_LIST_FOR_CACHING,writers);
            }
            return Ok(writers);
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id) => Ok(_libraryContext.Writers.FirstOrDefault(b => b.Id == id));

        [HttpPost]
        public IActionResult Post(AddWriterDto addWriter)
        {
            var writer = new Writer { Name = addWriter.Name, SurName = addWriter.SurName };
            _libraryContext.Add(writer);
            _libraryContext.SaveChanges();
            _customCache.Remove(WRITERS_LIST_FOR_CACHING);
            return Ok(writer);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var writer = _libraryContext.Writers.FirstOrDefault(b => b.Id == id);
            if (writer is null) return NotFound();

            _libraryContext.Remove(writer);
            _libraryContext.SaveChanges();
            _customCache.Remove(WRITERS_LIST_FOR_CACHING);
            return Ok();
        }

        [HttpPut]
        public IActionResult Update(UpdateWriterDto updateWriter)
        {
            var writer = _libraryContext.Writers.FirstOrDefault(b => b.Id == updateWriter.Id);
            if (writer is null) return NotFound();

            writer.Name = updateWriter.Name;
            writer.SurName = updateWriter.SurName;
            _libraryContext.Update(writer);
            _libraryContext.SaveChanges();
            _customCache.Remove(WRITERS_LIST_FOR_CACHING);
            return Ok();
        }
    }
}
