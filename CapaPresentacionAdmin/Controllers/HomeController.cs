using CapaEntidad;
using CapaPresentacionAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using CapaNegocio;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;


namespace CapaPresentacionAdmin.Controllers


{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Usuarios()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GuardarUsuario( Usuario objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.ID_Usuario == 0)
            {
                resultado = new CN_Usuarios().Registrar(objeto, out mensaje);
               
            }
            else
            {
                resultado = new CN_Usuarios().Editar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje });

        }

        [HttpPost]
        public JsonResult EliminarUsuario(int id )
        {

            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Usuarios().Eliminar(id, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje });

        }

        [HttpGet]
        public JsonResult ListarUsuarios() { 
            List<Usuario> oLista = new List<Usuario>();
            oLista = new CN_Usuarios().Listar();

            return Json(new { data = oLista });
        }


        [HttpGet]
        public JsonResult VistaDashBoard()
        {
            Dashboard objeto = new CN_Reporte().VerDashboard();

            return Json(new { resultado = objeto});
        }


        [HttpGet]
        public JsonResult ListaReporte(string fechainicio, string fechafin, string idtransaccion)
        {
            List<Reporte> oLista = new List<Reporte>();

            oLista = new CN_Reporte().Ventas(fechainicio, fechafin, idtransaccion);

            return Json(new { data = oLista });
        }


            

        [HttpPost]
        public FileResult ExportarVenta(string fechainicio, string fechafin, string idtransaccion)
        {
            List<Reporte> oLista = new List<Reporte>();
            oLista = new CN_Reporte().Ventas(fechainicio, fechafin, idtransaccion);
            

            //CREA UN ELEMENTO DATATABLE CON LAS COLUMNAS Y LA DATA EN TIEMPO DE EJECUCION
            DataTable dt = new DataTable();

            dt.Locale = new System.Globalization.CultureInfo("hn-HN");
            dt.Columns.Add("Fecha Venta", typeof(string));
            dt.Columns.Add("Cliente", typeof(string));
            dt.Columns.Add("Producto", typeof(string));
            dt.Columns.Add("Precio", typeof(decimal));
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("ID_Transaccion", typeof(string));


            foreach (Reporte rp in oLista) {
                dt.Rows.Add(new object[]
                {
                    rp.FechaVenta,
                    rp.Cliente,
                    rp.Producto,
                    rp.Precio,
                    rp.Cantidad,
                    rp.Total,
                    rp.ID_Transaccion

                });

            }


            dt.TableName = "Datos";


            //ESTO ES PARA GUARDARLO COMO LIBRO DE EXCEL
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte Venta" + DateTime.Now.ToString() + ".xlsx");
                }
            }

        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
