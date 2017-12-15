using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dextra.Models
{
    [Serializable]
    public class IngredienteModels
    {
        public int ID { get; set; }

        public string Nome { get; set; }

        public string Valor { get; set; }
    }
}