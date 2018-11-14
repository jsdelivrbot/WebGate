using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using WebGateApi.Base;
using WebGateCr.Models.Data;

namespace WebGateCr.Models
{
    ///    _  _  _ _  ____
    ///   / \/ |( / |/ /_ \
    ///  /     / /   _/ __/
    ///  \/\/\_\/\/\_\___/
    ///
    /// Табло прилета вылета 
    /// 
    
    /// Направление 
    public enum FlightDirection {Arrival=0, Departure=1}

    /// <summary>
    /// Строка суточного плана как прилет или вылет
    /// </summary>
    public class NvaOmaFlightScheduleEx
    {
        public NVAOMAFLIGHTSCHEDULE Flight      { get; set; }
        public FlightDirection      Direction   { get; set; }
    }

    #region Base Entities: FlightFids, FlightFidsArrival,  FlightFidsDeparture
    /// Fids item base
    public class FlightFids
    {
        public long ID { get; set; }
        public DateTime TransDate { get; set; }

        [XmlElement(ElementName = "company")]
        public string Company { get; set; }
        [XmlElement(ElementName = "flight")]
        public string Flight { get; set; }
        [XmlElement(ElementName = "iconfile")]
        public string Iconfile { get; set; }
        [XmlElement(ElementName = "dest")]
        public string Dest { get; set; }
        [XmlElement(ElementName = "trans")]
        public string Trans { get; set; }
        [XmlElement(ElementName = "time")]
        public DateTime Time { get; set; }
        [XmlElement(ElementName = "timedelay")]
        public DateTime Timedelay { get; set; }
        [XmlElement(ElementName = "tlugbegin")]
        public DateTime Tlugbegin { get; set; }
        [XmlElement(ElementName = "tlugend")]
        public DateTime Tlugend { get; set; }
        [XmlElement(ElementName = "hall")]
        public string Hall { get; set; }
        [XmlElement(ElementName = "gate")]
        public string Gate { get; set; }
        [XmlElement(ElementName = "desk")]
        public string Desk { get; set; }

        public FlightDirection Direction { get; set; }
        public FlightFids() { }
    }

    public class FlightFidsArrival : FlightFids
    {

    }

    public class FlightFidsDeparture: FlightFids
    {
    }
    #endregion

    #region Converter
    public class FlightFidsConverter
    {
        public FlightFids ConvertFromFlightShedule(NVAOMAFLIGHTSCHEDULE from , FlightDirection direction = FlightDirection.Arrival)
        {
            var ret = new FlightFids();
            this.FillFromFlightShedule(from, ret, direction);
            return ret;
        }

        public void FillFromFlightShedule(NVAOMAFLIGHTSCHEDULE from, FlightFids to, FlightDirection direction = FlightDirection.Arrival)
        {
            to.Direction = direction;
            to.ID = direction == FlightDirection.Arrival ? from.RECID : (- from.RECID) ;
            to.TransDate = from.TRANSDATE;
            to.Company = from.CUSTACCOUNT;
            to.Flight = (direction == FlightDirection.Arrival) ? from.FLIGHTCODEARRIVAL : from.FLIGHTCODEDEPARTURE;
            to.Dest  = (direction == FlightDirection.Arrival) ? from.STARTPORT : from.ENDPORTDEP;
            to.Trans = (direction == FlightDirection.Arrival) ? (from.STOPOVER1 + " " + from.STOPOVER2) : (from.STOPOVER1DEP + " " + from.STOPOVER2DEP);
            //....
        }
    }

    
    public class FlightFidsToXmlDoc
    {
            
        public static XmlDocument Convert(IEnumerable<FlightFids> items)
        {
            //XmlWriterSettings settings = new XmlWriterSettings()
            //{
            //    Encoding = Encoding.UTF8 // no BOM in a .NET string
            //};

            XmlDocument doc = new XmlDocument();
            //XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "", null);
            //var nd =  doc.AppendChild(xmlDeclaration);
            
            

            //using (var sw = new StringWriterUtf8())
            //{
                using (var xw = doc.CreateNavigator().AppendChild())
                {
                    //xw.Settings.NewLineChars = "\r";
                    int l = 0;
                    var t = "";
                    Func<FlightFids, String> fd = (x => x.Direction == FlightDirection.Arrival ? "A" : "D");

                    foreach (var i in items)
                    {
                        l++;
                        t = (t == ""?fd(i):t);
                        t = (t == "AD" || t == fd(i)) ? t : "AD";
                    }
                    var ns = new XmlSerializerNamespaces();
                    ns.Add("", "");

                    xw.WriteStartDocument();
                    xw.WriteStartElement("itemlist");
                    xw.WriteAttributeString("type", t);
                    xw.WriteAttributeString("count", l.ToString());
                    
                    l = 0;
                    foreach (var i in items)
                    {
                        new XmlSerializer(typeof(FlightFids), new XmlRootAttribute("item_" + (l++).ToString())).Serialize(xw, i, ns  );
                    }

                    //if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                    //{
                    //    XmlDeclaration xmlDeclaration = (XmlDeclaration)doc.FirstChild;
                    //    xmlDeclaration.Encoding = "utf-16";
                    //}

                }
                return doc;
            //}
        }
    }
    #endregion

    #region Serializer
    public class FlightFidsSerializer
    {
        public string Serialize(FlightFids item)
        {
            using (var sw = new StringWriter())
            {
                using (var xw = XmlWriter.Create(sw))
                {
                    var s = new XmlSerializer(typeof(FlightFids));
                    s.Serialize(xw, item);
                }
                return sw.ToString();
            }
        }
    }
    #endregion


    public interface IFlightBoard
    {
        IDal<FlightFids> FlightFids { get; }
    }


    #region  FlightBoard : IFlightBoard
    public class FlightBoard : IFlightBoard
    {
        private AxaptaContext _context;
        private IDalBuilder<FlightFids, AxaptaContext> _dalBuilder;
        public IDal<FlightFids> _dal; 

        public IDal<FlightFids> FlightFids => _dal;

        #region legasy        
        // private IModelEntityHelperBuilderFactory _helperFactory;
        // Helper initialize
        //private IModelEntityHelper<FlightFids> Helper =>                                        
        //    _helperFactory.Create<FlightFids>()                                               // Get object IModelEntityHelperBuilder from injecting IModelEntityHelperBuilderFactory    
        //    .AddBuilder<NvaOmaFlightScheduleEx>(                                              // Add new builder from type NVAOMAFLIGHTSCHEDULE
        //        _helperFactory.CreateBulder<NvaOmaFlightScheduleEx, FlightFids>(              // Get object IModelEntityBuilder from injecting IModelEntityHelperBuilderFactory
        //            src => new FlightFidsConverter().ConvertFromFlightShedule(src.Flight,src.Direction)
        //            )
        //        )
        //    .AddConverter<String>(
        //        _helperFactory.CreateConverter<String, FlightFids>(
        //            ff =>{
        //                using (var sw = new StringWriter())
        //                    {
        //                        using (var xw = XmlWriter.Create(sw))
        //                        {   var s = new XmlSerializer(typeof(FlightFids));  s.Serialize(xw, ff);  }
        //                        return sw.ToString();
        //                    }
        //                }
        //            )
        //        ) 
        //    .Build();                                                                       // Build      

        //private IModelEntityHelper<FlightFids> Helper =>
        //    _helperFactory.Create<FlightFids>()
        //        .AddFromConverterFunc<NvaOmaFlightScheduleEx>(src => new FlightFidsConverter().ConvertFromFlightShedule(src.Flight, src.Direction))
        //        .Build();

        ////FlightFids  
        //public IDalAccess<FlightFids> FlightFids =>                                         // Creating acess to virtual DBSet    
        //    _accessBuilderFactory.Create<FlightFids, AxaptaContext>()                       // Get object IDalProvAccessBuilder from injecting IDalProvAccessBuilderFactory
        //    .SetSource(_context)                                                            // Set AxaptaContext as source-context  
        //    .SetReadAccess(
        //        FuncSelect 
        //        ,FuncFind
        //        , FuncSelectInterval                
        //        ) 
        //    .Build();
        #endregion 

        #region  Behavior functions 
        private Func<NVAOMAFLIGHTSCHEDULE, FlightDirection, FlightFids> FuncFromFS => 
            ((fs,dr)  => _dal.Convert.GetFromConverter<NvaOmaFlightScheduleEx>().Convert(new NvaOmaFlightScheduleEx() { Flight = fs, Direction = dr }));

        private Func<FlightFids, String> FuncToXml  => (fs => _dal.Convert.GetToConverter<String>().Convert(fs));

        private Func<AxaptaContext, IEnumerable<FlightFids>> FuncSelect => 
            (src => 
                src.NVAOMAFLIGHTSCHEDULE.Where<NVAOMAFLIGHTSCHEDULE>(item => item.NFY_Accessibility > 0)
                .Select( itm => FuncFromFS(itm, FlightDirection.Arrival ))
                .Concat<FlightFids>(
                    src.NVAOMAFLIGHTSCHEDULE.Where<NVAOMAFLIGHTSCHEDULE>(item => item.NFY_Accessibility > 0)
                    .Select(itm => FuncFromFS(itm, FlightDirection.Departure))
                    )
            );

        private Func<AxaptaContext, IEnumerable<KeyValuePair<string, object>>, IEnumerable<FlightFids>> FuncSelectInterval =>  
            ( (src, pars) =>    FuncSelectPars( FuncSelect(src), pars));


        private SelectFunc<FlightFids> FuncSelectPars =>
            ParSelMath.And<FlightFids>(
                  IntervalParameters.GetSelectFunction<FlightFids>(x => { DateTime? r = null; if (x.TransDate.Year > 1901) r = x.TransDate; return r; })
                , EntityParHelper.GetSelectFunction<FlightFids>());

        //IntervalParameters.GetSelectFunction<FlightFids>( x => { DateTime? r = null; if (x.TransDate.Year > 1901) r = x.TransDate; return r; }

        // парсит и модулит ключ
        private Func<object, Int64> PrsKey => (v) => Int64.TryParse(v.ToString(), out long r) ? r : 0 ;

        // 
        private Func<AxaptaContext, object, FlightFids> FuncFind => 
            (src, key) =>  FuncFromFS( src.Find<NVAOMAFLIGHTSCHEDULE>( new object[] { Math.Abs(PrsKey(key)) } ), PrsKey(key)>0 ? FlightDirection.Arrival : FlightDirection.Departure );

        
        #endregion

        public FlightBoard(AxaptaContext context, IDalBuilderFactory dalBuilderFactory)
        {
            _context = context;
            _dalBuilder = dalBuilderFactory.Create<FlightFids, AxaptaContext>();

            _dal = _dalBuilder
                .SetSource(context)                                        // контекст
                .SetReadAccess(FuncSelect, FuncFind, FuncSelectInterval)   // Доступ
                .AddFromFunc<NvaOmaFlightScheduleEx>(src => new FlightFidsConverter().ConvertFromFlightShedule(src.Flight, src.Direction))
                .AddListToFunc<XmlDocument>(s => FlightFidsToXmlDoc.Convert(s))            
                .Build();
        }
    }
    #endregion

}
