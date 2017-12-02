using Microsoft.AspNetCore.Mvc;
using app.Caching;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;

namespace app.Controllers
{
    public class HomeController : Controller
    {
        public RedisCacheProvider _cacheProvider;

        public HomeController(RedisCacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TestSetRedis()
        {
            _cacheProvider.Set<string>("Hello", "World");
            return Content("Redis item is added!");
        }

        public IActionResult TestGetRedis()
        {
            var result = _cacheProvider.Get<string>("Hello");
            return Content(result ?? "Redis item is not found!");
        }

        public IActionResult SetSession()
        {
            if (HttpContext.Session.IsAvailable)
            {
                HttpContext.Session.SetString("Username", "@ustasoglu");
                ViewData["InnerHtml"] = GetHeaders();
            }
            else
            {
                ViewData["InnerHtml"] = "Session is not available yet!";
            }
            return View();
        }

        public IActionResult GetSession()
        {
            var response = new StringBuilder();
            response.Append("<br />");
            response.Append($"HttpContext.Session.IsAvailable -> {HttpContext.Session.IsAvailable}");
            response.Append("<br />");
            response.Append($"HttpContext.Session.Keys.Count() -> {HttpContext.Session.Keys.Count()}");
            response.Append("<br />");
            response.Append($"{HttpContext.Session.Id} -> {HttpContext.Session.GetString("Username")}");
            response.Append("<hr />");
            response.Append(GetHeaders());
            ViewData["InnerHtml"] = response.ToString();
            return View();
        }

        private string GetHeaders()
        {
            var response = new StringBuilder();
            response.Append($"BACKEND-SERVER -> {GetCustomServerName()}");
            response.Append("<br />");
            foreach (var item in HttpContext.Request.Headers)
            {
                response.Append($"{item.Key} > {item.Value}<br />");
            }
            return response.ToString();
        }

        public static string GetCustomServerName()
        {
            var envValue = Environment.GetEnvironmentVariable("X_BACKEND_SERVER");
            if (!string.IsNullOrWhiteSpace(envValue))
            {
                return envValue;
            }
            return string.Empty;
        }
    }
}
