using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MemoryCache.BackgroundAccess.Controllers
{
    [Route("api/[controller]")]
    public class MemoryCacheController : Controller
    {
        private readonly IMemoryCache memoryCache;

        public MemoryCacheController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public IActionResult AccessCache()
        {
            if (!this.memoryCache.TryGetValue("x", out var message))
            {
                this.memoryCache.CreateEntry("x");
                this.memoryCache.Set("x", new Message {Text = "I am the message!"});
            }

            Task.Run(async () =>
            {
                await Task.Delay(5000);
                var innerMessage = this.memoryCache.Get<Message>("x");
                Console.WriteLine(innerMessage.Text);
            });
            
            return new OkObjectResult(message);
        }
    }

    public class Message
    {
        public string Text { get; set; }
    }
}