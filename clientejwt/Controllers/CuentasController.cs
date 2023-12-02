using clientejwt.Models;
using clientejwt.Services.Backend;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace clientejwt.Controllers
{
    [Authorize]
    public class CuentasController : Controller
    {
        private readonly IBackend _backend;

        public CuentasController(IBackend backend)
        {
            _backend = backend;
        }

        //Este atributo permite que /Cuentas/Login si pueda ser accedido sin estar logueado
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    // Funcion que verifica en backend que el coreeo y contraseña sean validos
                    var authUser = await _backend.AutenticationAsync(model.Correo, model.Password);
                    //Regresa null si no es valido

                    if (authUser == null)
                    {
                        ModelState.AddModelError("Correo", "Credenciales no válidas. Inténtelo nuevamente.");
                    }
                    else 
                    {
                        var claims = new List<Claim>
                        {
                            // Informacion de la Cookie
                            new Claim(ClaimTypes.Name, authUser.Email),
                            new Claim(ClaimTypes.GivenName, authUser.Nombre),
                            new Claim(ClaimTypes.Email, authUser.Email),
                            new Claim("token", authUser.AccessToken),
                            new Claim(ClaimTypes.Role, authUser.Rol),
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties();
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                        //Usuario válido, lo envia al Home
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch (Exception ex) 
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> PerfilAsync()
        {
            //Obtiene el perfil del usuario
            var token = User.FindFirstValue("token");
            var correo = User.FindFirstValue(ClaimTypes.Email);
            ViewData["token"] = User.FindFirstValue("token");

            var usurio = await _backend.GetUsuarioAsync(correo, token);

            return View(usurio);
        }

        public async Task<IActionResult> LogoutAsync(string returnUrl = null)
        {
            // Cerrar la sesión
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (returnUrl != null)
            {
                // Si hay una accion donde regresr, se realiza un redirect
                return LocalRedirect(returnUrl);
            }
            else 
            {
                // Sino, se redirige al inicio de sesion
                return RedirectToAction("Login");
            }
        }

        public IActionResult AccessDenied()
        { 
            //Regresa una Vista con un mensaje de acceso denegado
            return View();
        }
    }
}
