using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebGateCr.Models.NvaSd;
using WebGateApi.Base;
using Microsoft.AspNetCore.Http;

namespace WebGateCr.Controllers.NvaSd2
{

    [Route("api/[controller]")]
    public class NvaSd2Controller : Controller
    {
        private readonly INvaSd context;
        private readonly IHttpErrorBuilder errorBuilder;
        private readonly DataContextHelper contextHelper;
        private EntytyMetadatasHelper mdHelper;

        public NvaSd2Controller(INvaSd _dbcontext, IHttpErrorBuilder _errorBuilder, DataContextHelper _contextHelper, EntytyMetadatasHelper _mdHelper)
        {
            context         = _dbcontext;
            errorBuilder    = _errorBuilder;
            contextHelper   = _contextHelper;
            mdHelper        = _mdHelper;
        }

        [HttpGet("{subPath}")]//IEnumerable<AxEnum>
        [HttpGet("{subPath}/Md")]//IEnumerable<AxEnum>
        [HttpGet("{subPath}/Md/{fieldId}")]//IEnumerable<AxEnum>
        public object Get(String subPath, String fieldId, [FromQuery]IntervalParameters parameters)
        {
            ICommonError err = errorBuilder.Success("");
            object ret;

            var uri = this.HttpContext.Request.Path.ToUriComponent();

            if (uri.Substring(uri.Length - 3) == "/Md")
            {
                var md = contextHelper.GetMetadata(context, subPath) as IMetadataBase<object, string>;
                ret = (md != null) ? mdHelper.GetMetadataByQuery(this.Request.Query, md) : this.BadReqErr("Resource [" + subPath + "] not found");
            }
            else if (fieldId != null)
            {
                var md = contextHelper.GetMetadataField(context, subPath, fieldId) as IMetadataBase<object, string>;
                ret = (md != null) ? mdHelper.GetMetadataByQuery(this.Request.Query, md) : this.BadReqErr("Resource [" + subPath + "] not found");
            }
            else
            {
                if (this.Request.Query.Count > 0)
                {
                    object[] par = { contextHelper.ToKeyValuePairFromRequest(this.Request) };
                    var resp = contextHelper.InvokeReaderMethod(context, subPath, "Get", par);
                    ret = resp ?? this.BadReqErr("Resource [" + subPath + "] not found");
                }
                else
                {
                    var resp = contextHelper.InvokeReaderMethod(context, subPath, "GetAll");
                    ret = resp ?? this.BadReqErr("Resource [" + subPath + "] not found");
                }
            }
            return ret;
        }



        // GET api/values/5
        [HttpGet("{subPath}/{id}")]
        public object Get(String subPath, long id)
        {
            object ret;
            object[] par = { id };

            var resp = contextHelper.InvokeReaderMethod(context, subPath, "Find", par);
            ret = resp ?? this.BadReqErr(
                contextHelper.ExistIDal(context, subPath) ?
                "Key [" + id + "] of [" + subPath + "] not found" :
                "Resource [" + subPath + "] not found"
                );
            return ret;
        }


        // Build Error
        private ICommonError BadReqErr(string msg)
        {
            ICommonError err = errorBuilder.ClientError("Bad Request: " + msg);
            err.StatusCode = 400;
            //err.Message = msg;
            errorBuilder.EerectTo(err, this.Response);
            return err;
        }

        //New record template concept by GET special path 

        [HttpGet("{subPath}/!Template")]
        public object Get(String subPath)
        {
            return contextHelper
                    .InvokeInserterMethod(context, subPath, "GetTemplate") 
                     ?? this.BadReqErr("Resource [" + subPath + "] not found");
        }


        [HttpPost("{subPath}")]
        public ICommonError Create(String subPath , [FromBody] object content)
        {
            #region ебля
            //Func<object, Func<object, object>, object> step = (x, f) => (x as ICommonError) != null ? f(x) : x;
            //Func<object, Func<object, ICommonError>, ICommonError, ICommonError> isNull = (x, ifCheckFunc, notCheck) => (x != null) ? ifCheckFunc(x) : notCheck;
            //Func<object, Func<object, object>, Func<string, object>, object> tryCatch = (x, execf, errMsgFunc) => { try { return execf(x); } catch (Exception e) { return errMsgFunc(e.Message); } };

            //return
            //   isNull(
            //        contextHelper.ToEntytyType(context, subPath, content)
            //        , x => step( x
            //            ,y => tryCatch(y
            //                    ,z => contextHelper.InvokeInserterMethod(context, subPath, "Add", new object[] { z } )
            //                    ,m => errorBuilder.ServerError(m)
            //                )
            //            ) as ICommonError
            //       ,errorBuilder.ClientError("Resource [" + subPath + "] not found or incoming data [" + content.ToString() + "] is bad...")
            //   );



            //Func<object, Func<object, ICommonError>, ICommonError, ICommonError> isNull = (x, ifCheckFunc, notCheck ) => (x != null) ? ifCheckFunc(x) : notCheck;
            //Func<object, (object,ICommonError)> insertRecord =
            //    (x => {
            //        try 

            //        return x;
            //    });

            //return 
            //    isNull(
            //        contextHelper.ToEntytyType(context, subPath, content)
            //        , x => {
            //            try
            //            {

            //            }
            //            catch (Exception e) { errorBuilder.ServerError(e.Message); }

            //        }
            //        ,errorBuilder.ClientError("Resource [" + subPath + "] not found or incoming data [" + content.ToString() + "] is bad...")   
            //    )

            #endregion

            #region legasy release
            //ICommonError err;
            //var row = contextHelper.ToEntytyType(context, subPath, content);

            //if (row != null)
            //{
            //    try
            //    {
            //        var s = contextHelper.InvokeInserterMethod(context, subPath, "Add", new object[] { row });
            //        var msg = context.Save();
            //        if (msg == null)
            //        {
            //            var id = s
            //                .GetType()
            //                .GetProperty(mdHelper.getIdName(s.GetType()))
            //                .GetValue(s, null);

            //            var headers = new HeaderDictionary() {
            //                    { "Location", (this.Request.Path.Value + "/" + id.ToString())  },
            //                    { "id", ( id.ToString() )  }
            //                };

            //            err = errorBuilder.SuccessPost(
            //                @"Ресурс был успешно создан в ответ на POST-запрос. Заголовок Location содержит URL, указывающий на только что созданный ресурс. Заголовок Id содержит его идентификатор."
            //                , headers
            //            );

            //        }
            //        else
            //        {
            //            err = errorBuilder.ServerError(msg);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        err = errorBuilder.ServerError(e.Message);
            //    }
            //}
            //else
            //{ // клиент error
            //    err = errorBuilder.ClientError("Resource [" + subPath + "] not found or incoming data [" + content.ToString() + "] is bad...");
            //}

            //errorBuilder.EerectTo(err, this.Response);
            //return err;
            #endregion

            return CreateAlt2(subPath, content);
        }

        #region Untyped functor release
        private ICommonError CreateAlt(String subPath, object content)
        {
            object isCheck(object x, Func<object, bool> isCh, Func<object, object> f, Func<object, object> fErr) => isCh(x) ? f(x) : fErr(x);
            object isErr(object x, Func<object, object> f) => isCheck(x, y => (x as ICommonError) == null, f, z => z);
            (object r, string m)? toTupl(object x) => (x as (object r, string m)?);  
            var errMsg = @"Resource [" + subPath + "] not found or incoming data [" + content.ToString() + "] is bad...";
            var sucMsg = @"Ресурс был успешно создан в ответ на POST-запрос. Заголовок Location содержит URL, указывающий на только что созданный ресурс. Заголовок Id содержит его идентификатор.";

            try
            {
                return
                    new Foo<object>(contextHelper.ToEntytyType(context, subPath, content))
                        .Map(x => isCheck(x, (y => y != null), (v => v), (e => errMsg)))
                        .Map(x => isErr(x, y => contextHelper.InvokeInserterMethod(context, subPath, "Add", new object[] { y })))
                        .Map(x => isErr(x, y => (x, context.Save())))
                        //.Map(x=> new Fo(1))
                        .Map(x => isErr(x,
                                         y => isCheck(y,
                                                       z => toTupl(z) != null && toTupl(z).Value.m == null,
                                                       r => toTupl(r).Value.r,
                                                       e => errorBuilder.ServerError(toTupl(e).Value.m)
                                             )))
                        .Map(x => isErr(x, y =>
                                          y.GetType()
                                           .GetProperty(mdHelper.getIdName(y.GetType()))
                                           .GetValue(y, null)))
                        .Map(x => isErr(x, y =>
                                           errorBuilder.SuccessPost(
                                               sucMsg,
                                               new HeaderDictionary() {
                                                   { "Location", (this.Request.Path.Value + "/" + y.ToString())  },
                                                   { "id", ( y.ToString() )  }
                                               }
                                           )
                                   ))
                        .Val as ICommonError;
            }
            catch (Exception e)
            {
                return errorBuilder.ServerError(e.Message);
            }
        }
        #endregion

        private ICommonError CreateAlt2(String subPath, object content)
        {
            var errMsg = @"Resource [" + subPath + "] not found or incoming data [" + content.ToString() + "] is bad...";
            var sucMsg = @"Ресурс был успешно создан в ответ на POST-запрос. Заголовок Location содержит URL, указывающий на только что созданный ресурс. Заголовок Id содержит его идентификатор.";
            bool notNull(object x) => (x != null);

            try
            {
                return 
                    Resp<object>.Of(content)
                        .MapIf(x => contextHelper.ToEntytyType(context, subPath, x), notNull, e => errorBuilder.ServerError(errMsg))
                        .Map(x => contextHelper.InvokeInserterMethod(context, subPath, "Add", new object[] { x }))
                        .MapIf(x => (x, context.Save()), y => (y.Item1 != null && y.Item2 == null), e => errorBuilder.ServerError(e.Item2))
                        .Map(x => x.Item1)
                        .Map(x => x.GetType()
                                     .GetProperty(mdHelper.getIdName(x.GetType()))
                                     .GetValue(x, null))
                        .If((x => false), e => errorBuilder.SuccessPost(
                                                        sucMsg,
                                                        new HeaderDictionary() {
                                                               { "Location", (this.Request.Path.Value + "/" + e.ToString())  },
                                                               { "id", ( e.ToString() )  }
                                                        }))
                        .Responce;                                                                
            }
            catch (Exception e)
            {
                return errorBuilder.ServerError(e.Message);
            }
        }

    }

    class Fo {
        object Val { get; }
        public Fo(object val) { Val = val; }
        public Fo Map(Func<object, object> f) => new Fo( f(this.Val));
        //public object Ret() => this.Val;
    }

    class Foo<T> {
        public T Val { get; }
        public Foo(T val) { Val = val; }
        public Foo<U> Map<U>(Func<T, U> f) => new Foo<U>(f(this.Val));

    }


    // моноид для построения ответа сервиса 
    class Resp<T>
    {
        readonly ICommonError resp;
        readonly T val;
        public ICommonError Responce { get { return HasResponse ? resp : throw new InvalidOperationException(); }  }
        public bool HasResponse => (resp != null); 
        private Resp(T value) { val = value; }
        private Resp(ICommonError responce) { resp = responce;}
        public static Resp<T> Of(T value) => new Resp<T>(value);
        public static Resp<T> End(ICommonError value) => new Resp<T>(value);
        public Resp<U> Map<U> (Func<T, U> f) => HasResponse ? new Resp<U>(resp) : new Resp<U>( f(val)) ;
        //public Resp<U> Bind<U>(Func<T, Resp<U>> f) => HasResponse ? new Resp<U>(resp) : f(val);
        public Resp<T> If (Func<T, bool> check , Func<T, ICommonError> toResp ) => HasResponse || check(val) ? this : Resp<T>.End(toResp(val)) ;
        public Resp<U> MapIf<U>(Func<T, U> f, Func<U, bool> check, Func<U, ICommonError> rsp) => Map(f).If(check, rsp);
    } 

}