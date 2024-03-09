using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UD4Tarea4Angel.Models;

namespace UD4Tarea4Angel.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SelectorAlumnosViewModel
    {
        public ObservableCollection<Persona> AlumnosCollection { get; set; }

        public SelectorAlumnosViewModel()
        {
            AlumnosCollection = new ObservableCollection<Persona>();
        }
    }
}
