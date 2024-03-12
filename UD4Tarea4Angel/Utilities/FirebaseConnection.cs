using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Firebase.Storage;
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
    /// 
    public static class FirebaseConnection
    {
        public static FirebaseClient firebaseClient = new FirebaseClient("https://fir-angel-1c1f8-default-rtdb.europe-west1.firebasedatabase.app/");

        private static string authDomain = "fir-angel-1c1f8.firebaseapp.com";

        private static string apiKey = "AIzaSyCcAG8QcfMiAue_GI5T_DbhoUsmGOppY9I";

        private static string token = string.Empty;

        //Para la subida de archivos a storage.
        private static string rutaStorage = "fir-angel-1c1f8.appspot.com";

        private static string emailAdmin = "registrosfbangel@fb.es";
        private static string passAdmin = "registrosfb872";


        public static FirebaseAuthClient fbAuthClient;
        public static async Task obtenerTokenRegistro()
        {
            fbAuthClient = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = apiKey,
                AuthDomain = authDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            });
            var credenciales = await fbAuthClient.SignInWithEmailAndPasswordAsync(emailAdmin, passAdmin);
            token = await credenciales.User.GetIdTokenAsync();
        }

        public static async Task obtenerToken(string email, string password)
        {
            fbAuthClient = new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = apiKey,
                AuthDomain = authDomain,
                Providers = new FirebaseAuthProvider[]
                {
                    new EmailProvider()
                }
            });
            var credenciales = await fbAuthClient.SignInWithEmailAndPasswordAsync(email, password);
            token = await credenciales.User.GetIdTokenAsync();
        }

        public static void cerrarFirebase()
        {
            fbAuthClient = null;
            token = string.Empty;
        }

        public static async Task<string> storageUploadPhoto()
        {

            var foto = await MediaPicker.PickPhotoAsync();
            if (foto != null)
            {
                var task = new FirebaseStorage(
                    rutaStorage,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(token),
                        ThrowOnCancel = true
                    }
                )
                .Child("Imagenes")
                .Child(foto.FileName)
                .PutAsync(await foto.OpenReadAsync());

                var urlDescarga = await task;
                return urlDescarga;

            }
            else
            {
                return "";
            }
        }

    }

 
}
