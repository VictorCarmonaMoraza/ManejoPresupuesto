using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
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
                return View();
            }
            catch (Exception ex)
            {

                // Manejo de la excepción, posiblemente registrándola y mostrando un ensaje al usuario
                ModelState.AddModelError(string.Empty, "Ocurrió un error al crear el tipo de cuenta.");
                return View(tipoCuenta);
            }
        }
    } 
}
