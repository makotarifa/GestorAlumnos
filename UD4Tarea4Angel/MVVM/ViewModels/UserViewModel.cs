using PropertyChanged;
using UD4Tarea4Angel.Models;

namespace UD4Tarea4Angel.ViewModels
{
    /// <summary>
    /// Clase que representa el ViewModel para el inicio de sesion del usuario.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class UserViewModel
    {
        /// <summary>
        /// Usuario que se va a registrar o iniciar sesion.
        /// </summary>
        public UserItem UserItem { get; set; }

        /// <summary>
        /// Constructor de la clase UserViewModel.
        /// </summary>
        public UserViewModel()
        {
            UserItem = new UserItem();
        }
    }
}