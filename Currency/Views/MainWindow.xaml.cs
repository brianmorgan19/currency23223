using Currency.Models;
using Currency.Service;
using Currency.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Currency
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnAddRates_Click(object sender, RoutedEventArgs e)
        {
            var httpClient = new HttpClient();

            try
            {
                string url = "https://open.er-api.com/v6/latest/RUB";
                HttpResponseMessage responseMessage = await httpClient.GetAsync(url);

                if (responseMessage.IsSuccessStatusCode)
                {
                    string json = await responseMessage.Content.ReadAsStringAsync();

                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var result = System.Text.Json.JsonSerializer.Deserialize<ExchangeRateResponse>(json, options);

                    if (result != null && result.Rates != null)
                    {
                        double usd = Math.Round(1 / result.Rates["USD"], 2);
                        double eur = Math.Round(1 / result.Rates["EUR"], 2);
                        double cny = Math.Round(1 / result.Rates["CNY"], 2);

                        string time = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
                        string line = $"{time} | 1 USD = {usd} RUB | 1 EUR = {eur} RUB | 1 CNY = {cny} RUB";

                        // Сохраняем в файл
                        File.AppendAllLines("currency_rates.txt", new[] { line });

                        MessageBox.Show("Курсы валют успешно добавлены:\n" + line, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обработке ответа от сервера.");
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось получить данные с сервера.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            finally
            {
                httpClient.Dispose();
            }
        }





        private void btnViewRates_Click(object sender, RoutedEventArgs e)
        {
            var viewer = new CurrencyViewerWindow();
            viewer.Show();
        }

    }
}
