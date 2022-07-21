using System;
using System.Collections.Generic;

namespace Platform
{
    public class ResultRoute
    {
        public int Id;
        public int TimeRoute;
        public int PriceRoute;
        public TimeSpan? ArrivalTime;
        public List<RoutePoint> RoutePoints;
        public ResultRoute(int id, int timeRoute, int priceRoute, TimeSpan? arrivalTime, List<RoutePoint> routePoints)
        {
            Id = id;
            TimeRoute = timeRoute;
            PriceRoute = priceRoute;
            ArrivalTime = arrivalTime;
            RoutePoints = routePoints;
        }
    }
}
