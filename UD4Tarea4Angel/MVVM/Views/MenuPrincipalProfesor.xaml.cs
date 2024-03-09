namespace UD4Tarea4Angel.MVVM.Views;

public partial class MenuPrincipalProfesor : ContentPage
{
    string currentUsername;
    public MenuPrincipalProfesor(string username)
    {
        currentUsername = username;
        InitializeComponent();

        labelCabecera.Text = $"¡Bienvenido {currentUsername}!";

    }

    private async void OnAlumnosClicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new SelectorAlumnosView(currentUsername));
    }


    private async void OnGestionarClicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new EditarInfoAlumnoView(currentUsername));
    }
}