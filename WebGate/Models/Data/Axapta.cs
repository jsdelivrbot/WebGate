using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebGate.Models
{

    #region AxaptaContext
    /// Отражение на БД Axapta 
    public class NVAOMAAIRCRAFTTYPE
    {
        [Key]
        public long RECID { get; set; }
        public string AIRCRAFTTYPEID { get; set; }
        public NVAOMAFlyType FLYTYPE { get; set; }

        public string DATAAREAID { get; set; }
        public int RECVERSION { get; set; }
    }

    #region NVAOMAFLIGHTSCHEDULE
    public class NVAOMAFLIGHTSCHEDULE
    {
        [Key]
        public long     RECID { get; set; }
        //public string   AIRCRAFTTYPE { get; set; }
        public string   FLIGHTCODEARRIVAL { get; set; }
        public string   FLIGHTCODEDEPARTURE { get; set; }
        public string   AIRCRAFT { get; set; }
        public DateTime TRANSDATE { get; set; }
        public int      NFY_Accessibility { get; set; }


        public string   CUSTACCOUNT { get; set; }
        public string   STARTPORT { get; set; }
        public string   STOPOVER1 { get; set; }
        public string   STOPOVER2 { get; set; }

        public string ENDPORTDEP   { get; set; }
        public string STOPOVER1DEP { get; set; }
        public string STOPOVER2DEP { get; set; }




        //public string CREWCOMMANDER { get; set; }



        //public decimal DEL_LOADFACT1 { get; set; }
        //public string AIRCRAFTTYPE { get; set; }
        //public string FLIGHTCODEARRIVAL { get; set; }
        //public string ENDPORT { get; set; }
        //public string STOPOVER1 { get; set; }
        //public int AIRLINESTYPE { get; set; }
        //public int TIMEDEPARTURE { get; set; }
        //public int TIMEARRIVAL { get; set; }
        //public DateTime DATEDEPARTURE { get; set; }
        //public DateTime DATEARRIVAL { get; set; }
        //public int TMPSAVEDATAFLAG { get; set; }
        //public int TIMEDEPARTUREFACT { get; set; }
        //public string TMPROUTETYPE { get; set; }
        //public int TIMEARRIVALFACT { get; set; }
        //public string FLIGHTCODEDEPARTURE { get; set; }
        //public decimal DEL_MASSA { get; set; }
        //public decimal DEL_BAGGAGE { get; set; }
        //public string STOPOVER2 { get; set; }
        //public string STARTPORTDEP { get; set; }
        //public string ENDPORTDEP { get; set; }
        //public string STOPOVER1DEP { get; set; }
        //public string STOPOVER2DEP { get; set; }
        //public DateTime DATEDEPARTUREFACT { get; set; }
        //public DateTime DATEARRIVALFACT { get; set; }
        //public string AIRCRAFT { get; set; }
        //public string DEL_AIRCRAFTD { get; set; }
        //public int PASSARR { get; set; }
        //public int PASSARRVIP { get; set; }
        //public int PASSARRCHILD { get; set; }
        //public int PASSARRCHILDVIP { get; set; }
        //public int PASSDEP { get; set; }
        //public int PASSDEPCHILD { get; set; }
        //public int PASSDEPVIP { get; set; }
        //public int PASSDEPCHILDVIP { get; set; }
        //public int PASSTRANS { get; set; }
        //public int PASSTRANSCHILD { get; set; }
        //public int PASSTRANSVIP { get; set; }
        //public int PASSTRANSCHILDVIP { get; set; }
        //public int CARGODEP { get; set; }
        //public int CARGOARR { get; set; }
        //public int MAILARR { get; set; }
        //public int MAILDEP { get; set; }
        //public string AIRPARKINGID { get; set; }
        //public int CARGODEPOVERNORM { get; set; }
        //public int CARGOARROVERNORM { get; set; }
        //public DateTime TARGETDATEARRIVAL { get; set; }
        //public DateTime TARGETDATEARRIVALFACT { get; set; }
        //public int TARGETTIMEARRIVAL { get; set; }
        //public int TARGETTIMEARRIVALFACT { get; set; }
        //public DateTime SOURCEDATEDEPARTURE { get; set; }
        //public DateTime SOURCEDATEDEPARTUREFACT { get; set; }
        //public int SOURCETIMEDEPARTURE { get; set; }
        //public int SOURCETIMEDEPARTUREFACT { get; set; }
        //public int DEL_TIMELANDFACT { get; set; }
        //public decimal MAXTAKEOFFWEIGHT { get; set; }
        //public string ARRIVALCOMMENT { get; set; }
        //public string DEPARTURECOMMENT { get; set; }
        //public DateTime EXPECTARRIVALDATE { get; set; }
        //public int EXPECTARRIVALTIME { get; set; }
        //public string CUSTACCOUNT { get; set; }
        //public string CREWCOMMANDER { get; set; }
        //public int CREWFLIGHTCNT { get; set; }
        //public int CREWCABINCNT { get; set; }
        //public int ISFREEGSM { get; set; }
        //public int CC_LOAD_SALENORM { get; set; }
        //public int CC_LOAD_CHAIRCOUNT { get; set; }
        //public int CC_LOAD_BOOKINGTOUR { get; set; }
        //public int CC_LOAD_FACTTOUR { get; set; }
        //public int DEL_CC_LOAD_VEHICLE { get; set; }
        //public int DEL_CC_LOAD_BUSCOUNT { get; set; }
        //public int CC_LOAD_LOADERSSHIFT { get; set; }
        //public int CC_LOAD_ARRCHILD { get; set; }
        //public int CC_LOAD_ARRCHILDVIP { get; set; }
        //public int CC_LOAD_ARR { get; set; }
        //public int CC_LOAD_ARRVIP { get; set; }
        //public int CC_LOAD_DEPCHILD { get; set; }
        //public int CC_LOAD_DEPCHILDVIP { get; set; }
        //public int CC_LOAD_DEP { get; set; }
        //public int CC_LOAD_DEPVIP { get; set; }
        //public int CC_LOAD_TRANSCHILD { get; set; }
        //public int CC_LOAD_TRANSCHILDVIP { get; set; }
        //public int CC_LOAD_TRANS { get; set; }
        //public int CC_LOAD_TRANSVIP { get; set; }
        //public int CC_LOAD_ORDERNUM { get; set; }
        //public decimal CC_LOAD_FL_TIMEGROUPI { get; set; }
        //public decimal CC_LOAD_FL_TIMEGROUPIII { get; set; }
        //public decimal CC_LOAD_GENERALFL_TIME { get; set; }
        //public decimal CC_LOAD_FL_TIMEGROUND { get; set; }
        //public decimal CC_LOAD_FL_TIMEAIR { get; set; }
        //public decimal CC_LOAD_ENGINESGROUND { get; set; }
        //public decimal DEL_CC_LOAD_UVD { get; set; }
        //public decimal DEL_CC_LOAD_INOUT { get; set; }
        //public decimal DEL_CC_LOAD_MO { get; set; }
        //public decimal DEL_CC_LOAD_TO { get; set; }
        //public decimal DEL_CC_LOAD_KO { get; set; }
        //public decimal DEL_CC_LOAD_GSM { get; set; }
        //public decimal DEL_CC_LOAD_OTHER { get; set; }
        //public int CC_LOAD_CARGODEP { get; set; }
        //public int CC_LOAD_CARGOARR { get; set; }
        //public int CC_LOAD_MAILARR { get; set; }
        //public int CC_LOAD_MAILDEP { get; set; }
        //public int CC_LOAD_BAGGARR { get; set; }
        //public int CC_LOAD_BAGGPAYARR { get; set; }
        //public int CC_LOAD_BAGGDEP { get; set; }
        //public int CC_LOAD_BAGGPAYDEP { get; set; }
        //public int CC_LOAD_MAXLOAD { get; set; }
        //public int CC_LOAD_FACTLOAD { get; set; }
        //public long DIVIDEDFLIGHTRECID { get; set; }
        //public int DEL_ISSPIN { get; set; }
        //public string MINKVS { get; set; }
        //public int SALENORM { get; set; }
        //public int CHAIRS { get; set; }
        //public int BOOKINGTOUR { get; set; }
        //public int FACTTOUR { get; set; }
        //public decimal LOADLIMIT { get; set; }
        //public decimal LOADFACT { get; set; }
        //public string MOVERSSHIFT { get; set; }
        //public string REQUESTNUMBER { get; set; }
        //public string DEL_PILOTNAME { get; set; }
        //public int FLIGHTGR1 { get; set; }
        //public int FLIGHTGR3 { get; set; }
        //public int DEL_AMOUNTFLIGHT { get; set; }
        //public int DEL_FLIGHTGROUND { get; set; }
        //public int DEL_FLIGHTAIR { get; set; }
        //public int ENGINESGROUND { get; set; }
        //public int SALESSYNCFLAG { get; set; }
        //public string FUELINVENTOWNERID_RU { get; set; }
        //public long DEL_FLIGHTCHANGEDPAYERRECID { get; set; }
        //public int TYPEPANH { get; set; }
        //public int ISCANCELED { get; set; }
        //public int FLIGHTTIMEPLAN { get; set; }
        //public int FLIGHTTIMEGROUNDPLAN { get; set; }
        //public decimal FUELPLAN { get; set; }
        //public long ROUTECURSOR { get; set; }
        //public int ISWITHLANDING { get; set; }
        //public string CUSTCODEPANH { get; set; }
        //public decimal DISCPERCENT { get; set; }
        //public DateTime TRANSDATE { get; set; }
        //public string DATAAREAID { get; set; }
        //public int RECVERSION { get; set; }
        //public long RECID { get; set; }
        //public long SUBSTITUTIONALFLIGHTRECID { get; set; }
        //public int TERMINALNUM { get; set; }
        //public DateTime CREATEDDATETIME { get; set; }
        //public string CREATEDBY { get; set; }
        //public int NVAOMAFLIGHTACCTYPE { get; set; }
        //public int CODELINE { get; set; }
        //public int INCLUDEINREGULARITYREPORTDEP { get; set; }
        //public int ISLATEARRIVAL { get; set; }
        //public int VALUESHASHCODE { get; set; }
        //public int ALLOWANCEVALUE { get; set; }
        //public decimal PRECALCTAXAMOUNT { get; set; }
        //public decimal PRECALCAMOUNT { get; set; }
        //public int PRECALCTYPE { get; set; }
        //public string SZVCREATOREMPLID { get; set; }
        //public string RCONTRACTACCOUNT { get; set; }
        //public string RCONTRACTCODE { get; set; }
        //public int TERMINALHALL { get; set; }
        //public int PASSARRNOTDOTATION { get; set; }
        //public int PASSARRVIPNOTDOTATION { get; set; }
        //public int PASSARRCHILDNOTDOTATION { get; set; }
        //public int PASSARRCHILDVIPNOTDOTATION { get; set; }
    }
    #endregion

    public enum NVAOMAFlyType { Plane = 0, Helicopter = 1, Drone = 2 }

    public class AxaptaContext : DbContext
    {
        public AxaptaContext(DbContextOptions<AxaptaContext> options) : base(options)
        {
        }

        public DbSet<NVAOMAAIRCRAFTTYPE>   NVAOMAAIRCRAFTTYPE   { get; set; }
        public DbSet<NVAOMAFLIGHTSCHEDULE> NVAOMAFLIGHTSCHEDULE { get; set; }

    }

    #endregion AxExtentionContext

}
