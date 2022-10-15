using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UnionWebApi.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        public RoomController(Database db)
        {
            Db = db;
        }

        // GET api/Book
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            await Db.Connection.OpenAsync();
            var query = new Room(Db);
            var result = await query.GetAllAsync();
            return new OkObjectResult(result);
        }

        // GET api/Book/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new Room(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

        // POST api/Book
        [BasicAuthorization]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Room body)
        {
            await Db.Connection.OpenAsync();
            body.Db = Db;
            int result=await body.InsertAsync();
            Console.WriteLine("inserted id="+result);
            return new OkObjectResult(result);
        }

        // PUT api/Book/5
        [BasicAuthorization]
        [HttpPut("{id}")]
        
        public async Task<IActionResult> PutOne(int id, [FromBody]Room body)
        {
            await Db.Connection.OpenAsync();
            var query = new Room(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            result.room_id = body.room_id;    
            result.name = body.name;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }

        // DELETE api/Book/5
        [BasicAuthorization]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new Room(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            await result.DeleteAsync();
            return new OkObjectResult(result);
        }

        public Database Db { get; }
    }
}