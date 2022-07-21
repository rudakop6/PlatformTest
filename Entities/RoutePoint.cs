using System;
using System.Collections.Generic;

namespace Platform
{
    public class RoutePoint
    {
        public int PointId;
        public int BusId;
        public RoutePoint? PrevPoint;
        public RoutePoint? NextPoint;
        public int TransitTime;
        public List<RoutePoint> ListPrevPoints;
        public List<RoutePoint> ListNextPoints;
        public RoutePoint(int pointId, int busId, int time)
        {
            PointId = pointId;
            BusId = busId;
            TransitTime = time;
            PrevPoint = null;
            NextPoint = null;
            ListPrevPoints = new List<RoutePoint>();
            ListNextPoints = new List<RoutePoint>();
        }
    }
}
