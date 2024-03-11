﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UD4Tarea4Angel.Models
{
    /// <summary>
    /// Clase que representa un usuario del sistema.
    /// </summary>
    public class UserItem
    {
        /// <summary>
        /// Usuario identificativo del usuario.
        /// </summary>
        public string UserName {  get; set; }
        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Email del usuario.
        /// </summary>
        public string Email { get; set; } 
        
        public string TipoCuenta { get; set; }
    }
}
