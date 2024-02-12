using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;

namespace ManejoPresupuesto.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar las operaciones sobre las cuentas en la aplicación.
    /// </summary>
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
        private readonly IServicioUsuarios _servicioUsuarios;
        private readonly IRepositorioCuentas _repositorioCuentas;

        /// <summary>
        /// Inicializa una nueva instancia del controlador CuentasController.
        /// </summary>
        /// <param name="repositorioTiposCuentas">Repositorio para operaciones relacionadas con los tipos de cuentas.</param>
        /// <param name="servicioUsuarios">Servicio para obtener información y operaciones relacionadas con los usuarios.</param>
        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas)
        {
            _repositorioTiposCuentas = repositorioTiposCuentas;
            _servicioUsuarios = servicioUsuarios;
            _repositorioCuentas = repositorioCuentas;
        }

        /// <summary>
        /// Muestra la vista para la creación de una nueva cuenta, proporcionando los tipos de cuentas disponibles para el usuario actual.
        /// </summary>
        /// <returns>
        /// Retorna la vista 'Crear' junto con un modelo que incluye los tipos de cuentas disponibles para el usuario actual,
        /// permitiendo seleccionar uno de ellos al crear la cuenta.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await _repositorioTiposCuentas.ListarPorUsuarioId(usuarioId);
            var modelo = new CuentaCreacionViewModel();

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);
        }

        //Cera una cuenta
        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {

            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();

            var tiposCuenta = await _repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId,usuarioId);

            if (tiposCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }


            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }
            await _repositorioCuentas.Crear(cuenta);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await _repositorioTiposCuentas.ListarPorUsuarioId(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
