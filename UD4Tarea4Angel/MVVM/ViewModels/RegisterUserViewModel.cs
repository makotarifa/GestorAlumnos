using PropertyChanged;
using UD4Tarea4Angel.Models;

namespace UD4Tarea4Angel.ViewModels
{
    /// <summary>
    /// ViewModel para el registro de un usuario.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class RegisterUserViewModel
    {
        /// <summary>
        /// Usuario que se va a registrar.
        /// </summary>
        public UserItem UserItem { get; set; }
        /// <summary>
        /// Datos personales del usuario que se va a registrar.
        /// </summary>
        public Persona Persona { get; set; }

        /// <summary>
        /// Constructor de la clase RegisterUserViewModel.
        /// </summary>
        public RegisterUserViewModel()
        {
            //Se generan las instancias de los objetos UserItem y Persona, que se utilizarán para el registro de un usuario y guardar sus datos personales.
            UserItem = new UserItem();
            Persona = new Persona();
        }
    }
}