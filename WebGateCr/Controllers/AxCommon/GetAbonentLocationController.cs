using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebGateCr.Models;
using WebGateCr.Models.Data;
using WebGateApi.Base;
using WebGateCr.Models.AxaptaModel.ATS;

namespace WebGateCr.Controllers.AxCommon
{
    [Route("[controller]")]
    public class GetAbonentLocationController : Controller
    {
        private readonly IAxCommon context;
        private readonly Resp112Helper  atsHelper;

        public GetAbonentLocationController(IAxCommon _dbcontext, Resp112Helper _atsHelper)
        {
            context = _dbcontext;
            atsHelper = _atsHelper;
        }


        [HttpGet]
        public object Get([FromQuery] object prs)
        {
            //if (this.Request.Query.Count > 0)
            //{
                //var pars = ControllerHelper.ToKeyValuePair<NVAATS_PHONES>(this.Request);
                //return context.NvaAtsPhones.Access.Reader.Get(pars);
                var pars = ControllerHelper.ToEntyty<NVAATS_PARAMS>(this.Request);
                return atsHelper.BuildResponse(pars, context);

            //}

            //return context.NvaAtsPhones.Access.Reader.GetAll();
        }
    }
}