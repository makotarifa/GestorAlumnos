using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Database.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UD4Tarea4Angel.Utilities
{
    /// <summary>
    /// Clase que representa la conexión con Firebase.
    /// </summary>
    public static class FirebaseConnection
    {
        public static FirebaseClient firebaseClient = new FirebaseClient("https://fir-angel-1c1f8-default-rtdb.europe-west1.firebasedatabase.app/");
    }

 
}
