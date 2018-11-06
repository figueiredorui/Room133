using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Room133.Context;
using Microsoft.AspNetCore.Mvc;
using Room133.Models;

namespace Room133.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        readonly LogContext ctx;
        public LogsController()
        {
            ctx = new LogContext();
            
        }

        // GET api/logs
        [HttpGet]
        public ActionResult<IEnumerable<Log>> Get()
        {
            try
            {
                var result = ctx.Logs
                            .Where(q=>q.Date > DateTime.Now.AddDays(-2))
                            .OrderByDescending(q=>q.Date).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("history/{daysBack?}")]
        public ActionResult<IEnumerable<LogHistory>> GetHistory(int? daysBack)
        {
            try
            {
                if (daysBack.HasValue == false)
                    daysBack = 0;

                var date = DateTime.Now.AddDays(-daysBack.Value).Date;

                var result = (from log in ctx.Logs
                              where log.Date.Date == date
                              where (log.Date.Hour >= 7)
                                && log.Date.Hour <= 19

                              group log by new { Day = log.Date.Day, Hour = log.Date.Hour, Minute = (log.Date.Minute / 15) * 15 } into grp
                              select new LogHistory()
                              {
                                  PointInTime = $"{grp.Key.Day}@{grp.Key.Hour}h{grp.Key.Minute}",
                                  Temperature = Math.Round(grp.Average(a => a.Temperature), 1)
                              }).ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // POST api/logs
        [HttpPost]
        public ActionResult Post([FromBody] Log log)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                ctx.Logs.Add(log);
                int r = ctx.SaveChanges();

                return Ok(log);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/logs/notify
        [HttpPost("notify")]
        public ActionResult PostNotification([FromBody] NotifyMessage msg)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var json = GetMessageCard(msg.Temp);

                var url = "https://outlook.office.com/webhook/219ac914-90f3-47dc-a870-05912fd508ea@281bddd1-3d7b-4d7a-9e9d-a8fb045161df/IncomingWebhook/31c7716bbee343f1a6f95f741f122462/aa59c73d-3de0-4b71-b5f1-2d681b90aa52";
                var client = new HttpClient();

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = client.PostAsync(url, content).Result;

                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GetMessageCard(decimal temp)
        {
            //https://messagecardplayground.azurewebsites.net/
            var str = new StringBuilder();
            str.Append("{");
            str.Append("'@context': 'https://schema.org/extensions',");
            str.Append("'@type': 'MessageCard',");
            str.Append("'themeColor': 'fff000',");
            str.Append($"'title': 'Current Room Temperature: {temp}',");
            str.Append("'text': 'Click **View More** to view the temperature chart',");
            str.Append("'potentialAction': [");
            str.Append("{");
            str.Append("'@type': 'OpenUri',");
            str.Append("'name': 'View More',");
            str.Append("'targets': [");
            str.Append("{");
            str.Append("'os': 'default',");
            str.Append("'uri': 'https://room133.azurewebsites.net'");
            str.Append("}");
            str.Append(" ]");
            str.Append(" }");
            str.Append("]");
            str.Append("}");
            return str.ToString();
        }

        /*
          var s = string.Format(@" {
                                '@context': 'https://schema.org/extensions',
                                '@type': 'MessageCard',
                                'themeColor': 'fff000',
                                'title': 'Current Room Temperature: {0}',
                                'text': 'Click **View More** to view the temperature chart',
                                'potentialAction': [
                                    {
                                        '@type': 'OpenUri',
                                        'name': 'View More',
                                        'targets': [
                                            {
                                                'os': 'default',
                                                'uri': 'http://dht22.azurewebsites.net/api/logs'
                                            }
                                        ]
                                    }
                                ]
                            }", temp);
         */

    }
}
