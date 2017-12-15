using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Dextra.Models
{
    [Serializable]
    public class CardapioViewModels
    {
        public int ID { get; set; }
        public int CardapioTipoID { get; set; }

        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Maior do que zero")]
        public int Quantidade { get; set; }

        public string Descricao { get; set; }

        public string Valor { get; set; }

        public bool Check { get; set; }

        public List<IngredienteModels> ListIngredientes { get; set; }
    }
}