using System;
using System.Collections.Generic;
using System.Linq;

namespace textGame
{
    class Road
    {
        public string Name { get; set; }

        // все пункты на дороге
        List<CrossPoint> crossPoints = new List<CrossPoint>();
        CrossPoint start { get; set; }
        CrossPoint finish { get; set; }
        static Random random = new Random();

        // новая дорога от и до
        public Road(CrossPoint startPoint, CrossPoint finishPoint)
        {
            crossPoints.Add(startPoint);
            crossPoints.Add(finishPoint);
            start = startPoint;
            finish = finishPoint;
        }

        // получить все пункты
        public List<CrossPoint> GetCrossPoints()
        {
            return crossPoints.ToList();
        }

        // добавлене пункта на дорогу
        public void AddPoint(CrossPoint point)
        {
            crossPoints.Add(point);
        }

        // добавление дорог между 4мя пунктами
        public static List<Road> AddRoadsRect(List<CrossPoint> pts)
        {
            List<Road> roads = new List<Road>();

            // по часовой стрелке
            roads.Add(new Road(pts[0], pts[1]));
            roads[0].Name = "0";
            roads.Add(new Road(pts[1], pts[2]));
            roads[1].Name = "1";
            roads.Add(new Road(pts[2], pts[3]));
            roads[2].Name = "2";
            roads.Add(new Road(pts[3], pts[0]));
            roads[3].Name = "3";

            return roads;
        }

        // создание новых дорог
        public static List<Road> AddNewRoads(int count, int width, int height, List<Road> roads, List<CrossPoint> oldPoints, out List<CrossPoint> newPoints)
        {
            newPoints = new List<CrossPoint>();

            for (int i = 0; i < count; i++)
            {
                if (i % 2 == 0)
                {
                    // точка на AB
                    int ab = random.Next(0, width);
                    CrossPoint pt1 = new CrossPoint(ab, 0);

                    // добавляем точку
                    oldPoints.Add(pt1);

                    // добавляем точку на дорогу AB
                    roads[0].AddPoint(pt1);

                    // точка на CD
                    int cd = random.Next(0, width);
                    CrossPoint pt2 = new CrossPoint(cd, height);
                    oldPoints.Add(pt2);
                    roads[2].AddPoint(pt2);

                    // дорога между AB и CD
                    roads.Add(new Road(pt1, pt2));
                    roads[4 + i].Name = (int.Parse(roads[4 + i - 1].Name) + 1).ToString();
                }
                else
                {
                    // точка на DA 
                    int da = random.Next(0, height);
                    CrossPoint pt3 = new CrossPoint(0, da);
                    oldPoints.Add(pt3);
                    roads[3].AddPoint(pt3);

                    // точка на BC
                    int bc = random.Next(0, height);
                    CrossPoint pt4 = new CrossPoint(width, bc);
                    oldPoints.Add(pt4);
                    roads[1].AddPoint(pt4);

                    roads.Add(new Road(pt3, pt4));
                    roads[4 + i].Name = (int.Parse(roads[4 + i - 1].Name) + 1).ToString();
                }
            }

            newPoints.AddRange(oldPoints);
            return roads;
        }

        public static List<Road> CheckCrossRoads(List<Road> roads, int width, int height, out List<CrossPoint> points)
        {
            points = new List<CrossPoint>();

            // проверяем с roads[4] так как 0-3 это граничные дороги
            // перебор дорог, КОТОРЫЕ сравниваем
            for (int i = 4; i < roads.Count; i++)
            {
                // перебор дорог С КОТОРЫМИ сравиниваем
                for (int j = i; j < roads.Count; j++)
                {
                    // исключая себя из проверки
                    if (i != j)
                    {
                        // потенциальная точка 
                        CrossPoint newPoint = Intersection(roads[i].start, roads[i].finish, roads[j].start, roads[j].finish);

                        // точка в границах города
                        if (newPoint.X > 0 && newPoint.X < width &&
                            newPoint.Y > 0 && newPoint.Y < height)
                        {
                            // новая точка
                            points.Add(newPoint);

                            // добавить точку на дороги
                            if (!roads[i].GetCrossPoints().Contains(newPoint))
                                roads[i].AddPoint(newPoint);
                            if (!roads[j].GetCrossPoints().Contains(newPoint))
                                roads[j].AddPoint(newPoint);
                        }
                    }
                }
            }

            return roads;
        }

        // метод, проверяющий пересекаются ли 2 отрезка [p1, p2] и [p3, p4]
        public static bool CheckCross(CrossPoint p1, CrossPoint p2, CrossPoint p3, CrossPoint p4)
        {
            // сначала расставим точки по порядку, т.е. чтобы было p1.x <= p2.x
            if (p2.X < p1.X)
            {
                CrossPoint tmp = p1;
                p1 = p2;
                p2 = tmp;
            }

            // и p3.x <= p4.x
            if (p4.X < p3.X)
            {
                CrossPoint tmp = p3;
                p3 = p4;
                p4 = tmp;
            }

            // проверим существование потенциального интервала для точки пересечения отрезков
            if (p2.X < p3.X)
            {
                // нету взаимной абсциссы
                return false;
            }

            // если оба отрезка вертикальные
            if ((p1.X - p2.X == 0) && (p3.X - p4.X == 0))
            {
                // если они лежат на одном X
                if (p1.X == p3.X)
                {
                    // проверим пересекаются ли они, т.е. есть ли у них общий Y                    
                    // для этого возьмём отрицание от случая, когда они НЕ пересекаются
                    if (!((Math.Max(p1.Y, p2.Y) < Math.Min(p3.Y, p4.Y)) ||
                        (Math.Min(p1.Y, p2.Y) > Math.Max(p3.Y, p4.Y))))
                    {
                        return true;
                    }
                }
                return false;
            }

            // коэффициенты уравнений, содержащих отрезки
            // f1(x) = A1*x + b1 = y
            // f2(x) = A2*x + b2 = y
            double Xa;
            double Ya;
            double A1;
            double A2;
            double b1;
            double b2;

            // если первый отрезок вертикальный
            if (p1.X - p2.X == 0)
            {
                //найдём Xa, Ya - точки пересечения двух прямых
                Xa = p1.X;
                A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
                b2 = p3.Y - A2 * p3.X;
                Ya = A2 * Xa + b2;

                if (p3.X <= Xa && p4.X >= Xa &&
                    Math.Min(p1.Y, p2.Y) <= Ya &&
                    Math.Max(p1.Y, p2.Y) >= Ya)
                {
                    return true;
                }
                return false;
            }

            // если второй отрезок вертикальный
            if (p3.X - p4.X == 0)
            {
                // найдём Xa, Ya - точки пересечения двух прямых
                Xa = p3.X;
                A1 = (p1.Y - p2.Y) / (p1.X - p2.X);
                b1 = p1.Y - A1 * p1.X;
                Ya = A1 * Xa + b1;

                if (p1.X <= Xa && p2.X >= Xa &&
                    Math.Min(p3.Y, p4.Y) <= Ya &&
                    Math.Max(p3.Y, p4.Y) >= Ya)
                {
                    return true;
                }
                return false;
            }

            // оба отрезка невертикальные
            A1 = (p1.Y - p2.Y) / (p1.X - p2.X);
            A2 = (p3.Y - p4.Y) / (p3.X - p4.X);
            b1 = p1.Y - A1 * p1.X;
            b2 = p3.Y - A2 * p3.X;

            // отрезки параллельны
            if (A1 == A2)
            {
                return false;
            }

            // Xa - абсцисса точки пересечения двух прямых
            Xa = (b2 - b1) / (A1 - A2);

            // точка Xa находится вне пересечения проекций отрезков на ось X
            if ((Xa < Math.Max(p1.X, p3.X)) || (Xa > Math.Min(p2.X, p4.X)))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static CrossPoint Intersection(CrossPoint A, CrossPoint B, CrossPoint C, CrossPoint D)
        {
            double xo = A.X, yo = A.Y;
            double p = B.X - A.X, q = B.Y - A.Y;

            double x1 = C.X, y1 = C.Y;
            double p1 = D.X - C.X, q1 = D.Y - C.Y;

            double x = (xo * q * p1 - x1 * q1 * p - yo * p * p1 + y1 * p * p1) /
                (q * p1 - q1 * p);
            double y = (yo * p * q1 - y1 * p1 * q - xo * q * q1 + x1 * q * q1) /
                (p * q1 - p1 * q);

            return new CrossPoint((int)x, (int)y);
        }
    }
}