namespace UD4Tarea4Angel.MVVM.Views;

/// <summary>
/// Clase que representa la vista del menú principal del profesor.
/// </summary>
public partial class MenuPrincipalProfesor : ContentPage
{
    string currentUsername;
    public MenuPrincipalProfesor(string username)
    {
        currentUsername = username;
        InitializeComponent();
        //Cabecera de la página
        labelCabecera.Text = $"¡Bienvenido {currentUsername}!";

    }

    /// <summary>
    /// Al pulsar el botón de "Ver días de los alumnos", se abre la vista de selección de alumnos.
    /// </summary>
    private async void OnAlumnosClicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new SelectorAlumnosView(currentUsername));
    }

    /// <summary>
    /// Al pulsar el botón de "Gestionar información", se abre la vista de edición de los datos persoales del profesor.
    /// </summary>
    private async void OnGestionarClicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new EditarInfoProfesorView(currentUsername));
    }
}