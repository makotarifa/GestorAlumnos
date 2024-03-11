using Firebase.Database;
using System.Collections.ObjectModel;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.MVVM.ViewModels;
using UD4Tarea4Angel.Utilities;

namespace UD4Tarea4Angel.MVVM.Views;

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

    private async void InitializeAsync()
    {
        SAVM.AlumnosCollection = await GetAllPersonasFromProfesor(currentUsername);
    }

    private void OnAlumnoTapped(object sender, ItemTappedEventArgs e)
    {
        Persona personaSeleccionada = (Persona)e.Item;

        Navigation.PushAsync(new VerDiasAlumnoView(personaSeleccionada));
    }

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