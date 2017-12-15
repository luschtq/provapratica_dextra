using Dextra.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dextra.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                bool arquivosExistentes = false;

                if (!Directory.Exists(@ConfigurationManager.AppSettings["EnderecoArquivos"]))
                {
                    Directory.CreateDirectory(@ConfigurationManager.AppSettings["EnderecoArquivos"]);

                    if (!System.IO.File.Exists(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "Ingrediente.json"))
                        System.IO.File.Create(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "Ingrediente.json");

                    if (!System.IO.File.Exists(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "Inflacao.json"))
                        System.IO.File.Create(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "Inflacao.json");

                    if (!System.IO.File.Exists(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "Lanche.json"))
                        System.IO.File.Create(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "Lanche.json");
                    if (!System.IO.File.Exists(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "LancheIngrediente.json"))
                        System.IO.File.Create(@ConfigurationManager.AppSettings["EnderecoArquivos"] + "LancheIngrediente.json");
                }
                else
                    arquivosExistentes = true;

                if (!arquivosExistentes)
                    ViewBag.Arquivos = "Arquivos criados com sucesso";
            }
            catch
            {
                ViewBag.Arquivos = "Erro na criação dos arquivos";
            }


            return View();
        }
    }
}