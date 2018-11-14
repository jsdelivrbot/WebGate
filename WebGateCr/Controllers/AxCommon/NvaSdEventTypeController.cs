using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebGateCr.Models;
using WebGateCr.Models.Data;
using WebGateApi.Base;

namespace WebGateCr.Controllers.AxCommon
{
    [Route("api/Ax/[controller]")]
    public class NvaSdEventTypeController : Controller
    {
        private readonly IAxCommon context;

        public NvaSdEventTypeController(IAxCommon _dbcontext)
        {
            context = _dbcontext;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<NVASDEVENTTYPE>  Get([FromQuery] object prs)
        {

            if (this.Request.Query.Count > 0)
            {
                var pars = ControllerHelper.ToKeyValuePair<NVASDEVENTTYPE>(this.Request);
                //return _context.NvaSdIncoming.Access.Reader.Get(prs);
                return context.NvaSdEventType.Access.Reader.Get(pars);
            }


            return context.NvaSdEventType.Access.Reader.GetAll();
        }

        [HttpGet("{id}")]
        public NVASDEVENTTYPE Get(string id)
        {
            return context.NvaSdEventType.Access.Reader.Find(id);
            
        }
    }

    [Route("api/Ax/NvaSdEventType/[controller]")]
    public class MdController : Controller
    {
        private readonly IAxCommon context;
        private EntytyMetadatasHelper mdHelper;

        public MdController(IAxCommon _dbcontext, EntytyMetadatasHelper _mdHelper)
        {
            context = _dbcontext;
            mdHelper = _mdHelper;
        }


        [HttpGet]
        public object Get()
        {
            //todo: Скрытая связанность с статик хелпером !!! (Инжектировать нада!!!!)
            return (mdHelper.GetMetadataByQuery(this.Request.Query, context.NvaSdEventType.Metadatas.Metadata));
        }

        // GET api/values/5
        [HttpGet("{fieldId}")]
        public object Get(string fieldId)
        {
            object ret = null;
            var mdata = context.NvaSdEventType.Metadatas.MetadataField[fieldId];

            if (mdata != null)
            {
                //todo: Скрытая связанность с статик хелпером !!! (Инжектировать нада!!!!)
                ret = (mdHelper.GetMetadataByQuery(this.Request.Query, mdata));
            }


            return ret;
        }
    }


    //[Route("api/Ax/[controller]")]
    //public class NvaSdEventTypeMdController : Controller
    //{
    //    private readonly IAxCommon context;

    //    public NvaSdEventTypeMdController(IAxCommon _dbcontext)
    //    {
    //        context = _dbcontext;
    //    }


    //    [HttpGet]
    //    public object Get()
    //    {
    //        //todo: Скрытая связанность с статик хелпером !!! (Инжектировать нада!!!!)
    //        return (EntytyMetadatasHelper.GetMetadataByQuery(this.Request.Query, context.NvaSdEventType.Metadatas.Metadata));  
    //    }

    //    // GET api/values/5
    //    [HttpGet("{fieldId}")]
    //    public object Get(string fieldId)
    //    {
    //        object ret = null;
    //        var mdata = context.NvaSdEventType.Metadatas.MetadataField[fieldId];

    //        if (mdata != null)
    //        {
    //            //todo: Скрытая связанность с статик хелпером !!! (Инжектировать нада!!!!)
    //            ret = (EntytyMetadatasHelper.GetMetadataByQuery(this.Request.Query, mdata));
    //        }
            

    //        return ret;
    //    }
    //}
}
