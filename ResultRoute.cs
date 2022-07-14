using System;
using System.Collections.Generic;

namespace Platform
{
    public class ResultRoute
    {
        private static int Counter = 0;

        public int Id;
        public int TimeRoute;
        public int PriceRoute;
        public TimeSpan? TimeArrived;
        public List<RoutePoint> RoutePoints;
        public ResultRoute()
        {
            Id = Counter;
            TimeRoute = 0;
            PriceRoute = 0;
            RoutePoints = new List<RoutePoint>();

            Counter++;
        }
        public static void RefreshCounter()
        {
            Counter = 0;
        }
    }
}
