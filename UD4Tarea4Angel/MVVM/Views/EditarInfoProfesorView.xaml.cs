using Firebase.Database;
using Firebase.Database.Query;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.Utilities;
using UD4Tarea4Angel.ViewModels;

namespace UD4Tarea4Angel.MVVM.Views;

public partial class EditarInfoProfesorView : ContentPage
{
	string currentUsername;
    RegisterUserViewModel RUVM = new RegisterUserViewModel();

    public EditarInfoProfesorView(string userName)
	{
		currentUsername = userName;
		InitializeComponent();
        InitializePageAsync();
    }

    private async void InitializePageAsync()
    {
        RUVM.Persona = await GetPersona(currentUsername);

        BindingContext = RUVM;
    }

    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(RUVM.Persona.CentroEstudio))
        {
            RUVM.Persona.CentroEstudio = "No indicado";
        }

        if (!string.IsNullOrWhiteSpace(RUVM.Persona.Nombre) && !string.IsNullOrWhiteSpace(RUVM.Persona.Apellidos) && !string.IsNullOrWhiteSpace(RUVM.Persona.DNI)
             && !string.IsNullOrWhiteSpace(RUVM.Persona.CentroEstudio))
        {

            await FirebaseConnection.firebaseClient.Child("DatosProfesor").Child(RUVM.Persona.Key).DeleteAsync();

            await FirebaseConnection.firebaseClient.Child("DatosProfesor").Child(RUVM.Persona.Key).PutAsync(RUVM.Persona);


                await this.DisplayAlert("Confirmacion", "Informacion modificada con exito.", "Vale");

                await Navigation.PopAsync();
            
        }
        else
        {
            await this.DisplayAlert("Error", "Revisa los campos obligatorios.", "Vale");
        }
    }

    private async Task<Persona> GetPersona(string userName)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var datosPersonas = await FirebaseConnection.firebaseClient.Child("DatosProfesor").OnceAsync<Persona>();

        // Devuelve si existe algun objeto
        var persona = datosPersonas.FirstOrDefault(u => u.Object.UserName == userName);

        return persona.Object;

    }
}