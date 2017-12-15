using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dextra.Models
{
    public class PedidoModels
    {
        public int ID { get; set; }
        public int? LancheID { get; set; }

        public string LancheNome { get; set; }
        public bool DescontoLight { get; set; }
        public bool DescontoMuitaCarne { get; set; }
        public bool DescontoMuitoQueijo { get; set; }
        public decimal TotalDescontoMuitaCarne { get; set; }

        public decimal TotalDescontoMuitoQueijo { get; set; }
        public decimal TotalDescontoLight { get; set; }
        public decimal TotalPedido { get; set; }
        public decimal TotalDesconto { get; set; }

        public bool PossuiDesconto { get; set; }

        public List<IngredienteQuantidadeViewModels> IngredienteQuantidade { get; set; }
    }
}