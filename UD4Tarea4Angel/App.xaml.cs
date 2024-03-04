using UD4Tarea4Angel.MVVM.Views;

namespace UD4Tarea4Angel
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new InicioSesionView());
        }
    }
}
