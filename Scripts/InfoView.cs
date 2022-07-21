using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Platform
{
    public static class InfoView
    {
        public static string LogRead(FileData fileData)
        {
            string log = string.Empty;
            log += fileData.NumberBus + "\n";
            log += fileData.NumberStop + "\n";
            log += fileData.DepartureTimes + "\n";
            log += fileData.TicketPrices + "\n";
            foreach (var route in fileData.Routes)
            {
                log += route + "\n";
            }
            return log;
        }
        
        public static string LogAfterRead(List<Bus> buses)
        {
            string log = "\n";
            foreach (var bus in buses)
            {
                log += string.Format("Номер автобуса: {0}\n", bus.Id);
                log += string.Format("Начало движения в: {0}\n", bus.TimeDeparture);
                log += string.Format("Стоимость проезда: {0}\n\n", bus.TicketPrice);
            }
            return log;
        }
        public static string LogResult(List<ResultRoute> resultRoutes, List<Bus> buses)
        {
            string log = string.Empty;
            string finalRoute;

            ResultRoute? fasterRoute = null;
            ResultRoute? lowCostRoute = null;

            foreach (var item in resultRoutes)
            {
                finalRoute = string.Empty;
                if (fasterRoute == null || fasterRoute.TimeRoute > 0 && fasterRoute.TimeRoute > item.TimeRoute)
                {
                    fasterRoute = item;
                }
                if (lowCostRoute == null || lowCostRoute.PriceRoute > item.PriceRoute)
                {
                    lowCostRoute = item;
                }

                for (int k = 0; k < item.RoutePoints.Count - 1; k++)
                {
                    Bus bus = buses.First(bus => bus.Id == item.RoutePoints.ElementAt(k).BusId);
                    finalRoute += item.RoutePoints.ElementAt(k).PointId + string.Format("--({0}={1})", bus.Id, bus.TicketPrice) + "-->";
                }
                finalRoute += item.RoutePoints.ElementAt(item.RoutePoints.Count - 1).PointId;

                if (item.TimeRoute <= 0)
                    log += string.Format("\nМаршрут: {0}\n{1}\nЗатраченное время: {2}\nСтоимость поездки: {3}\nВремя прибытия: {4}",
                                        item.Id, finalRoute, "время прибытия позже 0:00", item.PriceRoute, item.ArrivalTime);
                else
                    log += string.Format("\nМаршрут: {0}\n{1}\nЗатраченное время: {2}\nСтоимость поездки: {3}\nВремя прибытия: {4}",
                                        item.Id, finalRoute, item.TimeRoute, item.PriceRoute, item.ArrivalTime);
            }

            log += string.Format("\n\nРезультат: \nСамый быстрый маршрут: {0}\nСамый дешёвый маршрут: {1}",
                fasterRoute.Id, lowCostRoute.Id);

            return log;
        }
        public static void InfoMessage(string message)
        {
            MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
