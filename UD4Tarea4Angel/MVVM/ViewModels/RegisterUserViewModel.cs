using PropertyChanged;
using UD4Tarea4Angel.Models;

namespace UD4Tarea4Angel.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class RegisterUserViewModel
    {
        public UserItem UserItem { get; set; }
        public Persona Persona { get; set; }

        public RegisterUserViewModel()
        {
            UserItem = new UserItem();
            Persona = new Persona();
        }
    }
}