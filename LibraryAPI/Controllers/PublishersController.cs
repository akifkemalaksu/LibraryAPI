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
    public class PublishersController : ControllerBase
    {
        private const string PUBLISHERS_LIST_FOR_CACHING = "publishers";

        private readonly LibraryContext _libraryContext;
        private readonly IDistributedCache _distributedCache;

        public PublishersController(LibraryContext libraryContext, IDistributedCache distributedCache)
        {
            _libraryContext = libraryContext;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var publishersAsJsonString = _distributedCache.GetString(PUBLISHERS_LIST_FOR_CACHING);
            if (string.IsNullOrEmpty(publishersAsJsonString))
            {
                var publishersFromDb = _libraryContext.Publishers.ToList();
                var publishersFromDbAsJsonString = JsonSerializer.Serialize(publishersFromDb);
                _distributedCache.SetString(PUBLISHERS_LIST_FOR_CACHING, publishersFromDbAsJsonString);
                return Ok(publishersFromDb);
            }

            var publishers = JsonSerializer.Deserialize<List<Publisher>>(publishersAsJsonString);
            return Ok(publishers);
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id) => Ok(_libraryContext.Publishers.FirstOrDefault(b => b.Id == id));

        [HttpPost]
        public IActionResult Post(AddWriterDto addPublisher)
        {
            var publisher = new Publisher { Name = addPublisher.Name };
            _libraryContext.Add(publisher);
            _libraryContext.SaveChanges();
            _distributedCache.Remove(PUBLISHERS_LIST_FOR_CACHING);
            return Ok(publisher);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var publisher = _libraryContext.Publishers.FirstOrDefault(b => b.Id == id);
            if (publisher is null) return NotFound();

            _libraryContext.Remove(publisher);
            _libraryContext.SaveChanges();
            _distributedCache.Remove(PUBLISHERS_LIST_FOR_CACHING);
            return Ok();
        }

        [HttpPut]
        public IActionResult Update(UpdatePublisherDto updatePublisher)
        {
            var publisher = _libraryContext.Publishers.FirstOrDefault(b => b.Id == updatePublisher.Id);
            if (publisher is null) return NotFound();

            publisher.Name = updatePublisher.Name;
            _libraryContext.Update(publisher);
            _libraryContext.SaveChanges();
            _distributedCache.Remove(PUBLISHERS_LIST_FOR_CACHING);
            return Ok();
        }
    }
}
