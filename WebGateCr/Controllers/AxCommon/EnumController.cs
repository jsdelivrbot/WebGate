using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebGateCr.Models.Data;
using WebGateCr.Models;

namespace WebGateCr.Controllers.AxCommon
{
    [Route("api/Ax/[controller]")]

    public class EnumController : Controller
    {
        private readonly IAxCommon context;

        public EnumController(IAxCommon _dbcontext)
        {
            context = _dbcontext;
        }

        //GET api/values/5
        [HttpGet("{enumName}")]//IEnumerable<AxEnum>
        [HttpGet("{enumName}/Md")]//IEnumerable<AxEnum>
        public object Get(String enumName)
        {

            var a = this.HttpContext.Request.Path.ToUriComponent();

            if (a.Substring(a.Length - 3) == "/Md" 
                && context.GetEnumByName( enumName ).Access.Reader != null  )
            {
                return context.GetEnumByName(enumName).Metadatas.Metadata.GetAll(); ;
            }
            return context.GetEnumByName(enumName).Access.Reader.GetAll();
        }

        [HttpGet("{enumName}/{id}")]//IEnumerable<AxEnum>
        public object Get(String enumName, long id)
        {
            return context.GetEnumByName(enumName).Access.Reader.Find(id);
        }


    }

}