using System;

namespace Platform
{
    public class Bus
    {        
        public int Id;
        public int TicketPrice;
        public TimeSpan TimeDeparture;
        public RoutePoint[] RoutePoints;
        public Bus(int id, int price, TimeSpan date, RoutePoint[] routePoints)
        {
            Id = id;
            TicketPrice = price;
            TimeDeparture = date;
            RoutePoints = routePoints;
        }
    }
}
