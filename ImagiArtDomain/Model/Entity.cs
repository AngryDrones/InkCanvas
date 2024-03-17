using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagiArtDomain.Model
{
    public abstract class Entity
    {
        [Display(Name = "ID")]
        public int Id { get; set; }
    }
}
