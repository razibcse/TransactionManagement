using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TransactionManagement.Service
{
    public class UserService : IUserService
    {
        private IHttpContextAccessor _httpContext;

        public UserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public string GetUserId()
        {
            return _httpContext.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        public string FullName()
        {
            return _httpContext.HttpContext.User?.FindFirst(ClaimTypes.Name).Value;
        }

         public string Email()
        {
            return _httpContext.HttpContext.User?.FindFirst(ClaimTypes.Name).Value;
        }

        public bool isAuthenticated()
        {
            return _httpContext.HttpContext.User.Identity.IsAuthenticated;
        }

       
    }
}
