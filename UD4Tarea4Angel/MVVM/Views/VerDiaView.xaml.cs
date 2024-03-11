using Firebase.Database;
using System.Collections.ObjectModel;
using UD4Tarea4Angel.MVVM.Models;
using UD4Tarea4Angel.MVVM.ViewModels;
using UD4Tarea4Angel.Utilities;

namespace UD4Tarea4Angel.MVVM.Views;

/// <summary>
/// Clase que representa la vista de la interfaz para la visualizaci�n de un d�a.
/// </summary>
public partial class VerDiaView : ContentPage
{
	SelectorDiaViewModel SDVM = new SelectorDiaViewModel();
    public VerDiaView(Dia diaElegido)
	{
		InitializeComponent();
		SDVM.Dia = diaElegido;
		BindingContext = SDVM;
        InitializationAsync();
    }

    /// <summary>
    /// Metodo que inicializa la vista con los datos del d�a seleccionado en asincrono.
    /// </summary>
    private async void InitializationAsync()
    {
        // Se actualiza la cabecera con la fecha del d�a con el formato corto del Datetime.
        cabeceraDia.Text = cabeceraDia.Text + " " + SDVM.Dia.Fecha.ToShortDateString();

        // Se obtienen las actividades del d�a seleccionado.
        SDVM.Actividades = await GetAllActivitiesFromDay(SDVM.Dia.Key);
    }

    /// <summary>
    /// Metodo que obtiene todas las actividades de un d�a.
    /// </summary>
    /// <param name="dayKey">
    ///  El identificador �nico del d�a que se va a buscar.
    /// </param>
    /// <returns></returns>
    private async Task<ObservableCollection<Actividad>> GetAllActivitiesFromDay(string dayKey)
    {
        //Obtiene la lista de actividades de la base de datos del child "Actividades".
        var activities = await FirebaseConnection.firebaseClient
            .Child("Actividades")
            .OnceAsync<Actividad>();

        // Filtra las actividades por DiaKey
        List<Actividad> actividadesLista = activities
            .Where(activitySnapshot => activitySnapshot.Object.DiaKey == dayKey)
            .Select(activitySnapshot => activitySnapshot.Object)
            .ToList();

        // Convertir a ObservableCollection
        return new ObservableCollection<Actividad>(actividadesLista);
    }


}