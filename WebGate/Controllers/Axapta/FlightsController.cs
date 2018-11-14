using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebGate.Models.Logic;
using WebGate.Models;
using System.ComponentModel.DataAnnotations;
using WebGate.Models.DAL;

namespace WebGate.Controllers.Axapta
{


    [Route("api/Ax/[controller]")]
    public class FlightsController : Controller
    {
        private readonly IAxFlightService _context;

        public FlightsController(IAxFlightService dbcontext)
        {
            _context = dbcontext;
        }

        // GET api/values
        //[HttpGet]
        //public IEnumerable<NVAOMAFLIGHTSCHEDULE> Get([FromQuery]NVAOMAFLIGHTSCHEDULE parameters)
        //{
        //    if (this.Request.Query.Count > 0)
        //    {
        //        var pars = ControllerHelper.ToKeyValuePair<NVAOMAFLIGHTSCHEDULE> (parameters, this.Request);
        //        return _context.Flight.Reader.Get(pars);
        //    }

        //    return _context.FlightBoard.Reader.GetAll(); //.._context.FlightBoard.Reader.GetAll();
        //}

        // GET api/values
        [HttpGet]
        public IEnumerable<NVAOMAFLIGHTSCHEDULE> Get([FromQuery]IntervalParameters parameters)
        {
            if (parameters != null)
            {
                var tst = "www";
            }

            if (this.Request.Query.Count > 0)
            {
                var pars = ControllerHelper.ToKeyValuePair<IntervalParameters>(this.Request);
                return _context.Flight.Reader.Get(pars);
            }

            return _context.FlightBoard.Reader.GetAll(); //.._context.FlightBoard.Reader.GetAll();
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public NVAOMAFLIGHTSCHEDULE Get(long id)
        {
            return _context.Flight.Reader.Find(id);
        }
    }

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
            
            return _context.FlightFids.Reader.GetAll();
            //return null;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public FlightFids Get(long id)
        {
            return _context.FlightFids.Reader.Find(id);
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
        public IEnumerable<FlightFids> Get()
        {
            var pars = ControllerHelper.ToKeyValuePair<IntervalParameters>(new IntervalParameters() { From = DateTime.Now.AddDays(-1) });
            return  _context.FlightFids.Reader.Get(pars);
            //return null;
        }

    }


}