using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UD4Tarea4Angel.Models
{
    public class Persona : ISerializable
    {
        public string Key { get; set; }
        public string Nombre {  get; set; }
        public string Apellidos { get; set; }

        public string DNI { get; set; }
        public string CentroEstudio { get; set; }

        public string ProfesorTutor { get; set; }

        public string NombreGrado { get; set; }

        public string TipoGrado { get; set; }


        public string CentroTrabajo { get; set; }


        public string TutorLaboral { get; set; }

        public string UserName { get; set; }

        public Persona() {
            Key = Guid.NewGuid().ToString();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Key", Key);
            info.AddValue("Nombre", Nombre);
            info.AddValue("Apellidos", Apellidos);
            info.AddValue("DNI", DNI);
            info.AddValue("CentroEstudio", CentroEstudio);
            info.AddValue("ProfesorTutor", ProfesorTutor);
            info.AddValue("NombreGrado", NombreGrado);
            info.AddValue("TipoGrado", TipoGrado);
            info.AddValue("CentroTrabajo", CentroTrabajo);
            info.AddValue("TutorLaboral", TutorLaboral);
            info.AddValue("UserName", UserName);
        }
    }
}

