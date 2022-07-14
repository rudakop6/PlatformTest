using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace Platform
{
    public class ReaderTxtFile
    {
        private static ReaderTxtFile? _instance;
        private int _numberStop = 0;
        private string _pathToFile = string.Empty;
        private int[] _stops = Array.Empty<int>();
       
        public static ReaderTxtFile Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ReaderTxtFile();

                return _instance;
            }
        }
        public int NumberBus { get; set; }
        public int NumberStop
        {
            get { return _numberStop; }
            set
            {
                _numberStop = value;
                _stops = new int[_numberStop];
                for (int i = 1; i < _numberStop + 1; i++)
                {
                    _stops[i - 1] = i;
                }
                MainWindow.Instance.CB_StartStop.ItemsSource = MainWindow.Instance.CB_EndStop.ItemsSource = _stops;
                MainWindow.Instance.CB_StartStop.IsEnabled = MainWindow.Instance.CB_EndStop.IsEnabled = true;
            }
        }        

        public string GetRouteFilePath()
        {
            OpenFileDialog _dlgFile = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "*.txt*|*.txt*"
            };

            if (_dlgFile.ShowDialog() == true)
                return _pathToFile = _dlgFile.FileName.ToString();
            else
                return _pathToFile = string.Empty;                    
        }
        public void ReadFile()
        {
            string departureTimes = string.Empty;
            string ticketPrices = string.Empty;
            List<string> routes = new List<string>();

            if (File.Exists(_pathToFile) && !string.IsNullOrWhiteSpace(_pathToFile))
            {
                using (FileStream fileStream = new FileStream(_pathToFile, FileMode.Open))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    NumberBus = Convert.ToInt32(streamReader.ReadLine());
                    NumberStop = Convert.ToInt32(streamReader.ReadLine());

                    departureTimes = ConvertToString(streamReader.ReadLine());
                    ticketPrices = ConvertToString(streamReader.ReadLine());
                    for (int i = 0; i < NumberBus; i++)
                    {
                        routes.Add(ConvertToString(streamReader.ReadLine()));
                    }
                }
                RefreshList(departureTimes, ticketPrices, routes);
                MainWindow.Instance.Info.Text = LogRead(departureTimes, ticketPrices, routes);
            }
        }
        public void RefreshList(string departureTimes, string ticketPrices, List<string> routes)
        {
            TimeSpan[] departureTimesArray = new TimeSpan[NumberBus];
            int[] ticketPricesArray = new int[NumberBus];
            
            FillingDepartureTimes(departureTimes, departureTimesArray);
            FillingTicketPrices(ticketPrices, ticketPricesArray);
            FillingBuses(routes, departureTimesArray, ticketPricesArray);
        }
        public string LogRead(string departureTimes, string ticketPrices, List<string> routes)
        {
            string log = string.Empty;
            log += NumberBus + "\n";
            log += NumberStop + "\n";
            log += departureTimes + "\n";
            log += ticketPrices + "\n";
            foreach (var route in routes)
            {
                log += route + "\n";
            }
            return log;
        }

        private string ConvertToString(string? str)
        {
            return str ?? string.Empty;
        }
        private void FillingDepartureTimes(string departureTimes, TimeSpan[] departureTimesArray)
        {
            string[] times = departureTimes.Split(' ');
            for (int i = 0; i < NumberBus; i++)
            {
                string[] hours_minutes = times[i].Split(':');
                TimeSpan timeSpan = new TimeSpan(Convert.ToInt32(hours_minutes[0]), Convert.ToInt32(hours_minutes[1]), 0);
                departureTimesArray[i] = timeSpan;
            }
        }
        private void FillingTicketPrices(string ticketPrices, int[] ticketPricesArray)
        {
            string[] prices = ticketPrices.Split(' ');
            for (int i = 0; i < NumberBus; i++)
            {
                ticketPricesArray[i] = Convert.ToInt32(prices[i]);
            }
        }
        private void FillingBuses(List<string> routes, TimeSpan[] departureTimesArray, int[] ticketPricesArray)
        {
            FinderPaths.Instance.Buses.Clear();
            Bus.RefreshCounter();

            List<RoutePoint[]> listRoutes = FillingRoutes(routes);
            for (int i = 0; i < NumberBus; i++)
            {                
                Bus bus = new Bus(ticketPricesArray[i], departureTimesArray[i], listRoutes[i]);
                foreach (RoutePoint routePoint in bus.RoutePoints)
                {
                    routePoint.BusId = bus.Id;
                }
                FinderPaths.Instance.Buses.Add(bus);
            }
        }
        private List<RoutePoint[]> FillingRoutes(List<string> routes)
        {
            List<RoutePoint[]> listRoutes = new List<RoutePoint[]>();
            foreach (var route in routes)
            {
                string[] routeStr = route.Split(' ');
                int numberPoints = Convert.ToInt32(routeStr[0]);
                RoutePoint[] points = new RoutePoint[numberPoints];
                if (routeStr.Length - numberPoints > 1)
                {
                    for (int i = 1; i < routeStr.Length - numberPoints; i++)
                    {
                        RoutePoint point = new RoutePoint(Convert.ToInt32(routeStr[i]),
                            Convert.ToInt32(routeStr[i + numberPoints]));
                        
                        points[i - 1] = point;
                    }

                    for (int i = 0; i < points.Length; i++)
                    {
                        if (i + 1 < points.Length)
                        {
                            points[i].NextPoint = points[i + 1];
                        }
                        else
                        {
                            points[i].NextPoint = points[0];
                        }         
                    }
                    listRoutes.Add(points);
                }
            }
            return listRoutes;
        }
    }
}
