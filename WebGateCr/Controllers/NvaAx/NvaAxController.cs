using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebGateCr.Models;
using WebGateApi.Base;
using Microsoft.AspNetCore.Http;

/// <summary>
/// 09112018 ReadOnly Axapta reflection controller
/// </summary>

namespace WebGateCr.Controllers.NvaAx
{


    [Route("api/[controller]")]
    public class NvaAxController : Controller
    {
        const string MetaSfx = "Md";
        enum RequestType { Data, DataFiltered, MetadataTable, MetadataField };

        private readonly IFlightBoard context;
        private readonly IHttpErrorBuilder errorBuilder;
        private readonly DataContextHelper contextHelper;
        private readonly EntytyMetadatasHelper mdHelper;

        // 
        public NvaAxController(IFlightBoard dbcontext, IHttpErrorBuilder _errorBuilder, DataContextHelper _contextHelper, EntytyMetadatasHelper _mdHelper)
        {
            context = dbcontext;
            errorBuilder = _errorBuilder;
            contextHelper = _contextHelper;
            mdHelper = _mdHelper;
        }

        [HttpGet("{subPath}")]//IEnumerable<AxEnum>                                                     
        [HttpGet("{subPath}/" + MetaSfx)]//IEnumerable<AxEnum>
        [HttpGet("{subPath}/" + MetaSfx + "/{fieldId}")]//IEnumerable<AxEnum>
        public object Get(String subPath, String fieldId, [FromQuery]IntervalParameters parameters)
        {
            var errMsg = @"Resource [" + subPath + "] not found...";

            bool IfLastIs(string s, string t) => (s.Substring(s.Length - t.Length) == t);
            bool notNull(object x) => (x != null);

            RequestType RequestRoute(HttpRequest req, string fld) =>
                IfLastIs(req.Path.ToUriComponent(), "/" + MetaSfx) ?
                    RequestType.MetadataTable :
                        (fld != null ?
                            RequestType.MetadataField :
                            (req.Query.Count > 0 ? RequestType.DataFiltered : RequestType.Data)
                        );
            try
            {
                switch (RequestRoute(this.Request, fieldId))
                {
                    case RequestType.MetadataTable:
                        return
                            Resp<object>
                                .Of(contextHelper.GetMetadata(context, subPath))
                                .MapIf(x => x as IMetadataBase<object, string>, notNull, e => errorBuilder.ServerError(errMsg))
                                .Map(x => mdHelper.GetMetadataByQuery(this.Request.Query, x))
                                .Value;

                    case RequestType.MetadataField:
                        return
                            Resp<object>
                                .Of(contextHelper.GetMetadataField(context, subPath, fieldId))
                                .MapIf(x => x as IMetadataBase<object, string>, notNull, e => errorBuilder.ServerError(errMsg))
                                .Map(x => mdHelper.GetMetadataByQuery(this.Request.Query, x))
                                .Value;

                    case RequestType.DataFiltered:
                        return
                            Resp<HttpRequest>
                                .Of(this.Request)
                                .Map(x => new object[] { contextHelper.ToKeyValuePairFromRequest(x) })
                                .MapIf(x => contextHelper.InvokeReaderMethod(context, subPath, "Get", x), notNull, e => errorBuilder.ServerError(errMsg))
                                .Value;

                    case RequestType.Data:
                        return
                            Resp<string>
                                .Of(subPath)
                                .MapIf(x => contextHelper.InvokeReaderMethod(context, x, "GetAll"), notNull, e => errorBuilder.ServerError(errMsg))
                                .Value;

                    default:
                        return errorBuilder.ServerError(errMsg);
                }

            }
            catch (Exception e)
            {
                return errorBuilder.ServerError(e.Message);
            }

        }

        // моноид для построения ответа сервиса 
        class Resp<T>
        {
            readonly ICommonError resp;
            readonly T val;
            public ICommonError Responce { get { return HasResponse ? resp : throw new InvalidOperationException(); } }
            public object Value { get { return HasResponse ? resp as object : val as object; } }

            public bool HasResponse => (resp != null);

            private Resp(T value) { val = value; }
            private Resp(ICommonError responce) { resp = responce; }

            public static Resp<T> Of(T value) => new Resp<T>(value);
            public static Resp<T> End(ICommonError value) => new Resp<T>(value);
            public Resp<U> Map<U>(Func<T, U> f) => HasResponse ? new Resp<U>(resp) : new Resp<U>(f(val));
            public Resp<T> If(Func<T, bool> check, Func<T, ICommonError> toResp) => HasResponse || check(val) ? this : Resp<T>.End(toResp(val));
            public Resp<U> MapIf<U>(Func<T, U> f, Func<U, bool> check, Func<U, ICommonError> rsp) => Map(f).If(check, rsp);
        }

    }
}