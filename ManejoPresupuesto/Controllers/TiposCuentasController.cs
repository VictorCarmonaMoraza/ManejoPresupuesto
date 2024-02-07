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
        private readonly IServicioUsuarios _serviciosUsuarios;
        public TiposCuentasController(IRepositorioTiposCuentas respositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            _resitorioTiposCuentas = respositorioTiposCuentas;
            _serviciosUsuarios = servicioUsuarios;
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

            tipoCuenta.UsuarioId = _serviciosUsuarios.ObtenerUsuarioId();

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
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await _resitorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TipoCuenta tipoCuenta)
        {
            //Obtenemos el id del usuario
            var usuarioId = _serviciosUsuarios.ObtenerUsuarioId();

            //Comprobar que el tipo cuenta existe
            var tipoCuentaExiste = await _resitorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if (tipoCuentaExiste == null)
            {
                return RedirectToAction("No encontrado", "Home");
            }
            await _resitorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("ListarPorUsuarioId");
        }
        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuarioId();
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
            int usuarioId = _serviciosUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await _resitorioTiposCuentas.ListarPorUsuarioId(usuarioId);
            return View(tiposCuentas);
        }
    } 
}
