namespace Platform
{
    public class RoutePoint
    {
        public int Id;
        public int BusId;
        public RoutePoint? PrevPoint;
        public RoutePoint? NextPoint;
        public int TransitTime;
        public int TimeToPoint;

        public RoutePoint(int pointId, int time)
        {
            Id = pointId;
            BusId = -1;
            TransitTime = time;
            TimeToPoint = 0;
            PrevPoint = null;
            NextPoint = null;
        }
    }
}
