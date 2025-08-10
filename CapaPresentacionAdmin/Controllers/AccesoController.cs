using Microsoft.AspNetCore.Mvc;
using System.Web;
using CapaNegocio;
using CapaEntidad;
using DocumentFormat.OpenXml.Packaging;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CapaPresentacionAdmin.Controllers
{
    public class AccesoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CambiarClave()
        {
            return View();
        }

        public IActionResult Reestablecer()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Index(string correo, string clave)
        {
            Usuario oUsuario = new Usuario();
            oUsuario = new CN_Usuarios().Listar().Where(u => u.Correo == correo && u.Clave == CN_Recursos.ConvertirSha256(clave)).FirstOrDefault();


            if (oUsuario == null) {
                ViewBag.Error = "Correo o contraseña no correcta";
                return View();
           
            }
            else
            {

                if (oUsuario.Reestablecer)
                {
                    TempData["ID_Usuario"] = oUsuario.ID_Usuario;

                    return RedirectToAction("CambiarClave");
                }

                //********************//
                List<Claim> claims = new List<Claim>()
                {
                new Claim(ClaimTypes.Name, oUsuario.Nombres)
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
                //***********************************/


                ViewBag.Error = null;
                return RedirectToAction("Index", "Home");
            }


        }


        [HttpPost]
        public IActionResult CambiarClave(string idusuario, string claveactual, string nuevaclave, string confirmarclave)
        {
            Usuario oUsuario = new Usuario();

            //Devuelve el objeto de usuario que coincida con el id usuario 
            oUsuario = new CN_Usuarios().Listar().Where(u => u.ID_Usuario == int.Parse(idusuario) ).FirstOrDefault();


            

            if (oUsuario.Clave != CN_Recursos.ConvertirSha256(claveactual) )
            {
                TempData["ID_Usuario"] = idusuario;

                //Sirve para guardar datos asi como el viewbag
                ViewData["vclave"] = "";

                ViewBag.Error = "Contraseña actual incorrecta";
                return View();
            }else if (nuevaclave != confirmarclave)
            {
                TempData["ID_Usuario"] = idusuario;
                
                
                ViewData["vclave"] = claveactual;

                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }

            ViewData["vclave"] = "";

            nuevaclave = CN_Recursos.ConvertirSha256(nuevaclave); 

            string mensaje = string.Empty;

            bool respuesta = new CN_Usuarios().CambiarClave(int.Parse(idusuario), nuevaclave, out mensaje);

            if (respuesta)
            {
               

                return RedirectToAction("Index");
            }
            else
            {
                TempData["ID_Usuario"] = idusuario;
                ViewBag.Error = mensaje;

                return View();
            }

        }



        [HttpPost]
        public IActionResult Reestablecer(string correo)
        {
            Usuario oUsuario = new Usuario();

            oUsuario = new CN_Usuarios().Listar().Where(item => item.Correo == correo).FirstOrDefault();

            if (oUsuario == null)
            {
                ViewBag.Error = "No se encontro un usuario con ese correo";
                return View();
            }


            string mensaje = string.Empty;
            bool respuesta = new CN_Usuarios().ReestablecerClave(oUsuario.ID_Usuario, correo, out mensaje);

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




        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Acceso");
        }


    }
}
