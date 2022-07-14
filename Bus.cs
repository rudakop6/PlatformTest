using System;

namespace Platform
{
    public class Bus
    {
        private static int Counter = 0;
        
        public int Id;
        public int TicketPrice;
        public TimeSpan TimeDeparture;

        public RoutePoint[] RoutePoints;
        public Bus(int price, TimeSpan date, RoutePoint[] routePoints)
        {
            Id = Counter;
            TicketPrice = price;
            TimeDeparture = date;
            RoutePoints = routePoints;
            Counter++;
        }
        public static void RefreshCounter()
        {
            Counter = 0;
        }
    }
}
