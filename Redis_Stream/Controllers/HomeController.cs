using Microsoft.AspNetCore.Mvc;
using Redis_Stream.EventBus.Events;
using Redis_Stream.EventBus.Services;
using Redis_Stream.Models;
using StackExchange.Redis;
using System.Diagnostics;

namespace Redis_Stream.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventBus _eventBus;

        const string streamName = "telemetry";
        const string groupName = "avg";
        const string consumer = "avg-1";
        const string consumer2 = "avg-2";
        const string consumer3 = "avg-3";
        private readonly IConnectionMultiplexer _redis;

        //private readonly IDatabase db;
        public HomeController(ILogger<HomeController> logger, IConnectionMultiplexer redis, IEventBus eventBus)
        {

            _redis = redis;
            var muxer = ConnectionMultiplexer.Connect("localhost");


            //db = muxer.GetDatabase();
            _logger = logger;
            _eventBus = eventBus;
        }

        public IActionResult Index()
        {
           
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Hello()
        {
    
            var db = _redis.GetDatabase();

            var random = new Random();
            var entities = new NameValueEntry[] { new("temp", random.Next(50, 65)), new NameValueEntry("time", DateTimeOffset.Now.ToUnixTimeSeconds()) };
            //await db.StreamAddAsync(streamName, entities);

            await _eventBus.Publish(new TestEvent() { Name = "phunn" });

            return Ok(new { Success = false });
        }
        [HttpGet]
        public async Task<IActionResult> Hi()
        {

            var db = _redis.GetDatabase();

            var random = new Random();
            var entities = new NameValueEntry[] { new("temp", random.Next(50, 65)), new NameValueEntry("time", DateTimeOffset.Now.ToUnixTimeSeconds()) };
            //await db.StreamAddAsync(streamName, entities);

            await _eventBus.Publish(new Test1Event() { Name = "phunn" ,Email = "phunn@gmail.com", Password = "123456@A"});

            return Ok(new { Success = false });
        }
        Dictionary<string, string> ParseResult(StreamEntry entry) => entry.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        [HttpGet]
        public async Task<IActionResult> GetStreamData()
        {

            var db = _redis.GetDatabase();

            var result = await db.StreamRangeAsync(streamName, "-", "+", null, Order.Ascending);

            return Ok(result.Select(x => x.Id.ToString()));
        }

        // Dictionary<string, string> ParseResult(StreamEntry entry) => entry.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        [HttpGet]
        public async Task<IActionResult> GetStreamDataGroup()
        {
            
            var array = new List<string>();
            Thread t = new Thread(async () =>
            {
                //var muxer = ConnectionMultiplexer.Connect("localhost,abortConnect=false");
                var db = _redis.GetDatabase();
                var result = await db.StreamReadGroupAsync(streamName, groupName, consumer, ">", 1000);
                foreach (var item in result)
                {
                    Console.WriteLine($"{consumer} ----- {item.Id}");
                }
            });
            Thread t1 = new Thread(async () =>
            {
                //var muxer = ConnectionMultiplexer.Connect("localhost,abortConnect=false");
                var db = _redis.GetDatabase();
                var result = await db.StreamReadGroupAsync(streamName, groupName, consumer2, ">", 1000);
                foreach (var item in result)
                {
                    Console.WriteLine($"{consumer2} ----- {item.Id}");
                }
            });
            Thread t3 = new Thread(async () =>
            {
                //var muxer = ConnectionMultiplexer.Connect("localhost,abortConnect=false");
                var db = _redis.GetDatabase();
                var result = await db.StreamReadGroupAsync(streamName, groupName, consumer3, ">", null);
                foreach (var item in result)
                {
                    Console.WriteLine($"{consumer3} ----- {item.Id}");
                }
            });
            t.Start();
            t1.Start();
            t3.Start();

            return Ok(new { nss = 1 });
        }

        [HttpGet]
        public async Task<IActionResult> GetDataPending()
        {
            //var muxer = ConnectionMultiplexer.Connect("localhost");
            var db = _redis.GetDatabase();

            Thread t = new Thread(async () =>
            {

                var result = await db.StreamPendingMessagesAsync(streamName, groupName, 10000, consumer, "-", "+");
                foreach (var item in result)
                {
                    Console.WriteLine($"Pending : {consumer} ----- {item.MessageId.ToString()}");
                }
            });
            Thread t1 = new Thread(async () =>
            {

                var result = await db.StreamPendingMessagesAsync(streamName, groupName, 10000, consumer2, "-", "+");
                foreach (var item in result)
                {
                    Console.WriteLine($"Pending :{consumer2} ----- {item.MessageId.ToString()}");
                }
            });
            Thread t3 = new Thread(async () =>
            {

                var result = await db.StreamPendingMessagesAsync(streamName, groupName, 10000, consumer3, "-", "+");
                foreach (var item in result)
                {
                    Console.WriteLine($"Pending :{consumer3} ----- {item.MessageId.ToString()}");
                }
            });
            t.Start();
            t1.Start();
            t3.Start();

            return Ok("ok");
        }
        [HttpGet]
        public async Task<IActionResult> DoSomething()
        {
           // var muxer = ConnectionMultiplexer.Connect("localhost");
            var db = _redis.GetDatabase();

            var result = await db.StreamReadAsync(streamName, 0);

            return Ok(result);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}