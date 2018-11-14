using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGate.Models.DAL;

namespace WebGate.Models.Logic
{

    // 19.01.18 Это AxFlightService : IAxFlightService класс. Flight полный список  и FlightBoard подмножество для табло


    public interface IAxFlightService
    {
        IDalAccess<NVAOMAFLIGHTSCHEDULE> Flight                 { get; }
        IDalAccess<NVAOMAFLIGHTSCHEDULE> FlightBoard            { get; }
    }

    public class AxFlightService : IAxFlightService
    {
        private AxaptaContext _context;
        private IDalProvAccessBuilderFactory _accessBuilderFactory;

        public IDalAccess<NVAOMAFLIGHTSCHEDULE> Flight =>
            _accessBuilderFactory.Create<NVAOMAFLIGHTSCHEDULE, DbSet<NVAOMAFLIGHTSCHEDULE>>()
            .SetSource(_context.NVAOMAFLIGHTSCHEDULE)
            .SetReadAccess(
                 src => src
                ,(src,key) => src.Find(new object[] { key})
                //,(srs,vals) => new DbSetHelper<NVAOMAFLIGHTSCHEDULE>(srs).SelectAsEqual(vals)
                , IntervalParameters.GetSelectFunction<NVAOMAFLIGHTSCHEDULE>( x => { DateTime? r =  null; if( x.TRANSDATE.Year > 1901) r =  x.TRANSDATE ; return r; }  )
                )
            
            .Build();

        #region FlightBoard

        public IDalAccess<NVAOMAFLIGHTSCHEDULE> FlightBoard =>
            _accessBuilderFactory.Create<NVAOMAFLIGHTSCHEDULE, DbSet<NVAOMAFLIGHTSCHEDULE>>()
            .SetSource(_context.NVAOMAFLIGHTSCHEDULE)
            .SetReadAccess(
                 src => src.Where<NVAOMAFLIGHTSCHEDULE>( item => item.NFY_Accessibility > 0)
                , (src, key) => src.Find(new object[] { key })
                )
            .Build();

        #endregion


        public AxFlightService(AxaptaContext context, IDalProvAccessBuilderFactory accessBuilderFactory) // IDalAccessBuilderFactory
        {
            _context = context;
            _accessBuilderFactory = accessBuilderFactory;

            if (_context == null)
                throw new ArgumentNullException("AxFlightService  AxaptaContext is null");
            if (_accessBuilderFactory == null)
                throw new ArgumentNullException("AxFlightService  IDalAccessBuilderFactory is null");
                        
            var a = _accessBuilderFactory.Create < NVAOMAFLIGHTSCHEDULE, DbSet<NVAOMAFLIGHTSCHEDULE>> ();
        }

    }


}
