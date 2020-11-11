using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIAT_Project.Core.Models
{
    public class Modality
    {
        [Key]
        public string Name { get; set; }
    }
}
