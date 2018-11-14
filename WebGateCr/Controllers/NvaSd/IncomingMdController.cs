using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebGateCr.Models.NvaSd;
using WebGateApi.Base;
using Microsoft.Extensions.Primitives;

namespace WebGateCr.Controllers.NvaSd
{
    [Route("api/NvaSd/Incoming/[controller]")]
    public class MdController : Controller
    {
        private readonly INvaSd _context;
        private EntytyMetadatasHelper mdHelper;

        public MdController(INvaSd dbcontext, EntytyMetadatasHelper _mdHelper)
        {
            _context = dbcontext;
            mdHelper = _mdHelper;
        }

        [HttpGet]
        public object Get()
        {
            //IMetadataBase<object,string > vs = _context.NvaSdIncoming.Metadatas.Metadata as IMetadataBase<object, string>;
            //Metadata va = vs as IMetadata;

            return (mdHelper.GetMetadataByQuery(this.Request.Query, _context.NvaSdIncoming.Metadatas.Metadata));  //todo: Скрытая связанность с статик хелпером !!! (Инжектировать нада!!!!)

            #region old relise
            //object ret = null;  //IEnumerable<KeyValuePair<string, object>>
            //if (this.Request.Query.Keys.Contains(EntytyMetadatasHelper.NamesMetadataParameter))         //todo: Связанность скрытая !!!! надо конфигурить 
            //{
            //    StringValues vals = this.Request.Query[EntytyMetadatasHelper.NamesMetadataParameter];
            //    ret = _context.NvaSdIncoming.Metadatas.Metadata.Get(vals);
            //}
            //else if (this.Request.Query.Keys.Contains(EntytyMetadatasHelper.NameMetadataParameter))    //todo: Связанность скрытая !!!! надо конфигурить 
            //{
            //    StringValues vals = this.Request.Query[EntytyMetadatasHelper.NameMetadataParameter];   //todo: Связанность скрытая !!!! надо конфигурить
            //    if (vals.Count > 0)
            //    {
            //        ret = _context.NvaSdIncoming.Metadatas.Metadata.Find(vals[0]);
            //    }

            //}
            //else
            //{
            //    ret = _context.NvaSdIncoming.Metadatas.Metadata.GetAll();
            //}
            //return ret;//Json(ret);
            #endregion    
        }

        // GET api/values/5
        [HttpGet("{fieldId}")]
        public object Get(string fieldId)  
        {
            object ret = null;
            var mdata = _context.NvaSdIncoming.Metadatas.MetadataField[fieldId];

            if (mdata != null)
            {
                ret = (mdHelper.GetMetadataByQuery(this.Request.Query, mdata));   //todo: Скрытая связанность с статик хелпером !!! (Инжектировать нада!!!!)
            }

            return ret;
        }
    }
}