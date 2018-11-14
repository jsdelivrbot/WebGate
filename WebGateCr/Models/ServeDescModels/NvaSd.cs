using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGateApi.Base;
using WebGateCr.Models.Data;

namespace WebGateCr.Models.NvaSd
{
    public interface INvaSd
    {
        IDal<NVASD_Incoming> NvaSdIncoming { get; }
        IDal<JG_Mock_Table>  JgMockTable   { get; }

        string Save();
    }

    public class NvaSd : INvaSd
    {
        AxExtentionContext context;
        IDalBuilderFactory dalBuilderFactory;
        private EntytyMetadatasHelper mdHelper;

        public NvaSd(AxExtentionContext _context, IDalBuilderFactory _dalBuilderFactory, EntytyMetadatasHelper _mdHelper)
        {
            context = _context;
            dalBuilderFactory  = _dalBuilderFactory;
            mdHelper = _mdHelper;

            var s = mdHelper.GetMetadataByAttr(typeof(NVASD_Incoming));
        }

        private Func<AxExtentionContext, IEnumerable<NVASD_Incoming>> NvaSdIncomingSelFunc => (s => s.NVASD_Incoming.Select(x => x));
        private Func<AxExtentionContext, IEnumerable<JG_Mock_Table>>  JgMockTableSelFunc => (s => s.JG_Mock_Table.Select(x => x));

        public IDal<NVASD_Incoming> NvaSdIncoming =>
            dalBuilderFactory.Create<NVASD_Incoming, AxExtentionContext>()
            .SetSource(context)
            .SetReadAccess(
                NvaSdIncomingSelFunc
               , ( (s, id) => s.NVASD_Incoming.Find(new object[] { id })  )
               , EntityParHelper.ToBaseType<NVASD_Incoming>( EntityParHelper.GetSelectFunction<NVASD_Incoming>() )                   // некая скрытая связанность правда со статиком 
            )
            .SetInsertAccess((dbset, item) => dbset.Add(item).Entity, (ctx) =>  new NVASD_Incoming() { EventDateTime = DateTime.Now, CreatedDateTime = DateTime.Now })
            .Build();

        public IDal<JG_Mock_Table> JgMockTable =>
            dalBuilderFactory.Create<JG_Mock_Table, AxExtentionContext>()
            .SetSource(context)
            .SetReadAccess(
                JgMockTableSelFunc
               , ((s, id) => s.JG_Mock_Table.Find (new object[] { id }))
               , EntityParHelper.ToBaseType<JG_Mock_Table>(EntityParHelper.GetSelectFunction<JG_Mock_Table>())                        // некая скрытая связанность правда со статиком 
            )
            .SetInsertAccess((dbset, item) => dbset.Add(item).Entity, new JG_Mock_Table() { TextField = "Привет лунатикам !!!", BigCharField = "Панки Хой" })
            .Build();


        public string Save()
        {
            int modifiedCount;
            string ret = null;

            try
            {
                modifiedCount = context.SaveChanges();
                ret = modifiedCount > 0 ? ret : "Не удачно...."; 
            }
            catch (Exception e)
            {
                ret = e.Message;
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    ret = e.Message;
                }
            }
            return ret; 
        }
    }
}
