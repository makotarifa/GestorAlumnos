using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using UD4Tarea4Angel.MVVM.Models;
using UD4Tarea4Angel.MVVM.ViewModels;

namespace UD4Tarea4Angel.MVVM.Views;

public partial class SelectorDiasView : ContentPage
{
    SelectorDiaViewModel SDVM = new SelectorDiaViewModel();
    FirebaseClient firebaseClient = new FirebaseClient("https://fir-angel-1c1f8-default-rtdb.europe-west1.firebasedatabase.app/");
    string currentUsername;
    public SelectorDiasView(string userName)
	{
        currentUsername = userName;
		InitializeComponent();
        BindingContext = SDVM;
        miDatePicker.Date = DateTime.Now;

    }

    private async void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        bool existsDay;

        existsDay = await CheckIfDayExistsOnDatabase(SDVM.Dia.Fecha, currentUsername);

        // Si existe el día, coge el día de la base de datos, si no genera uno en blanco con la fecha de DatePicker

        if (existsDay)
        {
            SDVM.Dia = await GetDayFromDatabase(SDVM.Dia.Fecha, currentUsername);
            SDVM.Actividades = await GetAllActivitiesFromDay(SDVM.Dia.Key);

        }
        else
        {
            SDVM.Dia = new Dia { 
                Fecha = miDatePicker.Date,
                UserName = currentUsername,
            };

            SDVM.Actividades = new ObservableCollection<Actividad>();
            SDVM.ActividadActual = new Actividad();

        }

        SDVM.ActividadActual.DiaKey = SDVM.Dia.Key;

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

    private async Task<bool> CheckIfDayExistsOnDatabase(DateTime fecha, string userName)
    {
        // Realizar una consulta para verificar si el dia ya existe o no
        var users = await firebaseClient.Child("Days").OnceAsync<Dia>();

        // Devuelve si existe algun objeto
        return users.Any(u => u.Object.UserName == userName && u.Object.Fecha == fecha);

    }

    private async Task<Dia> GetDayFromDatabase(DateTime fecha, string userName)
    {
        // Realizar una consulta para verificar si el dia ya existe
        var days = await firebaseClient.Child("Days").OnceAsync<Dia>();

        // Buscar el día en la lista
        var dia = days.FirstOrDefault(d => d.Object.UserName == userName && d.Object.Fecha == fecha);

        // Devuelve el objeto Dia si se encuentra
        return dia.Object;
    }

    public async Task<bool> ActivityExistOnDay(String key, String dayKey)
    {
        // Realizar una consulta para verificar so la actividad ya existe
        var activities = await firebaseClient.Child("Actividades").OnceAsync<Actividad>();

        // Buscar el día en la lista
        return activities.Any(d => d.Object.Key == key && d.Object.DiaKey == dayKey);


    }

    public async Task<Actividad> GetActivity(String key, String dayKey)
    {
        // Realizar una consulta para verificar so la actividad ya existe
        var activities = await firebaseClient.Child("Actividades").OnceAsync<Actividad>();

        // Buscar el día en la lista
        var activity = activities.FirstOrDefault(d => d.Object.Key == key && d.Object.DiaKey == dayKey);

        // Devuelve el objeto Dia si se encuentra
        return activity.Object;

    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        var fecha = SDVM.Dia.Fecha;

        var actividadDesarrollada = SDVM.ActividadActual.ActividadDesarrollada;
        var tiempoEmpleado = SDVM.ActividadActual.TiempoEmpleado;
        var observaciones = SDVM.ActividadActual.Observaciones;

        bool existsDay;

        existsDay = await CheckIfDayExistsOnDatabase(fecha, currentUsername);

        if (!string.IsNullOrWhiteSpace(actividadDesarrollada))
        {
            if (string.IsNullOrWhiteSpace(observaciones))
            {
                SDVM.ActividadActual.Observaciones = "Sin observaciones.";
            }

            // Si el día ya existe, actualizar en lugar de agregar
            if (existsDay)
            {
                // Obtener el día existente desde la base de datos
                var existingDay = await GetDayFromDatabase(fecha, currentUsername);

                if (await ActivityExistOnDay(SDVM.ActividadActual.Key, SDVM.ActividadActual.DiaKey))
                {
                    await this.DisplayAlert("Confirmacion", "Entro aqui.", "Vale");

                    //Tener en cuenta que obtiene el objeto por la ActividadRealizada.
                    //Se borra el antiguo objeto Actividad con el mismo nombre.
                    await firebaseClient
                        .Child("Actividades")
                        .Child(SDVM.ActividadActual.Key) 
                        .DeleteAsync();

                    await firebaseClient
                        .Child("Actividades").Child(SDVM.ActividadActual.Key)
                        .PutAsync(SDVM.ActividadActual);

                    await this.DisplayAlert("Confirmacion", "Se ha actualizado la actividad.", "Vale");

                } else
                {
                    await firebaseClient
                        .Child("Actividades").Child(SDVM.ActividadActual.Key)
                        .PutAsync(SDVM.ActividadActual);

                    await this.DisplayAlert("Confirmacion", "Se ha agregado la actividad.", "Vale");
                }

                // Realizar la actualización en la base de datos


            }

            else

            {
                // Si la actividad no existe, agregar como nuevo Dia y nueva Activity

                await firebaseClient.Child("Days").Child(SDVM.Dia.Key).PutAsync(SDVM.Dia);

                await firebaseClient
                    .Child("Actividades").Child(SDVM.ActividadActual.Key)
                    .PutAsync(SDVM.ActividadActual);

                await this.DisplayAlert("Confirmacion", "Se ha agregado el dia y la actividad.", "Vale");

                AgregarEditarBotton.Text = "Editar";
            }

            SDVM.Actividades = await GetAllActivitiesFromDay(SDVM.Dia.Key);

            SDVM.ActividadActual = new Actividad
            {
                DiaKey = SDVM.Dia.Key,
            };


        }
        else
        {
            await this.DisplayAlert("Error", "Los campos no pueden estar en blanco.", "Vale");
        }

    }

    private void OnActividadTapped (object sender, ItemTappedEventArgs e)
    {
        Actividad actividadSeleccionada = (Actividad) e.Item;

        SDVM.ActividadActual = actividadSeleccionada;

    }



}