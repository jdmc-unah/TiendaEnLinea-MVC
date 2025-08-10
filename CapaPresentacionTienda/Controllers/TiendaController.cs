using Microsoft.AspNetCore.Mvc;
using CapaEntidad;
using CapaNegocio;
using CapaDatos;
using System.Web;
using CapaPresentacionTienda.Models;
using Microsoft.AspNetCore.Http;
using System.Runtime.CompilerServices;
using System.Data;
using Newtonsoft.Json;

using CapaEntidad.PayPal;
using System.Globalization;

namespace CapaPresentacionTienda.Controllers
{
    public class TiendaController : Controller
    {


        public IActionResult Index()
        {
            

            return View();
        }


        public IActionResult DetalleProducto(int idproducto)
        {
            

            Producto oProducto = new Producto();
            bool conversion;

            oProducto= new CN_Productos().Listar().Where( p => p.ID_Prod == idproducto  ).FirstOrDefault();

            if (oProducto != null)
            {
                oProducto.Base64 = CN_Recursos.ConvertirBase64(Path.Combine(oProducto.RutaImagen, oProducto.NombreImagen), out conversion);   
                oProducto.Extension = Path.GetExtension(oProducto.NombreImagen);
            }

            return View(oProducto);
        }



        [HttpGet]
        public JsonResult ListaCategorias()
        {
            List<Categoria> lista = new List<Categoria>();

            lista = new CN_Categoria().Listar();

            return Json(new {data = lista});
        }


        [HttpPost]
        public JsonResult ListaMarcaporCategoria(int idcategoria)
        {
            List<Marca> lista = new List<Marca>();

            lista = new CN_Marcas().ListarMarcaporCategoria(idcategoria);

            return Json(new { data = lista });
        }



        [HttpPost]
        public JsonResult ListarProducto(int idcategoria, int idmarca)
        {
            
            List<Producto> lista = new List<Producto>();

            bool conversion;

            lista = new CN_Productos().Listar().Select(p => new Producto()
            {
                ID_Prod = p.ID_Prod,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                oMarca = p.oMarca,
                oCategoria = p.oCategoria,
                Precio = p.Precio,
                Stock = p.Stock,
                RutaImagen = p.RutaImagen,
                Base64 = CN_Recursos.ConvertirBase64(Path.Combine(p.RutaImagen, p.NombreImagen), out conversion),
                Extension = Path.GetExtension(p.NombreImagen),
                Activo = p.Activo
            }).Where(p =>
                p.oCategoria.ID_Cat == (idcategoria == 0 ? p.oCategoria.ID_Cat : idcategoria ) &&
                p.oMarca.ID_Marca == (idmarca == 0 ? p.oMarca.ID_Marca : idmarca) &&
                p.Stock > 0 && p.Activo == true
            ).ToList();


            var jsonresult = Json(new { data = lista });
            //aca se tuvo que haber agregado el maxjsonlength pero no se puede por la version
            return jsonresult;
        
        }



        [HttpPost]
        public JsonResult AgregarCarrito(int idproducto)
        {
            //ojo aca posiblemente haya que agregar un try catch
            int idcliente = Convert.ToInt32( HttpContext.Session.GetString("ID_Cliente"));

            bool existe = new CN_Carrito().ExisteCarrito(idcliente, idproducto);

            bool respuesta = false;

            string mensaje = string.Empty;

            if (existe)
            {
                mensaje = "El producto ya esta agregado al carrito";
            }
            else
            {
                respuesta = new CN_Carrito().OperacionCarrito(idcliente, idproducto, true, out mensaje);
            }

            return Json(new { respuesta= respuesta, mensaje = mensaje });
        }



        [HttpGet]
        public JsonResult CantidadEnCarrito()
        {
            int idcliente = Convert.ToInt32(HttpContext.Session.GetString("ID_Cliente"));
            int cantidad = new CN_Carrito().CantidadEnCarrito(idcliente);

            return Json(new { cantidad = cantidad });

        }



        [HttpPost]
        public JsonResult ListarProductosCarrito()
        {
            int idcliente = Convert.ToInt32(HttpContext.Session.GetString("ID_Cliente"));

            List<Carrito> oLista = new List<Carrito>();

            bool conversion;

            oLista = new CN_Carrito().ListarProducto(idcliente).Select(oc => new Carrito()
            {
                oProducto = new Producto()
                {
                    ID_Prod = oc.oProducto.ID_Prod,
                    Nombre = oc.oProducto.Nombre,
                    oMarca = oc.oProducto.oMarca,
                    Precio = oc.oProducto.Precio,
                    RutaImagen = oc.oProducto.RutaImagen,
                    Base64 = CN_Recursos.ConvertirBase64(Path.Combine(oc.oProducto.RutaImagen, oc.oProducto.NombreImagen), out conversion),
                    Extension = Path.GetExtension(oc.oProducto.NombreImagen)
                },
                Cantidad = oc.Cantidad

            }).ToList();

            return Json(new { data = oLista });

        }



        [HttpPost]
        public JsonResult OperacionCarrito(int idproducto, bool sumar)
        {
            int idcliente = Convert.ToInt32(HttpContext.Session.GetString("ID_Cliente"));

            bool respuesta = false;

            string mensaje = string.Empty;

            respuesta = new CN_Carrito().OperacionCarrito(idcliente, idproducto, true, out mensaje);


            return Json(new { respuesta = respuesta, mensaje = mensaje });
        }


        [HttpPost]
        public JsonResult EliminarCarrito(int idproducto)
        {
            int idcliente = Convert.ToInt32(HttpContext.Session.GetString("ID_Cliente"));

            bool respuesta = false;

            string mensaje = string.Empty;

            respuesta = new CN_Carrito().EliminarCarrito(idcliente, idproducto);

            return Json(new { respuesta = respuesta, mensaje = mensaje });
        }


        [HttpPost]
        public JsonResult ObtenerDepartamento()
        {
            List<Departamento> oLista = new List<Departamento>();
        
            oLista = new CN_Ubicacion().ObtenerDepartamento();

            return Json(new{ lista = oLista });
        
        }



        [HttpPost]
        public JsonResult ObtenerMunicipio(string iddepartamento)
        {
            List<Municipio> oLista = new List<Municipio>();

            oLista = new CN_Ubicacion().ObtenerMunicipio(iddepartamento);

            return Json(new { lista = oLista });

        }



        public IActionResult Carrito()
        {

            return View();
        }



        [HttpPost]
        public async Task<JsonResult> ProcesarPago(List<Carrito> oListaCarrito, Venta oVenta)
        {
            decimal total = 0;

            DataTable detalle_venta = new DataTable();

            detalle_venta.Locale = new System.Globalization.CultureInfo("hn-HN");

            detalle_venta.Columns.Add("ID_Prod", typeof(string));
            detalle_venta.Columns.Add("Cantidad", typeof(int));
            detalle_venta.Columns.Add("Total", typeof(decimal));


            List<Item> oListaItem = new List<Item>();


            foreach (Carrito oCarrito in oListaCarrito) {

                decimal subtotal = Convert.ToDecimal(oCarrito.Cantidad.ToString()) * oCarrito.oProducto.Precio;

                total += subtotal;

                oListaItem.Add(new Item()
                {
                    name = oCarrito.oProducto.Nombre,
                    quantity = oCarrito.Cantidad.ToString(),
                    unit_amount = new UnitAmount()
                    {
                        currency_code = "USD",
                        value = oCarrito.oProducto.Precio.ToString("G", new CultureInfo("hn-HN"))
                    }
                });

                detalle_venta.Rows.Add( new object[]
                {
                    oCarrito.oProducto.ID_Prod,
                    oCarrito.Cantidad,
                    subtotal
                });

            }

            //TAMBIEN TIENE QUE VER CON PAYPAL
            PurchaseUnit purchaseUnit = new PurchaseUnit()
            {

                amount = new Amount()
                {
                    currency_code = "USD",
                    value = total.ToString("G", new CultureInfo("hn-HN")),
                    breakdown = new Breakdown()
                    {
                        item_total = new ItemTotal()
                        {
                            currency_code = "USD",
                            value = total.ToString("G", new CultureInfo("hn-HN")),
                        }
                    }
                },
                description = "compra de articulo de mi tienda",
                items = oListaItem

            };


            //ENVIA LA MOVIDA A PAYPAL
            Checkout_Order oCheckOutOrder = new Checkout_Order()
            {
                intent = "CAPTURE",
                purchase_units = new List<PurchaseUnit>(){purchaseUnit},
                application_context = new ApplicationContext()
                {
                    brand_name = "MiTienda.com",
                    landing_page = "NO_PREFERENCE",
                    user_action = "PAY_NOW",
                    return_url = "https://localhost:7181/Tienda/PagoEfectuado",
                    cancel_url = "https://localhost:7181/Tienda/Carrito"
                }
            };

        
            oVenta.MontoTotal = total;
            oVenta.ID_Cliente = Convert.ToInt32(HttpContext.Session.GetString("ID_Cliente"));


            TempData["Venta"] = JsonConvert.SerializeObject(oVenta);
            TempData["DetalleVenta"] = JsonConvert.SerializeObject(detalle_venta);


            CN_Paypal opaypal = new CN_Paypal();

            Response_Paypal<Response_Checkout> response_Paypal = new Response_Paypal<Response_Checkout>();

            response_Paypal = await opaypal.CrearSolicitud(oCheckOutOrder);



            return Json( response_Paypal);
            

        }



        public async Task<IActionResult> PagoEfectuado()
        {
            //aca paypal le manda un token por medio de la url de arriba
            string? token = Request.Query["token"];

            CN_Paypal opaypal = new CN_Paypal();

            Response_Paypal<Response_Capture> response_Paypal = new Response_Paypal<Response_Capture>();
            response_Paypal = await opaypal.AprobarPago(token);

            ViewData["Status"] = response_Paypal.Status;

            if (response_Paypal.Status)
            {
                Venta? oVenta = JsonConvert.DeserializeObject<Venta>((string)TempData["Venta"]);
                DataTable? detalle_venta = JsonConvert.DeserializeObject<DataTable>((string)TempData["DetalleVenta"]);

                oVenta.ID_Transaccion = response_Paypal.Response.purchase_units[0].payments.captures[0].id;

                string mensaje = string.Empty;

                bool respuesta = new CN_Venta().Registrar(oVenta, detalle_venta, out mensaje);

                ViewData["ID_Transaccion"] = oVenta.ID_Transaccion;

            }

            return View();
        }



       // [HttpPost]
        public IActionResult MisCompras()
        {
            int idcliente = Convert.ToInt32(HttpContext.Session.GetString("ID_Cliente"));

            List<DetalleVenta> oLista = new List<DetalleVenta>();

            bool conversion;

            oLista = new CN_Venta().ListarCompras(idcliente).Select(oc => new DetalleVenta()
            {
                oProducto = new Producto()
                {
                    Nombre = oc.oProducto.Nombre,
                    Precio = oc.oProducto.Precio,
                    Base64 = CN_Recursos.ConvertirBase64(Path.Combine(oc.oProducto.RutaImagen, oc.oProducto.NombreImagen), out conversion),
                    Extension = Path.GetExtension(oc.oProducto.NombreImagen)
                },
                Cantidad = oc.Cantidad,
                Total = oc.Total,
                ID_Transaccion = oc.ID_Transaccion

            }).ToList();

            return View(oLista);

        }






    }
}
