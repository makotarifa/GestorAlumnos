using Firebase.Database;
using Firebase.Database.Query;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.Utilities;
using UD4Tarea4Angel.ViewModels;

namespace UD4Tarea4Angel.MVVM.Views;

/// <summary>
/// View para la edici�n de la informaci�n de un profesor.
/// </summary>
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

    /// Como no se puede utilizar el await en el constructor, se inicializa la p�gina con los datos del usuario actual de manera as�ncrona.
    private async void InitializePageAsync()
    {
        RUVM.Persona = await GetPersona(currentUsername);

        BindingContext = RUVM;
    }

    /// <summary>
    /// Evento que se ejecuta cuando se pulsa el bot�n de actualizar. Se encarga de actualizar la informaci�n del usuario en la base de datos.
    /// </summary>
    private async void OnActualizarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(RUVM.Persona.CentroEstudio))
        {
            RUVM.Persona.CentroEstudio = "No indicado";
        }
        //Si no se cumplen los campos obligatorios, se muestra un mensaje de error.

        if (!string.IsNullOrWhiteSpace(RUVM.Persona.Nombre) && !string.IsNullOrWhiteSpace(RUVM.Persona.Apellidos) && !string.IsNullOrWhiteSpace(RUVM.Persona.DNI)
             && !string.IsNullOrWhiteSpace(RUVM.Persona.CentroEstudio))
        {

            //Se actualiza la informaci�n del usuario en la base de datos.
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

    /// <summary>
    /// M�todo que se encarga de obtener la informaci�n de un profesor mediante su nombre de usuario.
    /// </summary>
    private async Task<Persona> GetPersona(string userName)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var datosPersonas = await FirebaseConnection.firebaseClient.Child("DatosProfesor").OnceAsync<Persona>();

        // Devuelve si existe algun objeto
        var persona = datosPersonas.FirstOrDefault(u => u.Object.UserName == userName);

        return persona.Object;

    }
}