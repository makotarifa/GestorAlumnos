using PropertyChanged;
using System;
using System.Collections.Generic;
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

        public SelectorDiaViewModel()
        {
            Dia = new Dia();
        }
    }
}
