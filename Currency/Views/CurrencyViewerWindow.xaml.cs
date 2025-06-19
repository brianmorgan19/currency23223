using Currency.Models;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using LiveCharts.Defaults;
using LiveCharts.Configurations;

namespace Currency.Views
{
    public partial class CurrencyViewerWindow : Window
    {
        public CurrencyViewerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("currency_rates.txt"))
            {
                MessageBox.Show("Файл с курсами не найден.");
                return;
            }

                    var data = File.ReadAllLines("currency_rates.txt")
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                try
                {
                    if (line.Contains(';')) // Старый формат
                    {
                        var parts = line.Split(';');

                        return new CurrencyRate
                        {
                            Date = DateTime.ParseExact(parts[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                            USD = double.Parse(parts[1].Replace(',', '.'), CultureInfo.InvariantCulture),
                            EUR = double.Parse(parts[2].Replace(',', '.'), CultureInfo.InvariantCulture),
                            CNY = double.Parse(parts[3].Replace(',', '.'), CultureInfo.InvariantCulture)
                        };
                    }
                    else if (line.Contains('|')) // Новый формат
                    {
                        var parts = line.Split('|');

                        DateTime date = DateTime.ParseExact(parts[0].Trim(), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                        double usd = double.Parse(parts[1].Split('=')[1].Replace("RUB", "").Trim().Replace(',', '.'), CultureInfo.InvariantCulture);
                        double eur = double.Parse(parts[2].Split('=')[1].Replace("RUB", "").Trim().Replace(',', '.'), CultureInfo.InvariantCulture);
                        double cny = double.Parse(parts[3].Split('=')[1].Replace("RUB", "").Trim().Replace(',', '.'), CultureInfo.InvariantCulture);

                        return new CurrencyRate
                        {
                            Date = date,
                            USD = usd,
                            EUR = eur,
                            CNY = cny
                        };
                    }

                    return null; // неизвестный формат
                }
                catch
                {
                    return null; // ошибка парсинга
                }
            })
            .Where(r => r != null)
            .OrderBy(r => r.Date)
            .ToList();

            if (data.Count == 0)
            {
                MessageBox.Show("Файл пуст.");
                return;
            }

            CurrencyDataGrid.ItemsSource = data;

            CurrencyChart.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "USD",
                    Values = new ChartValues<double>(data.Select(r => r.USD)),
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 6,
                    LineSmoothness = 0.3
                },
                new LineSeries
                {
                    Title = "EUR",
                    Values = new ChartValues<double>(data.Select(r => r.EUR)),
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 6,
                    LineSmoothness = 0.3
                },
                new LineSeries
                {
                    Title = "CNY",
                    Values = new ChartValues<double>(data.Select(r => r.CNY)),
                    PointGeometry = DefaultGeometries.Triangle,
                    PointGeometrySize = 6,
                    LineSmoothness = 0.3
                }
            };

            CurrencyChart.AxisX.Clear();
            CurrencyChart.AxisX.Add(new Axis
            {
                Title = "Дата/время",
                Labels = data.Select(r => r.Date.ToString("dd.MM HH:mm")).ToList(),
                LabelsRotation = 20,
                Separator = new Separator { Step = 1 }
            });

            CurrencyChart.AxisY.Clear();
            CurrencyChart.AxisY.Add(new Axis
            {
                Title = "Курс (₽)",
                LabelFormatter = value => value.ToString("F4")
            });
        }
    }
}
