using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UD4Tarea4Angel.MVVM.Models
{
    public class Dia : ISerializable
    {
        public string Key { get; set; }
        public DateTime Fecha { get; set; }
        public string UserName { get; set; }

        public Dia()
        {
            Key = Guid.NewGuid().ToString();
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key", Key);
            info.AddValue("Fecha", Fecha);
            info.AddValue("UserName", UserName);
        }
    }
}
