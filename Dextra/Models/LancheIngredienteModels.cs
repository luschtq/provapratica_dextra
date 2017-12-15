using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dextra.Models
{
    public class LancheIngredienteModels
    {
        public int ID { get; set; }
        public int LancheID { get; set; }
        public int IngredienteID { get; set; }
    }
}