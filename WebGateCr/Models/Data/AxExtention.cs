using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebGateCr.Models.Data
{
    #region AxExtentionContext

    // Отражение на БД Axapta 


    [DisplayName("Мок-таблица")]
    [Description("Отладочная таблица")]
    [DisplayColumn("CharField")]
    public class JG_Mock_Table
    {
        [Key]
        [ReadOnly(true)]
        [Display(AutoGenerateField = true)]
        public long ID { get; set; }
        [Required(ErrorMessage = "Ты чо бля ввел ?")]
        [Display(Name = "CharFieldName", Description = "CharFieldDescription ", ShortName = "Код", AutoGenerateField = false)]
        public string CharField { get; set; }
        [Display(AutoGenerateField = true, AutoGenerateFilter = false  )]
        [ReadOnly(true)]
        public string BigCharField { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(AutoGenerateField = true, AutoGenerateFilter = true)]
        public string TextField { get; set; }
        public DateTime? DateField { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? DateTimeField { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateTimeAsDateField { get; set; }
        [RegularExpression(@"^-?\d+$", ErrorMessage = "Только, блять, целое !!!") ]
        [Range(-10, 10, ErrorMessage = "Только, блять, от -10 до 10! ")]
        public int? intField { get; set; }
        [Range(0.01, 100.00,ErrorMessage = "Price must be between 0.01 and 100.00")]
        [Display(Name = "RealFieldName", Description = "RealField Description ", ShortName = "Код")]
        public double? RealField { get; set; }
        [ForeignKey("./Ax/Enum/NVASDServiceDesc")]
        public int? intRefField { get; set; }
        [Display(Name = "Битовое поле2", Description = "Битовое поле Description ", ShortName = "Код", Order = -2)]
        public bool? bitField { get; set; }
    }


    [DisplayName("Входящее событие")]
    [Description("Входящее событие служб обслуживания и помощи")]
    [DisplayColumn("EventText")]
    public class NVASD_Incoming
    {
        [Key]
        //[System.Web.Mvc.HiddenInput(DisplayValue = false)]
        [Editable(false)]
        [Display(Name = "Код", Description = "Код события", ShortName = "Код", AutoGenerateField = true)]
        public long ID { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Время", Description = "Время события с которым связано обращение", ShortName = "Время",  Order = -95)]
        public DateTime EventDateTime { get; set; }

        [Editable(false)]
        [DataType(DataType.DateTime)]
        [Display(Name = "Время обращения", Order =  -110 )]
        public DateTime CreatedDateTime { get; set; }

        [Required]
        [Display(Name = "Служба", Description = "Идентификатор службы регистрации событий", ShortName = "Слж." , Order = -100) ]
        [ForeignKey("./Ax/Enum/NVASDServiceDesc")]
        //[ForeignKey("http://webgate.nvavia.ru:8080/api/Ax/Enum/NVASDServiceDesc")]
        public int ServiceDescID  { get; set; }

        [ForeignKey("./Ax/NvaSdEventType?servicedescid={ServiceDescID}")]
        [Display(Name = "Тип сообщения"  , Order = -90) ]
        public string EventTypeID { get; set; }
        [Display(AutoGenerateField = true)]
        public string EventTextID { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Сообщение", Description = "Текст сообщения о событии", Order = -10)]
        public string EventText   { get; set; }
    }


    public class AxExtentionContext : DbContext
    {
        public DbSet<NVASD_Incoming> NVASD_Incoming { get; set; }
        public DbSet<JG_Mock_Table> JG_Mock_Table  { get; set; }

        public AxExtentionContext(DbContextOptions<AxExtentionContext> options) : base(options)
        {
        }

        //public DbSet<OSCIncomings> OSCIncomings { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    #endregion AxExtentionContext
}
