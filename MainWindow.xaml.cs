using System;
using System.Linq;
using System.Windows;

namespace Platform
{
    public partial class MainWindow : Window
    {
        private readonly int[] _hours = new int[Constants.NumberHoursInDay];
        private readonly int[] _minutes = new int[Constants.NumberMinutesInHour];        

        private static MainWindow? _instance;
        public static MainWindow Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainWindow();

                return _instance;
            }
        }
        public MainWindow()
        {
            _instance = this;
            InitializeComponent();
            InitTimeCB();
        }
        private void InitTimeCB()
        {
            for (int i = 0; i < Constants.NumberHoursInDay; i++)
            {
                _hours[i] = i;
            }
            CB_TimeHour.ItemsSource = _hours;
            for (int i = 0; i < Constants.NumberMinutesInHour; i++)
            {
                _minutes[i] = i;
            }
            CB_TimeMinute.ItemsSource = _minutes;
        }

        private void ButtonPickRouteFile_Click(object sender, RoutedEventArgs e)
        {
            PathRouteFile.Text = ReaderTxtFile.Instance.GetRouteFilePath();
        }

        private void ButtonReader_Click(object sender, RoutedEventArgs e)
        {
            ReaderTxtFile.Instance.ReadFile();
            LogAfterRead();
        }
        public TimeSpan TimeArrive;
        private void ButtonCalculate_Click(object sender, RoutedEventArgs e)
        {
            int? start = CB_StartStop.SelectedItem as int?;
            int? end = CB_EndStop.SelectedItem as int?;

            int? hours = CB_TimeHour.SelectedItem as int?;
            int? minutes = CB_TimeMinute.SelectedItem as int?;
            if (start == null || end == null || start == end)
            {
                InfoMessage("Выберите стартовую и конечную точку (они должны отличаться)");
                return;
            }
            if (hours == null || minutes == null)
            {
                InfoMessage("Выберите время отправки");
                return;
            }
            TimeArrive = new TimeSpan(ConvertToInt(hours), ConvertToInt(minutes), 0);
            Info.Text = FinderPaths.Instance.CreateMap(ConvertToInt(start), ConvertToInt(end));
            ResultLog();
        }





        private int ConvertToInt(int? num)
        {
            return num ?? 0;
        }
        private void InfoMessage(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LogAfterRead()
        {
            string log = string.Empty;
            foreach (var bus in FinderPaths.Instance.Buses)
            {
                log += string.Format("Номер автобуса: {0}\n", bus.Id);
                log += string.Format("Начало движения в: {0}\n", bus.TimeDeparture);
                log += string.Format("Стоимость проезда: {0}\n\n", bus.TicketPrice);
            }
            Info.Text += log;
        }

        private void ResultLog()
        {
            string log = string.Empty;
            string finalRoute = string.Empty;

            ResultRoute? fasterRoute = null;
            ResultRoute? lowCostRoute = null;

            foreach(var item in FinderPaths.Instance.ResultRoutes)
            {                
                if(fasterRoute == null || fasterRoute.TimeRoute > 0 && fasterRoute.TimeRoute > item.TimeRoute)
                {
                    fasterRoute = item;
                }
                if (lowCostRoute == null || lowCostRoute.PriceRoute > item.PriceRoute)
                {
                    lowCostRoute = item;
                }


                for (int k = 0; k < item.RoutePoints.Count - 1; k++)
                {
                    Bus bus = FinderPaths.Instance.Buses.First(bus => bus.Id == item.RoutePoints.ElementAt(k).BusId);
                    finalRoute += item.RoutePoints.ElementAt(k).Id + string.Format("--({0}={1})", bus.Id, bus.TicketPrice) + "-->";
                }
                finalRoute += item.RoutePoints.ElementAt(item.RoutePoints.Count - 1).Id;
                
                if(item.TimeRoute <= 0)
                    log += string.Format("\nМаршрут: {0}\n{1}\nЗатраченное время: {2}\nСтоимость поездки: {3}\nВремя прибытия: {4}",
                                        item.Id, finalRoute, "время прибытия позже 0:00" , item.PriceRoute, item.TimeArrived);
                else
                    log += string.Format("\nМаршрут: {0}\n{1}\nЗатраченное время: {2}\nСтоимость поездки: {3}\nВремя прибытия: {4}",
                                        item.Id, finalRoute, item.TimeRoute, item.PriceRoute, item.TimeArrived);
            }

            log += string.Format("\n\nРезультат: \nСамый быстрый маршрут: {0}\nСамый дешёвый маршрут: {1}", 
                fasterRoute.Id, lowCostRoute.Id);

            Info1.Text = log;
        }
    }
}
