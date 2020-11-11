using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Models
{
    public class Study
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime Date { get; set; }

        [ForeignKey("Operator")]
        public int OperatorID { get; set; }
        public virtual Operator Operator { get; set; }

        [ForeignKey("Patient")]
        public int PatientID { get; set; }
        public virtual Patient Patient { get; set; }

        [ForeignKey("Modality")]
        public string ModalityName { get; set; }
        public Modality Modality { get; set; }

        [ForeignKey("BodyPart")]
        public string BodyPartName { get; set; }
        public string BodyPart { get; set; }
    }
}
