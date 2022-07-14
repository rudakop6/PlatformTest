using System;
using System.Collections.Generic;
using System.Linq;

namespace Platform
{
    public class FinderPaths
    {
        private static FinderPaths? _instance;
        private int _startNumberStop = 1;

        public List<ResultRoute> ResultRoutes = new List<ResultRoute>();
        public List<Bus> Buses = new List<Bus>();

        public static FinderPaths Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FinderPaths();

                return _instance;
            }
        }

        public string CreateMap(int startPoint, int endPoint)
        {
            ResultRoutes.Clear();
            ResultRoute.RefreshCounter();

            int countStop = ReaderTxtFile.Instance.NumberStop + 1;
            FillingMap(countStop, out RoutePoint[][] map);
            string log = LogMap(countStop, map);
            FindPaths(countStop, map, startPoint, endPoint);

            return log;
        }        
        private void FillingMap(int countStop, out RoutePoint[][] map)
        {
            map = new RoutePoint[countStop][];
            for (int i = _startNumberStop; i < countStop; i++)
            {
                map[i] = new RoutePoint[countStop];
                for (int j = _startNumberStop; j < countStop; j++)
                {
                    foreach (Bus bus in Buses)
                    {
                        if (bus.RoutePoints.Any(point => point.Id == i && point?.NextPoint?.Id == j))
                        {
                            RoutePoint point = bus.RoutePoints.First(point => point.Id == i && point?.NextPoint?.Id == j);
                            int time = point.TransitTime;
                            if (map[i][j] == null || map[i][j].TransitTime > time)
                            {
                                map[i][j] = point;
                            }
                        }
                    }
                }
            }
        }
        private string LogMap(int countStop, RoutePoint[][] map)
        {
            string log = string.Empty;
            for (int i = _startNumberStop; i < countStop; i++)
            {
                log += "\n[";
                for (int k = _startNumberStop; k < countStop; k++)
                {
                    if (map[i][k] != null)
                        log += string.Format(" {0}", map[i][k].TransitTime);
                    else
                        log += string.Format("null");
                }
                log += string.Format("]\n");
            }
            return log;
        }
        private void FindPaths(int countStop, RoutePoint[][] map, int startPoint, int endPoint)
        {
            bool[] used = new bool[countStop + 1];
            used[startPoint] = true;
            int prevStartPoint = -1;
            
            Queue<int> queuePoints = new Queue<int>();
            queuePoints.Enqueue(startPoint);
            while (queuePoints.Count != 0)
            {
                startPoint = queuePoints.Peek();
                queuePoints.Dequeue();

                for (int i = 0; i < map.Length; i++)
                {
                    if (map[startPoint][i] != null)
                    {
                        if (!used[i])
                        {
                            if (prevStartPoint >= 0)
                                map[startPoint][i].PrevPoint = map[prevStartPoint][startPoint];                          
                            else                            
                                map[startPoint][i].PrevPoint = null;                          

                            if (i == endPoint)
                            {
                                List<RoutePoint> list = GetRoute(map[startPoint][i]);
                                CalculateRoute(list);
                            }
                            else
                            {
                                queuePoints.Enqueue(i);
                            }
                        }
                    }
                }
                prevStartPoint = startPoint;
            }
        }
        private List<RoutePoint> GetRoute(RoutePoint point)
        {
            RoutePoint? prevPoint = point;
            List<RoutePoint> list = new List<RoutePoint>();
            list.Add(prevPoint.NextPoint);
            while (prevPoint != null)
            {
                list.Add(prevPoint);
                prevPoint = prevPoint?.PrevPoint;
            }
            list.Reverse();
            return list;
        }
        private void CalculateRoute(List<RoutePoint> routePoints)
        {
            TimeSpan? summTime = MainWindow.Instance.TimeArrive;
            int summPrice = 0;
            int test = -1;
            for (int k = 0; k < routePoints.Count - 1; k++)
            {
                Bus bus = Buses.First(bus => bus.Id == routePoints.ElementAt(k).BusId);

                summTime = GetArrivalTime(bus, routePoints.ElementAt(k).Id, summTime);
                if(summTime == null)
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

            ResultRoute resultRoute = new ResultRoute();
            resultRoute.RoutePoints = routePoints;
            resultRoute.PriceRoute = summPrice;

            if (summTime == null)
                resultRoute.TimeRoute = -1;
            else
                resultRoute.TimeRoute = Convert.ToInt32(summTime.Value.TotalMinutes - MainWindow.Instance.TimeArrive.TotalMinutes);

            resultRoute.TimeArrived = summTime;
            ResultRoutes.Add(resultRoute);
        }

        private TimeSpan? GetArrivalTime(Bus bus, int point, TimeSpan? time)
        {
            TimeSpan arrivalTime = bus.TimeDeparture;
            RoutePoint? currentPoint = bus.RoutePoints[0];
            if(time < arrivalTime)
            {
                time = arrivalTime;
            }
            while (arrivalTime < time || currentPoint?.Id != point)
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
