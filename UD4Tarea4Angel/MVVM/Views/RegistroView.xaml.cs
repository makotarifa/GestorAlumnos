using Firebase.Database;
using Firebase.Database.Query;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.Utilities;
using UD4Tarea4Angel.ViewModels;

namespace UD4Tarea4Angel.MVVM.Views;

public partial class RegistroView : ContentPage
{
    RegisterUserViewModel RUVM = new RegisterUserViewModel();
    FirebaseClient firebaseClient = new FirebaseClient("https://fir-angel-1c1f8-default-rtdb.europe-west1.firebasedatabase.app/");
    public RegistroView()
	{
        InitializeComponent();
        BindingContext = RUVM;

    }


    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        bool userExists;
        var userName = RUVM.UserItem.UserName;
        var password = RUVM.UserItem.Password;
        var email = RUVM.UserItem.Email;
        var nombre = RUVM.Persona.Nombre;
        var apellidos = RUVM.Persona.Apellidos;
        var dni = RUVM.Persona.DNI;
        var centro = RUVM.Persona.CentroEstudio;
        var tipoGrado = RUVM.Persona.TipoGrado;
        var nombreGrado = RUVM.Persona.NombreGrado;
        var profesor = RUVM.Persona.Profesor;
        var tutor = RUVM.Persona.Tutor;


        if (!string.IsNullOrWhiteSpace(nombre) && !string.IsNullOrWhiteSpace(apellidos))
        {
            if (string.IsNullOrWhiteSpace(centro))
            {
                centro = "No indicado";
            }

            userExists = await CheckIfUserExists(userName);

            if(!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
            {
                if (!userExists)
                {  // Agregar el nuevo usuario si no existe
                    await firebaseClient.Child("Users").PostAsync(new UserItem
                    {
                        UserName = userName,
                        Password = Encript.GetSHA256(password),
                        Email = email
                    });

                    await firebaseClient.Child("DatosPersona").PostAsync(new Persona
                    {
                        Nombre = nombre,
                        Apellidos = apellidos,
                        DNI = dni,
                        CentroEstudio = centro,
                        TipoGrado = tipoGrado,
                        NombreGrado = nombreGrado,
                        Profesor = profesor,
                        Tutor = tutor,
                        UserName = userName,
                    }) ;


                    await this.DisplayAlert("Confirmacion", "Usuario creado con exito.", "Vale");

                    await Navigation.PopAsync();
                }
                else
                {
                    await this.DisplayAlert("Error", "El usuario ya existe.", "Vale");
                }
            } else
            {
                await this.DisplayAlert("Error", "Los datos personales no pueden estar en blanco.", "Vale");
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
        var users = await firebaseClient.Child("Users").OnceAsync<UserItem>();

        // Devuelve si existe algun objeto
        return users.Any(u => u.Object.UserName == userName);

    }


}