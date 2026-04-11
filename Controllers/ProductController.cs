using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ProductAPI.Controllers;

[EnableRateLimiting("fixed")]
[ApiController]
[Route("api/product")]
//[Authorize(AuthenticationSchemes = "Bearer")]
public class ProductController : ControllerBase
{
    [HttpGet(Name = "GetAllProducts")]
    public async Task<IEnumerable<Product>> Get()
    {
        await Task.Delay(15);
        return new List<Product>
            {
                new() {Name="Computer"},
                new(){Name="Laptop"},
                new() {Name="Desktop"},
                new(){Name="Keyboard"},
                new(){Name="Mouse"},
            };
    }

    [AllowAnonymous]
    [HttpGet("{id}", Name = "GetProducts")] // This api will be called with the name GetProducts
    public async Task<IEnumerable<Product>> GetCommonProducts(int id)
    {
        await Task.Delay(10);
        return new List<Product>
            {
                new (){Name="Computer"},
                new (){Name="Laptop"},
                new (){Name="Desktop"}
            };
    }

    // This API will be accessible only for RequireAdmin policy, and it will be protected with JWT token
    [Authorize(Policy = "RequireAdmin")] // Applying at Action level, we can also apply at Controller level, then all the APIs in that controller will be protected with JWT token and only accessible for Admin role
    [HttpGet("secure-data")]
    public IActionResult GetSecureData()
    {
        return Ok("This is protected data only for Admins. Without valid JWT user cannot access this API");
    }

    // Only User can access
    [Authorize(Policy = "RequireUser")]
    [HttpPost("add-item")]
    public IActionResult AddItem([FromBody] string item)
    {
        // perform add logic
        return Ok($"Item '{item}' added by User.");
    }

    //ASP.NET Core has built-in Policy enforcement via [Authorize] attribute.
    // Shared endpoint for both Admin or Manager
    [Authorize(Policy = "RequireAdminOrManager")]
    [HttpGet("shared-data")]
    public IActionResult GetSharedData()
    {
        return Ok("This data is available to Admins or Manager as per policy defined for RequireAdminOrManager.");
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties { RedirectUri = "/api/auth/google-response" };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync();
        if (!result.Succeeded) return Unauthorized();

        var claims = result.Principal?.Identities.FirstOrDefault()?.Claims
            .Select(c => new { c.Type, c.Value });

        return Ok(claims);
    }
}
