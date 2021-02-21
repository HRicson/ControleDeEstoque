using ControleDeEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ControleDeEstoque.Web.Controllers.Cadastro
{
    public class CadUnidadeMedidaController : Controller
    {
        private const int _quantMaxLinhasPorPagina = 5;

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(
                new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
            ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
            ViewBag.PaginaAtual = 1;

            List<UnidadeMedidaModel> lista = UnidadeMedidaModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
            int quant = UnidadeMedidaModel.RecuperarQuantidade();

            int difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
            ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

            return View(lista);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult UnidadeMedidaPagina(int pagina, int tamPag)
        {
            List<UnidadeMedidaModel> lista = UnidadeMedidaModel.RecuperarLista(pagina, tamPag);

            return Json(lista);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult RecuperarUnidadeMedida(int id)
        {
            return Json(UnidadeMedidaModel.RecuperarPeloId(id));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult ExcluirUnidadeMedida(int id)
        {
            return Json(UnidadeMedidaModel.ExcluirPeloId(id));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public JsonResult SalvarUnidadeMedida(UnidadeMedidaModel model)
        {
            var resultado = "OK";
            var mensagens = new List<string>();
            var idSalvo = string.Empty;

            if (!ModelState.IsValid)
            {
                resultado = "AVISO";
                mensagens = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage)
                    .ToList();
            }
            else
            {
                try
                {
                    int id = model.Salvar();

                    if (id > 0)
                        idSalvo = id.ToString();
                    else
                        resultado = "ERRO";
                }
                catch (Exception ex)
                {
                    resultado = "ERRO: " + ex.Message;
                }
            }

            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }
    }
}