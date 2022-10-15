
using Microsoft.AspNetCore.Mvc;


namespace UnionWebApi.Controllers
{
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        public LoginController(Database db)
        {
            Db = db;
        }


        // POST api/login
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]User body)
        {
            Console.WriteLine(body.login);
            Console.WriteLine(body.pass);
            await Db.Connection.OpenAsync();
            var query = new Login(Db);
            var passwordFromDatabase = await query.GetPassword(body.login);
   
            if (passwordFromDatabase is null || ! BCrypt.Net.BCrypt.Verify(body.pass, passwordFromDatabase))
            {
                // authentication failed
                return new OkObjectResult(false);
            }
            else
            {
                // authentication successful
                return new OkObjectResult(true);
            }
            
        }

    

        public Database Db { get; }
    }
}