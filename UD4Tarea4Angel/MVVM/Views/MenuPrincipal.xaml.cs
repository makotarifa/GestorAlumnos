namespace UD4Tarea4Angel.MVVM.Views;

public partial class MenuPrincipal : ContentPage
{
	string currentUsername;
	public MenuPrincipal(string username)
	{
		currentUsername = username;
        InitializeComponent();

        labelCabecera.Text = $"¡Bienvenido {currentUsername}!";

    }

    private async void OnDiasClicked(object sender, EventArgs e)
	{

        await Navigation.PushAsync(new SelectorDiasView(currentUsername));
    }


    private async void OnGestionarClicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new EditarInfoAlumnoView(currentUsername));
    }




}