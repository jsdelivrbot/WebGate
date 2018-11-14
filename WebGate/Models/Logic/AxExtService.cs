using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGate.Models.DAL;

namespace WebGate.Models.Logic
{
    public interface IAxExtService
    {
        //IDal<WGD_Test> WGD_Test { get; }
        //IDal<NVAOMAAIRCRAFTTYPE> NVAOMAAIRCRAFTTYPE { get; }
        //IDal<WGD_Test> WGD_Test { get; }
        int Save();
        IDalAccess<WGD_Test> WGD_Test { get; }

    }

    public class AxExtService : IAxExtService
    {
        private AxExtentionContext _context;
        private IDalProvAccessBuilderFactory _accessBuilderFactory;

        //public IDal<WGD_Test>  WGD_Test { get ; }
        //public IDal<NVAOMAAIRCRAFTTYPE> NVAOMAAIRCRAFTTYPE { get;  }

        //public IDalAccess<WGD_Test> WGD_Test =>
        //    _accessBuilderFactory.Create< WGD_Test,  DbSet<WGD_Test> >()
        //    .SetSource(_context.WGD_Test)
        //    .SetReadAccess(dbset => new DalHelperDbSetReader<WGD_Test>(dbset) )
        //    .SetInsertAccess(dbset => new DalHelperDbSetInserter<WGD_Test>(dbset))
        //    .Build();

        public IDalAccess<WGD_Test> WGD_Test =>
            _accessBuilderFactory.Create<WGD_Test, DbSet<WGD_Test>>()
            .SetSource(_context.WGD_Test)
            .SetReadAccess(dbset => new DalHelperDbSetReader<WGD_Test>(dbset))
            .SetInsertAccess((dbset, item) => dbset.Add(item))
            //.SetUpdateAccess(dbset=> new DalHelperDbSetUpdater<WGD_Test>(dbset))
            .SetUpdateAccess((dbset, item) => dbset.Update(item))
            //.SetRemoveAccess(dbset=> new DalHelperDbSetRemover<WGD_Test>(dbset))
            .SetRemoveAccess(  (dbset, id) =>  
                {
                    var f = new DalHelperDbSetReader<WGD_Test>(dbset).Find(id);
                    if (f != null) { dbset.Remove(f); }
                    return f;
                } )
            .Build();

        

        public AxExtService(AxExtentionContext context, IDalProvAccessBuilderFactory accessBuilderFactory)
        {
            _context = context;
            _accessBuilderFactory = accessBuilderFactory;

            if (_context == null)
                throw new ArgumentNullException("AxExtService");
            if (_accessBuilderFactory == null)
                throw new ArgumentNullException("AxFlightService  IDalAccessBuilderFactory is null");

            //WGD_Test = new Dal<WGD_Test>(_context.WGD_Test);
            //NVAOMAAIRCRAFTTYPE = new Dal<NVAOMAAIRCRAFTTYPE>(_context.NVAOMAAIRCRAFTTYPE);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
