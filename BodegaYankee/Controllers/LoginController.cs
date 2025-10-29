using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using BodegaYankee.Models; 
using System.Collections.Generic;

namespace BodegaYankee.Controllers 
{
    public class LoginController : Controller
    {
        private Repuestos_AmericanosEntities db = new Repuestos_AmericanosEntities();

        // GET: Login/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login/Login
        [HttpPost]
        public ActionResult Login(string Correo, string Contrasena)
        {
            if (string.IsNullOrEmpty(Correo) || string.IsNullOrEmpty(Contrasena))
            {
                ViewBag.Mensaje = "Por favor, complete todos los campos.";
                return View();
            }

            // El código original maneja Correo y Contrasena como byte[], 
            // lo que sugiere que en la BD están almacenados como varbinary para seguridad/cifrado.
            byte[] correoBytes = Encoding.UTF8.GetBytes(Correo.Trim());
            byte[] contraBytes = Encoding.UTF8.GetBytes(Contrasena.Trim());

            // Traemos todos los usuarios activos y filtramos en memoria (AsEnumerable()).
            // Esto es necesario porque SequenceEqual en byte[] no es soportado por LINQ to Entities.
            // La tabla de la BD se llama 'Usuarios' y tiene un campo 'activo'.
            var usuario = db.Usuarios
                .Where(u => u.activo == true) // **NOTA:** Se usa 'activo' según el SQL.
                .AsEnumerable() // Fuerza la ejecución de la consulta hasta aquí y continua en memoria
                .FirstOrDefault(u =>
                    // Los campos Correo y Contrasena deben estar mapeados como byte[] en el modelo Usuarios
                    ((IEnumerable<byte>)u.correo).SequenceEqual(correoBytes) && // **NOTA:** Se usa 'correo' según el SQL.
                    ((IEnumerable<byte>)u.contrasena).SequenceEqual(contraBytes)); // **NOTA:** Se usa 'contrasena' según el SQL.


            if (usuario != null)
            {
                // **NOTA:** Asumo que EF mapeó id_usuario a UsuarioID
                Session["UsuarioID"] = usuario.id_usuario;
                Session["NombreUsuario"] = $"{usuario.nombres} {usuario.apellidos}";

                //Redirección al Home por defecto.
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Mensaje = "Correo o contraseña incorrectos.";
            return View();
        }

        // GET: Login/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Login/Register
        [HttpPost]
        public ActionResult Register(string Nombres, string Apellidos, DateTime FechaNacimiento, string Correo, string Contrasena)
        {
            if (string.IsNullOrEmpty(Correo) || string.IsNullOrEmpty(Contrasena))
            {
                ViewBag.Mensaje = "Todos los campos son obligatorios.";
                return View();
            }

            byte[] correoBytes = Encoding.UTF8.GetBytes(Correo.Trim());
            byte[] contraBytes = Encoding.UTF8.GetBytes(Contrasena.Trim());

            // Traemos todos los correos activos y filtramos en memoria.
            bool correoExiste = db.Usuarios
                .AsEnumerable() // Ejecuta la consulta antes de filtrar
                .Any(u => ((IEnumerable<byte>)u.correo).SequenceEqual(correoBytes)); // **NOTA:** Se usa 'correo' según el SQL.

            if (correoExiste)
            {
                ViewBag.Mensaje = "Ya existe un usuario con ese correo.";
                return View();
            }


            Usuarios nuevo = new Usuarios
            {
                nombres = Nombres,
                apellidos = Apellidos,
                fecha_nacimiento = FechaNacimiento,
                correo = correoBytes,
                contrasena = contraBytes,
                rol = 2, // Usuario común. **NOTA:** Se usa 'rol' según el SQL.
                activo = true
            };

            db.Usuarios.Add(nuevo);
            db.SaveChanges();

            ViewBag.Mensaje = "Registro exitoso. Ahora puede iniciar sesión.";
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}