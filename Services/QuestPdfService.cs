using System;
using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public class QuestPdfService : IQuestPdfService
    {
        // Paleta Institucional IPN
        private static readonly Color ColorGuindaIpn = Color.FromHex("#750946");
        private static readonly Color ColorGuindaOscuro = Color.FromHex("#4d0630");
        private static readonly Color ColorGrisFondo = Color.FromHex("#f8f9fa");
        private static readonly Color ColorBorde = Color.FromHex("#dee2e6");
        private static readonly Color ColorTextoSecundario = Color.FromHex("#495057");

        public byte[] GenerarReporteMetricasPdf(ReporteMetricasViewModel datos)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                    page.Header().Element(c => ComponerEncabezado(c, datos));
                    page.Content().Element(c => ComponerContenido(c, datos));
                    page.Footer().Element(c => ComponerPiePagina(c, datos));
                });
            });

            return document.GeneratePdf();
        }

        private void ComponerEncabezado(IContainer container, ReporteMetricasViewModel datos)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("INSTITUTO POLITÉCNICO NACIONAL")
                            .FontSize(14)
                            .Bold()
                            .FontColor(ColorGuindaIpn);

                        c.Item().Text("Unidad Profesional Interdisciplinaria de Ingeniería y Ciencias Sociales y Administrativas (UPIICSA)")
                            .FontSize(9)
                            .Bold()
                            .FontColor(ColorGuindaOscuro);

                        c.Item().Text("SISTEMA INSTITUCIONAL DE TUTORÍAS ACADÉMICAS")
                            .FontSize(8)
                            .FontColor(ColorTextoSecundario);
                    });

                    row.ConstantItem(160).Column(c =>
                    {
                        c.Item().AlignRight().Text("REPORTE OFICIAL")
                            .FontSize(11)
                            .Bold()
                            .FontColor(ColorGuindaIpn);

                        c.Item().AlignRight().Text($"Emisión: {datos.FechaGeneracion:dd/MM/yyyy HH:mm}")
                            .FontSize(8)
                            .FontColor(ColorTextoSecundario);

                        c.Item().AlignRight().Text($"Periodo: {datos.PeriodoTexto}")
                            .FontSize(8)
                            .Bold();
                    });
                });

                col.Item().PaddingTop(8).PaddingBottom(6).LineHorizontal(1.5f).LineColor(ColorGuindaIpn);
            });
        }

        private void ComponerContenido(IContainer container, ReporteMetricasViewModel datos)
        {
            container.PaddingTop(6).Column(col =>
            {
                // Tarjetas de Indicadores Principales (KPIs)
                col.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => TarjetaKpi(c, "Asistencias Confirmadas", datos.TotalAsistenciasConfirmadas.ToString(), "Total presenciales validadas"));
                    row.ConstantItem(10);
                    row.RelativeItem().Element(c => TarjetaKpi(c, "Alumnos Atendidos", datos.TotalAlumnosBeneficiados.ToString(), "Estudiantes únicos"));
                    row.ConstantItem(10);
                    row.RelativeItem().Element(c => TarjetaKpi(c, "Satisfacción General", $"{datos.PromedioSatisfaccion:0.0} / 5.0", $"{datos.TotalEvaluaciones} encuestas recibidas"));
                    row.ConstantItem(10);
                    row.RelativeItem().Element(c => TarjetaKpi(c, "Tutores Participantes", datos.TotalTutoresActivos.ToString(), "Docentes y alumnos tutores"));
                });

                col.Item().PaddingTop(14);

                // Sección Comparativa: Modalidad y Método
                col.Item().Row(row =>
                {
                    // Desglose por Tipo de Tutor (RF-29)
                    row.RelativeItem().Border(1).BorderColor(ColorBorde).Column(c =>
                    {
                        c.Item().Background(ColorGuindaOscuro).Padding(6).Text("ÍNDICE DE ATENCIÓN POR MODALIDAD")
                            .FontSize(9).Bold().FontColor(Colors.White);

                        c.Item().Padding(8).Column(inner =>
                        {
                            inner.Item().Row(r =>
                            {
                                r.RelativeItem().Text("Docente Tutor (Individual):").Bold();
                                r.ConstantItem(60).AlignRight().Text($"{datos.AsistenciasDocenteTutor} ({datos.PorcentajeDocenteTutor}%)");
                            });

                            inner.Item().PaddingTop(4).Row(r =>
                            {
                                r.RelativeItem().Text("Alumno Tutor (Entre Pares):").Bold();
                                r.ConstantItem(60).AlignRight().Text($"{datos.AsistenciasAlumnoTutor} ({datos.PorcentajeAlumnoTutor}%)");
                            });
                        });
                    });

                    row.ConstantItem(12);

                    // Desglose por Método de Registro
                    row.RelativeItem().Border(1).BorderColor(ColorBorde).Column(c =>
                    {
                        c.Item().Background(ColorGuindaOscuro).Padding(6).Text("MÉTODO DE ASISTENCIA REGISTRADO")
                            .FontSize(9).Bold().FontColor(Colors.White);

                        c.Item().Padding(8).Column(inner =>
                        {
                            inner.Item().Row(r =>
                            {
                                r.RelativeItem().Text("Código QR Dinámico:").Bold();
                                r.ConstantItem(60).AlignRight().Text($"{datos.AsistenciasQr} ({datos.PorcentajeQr}%)");
                            });

                            inner.Item().PaddingTop(4).Row(r =>
                            {
                                r.RelativeItem().Text("Registro Manual:").Bold();
                                r.ConstantItem(60).AlignRight().Text($"{datos.AsistenciasManual} ({datos.PorcentajeManual}%)");
                            });
                        });
                    });
                });

                col.Item().PaddingTop(14);

                // Ranking de Materias con Mayor Demanda (RF-29)
                col.Item().Text("RANKING DE UNIDADES DE APRENDIZAJE CON MAYOR DEMANDA")
                    .FontSize(10).Bold().FontColor(ColorGuindaIpn);

                col.Item().PaddingTop(4).Table(tabla =>
                {
                    tabla.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(25);  // #
                        cols.ConstantColumn(55);  // Clave
                        cols.RelativeColumn(3);   // Materia
                        cols.RelativeColumn(2);   // Carrera
                        cols.ConstantColumn(65);  // Asistencias
                        cols.ConstantColumn(55);  // Alumnos
                        cols.ConstantColumn(50);  // % Total
                    });

                    // Encabezado
                    tabla.Header(h =>
                    {
                        h.Cell().Background(ColorGuindaIpn).Padding(4).AlignCenter().Text("#").Bold().FontColor(Colors.White).FontSize(8);
                        h.Cell().Background(ColorGuindaIpn).Padding(4).Text("Clave").Bold().FontColor(Colors.White).FontSize(8);
                        h.Cell().Background(ColorGuindaIpn).Padding(4).Text("Materia").Bold().FontColor(Colors.White).FontSize(8);
                        h.Cell().Background(ColorGuindaIpn).Padding(4).Text("Carrera").Bold().FontColor(Colors.White).FontSize(8);
                        h.Cell().Background(ColorGuindaIpn).Padding(4).AlignCenter().Text("Asistencias").Bold().FontColor(Colors.White).FontSize(8);
                        h.Cell().Background(ColorGuindaIpn).Padding(4).AlignCenter().Text("Alumnos").Bold().FontColor(Colors.White).FontSize(8);
                        h.Cell().Background(ColorGuindaIpn).Padding(4).AlignCenter().Text("% Total").Bold().FontColor(Colors.White).FontSize(8);
                    });

                    if (datos.RankingMaterias.Count == 0)
                    {
                        tabla.Cell().ColumnSpan(7).Padding(8).AlignCenter().Text("No se encontraron asistencias en el periodo seleccionado.")
                            .FontSize(8).Italic().FontColor(ColorTextoSecundario);
                    }
                    else
                    {
                        for (int i = 0; i < datos.RankingMaterias.Count; i++)
                        {
                            var item = datos.RankingMaterias[i];
                            var bg = (i % 2 == 0) ? Colors.White : ColorGrisFondo;

                            tabla.Cell().Background(bg).Padding(4).AlignCenter().Text((i + 1).ToString()).FontSize(8);
                            tabla.Cell().Background(bg).Padding(4).Text(item.MateriaClave).FontSize(8);
                            tabla.Cell().Background(bg).Padding(4).Text(item.MateriaNombre).FontSize(8).Bold();
                            tabla.Cell().Background(bg).Padding(4).Text(item.CarreraNombre).FontSize(8);
                            tabla.Cell().Background(bg).Padding(4).AlignCenter().Text(item.TotalAsistencias.ToString()).FontSize(8).Bold();
                            tabla.Cell().Background(bg).Padding(4).AlignCenter().Text(item.TotalAlumnosDistintos.ToString()).FontSize(8);
                            tabla.Cell().Background(bg).Padding(4).AlignCenter().Text($"{item.PorcentajeTotal}%").FontSize(8);
                        }
                    }
                });

                // Distribución por Programa Académico
                if (datos.DistribucionCarreras.Count > 0)
                {
                    col.Item().PaddingTop(14);
                    col.Item().Text("DISTRIBUCIÓN DE ASISTENCIAS POR PROGRAMA ACADÉMICO")
                        .FontSize(10).Bold().FontColor(ColorGuindaIpn);

                    col.Item().PaddingTop(4).Table(tabla =>
                    {
                        tabla.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(4);
                            cols.ConstantColumn(80);
                            cols.ConstantColumn(70);
                        });

                        tabla.Header(h =>
                        {
                            h.Cell().Background(ColorGuindaIpn).Padding(4).Text("Programa Académico").Bold().FontColor(Colors.White).FontSize(8);
                            h.Cell().Background(ColorGuindaIpn).Padding(4).AlignCenter().Text("Asistencias").Bold().FontColor(Colors.White).FontSize(8);
                            h.Cell().Background(ColorGuindaIpn).Padding(4).AlignCenter().Text("Proporción").Bold().FontColor(Colors.White).FontSize(8);
                        });

                        for (int i = 0; i < datos.DistribucionCarreras.Count; i++)
                        {
                            var item = datos.DistribucionCarreras[i];
                            var bg = (i % 2 == 0) ? Colors.White : ColorGrisFondo;

                            tabla.Cell().Background(bg).Padding(4).Text(item.CarreraNombre).FontSize(8);
                            tabla.Cell().Background(bg).Padding(4).AlignCenter().Text(item.TotalAsistencias.ToString()).FontSize(8).Bold();
                            tabla.Cell().Background(bg).Padding(4).AlignCenter().Text($"{item.Porcentaje}%").FontSize(8);
                        }
                    });
                }
            });
        }

        private void TarjetaKpi(IContainer container, string titulo, string valor, string subtitulo)
        {
            container
                .Border(1)
                .BorderColor(ColorBorde)
                .Background(ColorGrisFondo)
                .Padding(6)
                .Column(c =>
                {
                    c.Item().Text(titulo).FontSize(8).Bold().FontColor(ColorTextoSecundario);
                    c.Item().PaddingTop(2).Text(valor).FontSize(13).Bold().FontColor(ColorGuindaIpn);
                    c.Item().PaddingTop(2).Text(subtitulo).FontSize(7).FontColor(ColorTextoSecundario);
                });
        }

        private void ComponerPiePagina(IContainer container, ReporteMetricasViewModel datos)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor(ColorBorde);
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text("Sistema Institucional de Tutorías Académicas UPIICSA - IPN")
                        .FontSize(7)
                        .FontColor(ColorTextoSecundario);

                    row.ConstantItem(120).AlignRight().Text(text =>
                    {
                        text.Span("Página ").FontSize(7).FontColor(ColorTextoSecundario);
                        text.CurrentPageNumber().FontSize(7).Bold().FontColor(ColorGuindaIpn);
                        text.Span(" de ").FontSize(7).FontColor(ColorTextoSecundario);
                        text.TotalPages().FontSize(7).Bold().FontColor(ColorGuindaIpn);
                    });
                });
            });
        }
    }
}
