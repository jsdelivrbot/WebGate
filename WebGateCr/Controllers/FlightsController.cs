using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebGateCr.Models;
using WebGateCr.Models.Data;
using WebGateApi.Base;
using System.Xml;
using System.IO;
using System.Text;

namespace WebGateCr.Controllers
{

    //[Route("api/Ax/[controller]")]
    //public class FlightsController : Controller
    //{
    //    private readonly IAxFlightService _context;

    //    public FlightsController(IAxFlightService dbcontext)
    //    {
    //        _context = dbcontext;
    //    }

    //    // GET api/values
    //    //[HttpGet]
    //    //public IEnumerable<NVAOMAFLIGHTSCHEDULE> Get([FromQuery]NVAOMAFLIGHTSCHEDULE parameters)
    //    //{
    //    //    if (this.Request.Query.Count > 0)
    //    //    {
    //    //        var pars = ControllerHelper.ToKeyValuePair<NVAOMAFLIGHTSCHEDULE> (parameters, this.Request);
    //    //        return _context.Flight.Reader.Get(pars);
    //    //    }

    //    //    return _context.FlightBoard.Reader.GetAll(); //.._context.FlightBoard.Reader.GetAll();
    //    //}

    //    // GET api/values
    //    [HttpGet]
    //    public IEnumerable<NVAOMAFLIGHTSCHEDULE> Get([FromQuery]IntervalParameters parameters)
    //    {
    //        if (parameters != null)
    //        {
    //            var tst = "www";
    //        }

    //        if (this.Request.Query.Count > 0)
    //        {
    //            var pars = ControllerHelper.ToKeyValuePair<IntervalParameters>(this.Request);
    //            return _context.Flight.Reader.Get(pars);
    //        }

    //        return _context.FlightBoard.Reader.GetAll(); //.._context.FlightBoard.Reader.GetAll();
    //    }


    //    // GET api/values/5
    //    [HttpGet("{id}")]
    //    public NVAOMAFLIGHTSCHEDULE Get(long id)
    //    {
    //        return _context.Flight.Reader.Find(id);
    //    }
    //}

    [Route("api/Ax/[controller]")]
    public class FlightBoardController : Controller
    {
        private readonly IFlightBoard _context;

        public FlightBoardController(IFlightBoard dbcontext)
        {
            _context = dbcontext;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<FlightFids> Get(DateTime from, DateTime to   )
        {
            return _context.FlightFids.Access.Reader.GetAll();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public FlightFids Get(long id)
        {
            return _context.FlightFids.Access.Reader.Find(id);
        }
    }

    [Route("api/Ax/[controller]")]
    public class FlightBoardAController : Controller
    {
        private readonly IFlightBoard _context;

        public FlightBoardAController(IFlightBoard dbcontext)
        {
            _context = dbcontext;
        }

        // GET api/values
        [HttpGet]
        public  String Get()  //IEnumerable<FlightFids>
        {
            string ret = "Вомх";

            var pars = ControllerHelper.ToKeyValuePair<IntervalParameters>(new IntervalParameters() { From = DateTime.Now.AddDays(-1) });
            pars = ControllerHelper.AddKeyValuePair(pars, new KeyValuePair<string, object>("Direction", "Arrival"));
            var cnv = _context.FlightFids.Convert.GetListToConverter<XmlDocument>();
            if (cnv!= null)
            {
                XmlDocument xml = cnv.Convert(_context.FlightFids.Access.Reader.Get(pars));
                using (var stringWriter = new StringWriterUtf16())
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    xml.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    ret =  stringWriter.GetStringBuilder().ToString();
                }
            }

            return ret;
        }
    }

    [Route("api/Ax/[controller]")]
    public class FlightBoardDController : Controller
    {
        private readonly IFlightBoard _context;

        public FlightBoardDController(IFlightBoard dbcontext)
        {
            _context = dbcontext;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            string ret = "Вом";
            var pars = ControllerHelper.ToKeyValuePair<IntervalParameters>(new IntervalParameters() { From = DateTime.Now.AddDays(-1) });
            pars = ControllerHelper.AddKeyValuePair(pars, new KeyValuePair<string, object>("Direction", "Departure"));
            var cnv = _context.FlightFids.Convert.GetListToConverter<XmlDocument>();
            if (cnv != null)
            {
                XmlDocument xml = cnv.Convert(_context.FlightFids.Access.Reader.Get(pars));
                using (var stringWriter = new StringWriterUtf8()) //StringWriterUtf8
                {
                    XmlWriterSettings settings = new XmlWriterSettings()
                    {
                        //Encoding = Encoding.UTF8 // Encoding.GetEncoding("utf-8") // no BOM in a .NET string koi8-r
                    };


                    //using (var xmlTextWriter = XmlWriter.Create(stringWriter, settings))
                    using (var xmlTextWriter = XmlWriter.Create(stringWriter, settings) )
                    {
                        xml.WriteTo(xmlTextWriter);
                        xmlTextWriter.Flush();
                        ret = stringWriter.GetStringBuilder().ToString();
                    }
                }
            }
            return Content(ret, "text/xml", Encoding.GetEncoding("utf-8"));
        }
    }

    public class StringWriterUtf8 : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.GetEncoding(1251); }
        }
    }
    public class StringWriterUtf16 : StringWriter
    {
        public override Encoding Encoding
        {
            get { return Encoding.Unicode; }
        }
    }

}