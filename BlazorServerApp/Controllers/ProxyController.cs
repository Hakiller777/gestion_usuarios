using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BlazorServerApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProxyController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProxyController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            // Agregamos el token JWT al header Authorization
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI3ZjZmY2JlZi03MjQ0LTRhM2EtODg5Ny00ZWNlOTE3NWQ0ZTkiLCJlbWFpbCI6Imh1Z28xNGVja2VydDE0QGdtYWlsLmNvbSIsImp0aSI6ImY1N2VlN2UwLTNhZTEtNDc3YS04OGI3LTVkMzFjMWY5OWU2YiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiN2Y2ZmNiZWYtNzI0NC00YTNhLTg4OTctNGVjZTkxNzVkNGU5IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6Imh1Z28xNGVja2VydDE0QGdtYWlsLmNvbSIsImV4cCI6MTc2MzA2NDE2MCwiaXNzIjoiR2VzdGlvblVzdWFyaW9zIiwiYXVkIjoiR2VzdGlvblVzdWFyaW9zQ2xpZW50In0.m9JMnDBpG6iNH4Z7t7n1rgmVPzKx9CChxSL3CmVBmKo");

            // Hacemos GET al endpoint real
            var response = await client.GetAsync("/api/user");
            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                message = "Intermediario funcionando 🚀",
                backendResponse = content
            });
        }
    }
}
