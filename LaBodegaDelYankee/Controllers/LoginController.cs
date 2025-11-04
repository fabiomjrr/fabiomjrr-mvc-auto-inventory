using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using LaBodegaDelYankee.Models;
using System.Collections.Generic;

namespace LaBodegaDelYankee.Controllers
{
    public class LoginController : Controller
    {
        // Contexto de BD (usando Repuestos_AmericanosEntities3)
        private Repuestos_AmericanosEntities1 db = new Repuestos_AmericanosEntities1();

        // GET: Login/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login/Login
        [HttpPost]
        // NOTA: Cambiamos 'Correo' a 'Usuario' para que coincida con el campo del modelo
        public ActionResult Login(string Usuario, string Contrasena)
        {
            if (string.IsNullOrEmpty(Usuario) || string.IsNullOrEmpty(Contrasena))
            {
                ViewBag.Mensaje = "Por favor, complete todos los campos.";
                return View();
            }

            // La lógica de byte[] para seguridad ya no es aplicable porque los campos son 'string'.
            // Ahora la comparación es de strings simples.

            var usuarioEncontrado = db.Usuarios
                // ✅ CORRECCIÓN DE CAMPOS: Usando 'usuario' y 'contrasena' en minúscula,
                // y eliminando el chequeo de 'Activo' porque no existe.
                .FirstOrDefault(u =>
                    u.usuario == Usuario.Trim() &&
                    u.contrasena == Contrasena.Trim());


            if (usuarioEncontrado != null)
            {
                // ✅ CORRECCIÓN DE SESIÓN: Usando 'id_usuario' y 'rol' (string)
                Session["UsuarioID"] = usuarioEncontrado.id_usuario;
                Session["NombreUsuario"] = usuarioEncontrado.usuario; // Usar el nombre de usuario como nombre en sesión
                Session["Rol"] = usuarioEncontrado.rol; // El rol ahora es un STRING (e.g., "Administrador", "Cajero")

                // Redirección al Home
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Mensaje = "Usuario o contraseña incorrectos.";
            return View();
        }

        // NOTA IMPORTANTE: La funcionalidad de Register (Registro) ya no es compatible
        // con el modelo simple que acabas de subir (le faltan campos como Nombres, Apellidos, Correo, Activo).
        // Por lo tanto, elimino o comento el código de registro para evitar errores de compilación.

        // GET: Login/Register
        public ActionResult Register()
        {
            // Solo redirigimos al login, ya que la página de registro requiere un modelo más completo.
            return RedirectToAction("Login");
        }

        // [HttpPost] Register method eliminado para evitar errores.

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}