using IdentityApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiCoreController : ControllerBase
    {
        private DataContext _dataContext;
        private IConfiguration _config;
        protected IConfiguration Configuration => _config ??= HttpContext.RequestServices.GetService<IConfiguration>();
        protected DataContext DataContext => _dataContext ??= HttpContext.RequestServices.GetService<DataContext>();
    }
}
