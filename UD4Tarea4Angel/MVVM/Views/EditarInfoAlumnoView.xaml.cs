using Firebase.Database;
using Firebase.Database.Query;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.ViewModels;

namespace UD4Tarea4Angel.MVVM.Views;

public partial class EditarInfoAlumnoView : ContentPage
{
	string currentUsername;
    RegisterUserViewModel RUVM = new RegisterUserViewModel();
    FirebaseClient firebaseClient = new FirebaseClient("https://fir-angel-1c1f8-default-rtdb.europe-west1.firebasedatabase.app/");

    public EditarInfoAlumnoView(string userName)
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
             && !string.IsNullOrWhiteSpace(RUVM.Persona.CentroEstudio) && !string.IsNullOrWhiteSpace(RUVM.Persona.TipoGrado) && !string.IsNullOrWhiteSpace(RUVM.Persona.NombreGrado)
              && !string.IsNullOrWhiteSpace(RUVM.Persona.ProfesorTutor) && !string.IsNullOrWhiteSpace(RUVM.Persona.CentroTrabajo) && !string.IsNullOrWhiteSpace(RUVM.Persona.TutorLaboral))
        {

            await firebaseClient.Child("DatosPersona").Child(RUVM.Persona.Key).DeleteAsync();

            await firebaseClient.Child("DatosPersona").Child(RUVM.Persona.Key).PutAsync(RUVM.Persona);


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
        var datosPersonas = await firebaseClient.Child("DatosPersona").OnceAsync<Persona>();

        // Devuelve si existe algun objeto
        var persona = datosPersonas.FirstOrDefault(u => u.Object.UserName == userName);

        return persona.Object;

    }
}