using Firebase.Database;
using System.Collections.ObjectModel;
using UD4Tarea4Angel.MVVM.Models;
using UD4Tarea4Angel.MVVM.ViewModels;

namespace UD4Tarea4Angel.MVVM.Views;

public partial class VerDiaView : ContentPage
{
	SelectorDiaViewModel SDVM = new SelectorDiaViewModel();
    FirebaseClient firebaseClient = new FirebaseClient("https://fir-angel-1c1f8-default-rtdb.europe-west1.firebasedatabase.app/");
    public VerDiaView(Dia diaElegido)
	{
		InitializeComponent();
		SDVM.Dia = diaElegido;
		BindingContext = SDVM;
        InitializationAsync();
    }

    private async void InitializationAsync()
    {
        cabeceraDia.Text = cabeceraDia.Text + " " + SDVM.Dia.Fecha;

        SDVM.Actividades = await GetAllActivitiesFromDay(SDVM.Dia.Key);
    }

    private async Task<ObservableCollection<Actividad>> GetAllActivitiesFromDay(string dayKey)
    {
        var activities = await firebaseClient
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