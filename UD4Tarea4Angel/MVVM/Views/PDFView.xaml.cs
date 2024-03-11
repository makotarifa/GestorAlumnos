using iText.IO.Image;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using System.Net;
using iText.Layout;
using UD4Tarea4Angel.Models;
using UD4Tarea4Angel.MVVM.Models;
using Firebase.Database;
using System.Collections.ObjectModel;
using System.Globalization;
using UD4Tarea4Angel.Utilities;

namespace UD4Tarea4Angel.MVVM.Views;

/// <summary>
/// Clase para visualizar un PDF con los datos de un alumno que se generara a partir de los datos de Firebase.
/// </summary>
public partial class PDFView : ContentPage
{
    String actualAlumnoUsername;
    public PDFView(string alumnoUsername)
    {
        InitializeComponent();
        actualAlumnoUsername = alumnoUsername;
        // Se encarga de inicializar la vista con el PDF generado, de forma correcta segun la plataforma.
        ControlPDF();
        // Se encarga de generar el PDF con los datos del alumno.
        GenerarPDF(actualAlumnoUsername);
    }

    /// <summary>
    /// Metodo para controlar el PDF segun la plataforma.
    /// </summary>
    private void ControlPDF()
    {
#if ANDROID
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("pdfviewer", (handler, View) =>
        {
            handler.PlatformView.Settings.AllowFileAccess = true;
            handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
            handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
        });

        pdfview.Source = $"file:///android_asset/pdfjs/web/viewer.html?file=file:///android_asset/{WebUtility.UrlEncode("mypdf.pdf")}";
#else
        pdfview.Source = "mypdf.pdf";
#endif
    }

    /// <summary>
    /// Metodo para generar el PDF con los datos del alumno. Se encarga de obtener los datos de Firebase y generar el PDF con iText.
    /// </summary>
    /// <param name="alumnoUsername"></param>
    private async void GenerarPDF(string alumnoUsername )
    {
        // Se obtiene la persona actual.
        Persona personaActual = await GetPersona(alumnoUsername);
        // Se obtiene el profesor tutor de la persona actual.
        Persona profesor = await GetProfesor(personaActual.ProfesorTutor);
        // Se agrupan los d�as por semana con el m�todo GroupDaysOnTheSameWeek.
        List<List<Dia>> diasAgrupadosPorSemana = await GroupDaysOnTheSameWeek(alumnoUsername);
        // Se obtienen las actividades de la persona actual.
        List<Actividad> actividadesPersonaActual;

        // Se genera el PDF con iText, que tendra este nombre.
        string fileName = "mauidotnet.pdf";

#if ANDROID
		var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
		var filePath = Path.Combine(docsDirectory.AbsoluteFile.Path, fileName);
#else
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
#endif
        using (PdfWriter writer = new PdfWriter(filePath))
        {
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            //Se crean las tablas y celdas necesarias para el PDF.
            Table tablaSuperior;
            Table tablaInferior;
            iText.Layout.Element.Cell celdaActividad, celdaTiempo, celdaObservaciones;

            //Cambia la orientacion del documento
            pdf.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A4.Rotate());

            //Bucle foreach por cada semana en diasAgrupadosPorSemana
            foreach (var semana in diasAgrupadosPorSemana)
            {
                //Textos superiores
                document.Add(new Paragraph("JUNTA DE ANDALUCIA                                                     CONSEJERIA DE EDUCACION")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(13));

                document.Add(new Paragraph("FORMACION EN CENTROS DE TRABAJO. FICHA SEMANAL DEL ALUMNO/ALUMNA")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(12));

                //Se genera una tabla de 2 columnas y 3 filas.
                tablaSuperior = new Table(new float[] { 1, 2 });
                //Que ocupe el 100% del ancho
                tablaSuperior.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                //se agrega la celda de la fecha.
                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                    .Add(new Paragraph($"Semana del {semana.FirstOrDefault().Fecha.Day} del {semana.FirstOrDefault().Fecha.Month} al {semana.LastOrDefault().Fecha.Day} del {semana.LastOrDefault().Fecha.Month} del {semana.LastOrDefault().Fecha.Year}"))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .SetFontSize(11));

                //Espacio en blanco
                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                .Add(new Paragraph(""))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(11));

                //Se agrega la celdas del centro educativo y del profesor.
                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                .Add(new Paragraph($"Centro docente: {personaActual.CentroEstudio} \n" +
                $"Profesor/a responsable seguimiento: {profesor.Nombre} {profesor.Apellidos}"))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(11));

                //Se agrega la celda del centro de trabajo y del alumno.
                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                .Add(new Paragraph($"Centro de trabajo colaborador:  {personaActual.CentroTrabajo} \n" +
                $"Tutor/a centro de trabajo: {personaActual.TutorLaboral}"))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(11));

                //Se agrega la celda del nombre del alumno.
                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                .Add(new Paragraph($"Alumno/a: {personaActual.Nombre} {personaActual.Apellidos}"))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(11));

                //Se agrega la celda del ciclo formativo y del grado.
                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                .Add(new Paragraph($"Ciclo formativo: {personaActual.NombreGrado} Grado: {personaActual.TipoGrado}"))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(11));

                //Se agrega la tabla superior al documento.
                document.Add(tablaSuperior);

                //Agregar espaciado entre tabla superior e inferior

                document.Add(new Paragraph("\n"));

                //Tabla inferior con los datos de las actividades de la semana.

                tablaInferior = new Table(new float[] { 1, 1, 1, 1 });
                tablaInferior.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                //Cabecear de la tabla inferior
                tablaInferior.AddCell(new iText.Layout.Element.Cell(1, 1).Add(new Paragraph("DIA")).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(11));
                tablaInferior.AddCell(new iText.Layout.Element.Cell(1, 1).Add(new Paragraph("ACTIVIDAD DESARROLLADA/PUESTO INFORMATIVO")).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(11));
                tablaInferior.AddCell(new iText.Layout.Element.Cell(1, 1).Add(new Paragraph("TIEMPO EMPLEADO")).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(11));
                tablaInferior.AddCell(new iText.Layout.Element.Cell(1, 1).Add(new Paragraph("OBSERVACIONES")).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(11));

                //Bucle foreach por cada dia en semana
                foreach (var dia in semana)
                {
                    //Se obtienen las actividades de la persona actual.
                    actividadesPersonaActual = await GetAllActivitiesFromDay(dia.Key);

                    //Celdas de la tabla inferior que se rellenaran con los datos de las actividades.
                    celdaActividad = new iText.Layout.Element.Cell(1, 1);
                    celdaTiempo = new iText.Layout.Element.Cell(1, 1);
                    celdaObservaciones = new iText.Layout.Element.Cell(1, 1);

                    //Se agrega la celda de la fecha.
                    tablaInferior.AddCell(new iText.Layout.Element.Cell(1, 1).Add(new Paragraph(dia.Fecha.Date.ToShortDateString())).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT).SetFontSize(11));
                    
                    //Bucle foreach por cada actividad
                    foreach (var actividad in actividadesPersonaActual)
                    {
                        //Se rellenan las celdas con los datos de las actividades.
                        celdaActividad.Add(new Paragraph(actividad.ActividadDesarrollada)).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT).SetFontSize(11);
                        celdaTiempo.Add(new Paragraph(actividad.TiempoEmpleado.ToString())).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(11);
                        celdaObservaciones.Add(new Paragraph(actividad.Observaciones)).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT).SetFontSize(11);

                        //Si hay mas de una actividad, se a�ade una linea separadora.
                        if (actividadesPersonaActual.Count > 1)
                        {
                            celdaActividad.Add(new LineSeparator(new SolidLine()));
                            celdaTiempo.Add(new LineSeparator(new SolidLine()));
                            celdaObservaciones.Add(new LineSeparator(new SolidLine()));
                        }
                    }

                    //Se a�aden las celdas a la tabla inferior.
                    tablaInferior.AddCell(celdaActividad);
                    tablaInferior.AddCell(celdaTiempo);
                    tablaInferior.AddCell(celdaObservaciones);
                }
                //Se a�ade la tabla inferior al documento.
                document.Add(tablaInferior);
                //Crear pagina nueva
                document.Add(new AreaBreak());
            }

            document.Close();
        }

#if ANDROID
        pdfview.Source = $"file:///android_asset/pdfjs/web/viewer.html?file=file://{WebUtility.UrlEncode(filePath)}";
#else
        pdfview.Source = filePath;
#endif
    }

    private string formatAllActivitiesToString(List<Actividad> actividadesPersonaActual)
    {
        string actividades = "";
        foreach (var actividad in actividadesPersonaActual)
        {
            actividades += $"Actividad: {actividad.ActividadDesarrollada} + \n" +
                $"Observaciones: {actividad.Observaciones} + \n" +
                $"--------------- \n";
        }
        return actividades;
    }


    private async Task<byte[]> ConvertImageSourceToStreamAsync(string imageName)
    {
        using var ms = new MemoryStream();
        using ( var stream = await FileSystem.OpenAppPackageFileAsync(imageName))
            await stream.CopyToAsync(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Se obtiene el profesor tutor de la persona actual mediante su nombre de usuario.
    /// </summary>
    private async Task<Persona> GetProfesor(string userName)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var datosProfesores = await FirebaseConnection.firebaseClient.Child("DatosProfesor").OnceAsync<Persona>();

        // Devuelve si existe algun objeto
        var profesor = datosProfesores.FirstOrDefault(u => u.Object.UserName == userName);

        return profesor.Object;

    }

    /// <summary>
    /// Se obtiene la persona actual mediante su nombre de usuario.
    /// </summary>
    private async Task<List<Actividad>> GetAllActivitiesFromDay(string dayKey)
    {
        var activities = await FirebaseConnection.firebaseClient
            .Child("Actividades")
            .OnceAsync<Actividad>();

        // Filtrar las actividades por DiaKey
        List<Actividad> actividadesLista = activities
            .Where(activitySnapshot => activitySnapshot.Object.DiaKey == dayKey)
            .Select(activitySnapshot => activitySnapshot.Object)
            .ToList();

        return actividadesLista;
    }

    /// <summary>
    /// Se obtiene la persona actual mediante su nombre de usuario.
    /// </summary>
    private async Task<Persona> GetPersona(string userName)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var datosPersonas = await FirebaseConnection.firebaseClient.Child("DatosPersona").OnceAsync<Persona>();

        // Devuelve si existe algun objeto
        var persona = datosPersonas.FirstOrDefault(u => u.Object.UserName == userName);

        return persona.Object;

    }
    /// <summary>
    /// Se agrupan los d�as que esten en la misma semana.
    /// </summary>
    private async Task<List<List<Dia>>> GroupDaysOnTheSameWeek(string userName)
    {
        var dias = await FirebaseConnection.firebaseClient
            .Child("Days")
            .OnceAsync<Dia>();

        // Filtrar los d�as que contengan el usuario de la persona actual
        List<Dia> diasLista = dias
            .Where(diaSnapshot => diaSnapshot.Object.UserName == userName).OrderBy(diaSnapshot => diaSnapshot.Object.Fecha)
            .Select(diaSnapshot => diaSnapshot.Object).ToList();

        // Agrupar los d�as por semana, teniendo en cuenta el d�a de la semana
        var diasAgrupados = diasLista.GroupBy(d =>
        {
            //Esto es para que la semana empiece en lunes, ya no se ni de donde lo saque
            var weekOfYear = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(d.Fecha, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            if (d.Fecha.DayOfWeek == DayOfWeek.Monday)
            {
                weekOfYear--;
            }
            return weekOfYear;
        });

        // Ordenar los grupos por fecha
        var diasAgrupadosOrdenados = diasAgrupados.OrderBy(g => g.First().Fecha);

        List<List<Dia>> diasAgrupadosPorSemana = new List<List<Dia>>();

        foreach (var grupo in diasAgrupadosOrdenados)
        {
            diasAgrupadosPorSemana.Add(new List<Dia>(grupo));
        }

        return diasAgrupadosPorSemana;
    }


}
