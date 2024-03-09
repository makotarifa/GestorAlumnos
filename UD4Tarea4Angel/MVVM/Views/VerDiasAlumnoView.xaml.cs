using Firebase.Database;
using System.Collections.ObjectModel;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.MVVM.Models;
using UD4Tarea4Angel.MVVM.ViewModels;

namespace UD4Tarea4Angel.MVVM.Views;

public partial class VerDiasAlumnoView : ContentPage
{
    VerDiasViewModel VDVM;
    FirebaseClient firebaseClient = new FirebaseClient("https://fir-angel-1c1f8-default-rtdb.europe-west1.firebasedatabase.app/");
    public VerDiasAlumnoView(Persona persona)
	{
		InitializeComponent();
        VDVM = new VerDiasViewModel(persona);
		BindingContext = VDVM;
        InitializeAsync();
	}

    private async void InitializeAsync()
    {
        alumnoCabecera.Text = alumnoCabecera.Text + " " + VDVM.PersonaActual.Nombre +" "+ VDVM.PersonaActual.Apellidos;
        VDVM.Dias = await GetAllDaysFromPersona();
    }

    private async Task<ObservableCollection<Dia>> GetAllDaysFromPersona()
    {
        var dias = await firebaseClient
            .Child("Days")
            .OnceAsync<Dia>();

        // Filtrar los dias que contengan el usuario de la persona actual
        List<Dia> diasLista = dias
            .Where(diaSnapshot => diaSnapshot.Object.UserName == VDVM.PersonaActual.UserName)
            .Select(diaSnapshot => diaSnapshot.Object)
            .ToList();

        // Convertir a ObservableCollection
        return new ObservableCollection<Dia>(diasLista);
    }

    private void OnDiaTapped(object sender, ItemTappedEventArgs e)
    {
        Dia diaSeleccionado = (Dia)e.Item;

       Navigation.PushAsync(new VerDiaView(diaSeleccionado));
    }

    //Genera el PDF en base a los dias del alumno.
    private void OnGeneratePDFClicked(object sender, EventArgs e)
    {
       
    }
}