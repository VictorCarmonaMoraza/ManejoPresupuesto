using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar las operaciones sobre las cuentas en la aplicación.
    /// </summary>
    public class CuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas _repositorioTiposCuentas;
        private readonly IServicioUsuarios _servicioUsuarios;

        /// <summary>
        /// Inicializa una nueva instancia del controlador CuentasController.
        /// </summary>
        /// <param name="repositorioTiposCuentas">Repositorio para operaciones relacionadas con los tipos de cuentas.</param>
        /// <param name="servicioUsuarios">Servicio para obtener información y operaciones relacionadas con los usuarios.</param>
        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            _repositorioTiposCuentas = repositorioTiposCuentas;
            _servicioUsuarios = servicioUsuarios;
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

            modelo.TiposCuentas = tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));

            return View(modelo);
        }
    }
}
