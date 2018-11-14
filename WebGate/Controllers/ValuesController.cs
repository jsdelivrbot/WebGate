using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebGate.Models;
using WebGate.Bus;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using WebGate.Services;
using WebGate.Services.Data;
using Microsoft.Extensions.Logging;

namespace WebGate.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly AxExtentionContext _context;
        ILogger log;

        private int val;

        public ValuesController(AxExtentionContext prov, IConfiguration cnf, ILogger lg)//AxExtentionContext context, IContextOptionService optServ , IBus context3)//, IConfigurationManager configuration)
        {
            var qprov =  prov;
            log = lg;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {

            //log.LogTrace()
            return new string[] { "value1", "value2", val.ToString() };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
