using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Claims;
using MySqlConnector;

namespace UnionWebApi;
public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
        )
: base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Response.Headers.Add("WWW-Authenticate", "Basic");

        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization header missing."));
        }

        // Get authorization key
        var authorizationHeader = Request.Headers["Authorization"].ToString();
        var authHeaderRegex = new Regex(@"Basic (.*)");

        if (!authHeaderRegex.IsMatch(authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization code not formatted properly."));
        }

        var authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(authorizationHeader, "$1")));
        var authSplit = authBase64.Split(Convert.ToChar(":"), 2);
        var authUsername = authSplit[0];
        var authPassword = authSplit.Length > 1 ? authSplit[1] : throw new Exception("Unable to get password");

        Database db = new Database("server=127.0.0.1;user id=netuser;password=netpass;port=3306;database=users;");

        Login login = new Login(db);
        db.Connection.Open();
        var passwordFromDatabase = login.GetPassword(authUsername).Result;
        
        //Console.WriteLine(passwordFromDatabase);

        if (passwordFromDatabase != null && BCrypt.Net.BCrypt.Verify(authPassword, passwordFromDatabase))
        {
            Console.WriteLine("RIGHT : ");
            var authenticatedUser = new AuthenticatedUser("BasicAuthentication", true, authUsername);
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(authenticatedUser));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));

        }
        else if(passwordFromDatabase == null )
        {
            Console.WriteLine("pass from db is null!!!!!");
            return Task.FromResult(AuthenticateResult.Fail("pass from db is null"));
        }
        else
        {
            Console.WriteLine("WRONG : ");
            Console.WriteLine(authUsername);
            Console.WriteLine(authPassword);
            Console.WriteLine(passwordFromDatabase);
            return Task.FromResult(AuthenticateResult.Fail("The username or password is not correct."));

        }

    }
}