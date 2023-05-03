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
        private readonly IDistributedCache _distributedCache;

        public WritersController(LibraryContext libraryContext, IDistributedCache distributedCache)
        {
            _libraryContext = libraryContext;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var writersAsJsonString = _distributedCache.GetString(WRITERS_LIST_FOR_CACHING);
            if (string.IsNullOrEmpty(writersAsJsonString))
            {
                var writersFromDb = _libraryContext.Writers.ToList();
                var writersFromDbAsJsonString = JsonSerializer.Serialize(writersFromDb);
                _distributedCache.SetString(WRITERS_LIST_FOR_CACHING, writersFromDbAsJsonString);
                return Ok(writersFromDb);
            }

            var writers = JsonSerializer.Deserialize<List<Writer>>(writersAsJsonString);
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
            _distributedCache.Remove(WRITERS_LIST_FOR_CACHING);
            return Ok(writer);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var writer = _libraryContext.Writers.FirstOrDefault(b => b.Id == id);
            if (writer is null) return NotFound();

            _libraryContext.Remove(writer);
            _libraryContext.SaveChanges();
            _distributedCache.Remove(WRITERS_LIST_FOR_CACHING);
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
            _distributedCache.Remove(WRITERS_LIST_FOR_CACHING);
            return Ok();
        }
    }
}
