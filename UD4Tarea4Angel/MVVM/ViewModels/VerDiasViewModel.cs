using Firebase.Database;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.MVVM.Models;

namespace UD4Tarea4Angel.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class VerDiasViewModel
    {
        public Persona PersonaActual { get; set; }

        public ObservableCollection<Dia> Dias { get; set; }

        public VerDiasViewModel(Persona persona)
        {
            Dias = new ObservableCollection<Dia>();
            PersonaActual = persona;
        }

    }
}
