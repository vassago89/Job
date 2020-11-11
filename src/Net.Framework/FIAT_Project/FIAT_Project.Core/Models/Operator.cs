using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Models
{
    public class Operator
    {
        [Key]
        public int ID { get; set; }
        public byte[] Hash { get; private set; }
    }
}
