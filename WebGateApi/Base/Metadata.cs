using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Linq;

namespace WebGateApi.Base
{
    /// <summary>
    /// Metadata Base Implimentation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TRet"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class MetadataBase< TRet, TKey> : IMetadataBase< TRet, TKey> , IDisposable where TRet : class
    {
        private Dictionary<TKey, TRet> _data = new Dictionary<TKey, TRet>();
        public TRet Find(TKey metadataKey) => _data.ContainsKey(metadataKey) ? _data[metadataKey] : null;

        public IEnumerable<KeyValuePair<TKey, TRet>> GetAll() => _data;
        public void Add(TKey key, TRet val) => _data.Add(key, val);

        public void Dispose()
        {
            _data = null;
        }


        public IEnumerable<KeyValuePair<TKey, TRet>> Get(IEnumerable<TKey> values)
        {
            //List<KeyValuePair<TKey, TRet>> ret = new List<KeyValuePair<TKey, TRet>>();
            Dictionary<TKey, TRet> ret = new Dictionary<TKey, TRet>();

            foreach (var k in values)
            {
                var v = this.Find(k);
                if (v != null)
                {
                    ret.Add(k, v);
                    //ret.Add( new KeyValuePair<TKey, TRet>(k,v));
                    //yield return (new KeyValuePair<TKey, TRet>(k, v));
                }
            }
            return ret;
        }
    }

    /// <summary>
    /// Metadata Indexed Base Implimentation : IMetadataBaseIdx
    /// </summary>
    public class MetadataBaseIdx<TRet, TKey, TIdx> : IMetadataBaseIdx<TRet, TKey, TIdx>, IDisposable where TRet : class
    {
        private Dictionary<TIdx, MetadataBase<TRet, TKey>> _data = new Dictionary<TIdx, MetadataBase<TRet, TKey>>();
        public IMetadataBase<TRet, TKey> this[TIdx idx] =>  _data.ContainsKey(idx) ? _data[idx] : null;

        public void Add(TIdx idx, TKey key, TRet val)
        {
            if (this[idx] == null)
            {
                _data.Add(idx, new MetadataBase<TRet, TKey>() );
            }
            ((MetadataBase<TRet, TKey>)this[idx]).Add(key, val);
        }

        public void Dispose()
        {
            _data = null;
        }
    }

    /// <summary>
    ///  MetadatasBase Base Implimentation : IMetadatasBase
    /// </summary>
    /// <typeparam name="TRet"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TIdx"></typeparam>
    public class MetadatasBase<TRet, TKey, TIdx> : IMetadatasBase<TRet, TKey, TIdx>, IDisposable where TRet : class
    {
        private MetadataBase<TRet, TKey> _metadata = new MetadataBase<TRet, TKey>();
        private MetadataBaseIdx<TRet, TKey, TIdx> _metadataFields = new MetadataBaseIdx<TRet, TKey, TIdx>();

        public IMetadataBase<TRet, TKey> Metadata => _metadata;
        public IMetadataBaseIdx<TRet, TKey, TIdx> MetadataField => _metadataFields;

        public void Dispose()
        {
            _metadata.Dispose();
            _metadataFields.Dispose();
        }
    }
    
    /// <summary>
    /// Metadata object - string  Implimentation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Metadata : MetadataBase< object, string>, IMetadata { }

    public class MetadataIdx : MetadataBaseIdx<object, string,string >, IMetadataIdx { }

    public class Metadatas : MetadatasBase<object, string, string> , IMetadatas { }


    /// <summary>
    /// Delegate for function convert object attribute metadata key value pair 
    /// </summary>
    /// <typeparam name="TAttr"></typeparam>
    /// <param name="source"></param>
    /// <param name="attr"></param>
    /// <returns></returns>
    public delegate IEnumerable< KeyValuePair<string,object>> ParseAttrFunc<TAttr>(object source , TAttr attr ) where TAttr : Attribute ;
    public class ParseAttrRules
    {
        #region ParseAttrFuncs
        private ParseAttrFunc<DisplayNameAttribute> dispNameFunc  = ((src, atr) => new KeyValuePair<string,object>[]  { new KeyValuePair<string, object>("DisplayName", atr.DisplayName) });
        private ParseAttrFunc<DescriptionAttribute> descFunc      = ((src, atr) => new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("Description", atr.Description) });
        #endregion
    }

    #region back EntytyMetadatasHelper
    /// <summary>
    /// Создает IMetadatas<TEntity> на основе атрибутов TEntity
    /// </summary>
    //public class EntytyMetadatasHelper
    //{
    //    public const string NamesMetadataParameter = @"Names";
    //    public const string NameMetadataParameter = @"Name";

    //    public const string FieldCharOpn = @"[";
    //    public const string FieldCharCls = @"]";

    //    public const string FieldIdTag = @"id";  //  

    //    public const string ListDevider = @",";
    //    //public const string FieldsMetadataParameter = @"Fields";

    //    public const string AllMetadataValue = @"All";

    //    public static readonly string[] FieldAtributeToRootList = { @"Key" };


    //    public static IMetadatas GetMetadataByAttr(Type sourceType)
    //    {
    //        var ret = new Metadatas();
    //        EntytyMetadatasHelper.ParseAttr(sourceType.GetCustomAttributes(true), ret);

    //        // Теперяча надо перебрать атрибуты полей
    //        var props = sourceType.GetProperties();
    //        foreach (var p in props)
    //        {
    //            EntytyMetadatasHelper.AddFieldsInfo(p, ret);                                //Metadata of Fields and types
    //            EntytyMetadatasHelper.ParseAttr(p.GetCustomAttributes(true), ret, p.Name, FieldAtributeToRootList );
    //            EntytyMetadatasHelper.AddFieldIdInfo(p.Name, ret);
    //        }

    //        return ret;
    //    }

    //    // Добавляет к метаданным таблицы ид полей 
    //    private static void AddFieldsInfo(PropertyInfo p, Metadatas toObject)
    //    {
    //        ((MetadataBase<object, string>)toObject.Metadata).Add(EntytyMetadatasHelper.FieldCharOpn+p.Name+ EntytyMetadatasHelper.FieldCharCls
    //            , EntytyMetadatasHelper.ConvertToBaseType( p.PropertyType ));
    //    }

    //    // Добавляет к метаданным поля его айдишник
    //    private static void AddFieldIdInfo(string fieldId , Metadatas toObject)
    //    {
    //        ((MetadataBaseIdx<object, string, string>)toObject.MetadataField).Add(fieldId, EntytyMetadatasHelper.FieldCharOpn + FieldIdTag + EntytyMetadatasHelper.FieldCharCls, fieldId);
    //    }


    //    private static int ParseAttr(object[] attrs, Metadatas toObject, string propName = null, string[] upToRootAttrList = null )
    //    {
    //        const string devider = ".";
    //        var ret = 0;
    //        List<string> exclude = new List<string>();// [ "TypeId" ];
    //        exclude.Add("TypeId");
    //        exclude.Add("AutoGenerateField");
    //        exclude.Add("AutoGenerateFilter");
    //        exclude.Add("Order");

    //        foreach (object atr in attrs)
    //        {
    //            var at = atr as Attribute;
    //            var atName = at.GetType().Name;
    //            var atClearName = atName.Substring(0, atName.Length - 9);
    //            // 130418

    //            if (!string.IsNullOrEmpty(propName) && upToRootAttrList != null && Array.IndexOf(upToRootAttrList, atClearName) >= 0)
    //            {
    //                var val = ((MetadataBase<object, string>)toObject.Metadata).Find(atClearName);
    //                ((MetadataBase<object, string>)toObject.Metadata).Add(atClearName,  (((val == null) ? "" : val + ListDevider) + propName));
    //            }

    //            if (at != null)
    //            {
    //                var atProp = at.GetType().GetProperties();
    //                foreach (var p in atProp)  // по свойствам аттрибута
    //                {
    //                    if (p.CanRead && !exclude.Contains(p.Name))
    //                    {
    //                        var val = p.GetValue(at);
    //                        if (val != null)
    //                        {
    //                            var name = (p.Name == atClearName) ? p.Name : atClearName + devider + p.Name;
    //                            if (propName == null)
    //                            {
    //                                ((MetadataBase<object, string>)toObject.Metadata).Add(name, val);
    //                            }
    //                            else
    //                            {
    //                                ((MetadataBaseIdx<object, string, string>)toObject.MetadataField).Add(propName, name, val);
    //                            }
    //                            ret++;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        return ret;
    //    }

    //    public static string ConvertToBaseType(Type tp)
    //    {
    //        string ret = "string";

    //        if (tp == typeof(int)
    //            || tp == typeof(Int16)
    //            || tp == typeof(Int32)
    //            || tp == typeof(long)
    //            || tp == typeof(short))
    //        {
    //            ret = "number";
    //        }
    //        else if (
    //            tp == typeof(Double)
    //            || tp == typeof(double)
    //            || tp == typeof(float)
    //            || tp == typeof(decimal)
    //            || tp == typeof(Decimal))
    //        {
    //            ret = "number";
    //        }
    //        else if (tp == typeof(DateTime))
    //        {
    //            ret = "Date";
    //        }
    //        else if (tp == typeof(Boolean) || tp == typeof(bool) )
    //        {
    //            ret = "boolean";
    //        }

    //        return ret;
    //    }

    //    public static object GetMetadataByQuery(IQueryCollection query, IMetadataBase<object,string> metadata)
    //    //public static object GetMetadataByQuery(IQueryCollection query, IMetadata metadata)
    //    {
    //        object ret = null;

    //        if (query.Keys.Contains(EntytyMetadatasHelper.NamesMetadataParameter))
    //        {
    //            StringValues vals = query[EntytyMetadatasHelper.NamesMetadataParameter];
    //            ret = metadata.Get(vals);
    //        }

    //        else if (query.Keys.Contains(EntytyMetadatasHelper.NameMetadataParameter))
    //        {
    //            StringValues vals = query[EntytyMetadatasHelper.NameMetadataParameter];
    //            if (vals.Count > 0)
    //            {
    //                ret = metadata.Find(vals[0]);
    //            }
    //        }

    //        else
    //        {
    //            ret = metadata.GetAll();
    //        }

    //        return ret;
    //    }

    //    public static object GetMetadataByQuery(IQueryCollection query, object metadata)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
    #endregion back EntytyMetadatasHelper

    /// <summary>
    /// Создает IMetadatas<TEntity> на основе атрибутов TEntity
    /// </summary>
    public class EntytyMetadatasHelper
    {
        public const string NamesMetadataParameter = @"Names";
        public const string NameMetadataParameter = @"Name";

        public const string FieldCharOpn = @"[";
        public const string FieldCharCls = @"]";

        public const string FieldIdTag = @"id";  //  

        public const string ListDevider = @",";
        //public const string FieldsMetadataParameter = @"Fields";

        public const string AllMetadataValue = @"All";

        public static readonly string[] FieldAtributeToRootList = { @"Key" };

        public static readonly string[] KeyFieldAtribute = { @"Key" };


        /// <summary>
        /// 231018 return id fieldname
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public string getIdName(Type sourceType)
        {
            Func<string,string> atClearName = (n => n?.Substring(0, n.Length - 9)); // xxxAttribute

            return sourceType
                    .GetProperties()
                    .ToList()
                    .Find(x => (x.GetCustomAttributes(true)
                                   .Select(a => atClearName((a as Attribute)?.GetType().Name))
                                   .ToList()
                                   .Find(n => KeyFieldAtribute.Contains(n))
                                   != null)
                    )?.Name;
        }



        public IMetadatas GetMetadataByAttr(Type sourceType)
        {
            var ret = new Metadatas();
            this.ParseAttr(sourceType.GetCustomAttributes(true), ret);

            //var s = this.getIdName(sourceType);

            // Теперяча надо перебрать атрибуты полей
            var props = sourceType.GetProperties();
            foreach (var p in props)
            {
                this.AddFieldsInfo(p, ret);                                //Metadata of Fields and types
                this.ParseAttr(p.GetCustomAttributes(true), ret, p.Name, FieldAtributeToRootList);
                this.AddFieldIdInfo(p.Name, ret);
            }

            return ret;
        }

        // Добавляет к метаданным таблицы ид полей 
        private void AddFieldsInfo(PropertyInfo p, Metadatas toObject)
        {
            ((MetadataBase<object, string>)toObject.Metadata).Add(FieldCharOpn + p.Name + FieldCharCls
                , this.ConvertToBaseType(p.PropertyType));
        }

        // Добавляет к метаданным поля его айдишник
        private void AddFieldIdInfo(string fieldId, Metadatas toObject)
        {
            ((MetadataBaseIdx<object, string, string>)toObject.MetadataField).Add(fieldId, FieldCharOpn + FieldIdTag + FieldCharCls, fieldId);
        }


        private int ParseAttr(object[] attrs, Metadatas toObject, string propName = null, string[] upToRootAttrList = null)
        {
            const string devider = ".";
            var ret = 0;
            List<string> exclude = new List<string>();// [ "TypeId" ];
            exclude.Add("TypeId");
            //exclude.Add("AutoGenerateField");
            //exclude.Add("AutoGenerateFilter");
            //exclude.Add("Order");

            foreach (object atr in attrs)
            {
                var at = atr as Attribute;
                var atName = at.GetType().Name;
                var atClearName = atName.Substring(0, atName.Length - 9);
                // 130418

                if (!string.IsNullOrEmpty(propName) && upToRootAttrList != null && Array.IndexOf(upToRootAttrList, atClearName) >= 0)
                {
                    var val = ((MetadataBase<object, string>)toObject.Metadata).Find(atClearName);
                    ((MetadataBase<object, string>)toObject.Metadata).Add(atClearName, (((val == null) ? "" : val + ListDevider) + propName));
                }

                if (at != null)
                {
                    var atProp = at.GetType().GetProperties();
                    foreach (var p in atProp)  // по свойствам аттрибута
                    {
                        if (p.CanRead && !exclude.Contains(p.Name))
                        {

                            ////051018 

                            



                            ////var v = (object)(p.GetGetMethod().Invoke(at,null));
                            //// Как не ебался по другому не вышло.
                            //try
                            //{
                            //    val = p.GetValue(at);
                            //}
                            //catch {

                            //    // var s = p.GetAccessors()[0].Invoke(at, null); 
                            //    // var d = s[0].Invoke(at, null);

                            //    var s = ((System.ComponentModel.DataAnnotations.DisplayAttribute)at).GetOrder();


                            //    var d = at.GetType().GetMethod("Get" + p.Name);
                            //    val = d.Invoke(at,null); 

                            //    val = null;
                            //}

                            object val = null;

                            // Ugly line
                            var getMethogInfo = at.GetType().GetMethod("Get" + p.Name);

                            if (getMethogInfo != null)
                            {
                                val = getMethogInfo.Invoke(at, null);
                            }
                            else
                            {
                                try
                                {
                                    val = p.GetValue(at);
                                }
                                catch
                                {
                                    val = null;
                                }
                            }

                            if (val != null)
                            {
                                var name = (p.Name == atClearName) ? p.Name : atClearName + devider + p.Name;
                                if (propName == null)
                                {
                                    ((MetadataBase<object, string>)toObject.Metadata).Add(name, val);
                                }
                                else
                                {
                                    ((MetadataBaseIdx<object, string, string>)toObject.MetadataField).Add(propName, name, val);
                                }
                                ret++;
                            }
                        }
                    }
                }
            }
            return ret;
        }

        public string ConvertToBaseType(Type tp)
        {
            string ret = "string";

            if (tp == typeof(int)
                || tp == typeof(Int16)
                || tp == typeof(Int32)
                || tp == typeof(Int64)
                || tp == typeof(long)
                || tp == typeof(short))
            {
                ret = "number";
            }
            else if (tp == typeof(int?)
                || tp == typeof(Int16?)
                || tp == typeof(Int32?)
                || tp == typeof(Int64?)
                || tp == typeof(long?)
                || tp == typeof(short?))
            {
                ret = "number?";
            }


            else if (
                tp == typeof(Double)
                || tp == typeof(double)
                || tp == typeof(float)
                || tp == typeof(decimal)
                || tp == typeof(Decimal))
            {
                ret = "number";
            }

            else if (
                tp == typeof(Double?)
                || tp == typeof(double?)
                || tp == typeof(float?)
                || tp == typeof(decimal?)
                || tp == typeof(Decimal?))
            {
                ret = "number?";
            }

            else if (tp == typeof(DateTime))
            {
                ret = "Date";
            }

            else if (tp == typeof(DateTime?))
            {
                ret = "Date?";
            }

            else if (tp == typeof(Boolean) || tp == typeof(bool))
            {
                ret = "boolean";
            }

            else if (tp == typeof(Boolean?) || tp == typeof(bool?))
            {
                ret = "boolean?";
            }

            return ret;
        }

        public object GetMetadataByQuery(IQueryCollection query, IMetadataBase<object, string> metadata)
        //public static object GetMetadataByQuery(IQueryCollection query, IMetadata metadata)
        {
            object ret = null;

            if (query.Keys.Contains(NamesMetadataParameter))
            {
                StringValues vals = query[NamesMetadataParameter];
                ret = metadata.Get(vals);
            }

            else if (query.Keys.Contains(NameMetadataParameter))
            {
                StringValues vals = query[NameMetadataParameter];
                if (vals.Count > 0)
                {
                    ret = metadata.Find(vals[0]);
                }
            }

            else
            {
                ret = metadata.GetAll();
            }

            return ret;
        }

        public object GetMetadataByQuery(IQueryCollection query, object metadata)
        {
            throw new NotImplementedException();
        }
    }


}
