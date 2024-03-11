using Firebase.Database;
using System.Collections.ObjectModel;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.MVVM.ViewModels;
using UD4Tarea4Angel.Utilities;

namespace UD4Tarea4Angel.MVVM.Views;

/// <summary>
/// Clase que representa la vista de selección de alumnos en la interfaz del profesor.
/// </summary>
public partial class SelectorAlumnosView : ContentPage
{
	string currentUsername;
    SelectorAlumnosViewModel SAVM = new SelectorAlumnosViewModel();
    public SelectorAlumnosView(string currentUsername)
	{
		InitializeComponent();
		this.currentUsername = currentUsername;
        BindingContext = SAVM;
        InitializeAsync();
	}

    /// <summary>
    /// Se inicializa la vista con los datos de los alumnos del profesor actual en asincrono.
    /// </summary>
    private async void InitializeAsync()
    {
        SAVM.AlumnosCollection = await GetAllPersonasFromProfesor(currentUsername);
    }
    /// <summary>
    /// Al pulsar un alumno de la lista, se abre la vista de VerDiasAlumnoView con el alumno seleccionado.
    /// </summary>
    private void OnAlumnoTapped(object sender, ItemTappedEventArgs e)
    {
        Persona personaSeleccionada = (Persona)e.Item;

        Navigation.PushAsync(new VerDiasAlumnoView(personaSeleccionada));
    }

    /// <summary>
    /// Obtiene todas las personas que tienen el usuario del profesor actual.
    /// </summary>
    private async Task<ObservableCollection<Persona>> GetAllPersonasFromProfesor(string dayKey)
    {
        var activities = await FirebaseConnection.firebaseClient
            .Child("DatosPersona")
            .OnceAsync<Persona>();

        // Filtrar las personas que tengan el usuario del profesor actual
        List<Persona> alumnosLista = activities
            .Where(activitySnapshot => activitySnapshot.Object.ProfesorTutor == currentUsername)
            .Select(activitySnapshot => activitySnapshot.Object)
            .ToList();


        // Convertir a ObservableCollection
        return new ObservableCollection<Persona>(alumnosLista);
    }

}