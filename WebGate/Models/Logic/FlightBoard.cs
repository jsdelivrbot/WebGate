using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using WebGate.Models.DAL;

namespace WebGate.Models.Logic
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

        public string Company { get; set; }
        public string Flight { get; set; }
        public string Iconfile { get; set; }

        public string Dest { get; set; }
        public string Trans { get; set; }

        public DateTime Time { get; set; }
        public DateTime Timedelay { get; set; }
        public DateTime Tlugbegin { get; set; }
        public DateTime Tlugend { get; set; }

        public string Hall { get; set; }
        public string Gate { get; set; }
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
        IDalAccess<FlightFids> FlightFids { get; }
    }

    #region  FlightBoard : IFlightBoard
    public class FlightBoard : IFlightBoard
    {
        private AxaptaContext _context;
        private IDalProvAccessBuilderFactory _accessBuilderFactory;
        private IModelEntityHelperBuilderFactory _helperFactory;

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

        private IModelEntityHelper<FlightFids> Helper =>
            _helperFactory.Create<FlightFids>()
                .AddFromConverterFunc<NvaOmaFlightScheduleEx>(src => new FlightFidsConverter().ConvertFromFlightShedule(src.Flight, src.Direction))
                .Build();

        //FlightFids  
        public IDalAccess<FlightFids> FlightFids =>                                         // Creating acess to virtual DBSet    
            _accessBuilderFactory.Create<FlightFids, AxaptaContext>()                       // Get object IDalProvAccessBuilder from injecting IDalProvAccessBuilderFactory
            .SetSource(_context)                                                            // Set AxaptaContext as source-context  
            .SetReadAccess(
                FuncSelect 
                ,FuncFind
                , FuncSelectInterval                
                ) 
            .Build();


        #region  Behavior functions 

        private Func<NVAOMAFLIGHTSCHEDULE, FlightDirection, FlightFids> FuncFromFS => 
            ((fs,dr)  => Helper.Converters.GetFromConverter<NvaOmaFlightScheduleEx>().Convert(new NvaOmaFlightScheduleEx() { Flight = fs, Direction = dr }));
        
        private Func<FlightFids, String> FuncToXml  => (fs => Helper.Converters.GetToConverter<String>().Convert(fs));

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
            ( (src, pars) =>      
                IntervalParameters.GetSelectFunction< FlightFids>(x => { DateTime? r = null; if (x.TransDate.Year > 1901) r = x.TransDate; return r; } )( FuncSelect(src), pars)
            );


        // парсит и модулит ключ
        private Func<object, Int64> PrsKey => (v) => Int64.TryParse(v.ToString(), out long r) ? r : 0 ;

        // 
        private Func<AxaptaContext, object, FlightFids> FuncFind => 
            (src, key) =>  FuncFromFS( src.Find<NVAOMAFLIGHTSCHEDULE>( new object[] { Math.Abs(PrsKey(key)) } ), PrsKey(key)>0 ? FlightDirection.Arrival : FlightDirection.Departure );
        #endregion


        public FlightBoard(AxaptaContext context, IDalProvAccessBuilderFactory accessBuilderFactory, IModelEntityHelperBuilderFactory helperFactory)
        {
            _context = context;
            _accessBuilderFactory = accessBuilderFactory;
            _helperFactory = helperFactory;
        }
    }
    #endregion
}
