using PropertyChanged;
using UD4Tarea4Angel.Models;

namespace UD4Tarea4Angel.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class UserViewModel
    {
        public UserItem UserItem { get; set; }

        public UserViewModel()
        {
            UserItem = new UserItem();
        }
    }
}