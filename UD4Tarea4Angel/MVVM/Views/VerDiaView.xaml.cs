using Firebase.Database;
using System.Collections.ObjectModel;
using UD4Tarea4Angel.MVVM.Models;
using UD4Tarea4Angel.MVVM.ViewModels;
using UD4Tarea4Angel.Utilities;

namespace UD4Tarea4Angel.MVVM.Views;

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

    private async void InitializationAsync()
    {
        cabeceraDia.Text = cabeceraDia.Text + " " + SDVM.Dia.Fecha.ToShortDateString();

        SDVM.Actividades = await GetAllActivitiesFromDay(SDVM.Dia.Key);
    }

    private async Task<ObservableCollection<Actividad>> GetAllActivitiesFromDay(string dayKey)
    {
        var activities = await FirebaseConnection.firebaseClient
            .Child("Actividades")
            .OnceAsync<Actividad>();

        // Filtrar las actividades por DiaKey
        List<Actividad> actividadesLista = activities
            .Where(activitySnapshot => activitySnapshot.Object.DiaKey == dayKey)
            .Select(activitySnapshot => activitySnapshot.Object)
            .ToList();

        // Convertir a ObservableCollection
        return new ObservableCollection<Actividad>(actividadesLista);
    }


}