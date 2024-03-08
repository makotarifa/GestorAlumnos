using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UD4Tarea4Angel.MVVM.Models;

namespace UD4Tarea4Angel.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SelectorDiaViewModel
    {
        public Dia Dia { get; set; }

        public Actividad ActividadActual { get; set; }

        public ObservableCollection<Actividad> Actividades { get; set; }

        public SelectorDiaViewModel()
        {
            Dia = new Dia();
            ActividadActual = new Actividad();
            Actividades = new ObservableCollection<Actividad>();
        }
    }
}
