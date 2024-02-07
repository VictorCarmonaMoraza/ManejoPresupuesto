using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {


        private readonly IRepositorioTiposCuentas _resitorioTiposCuentas;
        public TiposCuentasController(IRepositorioTiposCuentas respositorioTiposCuentas)
        {
            _resitorioTiposCuentas = respositorioTiposCuentas;
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            //Si el modelo no es valido lo enviamos de nuevo al formulario
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = 1;

            try
            {
                //Comprobamos que la data existe de la siguiente
                bool yaExisteTipoCuenta = await _resitorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

                if (yaExisteTipoCuenta)
                {
                    ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe");
                    return View(tipoCuenta);
                }

                await _resitorioTiposCuentas.Crear(tipoCuenta);
                //var nuevoTipoCuenta = new TipoCuenta();
                //return View(nuevoTipoCuenta);
                return RedirectToAction("ListarPorUsuarioId");
            }
            catch (Exception ex)
            {

                // Manejo de la excepción, posiblemente registrándola y mostrando un ensaje al usuario
                ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el tipo de cuenta.");
                return View(tipoCuenta);
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = 1;
            var yaExisteTipoCuenta = await _resitorioTiposCuentas.Existe(nombre, usuarioId);

            if (yaExisteTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }

        /// <summary>
        /// Lista todos los tipos de cuentas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            //var usuarioId = 1;
            var tiposCuentas = await _resitorioTiposCuentas.Listar();
            return View(tiposCuentas);
        }

        /// <summary>
        /// Lista de tipo de cuenta por usuario id 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ListarPorUsuarioId()
        {
            int usuarioId = 1;
            var tiposCuentas = await _resitorioTiposCuentas.ListarPorUsuarioId(usuarioId);
            return View(tiposCuentas);
        }
    } 
}
