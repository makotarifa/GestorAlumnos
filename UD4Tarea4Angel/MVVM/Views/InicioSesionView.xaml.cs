using Firebase.Database;
using Firebase.Database.Query;
using System.Runtime.CompilerServices;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.Utilities;
using UD4Tarea4Angel.ViewModels;

namespace UD4Tarea4Angel.MVVM.Views;

/// <summary>
/// View que representa la página de inicio de sesión del usuario.
/// </summary>
public partial class InicioSesionView : ContentPage
{
    UserViewModel uVM = new UserViewModel();
    bool modoProfesor = false;
    public InicioSesionView()
	{
        InitializeComponent();
        BindingContext = uVM;

    }

    //Texto que indica el modo seleccionado en el switch.
    private void OnSwitchToggled ( object sender, ToggledEventArgs e)
    {
        modoProfesor = e.Value;

        if (!modoProfesor)
        {
            lblModo.Text = "Modo Alumno";
        } else
        {
            lblModo.Text = "Modo Profesor";
        }

    }

    /// <summary>
    /// Evento que se ejecuta cuando se pulsa el botón de login. Se encarga de comprobar si el usuario y la contraseña son correctos y de abrir la página de menú principal.
    /// </summary>
    private async void OnLoginClicked (object sender, EventArgs e)
    {
        string hashPassword, actualUsername;
        var email = uVM.UserItem.Email;
        var password = uVM.UserItem.Password;
        bool userCorrect;


        //Si los campos de email y contraseña no están vacíos, se comprueba si el usuario y la contraseña son correctos.
        if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
        {
            hashPassword = Encript.GetSHA256(password);
            userCorrect = await CheckUserAndPass(email, hashPassword);

            if (userCorrect)
            {
                actualUsername = await GetUsername(email);
                //Si el usuario es correcto, se abre la página de menú principal. Segun el modo seleccionado, se abre el menú principal de profesor o de alumno.
                if (!modoProfesor)
                {
                    await Navigation.PushAsync(new MenuPrincipal(actualUsername));
                } else
                {
                    await Navigation.PushAsync(new MenuPrincipalProfesor(actualUsername));
                }


            } else
            {
                await this.DisplayAlert("Error", "El usuario o la contraseña no son correctos.", "Vale");
            }

        } else
        {
            await this.DisplayAlert("Error", "Debe rellenar los campos de Usuario y Contraseña.", "Vale");
        }

    }

    /// <summary>
    /// Metodo que devuelve el nombre de usuario a partir del email.
    /// </summary>
    private async Task<string> GetUsername (string email)
    {
        var users = await FirebaseConnection.firebaseClient.Child("AlumnoUsers").OnceAsync<UserItem>();

        //Si se ha seleccionado el modo profesor, se buscan los usuarios de profesor en "ProfesorUsers" si no, se buscan en "AlumnoUsers".
        if (modoProfesor)
        {
            users = await FirebaseConnection.firebaseClient.Child("ProfesorUsers").OnceAsync<UserItem>();
        }

        var user = users.Where(u => u.Object.Email == email).FirstOrDefault();

        return user.Object.UserName;
    }

    //Si se pulsa el boton de registro, se abre la pagina de registro correspondiente a si se ha seleccionado el modo profesor o no.
    private void OnRegisterClicked(object sender, EventArgs e)
    {
        if (!modoProfesor)
        {
            //Modo alumno
            Navigation.PushAsync(new RegistroView());
        } else
        {
            Navigation.PushAsync(new RegistroProfesorView());
        }

    }

    /// <summary>
    /// Metodo que comprueba si el usuario y la contraseña son correctos.
    /// </summary>
    private async Task<bool> CheckUserAndPass(string email, string hashPassword)
    {
        // Realizar una consulta para verificar si el usuario ya existe

        var users = await FirebaseConnection.firebaseClient.Child("ProfesorUsers").OnceAsync<UserItem>();

        // Si no se ha seleccionado el modo profesor, se buscan los usuarios de alumnos.
        if (!modoProfesor)
        {
            users = await FirebaseConnection.firebaseClient.Child("AlumnoUsers").OnceAsync<UserItem>();
        } 

        // Devuelve si existe algun objeto
        return users.Any(u => u.Object.Email == email && u.Object.Password == hashPassword);

    }


}