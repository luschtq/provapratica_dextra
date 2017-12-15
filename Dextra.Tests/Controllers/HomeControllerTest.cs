using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dextra;
using Dextra.Controllers;
using Dextra.DAO;

namespace Dextra.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PromocaoLight()
        {
            // Arrange
            PedidoController controller = new PedidoController();

            Dictionary<int, int> listaIngredienteQuantidade = new Dictionary<int, int>();

            bool promocao = true;

            // teste promoção Light 
            // primeiro item é referente a quantidade de alface
            // segundo item é referente a quantidade de bacon.
            // Se o lanche tem alface e não tem bacon, retorna true
            listaIngredienteQuantidade.Add(1, 3);
            listaIngredienteQuantidade.Add(2, 0);
            listaIngredienteQuantidade.Add(3, 1);

            var pedido = controller.Pedido_Gerar(null, null, listaIngredienteQuantidade);

            Assert.AreEqual(promocao, pedido.DescontoLight);

        }

        [TestMethod]
        public void PromocaoLight_Desconto()
        {
            // Arrange
            PedidoController controller = new PedidoController();

            Dictionary<int, int> listaIngredienteQuantidade = new Dictionary<int, int>();

            bool promocao = true;

            // teste promoção Light 
            // primeiro item é referente a quantidade de alface
            // segundo item é referente a quantidade de bacon.
            // Se o lanche tem alface e não tem bacon, retorna true
            // Valor de uma alface sem a inflação: 0.40
            // valor hamburger sem inflação: 3.00
            DadosDAO dadosDAO = new DadosDAO();

            decimal valorInflacao = dadosDAO.Inflacao_Listar().Sum(a => a.Valor);
            decimal valorDesconto = (Convert.ToDecimal(3.40) * (valorInflacao.Equals(0) ? 1 : valorInflacao)) * Convert.ToDecimal(0.10);

            listaIngredienteQuantidade.Add(1, 1);
            listaIngredienteQuantidade.Add(2, 0);
            listaIngredienteQuantidade.Add(3, 1);

            var pedido = controller.Pedido_Gerar(null, null, listaIngredienteQuantidade);

            Assert.AreEqual(valorDesconto, pedido.TotalDescontoLight);

        }

        [TestMethod]
        public void PromocaoMuitaCarne()
        {
            // Arrange
            PedidoController controller = new PedidoController();

            Dictionary<int, int> listaIngredienteQuantidade = new Dictionary<int, int>();

            bool promocao = true;

            // A cada 3 porções de carne o cliente só paga 2.Se o lanche tiver 6 porções, ocliente pagará 4. Assim por diante...
            listaIngredienteQuantidade.Add(1, 3);
            listaIngredienteQuantidade.Add(3, 4);
            var pedido = controller.Pedido_Gerar(null, null, listaIngredienteQuantidade);

            Assert.AreEqual(promocao, pedido.DescontoMuitaCarne);

        }

        [TestMethod]
        public void PromocaoMuitoQueijo()
        {
            // Arrange
            PedidoController controller = new PedidoController();

            Dictionary<int, int> listaIngredienteQuantidade = new Dictionary<int, int>();

            bool promocao = true;

            // A cada 3 porções de queijo o cliente só paga 2. Se o lanche tiver 6 porções, ocliente pagará 4. Assim por diante...
            listaIngredienteQuantidade.Add(1, 3);
            listaIngredienteQuantidade.Add(5, 4);
            var pedido = controller.Pedido_Gerar(null, null, listaIngredienteQuantidade);

            Assert.AreEqual(promocao, pedido.DescontoMuitoQueijo);

        }

        [TestMethod]
        public void Pedido_Gerar()
        {
            // Arrange
            PedidoController controller = new PedidoController();

            Dictionary<int, int> listaIngredienteQuantidade = new Dictionary<int, int>();

            bool promocao = true;

            // teste promoção Light 
            // primeiro item é referente a quantidade de alface
            // segundo item é referente a quantidade de bacon.
            // Se o lanche tem alface e não tem bacon, retorna true
            // Valor de uma alface sem a inflação: 0.40
            // valor hamburger sem inflação: 3.00
            DadosDAO dadosDAO = new DadosDAO();

            decimal valorInflacao = dadosDAO.Inflacao_Listar().Sum(a => a.Valor);

            listaIngredienteQuantidade.Add(2, 0);
            listaIngredienteQuantidade.Add(3, 3);
            listaIngredienteQuantidade.Add(4, 1);

            decimal totalPedido = (Convert.ToDecimal(0.40) + 6 + Convert.ToDecimal(0.8)) * (valorInflacao.Equals(0) ? 1 : valorInflacao);

            var pedido = controller.Pedido_Gerar(null, null, listaIngredienteQuantidade);

            Assert.AreEqual(totalPedido, pedido.TotalPedido);

        }

      
    }
}
