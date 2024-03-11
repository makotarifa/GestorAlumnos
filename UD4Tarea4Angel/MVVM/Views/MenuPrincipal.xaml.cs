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

    /// <summary>
    /// Se abre el gestor de dias de los alumnos para agregar actividades. (Digo agregar porque tengo el teclado en ingles y no puedo poner la tilde :( 
    /// </summary>
    private async void OnDiasClicked(object sender, EventArgs e)
	{

        await Navigation.PushAsync(new SelectorDiasView(currentUsername));
    }

    /// <summary>
    /// Se abre la View para editar la información personal del alumno.
    /// </summary>
    private async void OnGestionarClicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new EditarInfoAlumnoView(currentUsername));
    }




}