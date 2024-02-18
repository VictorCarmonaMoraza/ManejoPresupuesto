using AutoMapper;
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
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del controlador CuentasController.
        /// </summary>
        /// <param name="repositorioTiposCuentas">Repositorio para operaciones relacionadas con los tipos de cuentas.</param>
        /// <param name="servicioUsuarios">Servicio para obtener información y operaciones relacionadas con los usuarios.</param>
        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas, IMapper mapper)
        {
            _repositorioTiposCuentas = repositorioTiposCuentas;
            _servicioUsuarios = servicioUsuarios;
            _repositorioCuentas = repositorioCuentas;
            _mapper = mapper;
        }

        /// <summary>
        /// Método asincrónico que maneja la solicitud GET para la página de índice del área de cuentas del usuario.
        /// </summary>
        /// <remarks>
        /// Este método realiza las siguientes funciones:
        /// 1. Obtiene el ID del usuario actual utilizando el servicio de usuarios.
        /// 2. Recupera todas las cuentas asociadas al usuario a través del repositorio de cuentas.
        /// 3. Agrupa las cuentas por el tipo de cuenta.
        /// 4. Construye un modelo de vista con un listado de cuentas y sus respectivos tipos para ser mostrado en la vista.
        /// </remarks>
        /// <returns>
        /// Retorna una vista con el modelo de vista de índice de cuentas (<see cref="IndiceCuentasViewModel"/>) que contiene la información de las cuentas del usuario agrupadas por tipo de cuenta.
        /// </returns>
        /// <example>
        /// Para acceder a este método, el cliente enviará una solicitud GET al endpoint asociado al método Index del controlador donde está definido.
        /// </example>
        public async Task<IActionResult> Index()
        {
            // Obtiene el ID del usuario que ahora mismo está hardcodeado
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            // Busca las cuentas del usuario
            var cuentasConTipoCuenta = await _repositorioCuentas.Buscar(usuarioId);

            // Construye el modelo de vista a partir de las cuentas agrupadas por tipo de cuenta
            var modelo = cuentasConTipoCuenta
                .GroupBy(x => x.TipoCuenta)
                .Select(grupo => new IndiceCuentasViewModel
                {
                    TipoCuenta = grupo.Key,
                    Cuentas = grupo.AsEnumerable()
                }).ToList();

            // Retorna la vista con el modelo de vista construido
            return View(modelo);
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

        //Metodo para editar una cuenta
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await _repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            //var modelo = new CuentaCreacionViewModel()
            //{
            // Id = cuenta.Id,
            // Nombre = cuenta.Nombre,
            // Descripcion = cuenta.Descripcion,
            // Balance = cuenta.Balance,
            // TipoCuentaId = cuenta.TipoCuentaId
            //};
            var modelo = _mapper.Map<CuentaCreacionViewModel>(cuenta);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await _repositorioCuentas.ObtenerPorId(cuentaEditar.TipoCuentaId, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tiposCuenta = await _repositorioTiposCuentas.ObtenerPorId(cuentaEditar.TipoCuentaId, usuarioId);
            if (tiposCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }


            await _repositorioCuentas.Actualizar(cuentaEditar);
            return RedirectToAction("Index");
        }

    }
}
