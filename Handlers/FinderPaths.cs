using System;
using System.Collections.Generic;
using System.Linq;

namespace Platform
{
    public class FinderPaths
    {
        private TimeSpan _timeArrive;
        public List<ResultRoute> AllResultRoutes = new List<ResultRoute>();
        public FinderPaths(int numberStop, TimeSpan timeArrive)
        {
            _timeArrive = timeArrive;
        }

        public List<ResultRoute> CreateMap(int startPoint, int endPoint)
        {
            AllResultRoutes.Clear();
            Func(FillingMap(), startPoint, endPoint);
            return AllResultRoutes;
        }

        private List<RoutePoint> FillingMap()
        {
            List<Bus> buses = ViewModel.Instance.Buses;
            List<RoutePoint> listPoints = new List<RoutePoint>();

            List<RoutePoint> prevList = new List<RoutePoint>();
            List<RoutePoint> nextList = new List<RoutePoint>();
            foreach (Bus bus in buses)
            {
                foreach (RoutePoint routePoint in bus.RoutePoints)
                {
                    prevList.Add(routePoint.PrevPoint);
                    nextList.Add(routePoint.NextPoint);
                }
            }
            foreach (Bus bus in buses)
            {
                foreach (RoutePoint routePoint in bus.RoutePoints)
                {
                    routePoint.ListPrevPoints.Clear();
                    routePoint.ListNextPoints.Clear();  
                    routePoint.ListPrevPoints.AddRange(prevList.Where(point => point.NextPoint.PointId == routePoint.PointId));
                    routePoint.ListNextPoints.AddRange(nextList.Where(point => point.PrevPoint.PointId == routePoint.PointId));
                    if (!listPoints.Any(point => point.PointId == routePoint.PointId))
                    {
                        listPoints.Add(routePoint);
                    }
                }
            }

            //ViewModel.Instance.TextLogMap = string.Empty;
            //foreach (RoutePoint point in listPoints)
            //{
            //    ViewModel.Instance.TextLogMap += "--------\n";
            //    ViewModel.Instance.TextLogMap += String.Format("{0} {1} {2}\n",
            //            point.ListPrevPoints.Count, point.PointId, point.ListNextPoints.Count);
            //}
            return listPoints;
        }

        private void Func(List<RoutePoint> listPoints, int startPoint, int endPoint)
        {
            int start = startPoint;
            bool[,] used = new bool[listPoints.Count + 1, ViewModel.Instance.Buses.Count];
            Stack<RoutePoint> stackPoints = new Stack<RoutePoint>();
            RoutePoint currentPoint = listPoints.First(point => point.PointId == startPoint);

            stackPoints.Push(currentPoint);

            while (stackPoints.Count != 0)
            {
                currentPoint = stackPoints.Pop();
                for (int i = 0; i < currentPoint.ListPrevPoints.Count; i++)
                {
                    if (!used[currentPoint.ListNextPoints[i].PointId, currentPoint.ListNextPoints[i].BusId])
                    {
                        if (currentPoint.ListNextPoints[i].PointId == endPoint)
                        {
                            used[currentPoint.PointId, currentPoint.ListPrevPoints[i].BusId] = true;
                            List<RoutePoint> list = GetRoute(currentPoint.ListNextPoints[i], start, used);
                            CalculateRoute(list);
                        }
                        else
                        {
                            stackPoints.Push(currentPoint.ListNextPoints[i]);
                            used[currentPoint.PointId, currentPoint.ListPrevPoints[i].BusId] = true;
                        }
                    }
                }
            }
        }

        private List<RoutePoint> GetRoute(RoutePoint point, int startedPoint, bool[,] used)
        {
            RoutePoint? prevPoint = point;
            List<RoutePoint> list = new List<RoutePoint>();

            while (prevPoint.PointId != startedPoint)
            {
                list.Add(prevPoint);
                RoutePoint prev;
                switch (prevPoint.ListPrevPoints.Count)
                {
                    case 0:
                        break;
                    case 1:
                        prev = prevPoint.ListPrevPoints.Last();
                        prevPoint = prevPoint.ListPrevPoints.First();
                        break;
                    default:
                        prev = prevPoint.ListPrevPoints.Last(point => used[point.PointId, point.BusId]);
                        prevPoint.ListPrevPoints.Remove(prev);
                        prevPoint = prev;
                        
                        break;
                }
            }
            list.Add(prevPoint);
            list.Reverse();
            return list;
        }



        private void CalculateRoute(List<RoutePoint> routePoints)
        {
            TimeSpan? summTime = _timeArrive;
            int summPrice = 0;
            int test = -1;
            Bus bus;

            for (int k = 0; k < routePoints.Count - 1; k++)
            {
                bus = ViewModel.Instance.Buses.First(bus => bus.Id == routePoints.ElementAt(k).BusId);
                summTime = GetArrivalTime(bus, routePoints.ElementAt(k).PointId, summTime);
                if (summTime == null)
                {
                    break;
                }
                summTime = new TimeSpan(summTime.Value.Hours,
                    summTime.Value.Minutes + routePoints.ElementAt(k).TransitTime, summTime.Value.Seconds);

                if (summPrice == 0 || routePoints.ElementAt(k).BusId != test)
                {
                    test = routePoints.ElementAt(k).BusId;
                    summPrice += bus.TicketPrice;
                }
            }

            int timeRoute;
            if (summTime == null)
                timeRoute = -1;
            else
                timeRoute = Convert.ToInt32(summTime.Value.TotalMinutes - _timeArrive.TotalMinutes);

            ResultRoute resultRoute = new ResultRoute(AllResultRoutes.Count, timeRoute, summPrice, summTime, routePoints);
            AllResultRoutes.Add(resultRoute);
        }
        private TimeSpan? GetArrivalTime(Bus bus, int point, TimeSpan? time)
        {
            TimeSpan arrivalTime = bus.TimeDeparture;
            RoutePoint? currentPoint = bus.RoutePoints[0];
            if (time < arrivalTime)
            {
                time = arrivalTime;
            }
            while (arrivalTime < time || currentPoint?.PointId != point)
            {
                arrivalTime = new TimeSpan(arrivalTime.Hours,
                    arrivalTime.Minutes + currentPoint.TransitTime, arrivalTime.Seconds);

                currentPoint = currentPoint?.NextPoint;
            }

            if (arrivalTime > TimeSpan.Zero && arrivalTime < bus.TimeDeparture)
                return null;
            else
                return arrivalTime;
        }
    }
}
