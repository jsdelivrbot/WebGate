using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebGateCr.Models.NvaSd;
using WebGateCr.Models.Data;
using WebGateApi.Base;
using System.Web.Http.Cors;
using System.IO;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebGateCr.Controllers.NvaSd
{
    
    [Route("api/NvaSd/[controller]")]
    public class IncomingController : Controller
    {
        private readonly INvaSd _context;
        private readonly IHttpErrorBuilder _errorBuilder;

        public IncomingController(INvaSd dbcontext, IHttpErrorBuilder errorBuilder )
        {
            _context = dbcontext;
            _errorBuilder = errorBuilder;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<NVASD_Incoming> Get([FromQuery]IntervalParameters parameters)
        {
            //this.Response.StatusCode

            if (this.Request.Query.Count > 0)
            {
                var pars = ControllerHelper.ToKeyValuePair<IntervalParameters>(this.Request);
                return _context.NvaSdIncoming.Access.Reader.Get(pars);
            }

            return _context.NvaSdIncoming.Access.Reader.GetAll();
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public NVASD_Incoming Get(long id)
        {
            return _context.NvaSdIncoming.Access.Reader.Find(id);
        }

        //POST api/values
        [HttpPost]
        public object Post([FromBody] NVASD_Incoming value)
        {

            ICommonError err = _errorBuilder.Success("");
            object ret ;

            if (value != null)
            {
                _context.NvaSdIncoming.Access.Inserter.Add(value);
                var rt = _context.Save();

                if (rt != null && rt != "")
                {
                    err = _errorBuilder.ClientError(rt);
                    err.Name = "Unprocessable Entity";
                    err.StatusCode = 422;
                    _errorBuilder.EerectTo(err, this.Response);
                    ret = err;
                }
                else
                {
                    ret =  _context.NvaSdIncoming.Access.Reader.Find(value.ID);
                }

            }
            else
            {
                //this.Response.Body.Position = 0;
                var s = "";
                if (this.Request.Body.Length > 0)
                {
                    this.Request.Body.Position = 0;
                    s = new StreamReader(this.Request.Body, Encoding.UTF8).ReadToEnd();
                }

                err = _errorBuilder.ClientError("Bad Request");
                err.StatusCode = 400;

                err.Message = string.IsNullOrEmpty(s) ?  "Incoming data is empty" : "Incoming data is bad: ["+s+"]" ;
                _errorBuilder.EerectTo(err, this.Response);
                ret = err;
            }

            return ret;
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
