using Dextra.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Dextra.DAO
{
    public class DadosDAO
    {
        #region Ingrediente
        public List<IngredienteModels> Ingrediente_Listar()
        {
            var lista = new List<IngredienteModels>();
            try
            {
                var arquivoJson = System.IO.File.ReadAllText(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "Ingrediente.json", System.Text.Encoding.Default);
                lista = JsonConvert.DeserializeObject<List<IngredienteModels>>(arquivoJson);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lista;
        }

        public IngredienteModels Ingrediente_Selecionar(int ID)
        {
            try
            {
                var ingrediente = Ingrediente_Listar().Where(a => a.ID == ID).FirstOrDefault();

                var listaInflacao = Inflacao_Listar();

                foreach (InflacaoModels inflacao in listaInflacao)
                    ingrediente.Valor = (Convert.ToDecimal(ingrediente.Valor) + ((Convert.ToDecimal(ingrediente.Valor) * inflacao.Valor) / 100)).ToString();

                return ingrediente;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Ingrediente

        #region Lanche
        public List<LancheModels> Lanche_Listar()
        {
            var lista = new List<LancheModels>();
            try
            {
                var arquivoJson = System.IO.File.ReadAllText(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "Lanche.json", System.Text.Encoding.Default);
                lista = JsonConvert.DeserializeObject<List<LancheModels>>(arquivoJson);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lista;
        }

        public LancheModels Lanche_Selecionar(int ID)
        {
            try
            {
                return Lanche_Listar().Where(a => a.ID == ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion Lanche

        #region LancheIngrediente
        public List<LancheIngredienteModels> LancheIngrediente_Listar()
        {
            var lista = new List<LancheIngredienteModels>();
            try
            {
                var arquivoJson = System.IO.File.ReadAllText(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "LancheIngrediente.json", System.Text.Encoding.Default);
                lista = JsonConvert.DeserializeObject<List<LancheIngredienteModels>>(arquivoJson);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lista;
        }

        public List<LancheIngredienteModels> LancheIngrediente_ListarPorLancheID(int lancheID)
        {

            try
            {
                var lista = LancheIngrediente_Listar();
                return lista.Where(a => a.LancheID == lancheID).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LancheIngredienteModels LancheIngrediente_Selecionar(int ID)
        {
            try
            {
                return LancheIngrediente_Listar().Where(a => a.ID == ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion LancheIngrediente

        #region Inflação
        public List<InflacaoModels> Inflacao_Listar()
        {
            var lista = new List<InflacaoModels>();
            try
            {
                var arquivoJson = System.IO.File.ReadAllText(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "Inflacao.json", System.Text.Encoding.Default);
                lista = JsonConvert.DeserializeObject<List<InflacaoModels>>(arquivoJson);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return lista;
        }

        #endregion LancheIngrediente

        #region Cardapio
        public List<CardapioViewModels> Cardapio_Listar()
        {
            var listaIngredientes = Ingrediente_Listar();
            var listaLanches = Lanche_Listar();
            var listaLanchesIngredientes = LancheIngrediente_Listar();

            var listaCardapioView = new List<CardapioViewModels>();
            var cardapio = new CardapioViewModels();

            foreach (IngredienteModels ingrediente in listaIngredientes)
            {
                cardapio = new CardapioViewModels();
                cardapio.ID = ingrediente.ID;
                cardapio.Descricao = ingrediente.Nome;
                cardapio.Valor = ingrediente.Valor.ToString();
                cardapio.CardapioTipoID = 1;
                listaCardapioView.Add(cardapio);
            }

            var listaLancheIngredienteID = new List<int>();
            var listaIngredientesPesquisa = new List<IngredienteModels>();
            var ingredientePesquisa = new Models.IngredienteModels();

            foreach (LancheModels lanche in listaLanches)
            {
                cardapio = new CardapioViewModels();
                cardapio.Descricao = lanche.Nome;
                cardapio.ID = lanche.ID;
                cardapio.ListIngredientes = new List<IngredienteModels>();

                listaLancheIngredienteID = listaLanchesIngredientes.Where(a => a.LancheID == lanche.ID).Select(a => a.IngredienteID).ToList();
                listaIngredientesPesquisa = new List<IngredienteModels>();

                foreach (int ingredienteID in listaLancheIngredienteID)
                {
                    ingredientePesquisa = listaIngredientes.Where(a => a.ID == ingredienteID).FirstOrDefault();

                    if (!listaIngredientesPesquisa.Contains(ingredientePesquisa))
                        listaIngredientesPesquisa.Add(ingredientePesquisa);
                }

                cardapio.ListIngredientes.AddRange(listaIngredientesPesquisa);
                cardapio.CardapioTipoID = 2;

                listaCardapioView.Add(cardapio);
            }

            return listaCardapioView;
        }
        #endregion Cardapio
    }
}