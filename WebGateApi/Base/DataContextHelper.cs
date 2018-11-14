using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace WebGateApi.Base
{



    public class DataContextHelper
    {

        /// <summary>
        /// Check is object contain IDAL-object by name 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="idalName"></param>
        /// <returns></returns>
        public bool ExistIDal(object source, string idalName)
        {
            return source.GetType().GetProperties()
                .Any(x=> 
                    x.Name.ToUpper().Trim() == idalName.ToUpper().Trim()
                    && x.GetValue(source).GetType().GetInterfaces()
                        .Any( i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDal<>) )
                ); 
        }

        /// <summary>
        /// Return instance of Dal
        /// </summary>
        /// <param name="source"></param>
        /// <param name="idalName"></param>
        /// <returns></returns>
        public object GetInstance(object source, string idalName)
        {
            return this.ExistIDal(source, idalName) ?
                source.GetType().GetProperties()
                    .First(x => x.Name.ToUpper().Trim() == idalName.ToUpper().Trim())
                    .GetValue(source)
                : null;
        }

        /// <summary>
        ///  Run Reader method by reflection 
        /// </summary>
        /// <param name="mthName"></param>
        /// <param name="pars"></param>
        public object InvokeReaderMethod(object source, string idalName, string mthName, object[] pars = null  )
        {
            var inst = this.GetInstance(source, idalName);
            var accInst = inst?.GetType().GetProperty("Access").GetValue(inst);
            var rdrInst = accInst?.GetType().GetProperty("Reader").GetValue(accInst);
            var rdrInstMth = rdrInst?.GetType().GetMethod(mthName);

            return rdrInstMth?.Invoke(rdrInst, pars);
        }

        /// <summary>
        ///  Run Inserter method by reflection 
        /// </summary>
        /// <param name="mthName"></param>
        /// <param name="pars"></param>
        public object InvokeInserterMethod(object source, string idalName, string mthName, object[] pars = null)
        {
            var inst = this.GetInstance(source, idalName);
            var accInst = inst?.GetType().GetProperty("Access").GetValue(inst);
            var insrInst = accInst?.GetType().GetProperty("Inserter").GetValue(accInst);
            var insrInstMth = insrInst?.GetType().GetMethod(mthName); 

            return insrInstMth?.Invoke(insrInst, pars);
        }


        /// <summary>
        /// Convert incoming Json object to entyty type
        /// </summary>
        public object ToEntytyType(object source, string idalName, object entytyInstance)
        {
            Func<Type, Func<Type, bool>, Func<Type, Object>, Object> checkAndApl = (x, chf, aplf) => chf(x) ? aplf(x) : null;

            return  checkAndApl(
                        checkAndApl(                                          // get entyty type                              
                            this.GetInstance(source, idalName)?.GetType()
                            , x => x.IsGenericType
                            , x => x.GetGenericArguments()?[0]
                        ) as Type
                        , x => x != null
                        , (Type x) => (entytyInstance as Newtonsoft.Json.Linq.JObject)?.ToObject(x));
            
            //release v1 
            //Func<Type, Func<Type, bool>, Type> check = (x, chf) => chf(x) ? x : null;
            //var a = check(this.GetInstance(source, idalName)?.GetType(), x => x.IsGenericType)
            //            .GetGenericArguments()?[0];
            //return a ?? (entytyInstance as Newtonsoft.Json.Linq.JObject) ?.ToObject(a);

            //release legasy
            //var ret = null;
            //var inst = this.GetInstance(source, idalName);
            //Type type = inst.GetType();
            //if(type.IsGenericType)
            //{
            //    Type itemType = type.GetGenericArguments()[0];
            //    var json = entytyInstance as Newtonsoft.Json.Linq.JObject;
            //    var ret = json.ToObject(itemType);
            //}
            //return ret;
        }


        public object GetMetadata(object source, string idalName)
        {
            var inst = this.GetInstance(source, idalName);
            var accInst = inst?.GetType().GetProperty("Metadatas").GetValue(inst);
            var rdrInst = accInst?.GetType().GetProperty("Metadata").GetValue(accInst);
            return rdrInst;
        }

        public object GetMetadataField(object source, string idalName, string fieldId)
        {

            var inst = this.GetInstance(source, idalName);
            var accInst = inst?.GetType().GetProperty("Metadatas").GetValue(inst);
            var rdrInst = accInst?.GetType().GetProperty("MetadataField").GetValue(accInst) as IMetadataBaseIdx<object, string, string>;
            var ret = rdrInst?[fieldId];

            return ret;
        }

        //************************************************************************************************************************************
        // Функционал  ControllerHelper-а   
        #region ControllerHelper
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, object>> ToKeyValuePair<TEntity>(TEntity source) where TEntity : class
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            TEntity df = (TEntity)Activator.CreateInstance(typeof(TEntity));

            if (df != null)
            {
                foreach (var p in typeof(TEntity).GetProperties())
                {
                    var pInf = typeof(TEntity).GetProperty(p.Name);

                    if (pInf.GetValue(df) != pInf.GetValue(source))
                    {
                        ret.Add(p.Name, pInf.GetValue(source));
                    }
                }
            }
            return ret;
        }

        public IEnumerable<KeyValuePair<string, object>> AddKeyValuePair(IEnumerable<KeyValuePair<string, object>> source, KeyValuePair<string, object> val)
        {
            var l = new List<KeyValuePair<string, object>>();
            foreach (var a in source)
            {
                l.Add(a);
            }
            l.Add(val);
            return l;
        }

        /// <summary>
        ///  ВЫДЕЛЯЕТ ПАРАМЕТРЫ ПО МАСКЕ ТИПА
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public  IEnumerable<KeyValuePair<string, object>> ToKeyValuePair<TEntity>(HttpRequest source) where TEntity : class
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            foreach (var p in typeof(TEntity).GetProperties())
            {
                var pInf = typeof(TEntity).GetProperty(p.Name);
                if (source.Query.ContainsKey(p.Name))
                {
                    ret.Add(p.Name, source.Query[p.Name]);
                }
            }

            return ret;
        }

        /// <summary>
        ///  ВЫДЕЛЯЕТ ПАРАМЕТРЫ ПО МАСКЕ ТИПА
        ///  300518
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TEntity ToEntyty<TEntity>(HttpRequest source) where TEntity : class
        {
            TEntity ret = (TEntity)Activator.CreateInstance(typeof(TEntity));

            foreach (var p in typeof(TEntity).GetProperties())
            {
                var pInf = typeof(TEntity).GetProperty(p.Name);
                if (source.Query.ContainsKey(p.Name)
                    || source.Query.ContainsKey("\"" + p.Name + "\""))
                {
                    var val = source.Query.ContainsKey(p.Name) ? source.Query[p.Name] : source.Query["\"" + p.Name + "\""];
                    ret.GetType().InvokeMember(p.Name,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                        Type.DefaultBinder, ret, val);
                }
            }

            return ret;
        }


        /// <summary>
        ///  ВЫДЕЛЯЕТ ПАРАМЕТРЫ ПО МАСКЕ ТИПА
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public  IEnumerable<KeyValuePair<string, object>> ToKeyValuePair<TEntity>(TEntity source, HttpRequest request) where TEntity : class
        {

            Dictionary<string, object> ret = new Dictionary<string, object>();

            foreach (var p in typeof(TEntity).GetProperties())
            {
                var pInf = typeof(TEntity).GetProperty(p.Name);

                if (request.Query.ContainsKey(p.Name))
                {
                    ret.Add(p.Name, pInf.GetValue(source));
                }
            }

            return ret;
        }


        public IEnumerable<KeyValuePair<string, object>> ToKeyValuePairFromRequest( HttpRequest request) 
        {

            Dictionary<string, object> ret = new Dictionary<string, object>();
            foreach (var p in request.Query.Keys) {
                ret.Add(p, request.Query[p]);
            }
            return ret;
        }


        #endregion


    }
}
