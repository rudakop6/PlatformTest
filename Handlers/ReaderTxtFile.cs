using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace Platform
{
    public class ReaderTxtFile
    {
        public string GetRouteFilePath()
        {
            OpenFileDialog _dlgFile = new OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "*.txt*|*.txt*"
            };

            if (_dlgFile.ShowDialog() == true)
                return _dlgFile.FileName.ToString();
            else
                return string.Empty;                    
        }
        public (FileData, List<Bus>) ReadFile(string pathToFile)
        {
            if (File.Exists(pathToFile) && !string.IsNullOrWhiteSpace(pathToFile))
            {
                FileData fileData = new FileData();
                using (FileStream fileStream = new FileStream(pathToFile, FileMode.Open))
                using (StreamReader streamReader = new StreamReader(fileStream))
                {                    
                    fileData.NumberBus = Convert.ToInt32(streamReader.ReadLine());
                    fileData.NumberStop = Convert.ToInt32(streamReader.ReadLine());
                    fileData.DepartureTimes = ConvertToString(streamReader.ReadLine());
                    fileData.TicketPrices = ConvertToString(streamReader.ReadLine());
                    fileData.Routes = new List<string>();
                    for (int i = 0; i < fileData.NumberBus; i++)
                    {
                        fileData.Routes.Add(ConvertToString(streamReader.ReadLine()));
                    }
                }
                return (fileData, RefreshList(fileData));
            }
            throw new Exception();
        }
        private List<Bus> RefreshList(FileData fileData)
        {
            TimeSpan[] departureTimesArray = FillingDepartureTimes(fileData);
            int[] ticketPricesArray = FillingTicketPrices(fileData);
            return FillingBuses(fileData.Routes, departureTimesArray, ticketPricesArray);
        }        

        private string ConvertToString(string? str)
        {
            return str ?? string.Empty;
        }
        private TimeSpan[] FillingDepartureTimes(FileData fileData)
        {
            TimeSpan[] departureTimesArray = new TimeSpan[fileData.NumberBus];
            string[] times = fileData.DepartureTimes.Split(' ');
            for (int i = 0; i < fileData.NumberBus; i++)
            {
                string[] hours_minutes = times[i].Split(':');
                TimeSpan timeSpan = new TimeSpan(Convert.ToInt32(hours_minutes[0]), Convert.ToInt32(hours_minutes[1]), 0);
                departureTimesArray[i] = timeSpan;
            }
            return departureTimesArray;
        }
        private int[] FillingTicketPrices(FileData fileData)
        {
            int[] ticketPricesArray = new int[fileData.NumberBus];
            string[] prices = fileData.TicketPrices.Split(' ');
            for (int i = 0; i < fileData.NumberBus; i++)
            {
                ticketPricesArray[i] = Convert.ToInt32(prices[i]);
            }
            return ticketPricesArray;
        }

        private List<Bus> FillingBuses(List<string> routes, TimeSpan[] departureTimesArray, int[] ticketPricesArray)
        {
            List<Bus> buses = new List<Bus>();
            
            string[] routeStr;
            int numberPoints;
            Bus bus;
            for (int i = 0; i < routes.Count; i++)
            {
                routeStr = routes[i].Split(' ');
                numberPoints = Convert.ToInt32(routeStr[0]);
                bus = new Bus(i, ticketPricesArray[i], departureTimesArray[i], new RoutePoint[numberPoints]);

                if (routeStr.Length - numberPoints > 1)
                {
                    for (int j = 1; j < routeStr.Length - numberPoints; j++)// j = 1, т.к. первый элемент уже считали до цикла 
                    {
                        RoutePoint point = new RoutePoint(Convert.ToInt32(routeStr[j]), bus.Id,
                            Convert.ToInt32(routeStr[j + numberPoints]));

                        bus.RoutePoints[j - 1] = point;
                    }

                    for (int k = 0; k < bus.RoutePoints.Length; k++)
                    {
                        if (k + 1 < bus.RoutePoints.Length)
                            bus.RoutePoints[k].NextPoint = bus.RoutePoints[k + 1];                        
                        else
                            bus.RoutePoints[k].NextPoint = bus.RoutePoints[0];                        
                    }

                    for (int k = 0; k < bus.RoutePoints.Length; k++)
                    {
                        if (k - 1 < 0)
                            bus.RoutePoints[k].PrevPoint = bus.RoutePoints[bus.RoutePoints.Length - 1];
                        else
                            bus.RoutePoints[k].PrevPoint = bus.RoutePoints[k - 1];
                    }
                }
                buses.Add(bus);
            }
            return buses;
        }
    }
}
