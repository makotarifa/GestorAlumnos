using System.Runtime.CompilerServices;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.Utilities;
using UD4Tarea4Angel.ViewModels;

namespace UD4Tarea4Angel.MVVM.Views;

public partial class MenuPrincipal : ContentPage
{
	private string currentUsername;
    RegisterUserViewModel RUVM = new RegisterUserViewModel();
    public MenuPrincipal(string username)
	{
		currentUsername = username;
        InitializeComponent();

        labelCabecera.Text = $"¡Bienvenido {currentUsername}!";
        InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        RUVM.Persona = await GetPersona(currentUsername);
        fotoURL.Source = RUVM.Persona.FotoUrl;
    }

    /// <summary>
    /// Se abre el gestor de dias de los alumnos para agregar actividades. (Digo agregar porque tengo el teclado en ingles y no puedo poner la tilde :( 
    /// </summary>
    private async void OnDiasClicked(object sender, EventArgs e)
	{

        await Navigation.PushAsync(new SelectorDiasView(currentUsername));
    }

    /// <summary>
    /// Se abre la View para editar la información personal del alumno.
    /// </summary>
    private async void OnGestionarClicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new EditarInfoAlumnoView(currentUsername));
    }

    private async Task<Persona> GetPersona(string userName)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var datosPersona = await FirebaseConnection.firebaseClient.Child("DatosPersona").OnceAsync<Persona>();

        // Devuelve si existe algun objeto
        var persona = datosPersona.FirstOrDefault(u => u.Object.UserName == userName);

        return persona.Object;

    }




}