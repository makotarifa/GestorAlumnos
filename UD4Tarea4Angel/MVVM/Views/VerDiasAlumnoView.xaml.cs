using Firebase.Database;
using System.Collections.ObjectModel;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.MVVM.Models;
using UD4Tarea4Angel.MVVM.ViewModels;
using UD4Tarea4Angel.Utilities;

namespace UD4Tarea4Angel.MVVM.Views;

/// <summary>
/// Clase que representa la vista de la visualizaci�n de los d�as de un alumno.
/// </summary>
public partial class VerDiasAlumnoView : ContentPage
{
    VerDiasViewModel VDVM;
    public VerDiasAlumnoView(Persona persona)
	{
		InitializeComponent();
        VDVM = new VerDiasViewModel(persona);
		BindingContext = VDVM;
        InitializeAsync();
	}

    /// <summary>
    /// Metodo que inicializa la vista con los datos de los d�as del alumno seleccionado en asincrono, tambien cambia la cabecera para que muestre el nombre del alumno.
    /// </summary>
    private async void InitializeAsync()
    {
        alumnoCabecera.Text = alumnoCabecera.Text + " " + VDVM.PersonaActual.Nombre +" "+ VDVM.PersonaActual.Apellidos;
        VDVM.Dias = await GetAllDaysFromPersona();
    }


    /// <summary>
    /// Obtiene todos los d�as de la persona que se trabaja en el ViewModel.
    /// </summary>
    /// <returns>
    /// Una ObservableCollection de d�as de la persona.
    /// </returns>
    private async Task<ObservableCollection<Dia>> GetAllDaysFromPersona()
    {
        var dias = await FirebaseConnection.firebaseClient
            .Child("Days")
            .OnceAsync<Dia>();

        // Filtra los dias que contengan el usuario de la persona actual
        List<Dia> diasLista = dias
            .Where(diaSnapshot => diaSnapshot.Object.UserName == VDVM.PersonaActual.UserName)
            .Select(diaSnapshot => diaSnapshot.Object).OrderBy(dia => dia.Fecha)
            .ToList();

        // Convertir a ObservableCollection
        return new ObservableCollection<Dia>(diasLista);
    }

    /// <summary>
    /// Metodo que se ejecuta cuando se pulsa un d�a de la lista. Se encarga de abrir la vista de VerDiaView con el d�a seleccionado.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnDiaTapped(object sender, ItemTappedEventArgs e)
    {
        Dia diaSeleccionado = (Dia)e.Item;

       Navigation.PushAsync(new VerDiaView(diaSeleccionado));
    }

    /// <summary>
    ///  Evento que se ejecuta cuando se pulsa el bot�n de generar PDF.
    /// </summary>
    private void OnGeneratePDFClicked(object sender, EventArgs e)
    {
        //Se va a la view que genera el PDF en base a los dias del alumno.
        Navigation.PushAsync(new PDFView(VDVM.PersonaActual.UserName));
    }
}