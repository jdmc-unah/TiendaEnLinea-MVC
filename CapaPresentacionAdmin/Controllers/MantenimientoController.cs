using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Web;

namespace CapaPresentacionAdmin.Controllers
{
    [Authorize]
    public class MantenimientoController : Controller
    {
        public IActionResult Categorias()
        {
            return View();
        }
        public IActionResult Marcas()
        {
            return View();
        }
        public IActionResult Productos()
        {
            return View();
        }

        //**************METODOS CATEGORIAS***********************

        #region METODOS CATEGORIAS

        [HttpGet]
        public JsonResult ListarCategorias()
        {
            List<Categoria> oLista = new List<Categoria>();
            oLista = new CN_Categoria().Listar();

            return Json(new { data = oLista });
        }


        [HttpPost]
        public JsonResult GuardarCategorias(Categoria objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.ID_Cat == 0)
            {
                resultado = new CN_Categoria().Registrar(objeto, out mensaje);

            }
            else
            {
                resultado = new CN_Categoria().Editar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje });

        }

        [HttpPost]
        public JsonResult EliminarCategoria(Categoria id)
        {

            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Categoria().Eliminar(id.ID_Cat, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje });

        }

        #endregion


        //**************METODOS MARCAS***********************

        #region METODOS MARCAS

        [HttpGet]
        public JsonResult ListarMarcas()
        {
            List<Marca> oLista = new List<Marca>();
            oLista = new CN_Marcas().Listar();

            return Json(new { data = oLista });
        }


        [HttpPost]
        public JsonResult GuardarMarcas(Marca objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.ID_Marca == 0)
            {
                resultado = new CN_Marcas().Registrar(objeto, out mensaje);

            }
            else
            {
                resultado = new CN_Marcas().Editar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje });

        }

        [HttpPost]
        public JsonResult EliminarMarcas(Marca id)
        {

            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Marcas().Eliminar(id.ID_Marca, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje });

        }

        #endregion


        //**************METODOS PRODUCTOS***********************

        #region METODOS PRODUCTOS

        [HttpGet]
        public JsonResult ListarProductos()
        {
            List<Producto> oLista = new List<Producto>();
            oLista = new CN_Productos().Listar();

            return Json(new { data = oLista });
        }


        [HttpPost]
        public JsonResult GuardarProducto(string objeto,  IFormFile archivoImagen)
        {
            
            string mensaje = string.Empty;

            bool operacionExitosa = true;
            bool guardar_imagen_exito = true;

            Producto oProducto = new Producto();
            oProducto = JsonConvert.DeserializeObject<Producto>(objeto);

            decimal precio;

            //La sentencia dentro de la condicion convierte de decimal a string y de paso devuelve un true o false
            if (decimal.TryParse(oProducto.PrecioTexto, System.Globalization.NumberStyles.AllowDecimalPoint, new CultureInfo("hn-HN"), out precio))
            {
                oProducto.Precio = precio;
            }
            else
            {

                return Json(new { operacionExitosa = false, mensaje = "El formato del precio debe ser: ##.##" });

            }


            //Si lo de arriba funcion entonces lo manda a registrar o editar siguiendo la logica de siempre 
            if (oProducto.ID_Prod == 0)
            {

                int idProductoGenerado = new CN_Productos().Registrar(oProducto, out mensaje);

                if (idProductoGenerado != 0)
                {
                    oProducto.ID_Prod = idProductoGenerado;
                }
                else
                {
                    operacionExitosa = false;
                }

            }
            else
            {
                operacionExitosa = new CN_Productos().Editar(oProducto, out mensaje);
            }

            
            if (operacionExitosa)
            {

                if (archivoImagen != null)
                {
                    string ruta_guardar = "C:\\Users\\Daniel\\Documents\\Programacion C#\\Web con MVC\\Imagenes Tienda 1";
                    string extension = Path.GetExtension(archivoImagen.FileName);
                    string nombre_imagen = string.Concat(oProducto.ID_Prod.ToString(), extension);

                    try
                    {
                        //Lo de abajo es en si lo que guarda la imagen tomando en cuenta la ruta, nombre y extension

                        using (var stream = new FileStream((Path.Combine(ruta_guardar, nombre_imagen)), FileMode.Create))
                        {
                            archivoImagen.CopyTo(stream);

                        }

                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        guardar_imagen_exito = false;
                    }

                    if (guardar_imagen_exito == true)
                    {
                        oProducto.RutaImagen = ruta_guardar;
                        oProducto.NombreImagen = nombre_imagen;
                        bool rspta = new CN_Productos().GuardarDatosImagen(oProducto, out mensaje);
                    }
                    else
                    {
                        mensaje = "Seguardo el producto pero hubo problemas con la imagen";
                    }

                }



            }
            

            return Json(new { operacionExitosa = operacionExitosa, mensaje = mensaje, idGenerado = oProducto.ID_Prod });

        }



        [HttpPost]

        //METODO QUE DEUVLEVE LA IMAGEN COMO TEXTO EN UN FORMATO BASE 64
        public JsonResult ImagenProducto( int id) {

            bool conversion;
            // id = 11;

            Producto oproducto = new CN_Productos().Listar().Where(p => p.ID_Prod == id).FirstOrDefault();

            string textoBase64 = CN_Recursos.ConvertirBase64(Path.Combine(oproducto.RutaImagen, oproducto.NombreImagen), out conversion);
        

            return Json(new { conversion = conversion , textoBase64 =  textoBase64, extension = Path.GetExtension(oproducto.NombreImagen) });

        }



        
        [HttpPost]
        public JsonResult EliminarProductos(Producto id)
        {

            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Productos().Eliminar(id.ID_Prod, out mensaje);

            return Json(new { resultado = respuesta, mensaje = mensaje });

        }

        #endregion


    }
}
