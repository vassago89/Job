namespace FIAT_Project.Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    public class FIATDbContext : DbContext
    {
        public FIATDbContext()
            : base("name=FIATDb")
        {
        }
        
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Study> Studys { get; set; }
        public virtual DbSet<Modality> Modalitys { get; set; }
        public virtual DbSet<BodyPart> BodyParts { get; set; }
        public virtual DbSet<Operator> Operators { get; set; }
    }
}
