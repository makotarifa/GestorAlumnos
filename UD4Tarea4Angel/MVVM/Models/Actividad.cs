using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UD4Tarea4Angel.MVVM.Models
{
    public class Actividad : ISerializable
    {
        public string Key { get; set; }
        public string DiaKey { get; set; }
        public string ActividadDesarrollada { get; set; }
        public double TiempoEmpleado { get; set; }
        public string Observaciones { get; set; }

        public Actividad()
        {
            Key = Guid.NewGuid().ToString();
            TiempoEmpleado = 1;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key", Key);
            info.AddValue("ActividadDesarrollada", ActividadDesarrollada);
            info.AddValue("TiempoEmpleado", TiempoEmpleado);
            info.AddValue("Observaciones", Observaciones);
        }
    }
}
