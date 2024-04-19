using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Charts.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApiWeather
{
    public partial class Form1 : Form
    {
        List<DateTime> time;
        List<double> temp;
        List<int> humidity;
        public Form1()
        {
            InitializeComponent();
            double[] list1 = { 3.4, 5.6, 7.8, 8.9, 10.0 };
            double[] list2 = { 20, 40, 50, 60, 80, 100 };
            double max = Math.Floor(list1.Union(list2).Max()); 
            double min = Math.Ceiling(list1.Union(list2).Min());
            cartesianChart1.Series = new LiveCharts.SeriesCollection
            {
                new LineSeries
                {
                    Title="Температура",Values = new ChartValues<double>(list1)
                }                
            };
            cartesianChart1.AxisX.Add(new Axis
            {
                
                Labels = new[] { "1", "2", "3", "4", "5" }
            });
            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Температура",
                LabelFormatter = var => $"{var} C"
            });
            cartesianChart2.Series = new LiveCharts.SeriesCollection
            {                
                new LineSeries
                {
                    Title="Вологість",Values = new ChartValues<double>(list2),
                    //PointGeometry=null
                }
            };
            cartesianChart2.AxisX.Add(new Axis
            {
               
                Labels = new[] { "1", "2", "3", "4", "5" }
            });
            cartesianChart2.AxisY.Add(new Axis
            {
                Title = "Вологість",
                LabelFormatter = var => $"{var} г/м^3"
            });
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            time = new List<DateTime> ();
            temp = new List<double>();
            humidity = new List<int>();
            using(HttpClient client = new HttpClient())
            {
                HttpResponseMessage responseMessage = await client.GetAsync($@"https://api.openweathermap.org/data/2.5/onecall?lat=48.9244&lon=37.5722&exclude=current,minutely,daily,alerts&appid=ef5ebfda74903c4b21316165b1c7b5de&units=metric&lang=ua");
                string body = await responseMessage.Content.ReadAsStringAsync();
                Root data = JsonConvert.DeserializeObject<Root>(body);
                data.hourly.Select(d => new { Time = new DateTime(1970, 1, 1).AddSeconds(d.dt), Temp = d.temp, humidity = d.humidity });
                foreach(var item in data.hourly)
                {
                    humidity.Add(item.humidity);
                }
                foreach(var item in data.hourly)
                {
                    temp.Add(item.temp);
                }
                foreach (var item in data.hourly)
                {
                    time.Add(new DateTime(1970,1,1).AddSeconds(item.dt));
                }
                List<string> shortDateString = time.ConvertAll(p => p.ToString("dd/MM/yyyy HH:mm"));
                cartesianChart1.Series = new LiveCharts.SeriesCollection
            {
                new LineSeries
                {
                    Title="Температура",Values = new ChartValues<double>(temp)
                }
            };
                cartesianChart1.AxisX.Clear();
                cartesianChart1.AxisX.Add(new Axis
                {
                    
                    Labels = shortDateString
                });
                cartesianChart1.AxisY.Clear();
                cartesianChart1.AxisY.Add(new Axis
                {
                    Title = "Температура",
                    LabelFormatter = var => $"{var} C"
                });
                cartesianChart2.Series = new LiveCharts.SeriesCollection
            {
                new LineSeries
                {
                    Title="Вологість",Values = new ChartValues<int>(humidity),
                    //PointGeometry=null
                }
            };
                cartesianChart2.AxisX.Clear();
                cartesianChart2.AxisX.Add(new Axis
                {
                    
                    Labels = shortDateString
                });
                cartesianChart2.AxisY.Clear();
                cartesianChart2.AxisY.Add(new Axis
                {
                    Title = "Вологість",
                    LabelFormatter = var => $"{var} г/м^3"
                });
            }
        }
    }
}
