using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGateApi.Base;
using WebGateCr.Models.Data;

namespace WebGateCr.Models
{
    ///    _  _  _ _  ____
    ///   / \/ |( / |/ /_ \
    ///  /     / /   _/ __/
    ///  \/\/\_\/\/\_\___/
    ///
    /// Модель общих данных аксапты
     
    public interface IAxCommon : IFlightBoard
    {
        IDal<AxEnum> NVAOMAAirLinesType { get; }
        IDal<AxEnum> GetEnumByName(string enumName);
        IDal<NVASDEVENTTYPE> NvaSdEventType { get; }

        IDal<NVAATS_PHONES>   NvaAtsPhones    { get;}
        IDal<NVAATS_DEVICE>   NvaAtsDevice    { get;}
        IDal<NVAATS_BUILDING> NvaAtsBuilding  { get; }
        IDal<NVAATS_SETTING>  NvaAtsSetting   { get; }

        // OMA 
        IDal<NVAOMACUSTLOGO> NvaOmaCustLogo { get; }

    }

    public class AxCommon :IAxCommon
    {
        private AxaptaContext context;
        IFlightBoard flightBoardContext;

        private IDalBuilderFactory dalBuilderFactory;

        //IFlightBoard
        public IDal<FlightFids> FlightFids { get { return flightBoardContext.FlightFids;  } }

        // Явная частная реализация енума
        public IDal<AxEnum> NVAOMAAirLinesType { get; }

        // ServeDesc
        public IDal<NVASDEVENTTYPE> NvaSdEventType { get; }

        // АТС 
        public IDal<NVAATS_PHONES>   NvaAtsPhones { get; }
        public IDal<NVAATS_DEVICE>   NvaAtsDevice { get; }
        public IDal<NVAATS_BUILDING> NvaAtsBuilding { get; }
        public IDal<NVAATS_SETTING>  NvaAtsSetting { get; }

        //OMA
        public IDal<NVAOMACUSTLOGO> NvaOmaCustLogo { get; }



    public AxCommon(AxaptaContext _context, IFlightBoard _FlightBoardContext, IDalBuilderFactory _dalBuilderFactory)
        {
            context = _context;
            flightBoardContext = _FlightBoardContext;
            dalBuilderFactory = _dalBuilderFactory;

            var dalBuilder = dalBuilderFactory.Create<AxEnum, AxaptaContext>();

            #region init acessors
            
            this.NVAOMAAirLinesType = dalBuilderFactory.Create<AxEnum, AxaptaContext>()
                .SetSource(context)                                        // контекст
                .SetReadAccess(
                    (x => x.NVAOMAAirLinesType),
                    FuncFind
                    )   // Доступ
                .Build();

            // NVASDEVENTTYPE
            this.NvaSdEventType = dalBuilderFactory.Create<NVASDEVENTTYPE, AxaptaContext>()
                .SetSource(context)                                        // контекст
                .SetReadAccess(
                    (s => s.NvaSdEventType.Select(x => x)),
                    ((s, id) => s.NvaSdEventType.Find(new object[] { id })),
                    EntityParHelper.ToBaseType<NVASDEVENTTYPE>(EntityParHelper.GetSelectFunction<NVASDEVENTTYPE>())
                )   
                .Build();

            // NVASDEVENTTYPE
            //this.NvaSdEventType = dalBuilderFactory.Create<NVASDEVENTTYPE, AxaptaContext>()
            //    .SetSource(context)                                        // контекст
            //    .SetReadAccess(
            //        (s => s.NvaSdEventType.Select(x => x)),
            //        ((s, id) => s.NvaSdEventType.Find(new object[] { id })),
            //        EntityParHelper.ToBaseType<NVASDEVENTTYPE>(EntityParHelper.GetSelectFunction<NVASDEVENTTYPE>())
            //    )
            //    .Build();

            //
            // NVAOMACUSTLOGO
            this.NvaOmaCustLogo = dalBuilderFactory.Create<NVAOMACUSTLOGO, AxaptaContext>()
                .SetSource(context)                                        // контекст
                .SetReadAccess(
                    (s => s.NVAOMACUSTLOGO.Select(x => x)),
                    ((s, id) => s.NVAOMACUSTLOGO.Find(new object[] { id })),
                    EntityParHelper.ToBaseType<NVAOMACUSTLOGO>(EntityParHelper.GetSelectFunction<NVAOMACUSTLOGO>())
                )
                .Build();

            #region ATS acessors
            this.NvaAtsPhones = dalBuilderFactory.Create<NVAATS_PHONES, AxaptaContext>()
                .SetSource(context)                                        // контекст
                .SetReadAccess(
                    (s => s.NvaAts_Phones.Select(x => x)),
                    ((s, id) => s.NvaAts_Phones.Find(new object[] { id })),
                    EntityParHelper.ToBaseType<NVAATS_PHONES>(EntityParHelper.GetSelectFunction<NVAATS_PHONES>())
                )
                .Build();

            this.NvaAtsDevice = dalBuilderFactory.Create<NVAATS_DEVICE, AxaptaContext>()
                .SetSource(context)                                        // контекст
                .SetReadAccess(
                    (s => s.NvaAts_Device.Select(x => x)),
                    ((s, id) => s.NvaAts_Device.Find(new object[] { id })),
                    EntityParHelper.ToBaseType<NVAATS_DEVICE>(EntityParHelper.GetSelectFunction<NVAATS_DEVICE>())
                )
                .Build();

            this.NvaAtsBuilding = dalBuilderFactory.Create<NVAATS_BUILDING, AxaptaContext>()
                .SetSource(context)                                        // контекст
                .SetReadAccess(
                    (s => s.NvaAts_Building.Select(x => x)),
                    ((s, id) => s.NvaAts_Building.Find(new object[] { id })),
                    EntityParHelper.ToBaseType<NVAATS_BUILDING>(EntityParHelper.GetSelectFunction<NVAATS_BUILDING>())
                )
                .Build();

            this.NvaAtsSetting = dalBuilderFactory.Create<NVAATS_SETTING, AxaptaContext>()
                .SetSource(context)                                        // контекст
                .SetReadAccess(
                    (s => s.NvaAts_Setting.Select(x => x)),
                    ((s, id) => s.NvaAts_Setting.Find(new object[] { id })),
                    EntityParHelper.ToBaseType<NVAATS_SETTING>(EntityParHelper.GetSelectFunction<NVAATS_SETTING>())
                )
                .Build();


            #endregion


            #endregion
        }


        private Func<AxaptaContext, object, AxEnum> FuncFind =>
           (src, key) => src.NVAOMAAirLinesType.Find(x => (x.ID == Int64.Parse(key.ToString())));

        

        #region Ax Enum common acess implimentation
        private Func<AxaptaContext, object, AxEnum> GetFuncFindName(string enumName)
        {
            return (src, key) => src.GetEnum(enumName).Find(x => (x.ID == Int64.Parse(key.ToString())));
        }
                  
        public IDal<AxEnum> GetEnumByName(string enumName)
        {
            return  dalBuilderFactory.Create<AxEnum, AxaptaContext>()
                .SetSource(context)                                        // контекст
                .SetReadAccess(
                    (x => x.GetEnum(enumName)),
                    this.GetFuncFindName(enumName))                          // Доступ
                .Build();
        }
        #endregion



    }
}
