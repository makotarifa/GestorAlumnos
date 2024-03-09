using Firebase.Database;
using Firebase.Database.Query;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.Utilities;
using UD4Tarea4Angel.ViewModels;

namespace UD4Tarea4Angel.MVVM.Views;

public partial class InicioSesionView : ContentPage
{
    UserViewModel uVM = new UserViewModel();
    bool modoProfesor = false;
    FirebaseClient firebaseClient = new FirebaseClient("https://fir-angel-1c1f8-default-rtdb.europe-west1.firebasedatabase.app/");
    public InicioSesionView()
	{
        InitializeComponent();
        BindingContext = uVM;

    }

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

    private async void OnLoginClicked (object sender, EventArgs e)
    {
        string hashPassword;
        var userName = uVM.UserItem.UserName;
        var password = uVM.UserItem.Password;
        bool userCorrect;



        if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
        {
            hashPassword = Encript.GetSHA256(password);
            userCorrect = await CheckUserAndPass(userName, hashPassword);
            if (userCorrect)
            {
                if (!modoProfesor)
                {
                    await Navigation.PushAsync(new MenuPrincipal(userName));
                } else
                {
                    await Navigation.PushAsync(new MenuPrincipalProfesor(userName));
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

    private void OnRegisterClicked(object sender, EventArgs e)
    {
        if (!modoProfesor)
        {
            Navigation.PushAsync(new RegistroView());
        } else
        {
            Navigation.PushAsync(new RegistroProfesorView());
        }

    }

    private async Task<bool> CheckUserAndPass(string userName, string hashPassword)
    {
        // Realizar una consulta para verificar si el usuario ya existe

        var users = await firebaseClient.Child("ProfesorUsers").OnceAsync<UserItem>();

        if (!modoProfesor)
        {
            users = await firebaseClient.Child("AlumnoUsers").OnceAsync<UserItem>();
        } 

        // Devuelve si existe algun objeto
        return users.Any(u => u.Object.UserName == userName && u.Object.Password == hashPassword);

    }


}