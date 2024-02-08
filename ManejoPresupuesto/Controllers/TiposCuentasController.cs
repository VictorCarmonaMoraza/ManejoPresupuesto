using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    /// <summary>
    /// Controlador para manejar las operaciones de los tipos de cuentas.
    /// </summary>
    public class TiposCuentasController : Controller
    {

        #region variables
        private readonly IRepositorioTiposCuentas _resitorioTiposCuentas;
        private readonly IServicioUsuarios _serviciosUsuarios;
        #endregion variables

        #region constructor
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="TiposCuentasController"/> con los servicios necesarios.
        /// </summary>
        /// <param name="repositorioTiposCuentas">El repositorio para operaciones de tipos de cuentas.</param>
        /// <param name="servicioUsuarios">El servicio para operaciones de usuarios.</param>
        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios)
        {
            _resitorioTiposCuentas = repositorioTiposCuentas;
            _serviciosUsuarios = servicioUsuarios;
        }

        #endregion constructor
        public IActionResult Crear()
        {
            return View();
        }

        #region POST
        /// <summary>
        /// Procesa la solicitud de creación de un nuevo tipo de cuenta.
        /// </summary>
        /// <param name="tipoCuenta">La instancia de <see cref="TipoCuenta"/> que contiene los datos ingresados por el usuario.</param>
        /// <returns>
        /// Una vista de creación si el modelo no es válido o si la creación falla debido a un nombre de cuenta duplicado o un error inesperado.
        /// Redirige a la acción "ListarPorUsuarioId" si el tipo de cuenta se crea con éxito.
        /// </returns>
        /// <remarks>
        /// Este método realiza varias operaciones importantes:
        /// 1. Verifica si el modelo es válido. Si no lo es, retorna a la vista de creación con los datos ingresados para corrección por parte del usuario.
        /// 2. Asigna el ID del usuario actual al tipo de cuenta, basándose en la sesión o contexto del usuario.
        /// 3. Verifica si ya existe un tipo de cuenta con el mismo nombre para el usuario actual. Si existe, agrega un error al modelo y retorna a la vista de creación.
        /// 4. Intenta crear el nuevo tipo de cuenta en la base de datos. Si la operación es exitosa, redirige al usuario a la lista de sus tipos de cuenta.
        /// 5. Captura cualquier excepción no controlada durante el proceso de creación y agrega un mensaje de error genérico al modelo, retornando a la vista de creación para informar al usuario.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = _serviciosUsuarios.ObtenerUsuarioId();

            try
            {
                bool yaExisteTipoCuenta = await _resitorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);

                if (yaExisteTipoCuenta)
                {
                    ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe.");
                    return View(tipoCuenta);
                }

                await _resitorioTiposCuentas.Crear(tipoCuenta);
                return RedirectToAction("ListarPorUsuarioId");
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el tipo de cuenta.");
                return View(tipoCuenta);
            }
        }

        /// <summary>
        /// Procesa la solicitud de edición de un tipo de cuenta existente.
        /// </summary>
        /// <param name="tipoCuenta">La instancia de <see cref="TipoCuenta"/> que contiene los datos actualizados ingresados por el usuario.</param>
        /// <returns>
        /// Redirige a la acción "ListarPorUsuarioId" si la actualización es exitosa.
        /// Redirige a la acción "NoEncontrado" del controlador "Home" si el tipo de cuenta a editar no existe o no pertenece al usuario actual.
        /// </returns>
        /// <remarks>
        /// Este método realiza las siguientes operaciones:
        /// 1. Obtiene el ID del usuario actual para asegurar que solo se puedan editar los tipos de cuenta que pertenecen al usuario autenticado.
        /// 2. Verifica que el tipo de cuenta a editar exista en la base de datos y que pertenezca al usuario actual.
        /// 3. Si el tipo de cuenta existe y pertenece al usuario, se procede con la actualización de los datos en la base de datos.
        /// 4. Si la actualización es exitosa, redirige al usuario a la lista de sus tipos de cuenta.
        /// 5. Si el tipo de cuenta no se encuentra o no pertenece al usuario, redirige al usuario a una página de error o página no encontrada.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> EditarTipoCuenta(TipoCuenta tipoCuenta)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuarioId();

            var tipoCuentaExiste = await _resitorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            if (tipoCuentaExiste == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _resitorioTiposCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("ListarPorUsuarioId");
        }

        /// <summary>
        /// Procesa la solicitud de eliminación de un tipo de cuenta existente.
        /// </summary>
        /// <param name="tipoCuenta">La instancia de <see cref="TipoCuenta"/> que incluye el ID del tipo de cuenta a eliminar.</param>
        /// <returns>
        /// Redirige a la acción "ListarPorUsuarioId" si la eliminación es exitosa.
        /// Retorna una vista "NotFound" si el ID de usuario no se encuentra.
        /// Redirige a la acción "NoEncontrado" del controlador "Home" si el tipo de cuenta a eliminar no existe o no pertenece al usuario actual.
        /// </returns>
        /// <remarks>
        /// Este método realiza las siguientes operaciones:
        /// 1. Obtiene el ID del usuario actual para asegurar que solo se puedan eliminar los tipos de cuenta que pertenecen al usuario autenticado.
        /// 2. Verifica que el tipo de cuenta a eliminar exista en la base de datos y que pertenezca al usuario actual.
        /// 3. Si el tipo de cuenta existe y pertenece al usuario, procede con la eliminación del tipo de cuenta de la base de datos.
        /// 4. Si la eliminación es exitosa, redirige al usuario a la lista de sus tipos de cuenta.
        /// 5. Si el tipo de cuenta no se encuentra o no pertenece al usuario, redirige al usuario a una página de error o página no encontrada.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> EliminarPorId(TipoCuenta tipoCuenta)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuarioId();
            if (usuarioId == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var tipoCuentaExiste = await _resitorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);
            if (tipoCuentaExiste == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await _resitorioTiposCuentas.Eliminar(tipoCuenta.Id);
            return RedirectToAction("ListarPorUsuarioId");
        }

        /// <summary>
        /// Reordena los tipos de cuenta de un usuario basándose en una nueva secuencia de IDs proporcionada.
        /// </summary>
        /// <param name="ids">Una matriz de enteros que representa la nueva secuencia de IDs de los tipos de cuenta, en el orden deseado.</param>
        /// <returns>
        /// Retorna una respuesta HTTP 200 OK si la reordenación se completa con éxito.
        /// </returns>
        /// <remarks>
        /// Este método realiza las siguientes operaciones:
        /// 1. Obtiene el ID del usuario actual para asegurar que solo se puedan reordenar los tipos de cuenta que pertenecen al usuario autenticado.
        /// 2. Llama al método 'OrdenarForma1' del repositorio de tipos de cuentas, pasándole el ID del usuario y la nueva secuencia de IDs para actualizar el orden de los tipos de cuenta en la base de datos.
        /// 3. Retorna una respuesta HTTP 200 OK para indicar que la operación se completó con éxito.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> OrdenarTablaForma1([FromBody] int[] ids)
        {
            int usuarioId = _serviciosUsuarios.ObtenerUsuarioId();

            await _resitorioTiposCuentas.OrdenarForma1(usuarioId, ids);

            return Ok();
        }

        /// <summary>
        /// Reordena los tipos de cuenta de un usuario basándose en un arreglo de IDs proporcionado en el cuerpo de la solicitud.
        /// Este método garantiza que solo los tipos de cuenta que pertenecen al usuario actual sean reordenados, rechazando cualquier
        /// intento de modificar tipos de cuenta que no le pertenecen al usuario.
        /// </summary>
        /// <param name="ids">Arreglo de enteros que representa los IDs de los tipos de cuenta en el nuevo orden deseado.</param>
        /// <returns>
        /// Un resultado de acción Ok() si el reordenamiento es exitoso, indicando que los tipos de cuenta han sido actualizados correctamente.
        /// Un resultado de acción Forbid() si se detecta algún ID en la lista proporcionada que no pertenece a los tipos de cuenta del usuario actual,
        /// indicando un intento no autorizado de reordenar tipos de cuenta que no son del usuario.
        /// </returns>
        /// <remarks>
        /// El proceso de reordenamiento sigue los siguientes pasos:
        /// 1. Se obtiene el ID del usuario actual para asegurar la operación dentro del contexto del usuario autenticado.
        /// 2. Se recuperan todos los tipos de cuenta asociados al usuario actual y se extraen sus IDs para verificar la propiedad.
        /// 3. Se compara la lista de IDs proporcionada con la lista de IDs de los tipos de cuenta del usuario para asegurar que todos los IDs pertenezcan al usuario actual.
        ///    - Si se encuentra algún ID que no pertenezca a los tipos de cuenta del usuario, se retorna un resultado Forbid().
        /// 4. Para cada ID válido, se crea un nuevo objeto TipoCuenta con el orden correspondiente basado en su posición en la lista proporcionada.
        /// 5. Se invoca el método correspondiente en el repositorio para actualizar el orden de los tipos de cuenta en la base de datos.
        /// Este enfoque asegura que solo los tipos de cuenta que pertenecen al usuario sean reordenados, manteniendo la integridad y la seguridad de los datos del usuario.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> OrdenarTablaForma2([FromBody] int[] ids)
        {
            int usuarioId = _serviciosUsuarios.ObtenerUsuarioId();

            //Obtiene los tipos cuentas por usuarioId
            var tiposCuentas = await _resitorioTiposCuentas.ListarPorUsuarioId(usuarioId);
            //Obtenemos los id de los tiposCuentas obtenidos
            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0)
            {
                //return BadRequest("Los ids no pertenecen al usuario");
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) =>
            new TipoCuenta()
            {
                Id = valor,
                Orden = indice,
                Nombre = tiposCuentas.First(x => x.Id == valor).Nombre
            }).AsEnumerable();

            await _resitorioTiposCuentas.OrdenarForma2(tiposCuentasOrdenados);

            return Ok();
        }
        #endregion POST

        #region GET

        /// <summary>
        /// Muestra la vista para editar un tipo de cuenta existente.
        /// </summary>
        /// <param name="id">El identificador único del tipo de cuenta a editar.</param>
        /// <returns>
        /// Una vista de edición prellenada con los datos del tipo de cuenta si se encuentra.
        /// Redirige a la acción "NoEncontrado" del controlador "Home" si el tipo de cuenta no existe o no pertenece al usuario actual.
        /// </returns>
        /// <remarks>
        /// Este método realiza las siguientes operaciones:
        /// 1. Obtiene el ID del usuario actual para asegurar que el tipo de cuenta pertenezca al usuario que está intentando editarla.
        /// 2. Busca en la base de datos el tipo de cuenta correspondiente al ID proporcionado y que pertenezca al usuario actual.
        /// 3. Si el tipo de cuenta existe, retorna la vista de edición con los datos del tipo de cuenta para que el usuario pueda modificarlos.
        /// 4. Si el tipo de cuenta no se encuentra (es null), redirige al usuario a una vista de error o página no encontrada para indicar que el recurso solicitado no está disponible o no tiene permiso para acceder a él.
        /// </remarks>
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

        /// <summary>
        /// Verifica si ya existe un tipo de cuenta con el nombre especificado para el usuario actual.
        /// </summary>
        /// <param name="nombre">El nombre del tipo de cuenta a verificar.</param>
        /// <returns>
        /// Retorna un objeto JSON con un mensaje indicando que el nombre ya existe si se encuentra un tipo de cuenta con el mismo nombre para el usuario actual.
        /// Retorna un objeto JSON con valor true si no se encuentra un tipo de cuenta con el mismo nombre, indicando que el nombre está disponible.
        /// </returns>
        /// <remarks>
        /// Este método es útil para realizar validaciones del lado del cliente antes de intentar crear o actualizar un tipo de cuenta,
        /// asegurando que los nombres de los tipos de cuenta sean únicos para cada usuario. La verificación se realiza dentro del contexto
        /// del usuario autenticado para mantener la integridad de los datos y la privacidad entre los usuarios.
        /// Al retornar información en formato JSON, este método es ideal para ser consumido por llamadas AJAX, facilitando la creación de
        /// interfaces de usuario interactivas y dinámicas que responden en tiempo real a la entrada del usuario sin necesidad de recargar la página.
        /// </remarks>
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
        /// Obtiene y muestra una lista de todos los tipos de cuenta disponibles en el sistema.
        /// </summary>
        /// <returns>
        /// Retorna una vista con la lista completa de tipos de cuenta. Esta lista incluye todos los tipos de cuenta registrados en el sistema, 
        /// sin filtrar por usuario específico.
        /// </returns>
        /// <remarks>
        /// Este método es útil para obtener una visión general de todos los tipos de cuenta existentes en el sistema. Puede ser especialmente 
        /// relevante en contextos donde se requiere que los administradores o usuarios con roles específicos tengan una visión completa, 
        /// a diferencia de la vista filtrada por usuario proporcionada por otros métodos como <see cref="ListarPorUsuarioId"/>.
        /// Este enfoque asegura que se pueda tener acceso a una lista completa cuando sea necesario, mientras se mantienen separadas las vistas 
        /// específicas del usuario para operaciones cotidianas y de gestión personal.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            //var usuarioId = 1;
            var tiposCuentas = await _resitorioTiposCuentas.Listar();
            return View(tiposCuentas);
        }


        /// <summary>
        /// Obtiene y muestra una lista de todos los tipos de cuenta asociados al usuario actual.
        /// </summary>
        /// <returns>
        /// Retorna una vista con la lista de tipos de cuenta pertenecientes al usuario actual.
        /// Si el usuario no tiene tipos de cuenta, la lista estará vacía, pero aún así se mostrará la vista.
        /// </returns>
        /// <remarks>
        /// Este método realiza las siguientes operaciones:
        /// 1. Obtiene el identificador del usuario actual mediante el servicio de usuarios para asegurar que los datos recuperados pertenezcan al usuario autenticado.
        /// 2. Utiliza el repositorio de tipos de cuentas para obtener una lista de todos los tipos de cuenta asociados al identificador del usuario.
        /// 3. Retorna la vista correspondiente, pasando la lista de tipos de cuenta como modelo. Esta vista mostrará los tipos de cuenta al usuario, permitiéndole gestionarlos (editar, eliminar, etc.).
        /// Este enfoque asegura que los usuarios solo puedan ver y gestionar sus propios tipos de cuenta, manteniendo la privacidad y seguridad de los datos.
        /// </remarks>
        [HttpGet]
        public async Task<IActionResult> ListarPorUsuarioId()
        {
            int usuarioId = _serviciosUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await _resitorioTiposCuentas.ListarPorUsuarioId(usuarioId);
            return View(tiposCuentas);
        }

        #endregion GET

        #region funciones alternativas

        /// <summary>
        /// Muestra la vista de confirmación para eliminar un tipo de cuenta específico.
        /// </summary>
        /// <param name="id">El identificador del tipo de cuenta que se desea eliminar.</param>
        /// <returns>
        /// Retorna la vista de confirmación de eliminación si se encuentra el tipo de cuenta y pertenece al usuario actual.
        /// Retorna un resultado de acción NotFound si el ID del usuario no se encuentra, indicando que el usuario no está autenticado o no existe.
        /// Redirige a la acción "NoEncontrado" en el controlador "Home" si el tipo de cuenta no existe o no pertenece al usuario actual.
        /// </returns>
        /// <remarks>
        /// Este método realiza las siguientes operaciones:
        /// 1. Obtiene el ID del usuario actual para asegurar que la operación se realiza dentro del contexto de un usuario autenticado.
        /// 2. Intenta recuperar el tipo de cuenta especificado por el ID que pertenezca al usuario actual.
        /// 3. Si el tipo de cuenta no se encuentra o no pertenece al usuario, se redirige al usuario a una página de error o página no encontrada.
        /// 4. Si se encuentra el tipo de cuenta y pertenece al usuario, se muestra la vista de confirmación de eliminación con los detalles del tipo de cuenta.
        /// Este enfoque asegura que solo el propietario del tipo de cuenta pueda solicitar su eliminación, manteniendo la seguridad y la integridad de los datos del usuario.
        /// </remarks>
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuarioId = _serviciosUsuarios.ObtenerUsuarioId();
            if (usuarioId == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var tipoCuenta = await _resitorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }


            //await _resitorioTiposCuentas.Eliminar(id);
            return View(tipoCuenta);
        }

        #endregion funciones alternativas
    }
}
