using CapaEntidad;
using CapaNegocio;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CapaDatos;
using CapaPresentacionTienda.Models;

namespace CapaPresentacionTienda.Controllers
{
    public class AccesoController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            return View();
        }

        public IActionResult Reestablecer()
        {
            return View();
        }

        public IActionResult CambiarClave()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Registrar(Cliente objeto)
        {
            int resultado;
            string mensaje = string.Empty;

            ViewData["Nombres"] = string.IsNullOrEmpty(objeto.Nombres) ? "" : objeto.Nombres;
            ViewData["Apellidos"] = string.IsNullOrEmpty(objeto.Apellidos) ? "" : objeto.Apellidos;
            ViewData["Correo"] = string.IsNullOrEmpty(objeto.Correo) ? "" : objeto.Correo;


            if (objeto.Clave != objeto.ConfirmarClave)
            {
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }

            resultado = new CN_Cliente().Registrar(objeto, out mensaje);

             
            if (resultado > 0)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "Acceso");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }

        }




        [HttpPost]
        public async Task<IActionResult> Index(string correo, string clave)
        {
           
            Cliente oCliente = null;

            oCliente = new CN_Cliente().Listar().Where(item => item.Correo == correo && item.Clave == CN_Recursos.ConvertirSha256(clave)).FirstOrDefault();

            if (oCliente == null)
            {
                ViewBag.Error = "Correo o contraseña incorrecta";
                return View();
            }
            else
            {
                if (oCliente.Reestablecer)
                {
                    TempData["ID_Cliente"] = oCliente.ID_Cliente;

                    return RedirectToAction("CambiarClave", "Acceso");
                }
                else
                {
                    //*********AUTENTICACION***********//
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, oCliente.Correo)
                    };

                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        properties
                    );


                   
                    // Establecer un valor en la sesión
                    

                    HttpContext.Session.SetString("Cliente", oCliente.Nombres);
                    HttpContext.Session.SetString("ID_Cliente", (oCliente.ID_Cliente).ToString());
                    

                    //***********************************/

                    ViewBag.Error = null;
                    return RedirectToAction("Index", "Tienda");
                }
            }

        }






        [HttpPost]
        public IActionResult Reestablecer(string correo)
        {

            Cliente oCliente = new Cliente();

            oCliente = new CN_Cliente().Listar().Where(item => item.Correo == correo).FirstOrDefault();

            if (oCliente == null)
            {
                ViewBag.Error = "No se encontro un usuario con ese correo";
                return View();
            }


            string mensaje = string.Empty;
            bool respuesta = new CN_Cliente().ReestablecerClave(oCliente.ID_Cliente, correo, out mensaje);

            if (respuesta)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "Acceso");

            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }
       
        }


        [HttpPost]
        public IActionResult CambiarClave(string idcliente, string claveactual, string nuevaclave, string confirmarclave)
        {

            Cliente oCliente = new Cliente();

            //Devuelve el objeto de usuario que coincida con el id usuario 
            oCliente = new CN_Cliente().Listar().Where(u => u.ID_Cliente == int.Parse(idcliente)).FirstOrDefault();


            if (oCliente.Clave != CN_Recursos.ConvertirSha256(claveactual))
            {
                TempData["ID_Cliente"] = idcliente;

                //Sirve para guardar datos asi como el viewbag
                ViewData["vclave"] = "";

                ViewBag.Error = "Contraseña actual incorrecta";
                return View();
            }
            else if (nuevaclave != confirmarclave)
            {
                TempData["ID_Cliente"] = idcliente;

                ViewData["vclave"] = claveactual;

                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }

            ViewData["vclave"] = "";

            nuevaclave = CN_Recursos.ConvertirSha256(nuevaclave);

            string mensaje = string.Empty;

            bool respuesta = new CN_Cliente().CambiarClave(int.Parse(idcliente), nuevaclave, out mensaje);

            if (respuesta)
            {


                return RedirectToAction("Index");
            }
            else
            {
                TempData["ID_Cliente"] = idcliente;
                ViewBag.Error = mensaje;

                return View();
            }

        }




        public async Task<IActionResult> CerrarSesion()
        {

            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            

            return RedirectToAction("Index", "Acceso");
        }







    }
}
