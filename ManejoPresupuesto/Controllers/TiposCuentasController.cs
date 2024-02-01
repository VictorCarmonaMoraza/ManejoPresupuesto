using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController: Controller
    {

        private readonly string connectionString;
        public TiposCuentasController(IConfiguration configuration) 
        {
            connectionString = configuration.GetConnectionString("RutaServerSQL");
        }
        public IActionResult Crear()
        {

            using (var connection = new SqlConnection(connectionString))
            {
                var query = connection.Query("SELECT 1").FirstOrDefault();
                    
            }
                //var modelo = new TipoCuenta() { Nombre = "pedro" };
                //return View(modelo);
                return View();
        }

        [HttpPost]
        public IActionResult Crear(TipoCuenta tipoCuenta)
        {
            //Si el modelo no es valido lo enviamos de nuevo al formulario
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            return View();
        }
    }
}
