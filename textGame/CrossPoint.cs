using System;
using System.Collections.Generic;

namespace textGame
{
    enum PointType
    {
        PostOffice,
        Hospital,
        PoliceDepartment,

        Administration,
        School,
        Hotel
    }

    class CrossPoint
    {
        int max = Enum.GetNames(typeof(PointType)).Length;
        Random rnd = new Random();

        // свои координаты
        public int X { get; set; }
        public int Y { get; set; }
        public static string Name { get; set; }

        // установитьт/получить тип
        public string Type { get; set; }

        // новый пункт координаты и тип
        public CrossPoint(int x, int y)
        {
            X = x;
            Y = y;
            Type = RandomType();
        }

        // рандомный тип для пункта
        private string RandomType()
        {
            System.Threading.Thread.Sleep(1);
            PointType n = (PointType)rnd.Next(0, max);
            return n.ToString();
        }

        // 4 точки прямоугольника
        public static List<CrossPoint> AddRectPoints(int w, int h)
        {
            // по часовой стрелке
            CrossPoint A = new CrossPoint(0, 0);
            CrossPoint B = new CrossPoint(w, 0);
            CrossPoint C = new CrossPoint(w, h);
            CrossPoint D = new CrossPoint(0, h);

            List<CrossPoint> crossPoints = new List<CrossPoint>();
            crossPoints.Add(A);
            crossPoints.Add(B);
            crossPoints.Add(C);
            crossPoints.Add(D);

            return crossPoints;
        }

        public static double Distance(CrossPoint point1, CrossPoint point2)
        {
            // вычисление расстояния до точки
            int x1 = point1.X;
            int y1 = point1.Y;
            int x2 = point2.X;
            int y2 = point2.Y;

            double dist = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

            return dist;
        }
    }
}