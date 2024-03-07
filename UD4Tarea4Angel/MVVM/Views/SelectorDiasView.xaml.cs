using Firebase.Database;
using Firebase.Database.Query;
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

        
        //Si existe el dia, coge el dia de la base de datos, si no genera un en blanco con la fecha de DatePicker
        if (existsDay)
        {
            AgregarEditarBotton.Text = "Editar";
            SDVM.Dia = await GetDayFromDatabase(SDVM.Dia.Fecha, currentUsername);
        } else
        {
            AgregarEditarBotton.Text = "Agregar";
            SDVM.Dia = new Dia{
                Fecha = miDatePicker.Date,
                ActividadDesarrollada = "",
                TiempoEmpleado = 1,
                Observaciones = "",
                UserName = currentUsername,
            };
        }
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

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        var fecha = SDVM.Dia.Fecha;
        var actividadDesarrollada = SDVM.Dia.ActividadDesarrollada;
        var tiempoEmpleado = SDVM.Dia.TiempoEmpleado;
        var observaciones = SDVM.Dia.Observaciones;

        bool existsDay;

        existsDay = await CheckIfDayExistsOnDatabase(fecha, currentUsername);

        if (!string.IsNullOrWhiteSpace(actividadDesarrollada))
        {
            if (string.IsNullOrWhiteSpace(observaciones))
            {
                SDVM.Dia.Observaciones = "Sin observaciones.";
            }

            // Si el día ya existe, actualizar en lugar de agregar
            if (existsDay)
            {
                // Obtener el día existente desde la base de datos
                var existingDay = await GetDayFromDatabase(fecha, currentUsername);

                // Actualizar las propiedades del día existente
                existingDay.ActividadDesarrollada = actividadDesarrollada;
                existingDay.TiempoEmpleado = tiempoEmpleado;
                existingDay.Observaciones = observaciones;

                // Realizar la actualización en la base de datos
                await firebaseClient.Child("Days").Child($"{existingDay.Fecha:yyyyMMdd}_{existingDay.UserName}").PutAsync(existingDay);

                await this.DisplayAlert("Confirmacion", "Se ha actualizado el dia.", "Vale");
            }
            else
            {
                // Si el día no existe, agregar como nuevo
                await firebaseClient.Child("Days").PostAsync(new Dia
                {
                    Fecha = fecha,
                    ActividadDesarrollada = actividadDesarrollada,
                    TiempoEmpleado = tiempoEmpleado,
                    Observaciones = observaciones,
                    UserName = currentUsername,
                });

                await this.DisplayAlert("Confirmacion", "Se ha agregado el dia.", "Vale");

                AgregarEditarBotton.Text = "Editar";
            }
        }
        else
        {
            await this.DisplayAlert("Error", "Los campos no pueden estar en blanco.", "Vale");
        }
    }



}