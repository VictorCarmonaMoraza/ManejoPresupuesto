using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace ManejoPresupuesto.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioCategorias _repositorioCategorias;
        private readonly IServicioUsuarios _servicioUsuarios;

        // Constructor de la clase que inicializa las dependencias necesarias.
        // repositorioCategorias: Proporciona operaciones de acceso a datos para las categorías.
        // servicioUsuarios: Proporciona operaciones relacionadas con los usuarios, como obtener el ID del usuario actual.
        public CategoriasController(IRepositorioCategorias repositorioCategorias, IServicioUsuarios servicioUsuarios)
        {
            _repositorioCategorias = repositorioCategorias;
            _servicioUsuarios = servicioUsuarios;
        }

        // Acción HTTP GET para mostrar la vista de creación de una nueva categoría.
        // Retorna la vista correspondiente sin ningún modelo asociado.
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        // Acción HTTP POST para procesar la creación de una nueva categoría.
        // categoria: El objeto Categoria que contiene los datos ingresados por el usuario en el formulario.
        // Realiza la validación del modelo y, si es válido, asigna el ID del usuario a la categoría,
        // guarda la categoría en la base de datos y redirige a la acción "Index".
        // Si el modelo no es válido, vuelve a mostrar la vista de creación con los datos ingresados.
        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            categoria.UsuarioId = usuarioId;
            await _repositorioCategorias.Crear(categoria);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var categorias = await _repositorioCategorias.ObtenerCategoriasPorUsuario(usuarioId);
            return View(categorias);
        }
        
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var categoria = await _repositorioCategorias.ObtenerCategoriaPorId(id, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoriaEditar)
        {
            if (!ModelState.IsValid)
            {
                return View(categoriaEditar);
            }

            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var categoria = await _repositorioCategorias.ObtenerCategoriaPorId(categoriaEditar.Id, usuarioId);
            
            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            
            categoriaEditar.UsuarioId = usuarioId;

            await _repositorioCategorias.Actualizar(categoriaEditar);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var categoria = await _repositorioCategorias.ObtenerCategoriaPorId(id, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            return View(categoria);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(int id)
        {
            var usuarioId = _servicioUsuarios.ObtenerUsuarioId();
            var categoria = await _repositorioCategorias.ObtenerCategoriaPorId(id, usuarioId);

            if (categoria is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            
            await _repositorioCategorias.Borrar(id);
            return RedirectToAction("Index");
        }   
    }
}
