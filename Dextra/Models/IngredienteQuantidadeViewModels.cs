using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dextra.Models
{
    [Serializable]
    public class IngredienteQuantidadeViewModels
    {
        public int IngredienteID { get; set; }

        public string Nome { get; set; }

        public int Quantidade { get; set; }

        public decimal Valor { get; set; }
    }
}