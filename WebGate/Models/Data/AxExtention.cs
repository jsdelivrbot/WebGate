using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;





namespace WebGate.Models
{
    #region AxExtentionContext
    public class OSCIncomings
    {
        [Key]
        public long id { get; set; }
        public DateTime GettingDatetime { get; set; }
        public DateTime? RecivedDatetime { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string GettingSourceName { get; set; }
        public string GettingSourceType { get; set; }
        public string GettingSource { get; set; }
        public string LexerResult { get; set; }
        public string SyntaxResult { get; set; }
        public string SerializeResult { get; set; }
        public long? State { get; set; }
        public string IncomingData { get; set; }
    }

    public class WGD_Test
    {
        [Key]
        public long     id { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime? TimeStamp { get; set; }
        [StringLength(50)]
        public string   Name { get; set; }
        [StringLength(500)]
        public string   Description { get; set; }
    }

    public class WGD_Test2
    {
       
        public long id { get; set; }
        public string Name { get; set; }
    }


    public class AxExtentionContext : DbContext
    {
        public AxExtentionContext(DbContextOptions<AxExtentionContext> options): base(options)
        {
            //var extension = options.FindExtension<SqlServerOptionsExtension>();
            //extension.ConnectionString = connectionString;
            //var opt = options;
        }

        public DbSet<OSCIncomings> OSCIncomings { get; set; }
        public DbSet<WGD_Test>     WGD_Test     { get; set; }
        public DbSet<WGD_Test2>    WGD_Test2    { get; set; }
        public DbSet<NVAOMAAIRCRAFTTYPE> NVAOMAAIRCRAFTTYPE { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WGD_Test>().ToTable("WGD_Test");
            modelBuilder.Entity<NVAOMAAIRCRAFTTYPE>().ToTable("NVAOMA");

        }
    }

    #endregion AxExtentionContext
}
