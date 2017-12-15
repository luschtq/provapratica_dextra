using Dextra.DAO;
using Dextra.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dextra.Controllers
{
    public class PedidoController : Controller
    {
        private DadosDAO _dadosDAO;


        private DadosDAO DAODados
        {
            get
            {
                if (_dadosDAO == null)
                    _dadosDAO = new DadosDAO();
                return _dadosDAO;
            }
        }

        public ActionResult Index()
        {
            if (Session["Cardapio"] == null)
                Session["Cardapio"] = DAODados.Cardapio_Listar();

            return View((List<CardapioViewModels>)Session["Cardapio"]);
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            #region Variáveis

            bool lancheSelecionado = false;
            int? lancheIDSelecionado = null;
            Dictionary<int, int> listaIngredienteQuantidade = new Dictionary<int, int>();
            int countQuantidade = 0;
            string lancheNome = null;
            #endregion Variáveis

            #region Valores Form
            var listaID = formCollection["item.ID"].Split(',');
            var listaCardapioTipoID = formCollection["item.CardapioTipoID"].Split(',');

            if (formCollection["item.Check"] != null)
                lancheSelecionado = true;

            var listaQuantidade = formCollection["item.Quantidade"].Split(',');

            #endregion Valores Form

            for (int i = 0; i < listaID.Length; i++)
            {
                if (listaCardapioTipoID[i].Equals("2"))
                {
                    if (lancheSelecionado)
                    {
                        if (!lancheIDSelecionado.HasValue)
                        {
                            lancheIDSelecionado = Convert.ToInt32(formCollection["item.Check"]);
                            lancheNome = DAODados.Lanche_Selecionar(lancheIDSelecionado.Value).Nome;
                        }
                    }
                }
                else if (listaCardapioTipoID[i].Equals("1"))
                {
                    if (!string.IsNullOrEmpty(listaQuantidade[countQuantidade]))
                        listaIngredienteQuantidade.Add(Convert.ToInt32(listaID[i]), Convert.ToInt32(listaQuantidade[countQuantidade]));
                    countQuantidade++;
                }
            }

            var pedido = Pedido_Gerar(lancheIDSelecionado, lancheNome, listaIngredienteQuantidade);

            List<PedidoModels> listaPedidoSession = new List<PedidoModels>();

            if (Session["Pedido"] == null)
            {
                listaPedidoSession.Add(pedido);
                Session["Pedido"] = listaPedidoSession;
            }
            else
            {
                listaPedidoSession = (List<PedidoModels>)Session["Pedido"];
                listaPedidoSession.Add(pedido);
                Session["Pedido"] = listaPedidoSession;
            }

            ViewBag.FinalizarPedido = true;
            //return View(pedido);

            if (Session["Cardapio"] == null)
                Session["Cardapio"] = DAODados.Cardapio_Listar();

            return View((List<CardapioViewModels>)Session["Cardapio"]);
        }

        [HttpPost]
        public ActionResult FinalizarPedido()
        {
            try
            {
                var lista = (List<PedidoModels>)Session["Pedido"];
                Session.Remove("Pedido");
                Session.Remove("Cardapio");

                if (Session["PedidoNumero"] == null)
                    Session["PedidoNumero"] = 1;
                else
                    Session["PedidoNumero"] = Convert.ToInt32(Session["PedidoNumero"]) + 1;

                return View("Pedido", lista);
            }
            catch
            {
                return View("Index");
            }
        }

        public PedidoModels Pedido_Gerar(int? lancheID,
                                          string lancheNome,
                                          Dictionary<int, int> ListaIngredienteQuantidade)
        {
            #region Variáveis
            bool descontoLight = false;
            bool descontoMuitaCarne = false;
            bool descontoMuitoQueijo = false;
            #endregion Variáveis

            #region Dados
            try
            {
                if (lancheID.HasValue)
                {
                    int quantidade;
                    List<LancheIngredienteModels> listaIngredientesLanche = DAODados.LancheIngrediente_ListarPorLancheID(lancheID.Value);

                    foreach (LancheIngredienteModels lancheIngrediente in listaIngredientesLanche)
                    {
                        if (ListaIngredienteQuantidade.Where(a => a.Key == lancheIngrediente.IngredienteID).Count().Equals(0))
                            ListaIngredienteQuantidade.Add(lancheIngrediente.IngredienteID, 1);
                        else
                        {
                            quantidade = ListaIngredienteQuantidade.Where(a => a.Key == lancheIngrediente.IngredienteID).FirstOrDefault().Value;
                            ListaIngredienteQuantidade.Remove(lancheIngrediente.IngredienteID);
                            ListaIngredienteQuantidade.Add(lancheIngrediente.IngredienteID, quantidade + 1);
                        }
                    }
                }

                #endregion Dados

                #region Pedido
                PedidoModels pedido = new PedidoModels();
                pedido.LancheID = lancheID;
                pedido.LancheNome = lancheNome;
                pedido.IngredienteQuantidade = new List<IngredienteQuantidadeViewModels>();
                var ingredientePesquisa = new IngredienteQuantidadeViewModels();
                IngredienteModels ingrediente = new IngredienteModels();
                foreach (var item in ListaIngredienteQuantidade)
                {
                    ingredientePesquisa = new IngredienteQuantidadeViewModels();
                    ingredientePesquisa.IngredienteID = item.Key;
                    ingredientePesquisa.Quantidade = item.Value;
                    ingrediente = DAODados.Ingrediente_Selecionar(ingredientePesquisa.IngredienteID);
                    ingredientePesquisa.Nome = ingrediente.Nome;
                    ingredientePesquisa.Valor = Convert.ToDecimal(ingrediente.Valor) * ingredientePesquisa.Quantidade;
                    pedido.IngredienteQuantidade.Add(ingredientePesquisa);
                }

                pedido.ID = 1;
                #endregion Pedido


                #region Regras de negócio
                //Light - Se o lanche tem alface e não tem bacon, ganha 10 % de descon
                if (pedido.IngredienteQuantidade.Where(a => a.IngredienteID == 1 && a.Quantidade > 0).Count() > 0
                    && pedido.IngredienteQuantidade.Where(a => a.IngredienteID == 2 && a.Quantidade > 0).Count().Equals(0))
                {
                    descontoLight = true;
                    pedido.PossuiDesconto = true;
                    pedido.TotalDesconto = (pedido.IngredienteQuantidade.Sum(a => a.Valor) * 10) / 100;
                    pedido.TotalDescontoLight = pedido.TotalDesconto;
                }

                // Muita carne - A cada 3 porções de carne o cliente só paga 2.Se o lanche tiver 6 porções, ocliente pagará 4. Assim por diante...
                if (pedido.IngredienteQuantidade.Where(a => a.IngredienteID == 3 && a.Quantidade> 2).Count() > 0)
                {
                    var ingredienteMuitaCarne = pedido.IngredienteQuantidade.Where(a => a.IngredienteID == 3).FirstOrDefault();
                    descontoMuitaCarne = true;
                    int quantidade = ingredienteMuitaCarne.Quantidade / 3;
                    var valor = DAODados.Ingrediente_Selecionar(3).Valor;
                    pedido.TotalDescontoMuitaCarne = Convert.ToDecimal(valor) * quantidade;

                    pedido.TotalDesconto += pedido.TotalDescontoMuitaCarne;
                    pedido.PossuiDesconto = true;
                }

                // Muito queijo - A cada 3 porções de queijo o cliente só paga 2.Se o lanche tiver 6 porções, ocliente pagará 4.Assim por diante...
                if (pedido.IngredienteQuantidade.Where(a => a.IngredienteID == 5 && a.Quantidade > 2).Count() > 0)
                {
                    var ingredienteMuitoQueijo = pedido.IngredienteQuantidade.Where(a => a.IngredienteID == 5).FirstOrDefault();
                    descontoMuitoQueijo = true;
                    int quantidade = ingredienteMuitoQueijo.Quantidade / 3;
                    var valor = DAODados.Ingrediente_Selecionar(5).Valor;
                    pedido.TotalDescontoMuitoQueijo = Convert.ToDecimal(valor) * quantidade;
                    pedido.TotalDesconto += pedido.TotalDescontoMuitoQueijo;
                    pedido.PossuiDesconto = true;
                }

                // Inflação - Os valores dos ingredientes são alterados com frequência e não gastaríamos que isso influenciasse nos testes automatizados.

                #endregion Regras de negócio

                #region Pedido

                pedido.DescontoLight = descontoLight;
                pedido.DescontoMuitaCarne = descontoMuitaCarne;
                pedido.DescontoMuitoQueijo = descontoMuitoQueijo;

                pedido.TotalPedido = pedido.IngredienteQuantidade.Sum(a => a.Valor) - pedido.TotalDesconto;

                #endregion Pedido

                return pedido;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}