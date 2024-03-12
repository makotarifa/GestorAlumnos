using Firebase.Database;
using Firebase.Database.Query;
using System.Text.RegularExpressions;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.Utilities;
using UD4Tarea4Angel.ViewModels;

namespace UD4Tarea4Angel.MVVM.Views;

public partial class RegistroView : ContentPage
{
    RegisterUserViewModel RUVM = new RegisterUserViewModel();

    public RegistroView()
	{
        InitializeComponent();
        BindingContext = RUVM;
        MainThread.BeginInvokeOnMainThread(new Action(async () =>
        {
            await (FirebaseConnection.obtenerTokenRegistro());
        }));
    }


    /// <summary>
    /// Evento que se ejecuta cuando se pulsa el botón de registro. Se encarga de registrar un nuevo usuario en la base de datos.
    /// </summary>
    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        //Booleans de las comprobaciones
        bool userExists, profesorExists, correoExists, validEmail;
        string hashPass;

        // Si no se cumplen los campos obligatorios, se muestra un mensaje de error.
        if (!string.IsNullOrWhiteSpace(RUVM.UserItem.UserName) && !string.IsNullOrWhiteSpace(RUVM.UserItem.Password) && !string.IsNullOrWhiteSpace(RUVM.UserItem.Email))
        {
            if (string.IsNullOrWhiteSpace(RUVM.Persona.CentroEstudio))
            {
                RUVM.Persona.CentroEstudio = "No indicado";
            }

            //Comprobaciones de si el usuario y el correo ya existen
            userExists = await CheckIfUserExists(RUVM.UserItem.UserName);
            validEmail = CheckIfEmailValid(RUVM.UserItem.Email);
            correoExists = await CheckIfEmailExists(RUVM.UserItem.Email);

            //Si se cumplen los campos obligatorios, se comprueba si el profesor tutor indicado existe con el metodo CheckIfProfesorUserExists.
            if(!string.IsNullOrWhiteSpace(RUVM.Persona.Nombre) && !string.IsNullOrWhiteSpace(RUVM.Persona.Apellidos) && !string.IsNullOrWhiteSpace(RUVM.Persona.DNI)
                 && !string.IsNullOrWhiteSpace(RUVM.Persona.CentroEstudio) && !string.IsNullOrWhiteSpace(RUVM.Persona.TipoGrado) && !string.IsNullOrWhiteSpace(RUVM.Persona.NombreGrado)
                  && !string.IsNullOrWhiteSpace(RUVM.Persona.ProfesorTutor) && !string.IsNullOrWhiteSpace(RUVM.Persona.CentroTrabajo) && !string.IsNullOrWhiteSpace(RUVM.Persona.TutorLaboral) && !string.IsNullOrWhiteSpace(RUVM.Persona.FotoUrl))
            {
                profesorExists = await CheckIfProfesorUserExists(RUVM.Persona.ProfesorTutor);

                if (validEmail)
                {
                    if (profesorExists)
                    {
                        if (!correoExists)
                        {
                            if (!userExists)
                            {  
                                // Agregar el nuevo usuario si no existe

                                hashPass = Encript.GetSHA256(RUVM.UserItem.Password);

                                await FirebaseConnection.fbAuthClient.CreateUserWithEmailAndPasswordAsync(RUVM.UserItem.Email, hashPass);

                                await FirebaseConnection.firebaseClient.Child("AlumnoUsers").PostAsync(new UserItem
                                {
                                    UserName = RUVM.UserItem.UserName,
                                    //Se encripta la contraseña
                                    Password = hashPass,
                                    Email = RUVM.UserItem.Email
                                });

                                //Se asocian los datos de la persona con el usuario
                                RUVM.Persona.UserName = RUVM.UserItem.UserName;

                                //Se añade la persona a la base de datos
                                await FirebaseConnection.firebaseClient.Child("DatosPersona").Child(RUVM.Persona.Key).PutAsync(RUVM.Persona);


                                await this.DisplayAlert("Confirmacion", "Usuario creado con exito.", "Vale");

                                FirebaseConnection.cerrarFirebase();

                                await Navigation.PopAsync();
                            }
                            else
                            {
                                await this.DisplayAlert("Error", "El usuario ya existe.", "Vale");
                            }
                        }
                        else
                        {
                            await this.DisplayAlert("Error", "Ya existe un usuario con ese correo electronico.", "Vale");
                        }

                    }
                    else
                    {
                        await this.DisplayAlert("Error", "No existe ningun profesor con ese nombre de usuario.", "Vale");
                    }
                }
                else
                {
                    await this.DisplayAlert("Error", "El correo no tiene un formato correcto.", "Vale");
                }
            } else
            {
                await this.DisplayAlert("Error", "Revisa los campos obligatorios.", "Vale");
            }   
        }
        else
        {
            await this.DisplayAlert("Error", "Debe rellenar los campos de Usuario, Correo y Contraseña.", "Vale");
        }
    }

    /// <summary>
    /// Evento que se ejecuta cuando se pulsa el botón de subir imagen. Se encarga de subir una imagen a la base de datos.
    /// </summary>
    private async void OnSubirImagenClicked(object sender, EventArgs e)
    {
        var foto = await FirebaseConnection.storageUploadPhoto();
        RUVM.Persona.FotoUrl = foto.ToString();
        fotoURL.Source = foto.ToString();
    }

    /// <summary>
    /// Comprueba si el email tiene un formato valido.
    /// </summary>
    /// <param name="email">
    /// El email a comprobar.
    /// </param>
    /// <returns></returns>
    private bool CheckIfEmailValid(string email)
    {
        //Patron para comprobar si el email tiene un formato valido
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        return Regex.IsMatch(email, pattern);

    }

    private async Task<bool> CheckIfUserExists(string userName)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var users = await FirebaseConnection.firebaseClient.Child("AlumnoUsers").OnceAsync<UserItem>();

        // Devuelve si existe algun objeto
        return users.Any(u => u.Object.UserName == userName);

    }

    private async Task<bool> CheckIfEmailExists(string email)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var users = await FirebaseConnection.firebaseClient.Child("AlumnoUsers").OnceAsync<UserItem>();

        // Devuelve si existe algun objeto
        return users.Any(u => u.Object.Email == email);

    }

    private async Task<bool> CheckIfProfesorUserExists(string userName)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var users = await FirebaseConnection.firebaseClient.Child("ProfesorUsers").OnceAsync<UserItem>();

        // Devuelve si existe algun objeto
        return users.Any(u => u.Object.UserName == userName);

    }


}