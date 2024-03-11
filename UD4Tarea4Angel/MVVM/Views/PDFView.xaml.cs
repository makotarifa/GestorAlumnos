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

public partial class PDFView : ContentPage
{
    String actualAlumnoUsername;
    public PDFView(string alumnoUsername)
    {
        InitializeComponent();
        actualAlumnoUsername = alumnoUsername;
        ControlPDF();
        GenerarPDF(actualAlumnoUsername);
    }

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

    private async void GenerarPDF(string alumnoUsername )
    {
        Persona personaActual = await GetPersona(alumnoUsername);
        Persona profesor = await GetProfesor(personaActual.ProfesorTutor);
        List<List<Dia>> diasAgrupadosPorSemana = await GroupDaysOnTheSameWeek(alumnoUsername);
        List<Actividad> actividadesPersonaActual;

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
            Table tablaSuperior;
            Table tablaInferior;

            iText.Layout.Element.Cell celdaActividad, celdaTiempo, celdaObservaciones;

            //Cambiar la orientacion del documento
            pdf.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A4.Rotate());

            //Bucle foreach por cada semana en diasAgrupadosPorSemana
            foreach (var semana in diasAgrupadosPorSemana)
            {
                document.Add(new Paragraph("JUNTA DE ANDALUCIA                                                     CONSEJERIA DE EDUCACION")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(13));

                document.Add(new Paragraph("FORMACION EN CENTROS DE TRABAJO. FICHA SEMANAL DEL ALUMNO/ALUMNA")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(12));

                //Se genera una tabla de 2 columnas y 3 filas.
                tablaSuperior = new Table(new float[] { 1, 2 });
                tablaSuperior.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                    .Add(new Paragraph($"Semana del {semana.LastOrDefault().Fecha.Day} de {semana.FirstOrDefault().Fecha.Month} al {semana.LastOrDefault().Fecha.Day} del {semana.LastOrDefault().Fecha.Month} del {semana.LastOrDefault().Fecha.Year}"))
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                    .SetFontSize(11));
                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                .Add(new Paragraph(""))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(11));

                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                .Add(new Paragraph($"Centro docente: {personaActual.CentroEstudio} \n" +
                $"Profesor/a responsable seguimiento: {profesor.Nombre} {profesor.Apellidos}"))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(11));

                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                .Add(new Paragraph($"Centro de trabajo colaborador:  {personaActual.CentroTrabajo} \n" +
                $"Tutor/a centro de trabajo: {personaActual.TutorLaboral}"))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(11));

                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                .Add(new Paragraph($"Alumno/a: {personaActual.Nombre} {personaActual.Apellidos}"))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(11));

                tablaSuperior.AddCell(new iText.Layout.Element.Cell(1, 1)
                .Add(new Paragraph($"Ciclo formativo: {personaActual.NombreGrado} Grado: {personaActual.TipoGrado}"))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                .SetFontSize(11));

                document.Add(tablaSuperior);

                //Agregar espaciado entre tabla superior e inferior

                document.Add(new Paragraph("\n"));



                tablaInferior = new Table(new float[] { 1, 1, 1, 1 });
                tablaInferior.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                tablaInferior.AddCell(new iText.Layout.Element.Cell(1, 1).Add(new Paragraph("DIA")).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(11));
                tablaInferior.AddCell(new iText.Layout.Element.Cell(1, 1).Add(new Paragraph("ACTIVIDAD DESARROLLADA/PUESTO INFORMATIVO")).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(11));
                tablaInferior.AddCell(new iText.Layout.Element.Cell(1, 1).Add(new Paragraph("TIEMPO EMPLEADO")).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(11));
                tablaInferior.AddCell(new iText.Layout.Element.Cell(1, 1).Add(new Paragraph("OBSERVACIONES")).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(11));


                foreach (var dia in semana)
                {
                    actividadesPersonaActual = await GetAllActivitiesFromDay(dia.Key);

                    celdaActividad = new iText.Layout.Element.Cell(1, 1);
                    celdaTiempo = new iText.Layout.Element.Cell(1, 1);
                    celdaObservaciones = new iText.Layout.Element.Cell(1, 1);

                    tablaInferior.AddCell(new iText.Layout.Element.Cell(1, 1).Add(new Paragraph(dia.Fecha.Date.ToShortDateString())).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT).SetFontSize(11));
                    foreach (var actividad in actividadesPersonaActual)
                    {
                        celdaActividad.Add(new Paragraph(actividad.ActividadDesarrollada)).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT).SetFontSize(11);
                        celdaTiempo.Add(new Paragraph(actividad.TiempoEmpleado.ToString())).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(11);
                        celdaObservaciones.Add(new Paragraph(actividad.Observaciones)).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT).SetFontSize(11);

                        if (actividadesPersonaActual.Count > 1)
                        {
                            celdaActividad.Add(new LineSeparator(new SolidLine()));
                            celdaTiempo.Add(new LineSeparator(new SolidLine()));
                            celdaObservaciones.Add(new LineSeparator(new SolidLine()));
                        }
                    }

                    tablaInferior.AddCell(celdaActividad);
                    tablaInferior.AddCell(celdaTiempo);
                    tablaInferior.AddCell(celdaObservaciones);
                }

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

    private async Task<Persona> GetProfesor(string userName)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var datosProfesores = await FirebaseConnection.firebaseClient.Child("DatosProfesor").OnceAsync<Persona>();

        // Devuelve si existe algun objeto
        var profesor = datosProfesores.FirstOrDefault(u => u.Object.UserName == userName);

        return profesor.Object;

    }

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

    private async Task<Persona> GetPersona(string userName)
    {
        // Realizar una consulta para verificar si el usuario ya existe
        var datosPersonas = await FirebaseConnection.firebaseClient.Child("DatosPersona").OnceAsync<Persona>();

        // Devuelve si existe algun objeto
        var persona = datosPersonas.FirstOrDefault(u => u.Object.UserName == userName);

        return persona.Object;

    }

    private async Task<List<List<Dia>>> GroupDaysOnTheSameWeek(string userName)
    {
        var dias = await FirebaseConnection.firebaseClient
            .Child("Days")
            .OnceAsync<Dia>();

        // Filtrar los días que contengan el usuario de la persona actual
        List<Dia> diasLista = dias
            .Where(diaSnapshot => diaSnapshot.Object.UserName == userName).OrderBy(diaSnapshot => diaSnapshot.Object.Fecha)
            .Select(diaSnapshot => diaSnapshot.Object).ToList();

        // Agrupar los días por semana, teniendo en cuenta el día de la semana
        var diasAgrupados = diasLista.GroupBy(d =>
        {
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
