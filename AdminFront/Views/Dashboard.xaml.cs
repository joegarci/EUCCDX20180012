using MIA.Helpers;
using MIA.Model;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


namespace MIA.Views
{
    public partial class Dashboard : UserControl
    {
        public int IdAssitants = 0;
        public ChartValues<int> ValueTotal;
        public ChartValues<int> ValueProcesando;
        public ChartValues<int> ValueRechazado;
        public ChartValues<int> ValueTotalHour;
        public ChartValues<int> ValueTicket;
        public ChartValues<int> ValueRadicate;
        public ChartValues<int> ValueError;

        public Dashboard()
        {
            InitializeComponent();
            LoadDispatcher();
            Clear();
        }
        private void LoadDispatcher()
        {
            DispatcherTimer dispatch = new DispatcherTimer();
            dispatch.Tick += new EventHandler(Dispatch_Timer);
            dispatch.Interval = new TimeSpan(0, 0, 1);
            dispatch.Start();
        }
        private void Dispatch_Timer(object sender, EventArgs e)
        {
            ReloadDataBase();
        }
        private void ReloadDataBase()
        {
            using (ModelDB modeldb = new ModelDB())
            {
                foreach (System.Data.Entity.Infrastructure.DbEntityEntry item in modeldb.ChangeTracker.Entries())
                {
                    item.Reload();
                }
            }
            GetData();
        }
        public void GetData()
        {
            using (ModelDB modeldb = new ModelDB())
            {
            }
        }
        public void Clear()
        {
            cbAssitent.ItemsSource = null;
            List<Assistants> assistant = new List<Assistants>();
            CommonFunctions commonFunctions = new CommonFunctions();
            if (commonFunctions.GetAssitant().Count > 1)
            {
                assistant = commonFunctions.GetAssitant();
            }
            else
            {
                IdAssitants = commonFunctions.GetAssitant().Select(X => X.IdAssistant).FirstOrDefault();
                ViewInformation();
                cbAssitent.Visibility = Visibility.Collapsed;
            }
            assistant.Add(new Assistants { IdAssistant = 0, NameAssistant = "Todos los asistentes", CodAssistant = "ALL" });
            cbAssitent.ItemsSource = assistant;
        }
        public void ViewInformation()
        {
            if (IdAssitants != 0)
            {
                Refresh();
                using (ModelDB modeldb = new ModelDB())
                {
                    string name = modeldb.Assistants.Where(P => P.IdAssistant == IdAssitants).FirstOrDefault().CodAssistant;
                    switch (name)
                    {
                        case "Assis01":
                        case "Assis02":
                            Grid1.Visibility = Visibility.Visible;
                            TodosAsistentes.Visibility = Visibility.Hidden;
                            break;
                    }
                }
            }
            else
            {
                // Mostrar la informacion consolidada de todos los asistentes (Si aplica)
                Grid1.Visibility = Visibility.Hidden;
                TodosAsistentes.Visibility = Visibility.Visible;
            }
        }
        private void CbAssitent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbAssitent.SelectedValue != null)
            {
                IdAssitants = int.Parse(cbAssitent.SelectedValue.ToString());
            }
            ViewInformation();
        }
        public void Refresh()
        {
            DataContext = null;
            CardsInfo();
            LineGraph();
            PieGraph();
            BarGraph();
            HorizontalBarGraph();
            GaugeGraph();
            LineGraphTwo();
            DataContext = this;
        }

        // --- Funcion encargada para diligenciar las tarjetas ---- //
        public void CardsInfo()
        {
            // formato para valores monetarios //
            double data = 17811.31;
            txt_Card_1.Text = data.ToString("C0");

            data = 1123.31;
            txt_Card_2.Text = data.ToString("C0");

            data = 11231.31;
            txt_Card_3.Text = data.ToString("C0");

            data = 111234141.31;
            txt_Card_4.Text = data.ToString("C0");

            data = 113211.2131;
            txt_Card_5.Text = data.ToString("C0");
            // Formato normal tipo texto //
            txt_Card_6.Text = "1";
            txt_Card_7.Text = "2";
            txt_Card_8.Text = "3";
            txt_Card_9.Text = "4";
            txt_Card_10.Text = "5";
        }

        // Grafica de Lineas: Para mas información https://lvcharts.net/App/examples/v1/Wpf/Line //
        public void LineGraph()
        {
            // En Values puede ir desde un array hasta un list para los valores
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                },
                new LineSeries
                {
                    Title = "Series 2",
                    Values = new ChartValues<double> { 6, 7, 3, 4 ,6 },

                },
                new LineSeries
                {
                    Title = "Series 3",
                    Values = new ChartValues<double> { 4,2,7,2,7 },
                }
            };

            List<String> lista = new List<String>
            {
                "Enero",
                "Febrero",
                "Marzo",
                "Abril",
                "Mayo"
            };
            Labels = new ObservableCollection<String>(lista.Select(x => "" + x));
        }

        public SeriesCollection SeriesCollection { get; set; }
        public ObservableCollection<String> Labels { get; set; }

        // Grafica de Torta: Para mas información https://lvcharts.net/App/examples/v1/Wpf/Pie%20or%20Doughnut //
        public void PieGraph()
        {
            SeriesCollectionPie = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Chrome",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(8) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Mozilla",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(6) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Opera",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(10) },
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Explorer",
                    Values = new ChartValues<ObservableValue> { new ObservableValue(4) },
                    DataLabels = true
                }
            };
        }
        public SeriesCollection SeriesCollectionPie { get; set; }
        public Func<ChartPoint, string> PointLabel { get; set; }

        // Grafica de Barras: Para mas información https://lvcharts.net/App/examples/v1/Wpf/Column //

        public void BarGraph()
        {
            SeriesCollectionBar = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Serie 1",
                    Values = new ChartValues<double> { 10, 50, 39, 50 }
                },
                new ColumnSeries
                {
                    Title = "Serie 2",
                    Values = new ChartValues<double> { 11, 56, 42 }
                }
            };

            List<String> lista = new List<String>
            {
                "Enero",
                "Febrero",
                "Marzo",
                "Abril"
            };
            LabelsBar = new ObservableCollection<String>(lista.Select(x => "" + x));
        }
        public SeriesCollection SeriesCollectionBar { get; set; }
        public ObservableCollection<String> LabelsBar { get; set; }

        // Grafica de lineas Horizontal: Para mas información https://lvcharts.net/App/examples/v1/Wpf/Row//

        public void HorizontalBarGraph()
        {
            SeriesCollectionHBar = new SeriesCollection
            {
                new RowSeries
                {
                    Title = "Serie 1",
                    Values = new ChartValues<double> { 10, 50 }
                },
                new RowSeries
                {
                    Title = "Serie 2",
                    Values = new ChartValues<double> { 11, 56 }
                }
            };

            List<String> lista = new List<String>
            {
                "Enero",
                "Febrero"
            };
            LabelsHBar = new ObservableCollection<String>(lista.Select(x => "" + x));
        }
        public SeriesCollection SeriesCollectionHBar { get; set; }
        public ObservableCollection<String> LabelsHBar { get; set; }

        public double Value { get; set; }
        public double From { get; set; }
        public double To { get; set; }

        public void GaugeGraph()
        {
            From = 0;
            To = 300;
            Value = 160.9;
        }

        public void LineGraphTwo()
        {
            // En Values puede ir desde un array hasta un list para los valores
            SeriesCollectionTwo = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                }
            };

            List<String> lista =  new List<String>
            {
                "Enero"
            };
            LabelsTwo = new ObservableCollection<String>(lista.Select(x => "" + x));
        }
        public SeriesCollection SeriesCollectionTwo { get; set; }
        public ObservableCollection<String> LabelsTwo { get; set; }
    }
}
