using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGate.Models;
using WebGate.Models.Logic;

namespace WebGate.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        IAxExtService Context { get; }

        public TestController(IAxExtService context  ) : base()
        {
            Context = context;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<object> Get()
        {
            //var a = Context.NVAOMAAIRCRAFTTYPE.GetAll();

            ////var a = Context.WGD_Test.GetAll();
            //foreach (var n in a)
            //{
            //    var s = n.FLYTYPE;                    
            //}

            //return a;
            //return Context.WGD_Test.GetAll();

            return Context.WGD_Test.Reader.GetAll();

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public object Get(int id)
        {
            //var a = Context.NVAOMAAIRCRAFTTYPE.GetAll();
            //return Context.WGD_Test.Find(id);
            return Context.WGD_Test.Reader.Find(id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]WGD_Test value)
        {
            Context.WGD_Test.Inserter.Add(value);
            Context.Save();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]WGD_Test value)
        {
            value.id = id;
            Context.WGD_Test.Updater.Update(value);
            Context.Save();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            Context.WGD_Test.Remover.Remove(id);
            Context.Save();
        }
    }
}
