using Firebase.Database;
using Firebase.Database.Query;
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

    }


    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        bool userExists, profesorExists;


        if (!string.IsNullOrWhiteSpace(RUVM.UserItem.UserName) && !string.IsNullOrWhiteSpace(RUVM.UserItem.Password) )
        {
            if (string.IsNullOrWhiteSpace(RUVM.Persona.CentroEstudio))
            {
                RUVM.Persona.CentroEstudio = "No indicado";
            }

            userExists = await CheckIfUserExists(RUVM.UserItem.UserName);

            if(!string.IsNullOrWhiteSpace(RUVM.Persona.Nombre) && !string.IsNullOrWhiteSpace(RUVM.Persona.Apellidos) && !string.IsNullOrWhiteSpace(RUVM.Persona.DNI)
                 && !string.IsNullOrWhiteSpace(RUVM.Persona.CentroEstudio) && !string.IsNullOrWhiteSpace(RUVM.Persona.TipoGrado) && !string.IsNullOrWhiteSpace(RUVM.Persona.NombreGrado)
                  && !string.IsNullOrWhiteSpace(RUVM.Persona.ProfesorTutor) && !string.IsNullOrWhiteSpace(RUVM.Persona.CentroTrabajo) && !string.IsNullOrWhiteSpace(RUVM.Persona.TutorLaboral))
            {
                profesorExists = await CheckIfProfesorUserExists(RUVM.Persona.ProfesorTutor);

                if (profesorExists) {
                    if (!userExists)
                    {  // Agregar el nuevo usuario si no existe
                        await FirebaseConnection.firebaseClient.Child("AlumnoUsers").PostAsync(new UserItem
                        {
                            UserName = RUVM.UserItem.UserName,
                            Password = Encript.GetSHA256(RUVM.UserItem.Password),
                            Email = RUVM.UserItem.Email
                        });

                        RUVM.Persona.UserName = RUVM.UserItem.UserName;
                        await FirebaseConnection.firebaseClient.Child("DatosPersona").Child(RUVM.Persona.Key).PutAsync(RUVM.Persona);


                        await this.DisplayAlert("Confirmacion", "Usuario creado con exito.", "Vale");

                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await this.DisplayAlert("Error", "El usuario ya existe.", "Vale");
                    }
                } else
                {
                    await this.DisplayAlert("Error", "No existe ningun profesor con ese nombre de usuario.", "Vale");
                }
            } else
            {
                await this.DisplayAlert("Error", "Revisa los campos obligatorios.", "Vale");
            }

                
        }
        else
        {
            await this.DisplayAlert("Error", "Debe rellenar los campos de Usuario y Contraseña.", "Vale");
        }
    }

    private async Task<bool> CheckIfUserExists(string userName)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var users = await FirebaseConnection.firebaseClient.Child("AlumnoUsers").OnceAsync<UserItem>();

        // Devuelve si existe algun objeto
        return users.Any(u => u.Object.UserName == userName);

    }

    private async Task<bool> CheckIfProfesorUserExists(string userName)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var users = await FirebaseConnection.firebaseClient.Child("ProfesorUsers").OnceAsync<UserItem>();

        // Devuelve si existe algun objeto
        return users.Any(u => u.Object.UserName == userName);

    }


}