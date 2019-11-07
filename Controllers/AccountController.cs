using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JWT.Builder;
using JWT.Algorithms;

namespace aspnetcore_2_webapi.Controllers
{
    public class AccountController : Controller
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

        string clientSecret = "";
        public string GetApplicationRoot()
        {
          return "";
        }

        public AccountController(Microsoft.Extensions.Configuration.IConfiguration Configuration)
        {
            _config = Configuration;
            //attempt to read client secret configuration from either environment variables or appsettings.json, useful override for docker
            clientSecret = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("Authentication:ClientSecret")) ?  (string)Configuration["Authentication:ClientSecret"] : Environment.GetEnvironmentVariable("Authentication:ClientSecret");
        }

        [Route("api/Account/Login")]
        [HttpPost]
        public string Login(
            [FromBody]string username,
            [FromBody]string password){
            var token = CreateToken(username);
            return token;
        }

        //Integrate your own authentication system here
        [Route("api/Account/Token")]
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult Token(
            [FromForm]string username,
            [FromForm]string password,
            [FromQuery]string url){
            var allowedUsername = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("Authentication:ClientUsername")) ?  (string)_config["Authentication:ClientUsername"] : Environment.GetEnvironmentVariable("Authentication:ClientUsername");
            var allowedPassword = string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("Authentication:ClientPassword")) ?  (string)_config["Authentication:ClientPassword"] : Environment.GetEnvironmentVariable("Authentication:ClientPassword");
            
            if(
                (!string.IsNullOrWhiteSpace(username) 
                && !string.IsNullOrWhiteSpace(password)) 
                && username.ToLower()==allowedUsername
                && password == allowedPassword){
                var token = CreateToken(username);
                return Redirect(url+$"?token={token}");
            }else{
                return Redirect(url+$"?error=Incorrect+username+or+password");
            }
        }

        [Authorize]
        [Route("api/Account/DownloadToken")]
        [HttpPost]
        public object DownloadToken(string fileId){
            //retrieve token from header and decode it to get original user
            string authHeader = Request.Headers["Authorization"].ToString().Split(' ')[1];
            string tokenJson = DecodeToken(authHeader);
            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(tokenJson);

            //rerieve user i.d from this data
            string username = data["user"];

            var token = CreateDownloadToken(username);
            return new {
                token=token,
                valid=new DateTime().AddMinutes(5)
                };
        }

        private string CreateDownloadToken(string username){
            return CreateTokenWithClaim(username,"Use","Download");
        }

        private string CreateToken(string username){
             var token = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(clientSecret)
            .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(8).ToUnixTimeSeconds())
            .AddClaim("user", username)
            .Build();

            return token;
        }

        private string CreateTokenWithClaim(string username,string claimKey,string claimValue){
             var token = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(clientSecret)
            .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds())
            .AddClaim("user", username)
            .AddClaim(claimKey,claimValue)
            .Build();

            return token;
        }

        private string DecodeToken(string token){
            return new JwtBuilder()
            .WithSecret(clientSecret)
            .MustVerifySignature()
            .Decode(token);                    
        }
    }
}
