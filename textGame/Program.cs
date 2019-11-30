using System;
using System.Collections.Generic;

namespace textGame
{
    class Program
    {
        static int width = 100;
        static int height = 50;
        static int count = 5;
        static string search = "Hospital";

        static List<CrossPoint> points = new List<CrossPoint>();
        static List<Road> roads = new List<Road>();

        static void Main(string[] args)
        {
            Console.WriteLine("Карта размером " + width + "x" + height);

            // 4 начальные точки
            points.AddRange(CrossPoint.AddRectPoints(width, height));

            // дороги
            roads.AddRange(Road.AddRoadsRect(points));

            // центральные дороги
            roads = Road.AddNewRoads(count, width, height, roads, points, out List<CrossPoint> list);
            points = list;
            Console.WriteLine("\nДобавлено центральных дорог: " + count);

            // новые точки на пересечениях центральных дорог
            List<CrossPoint> newCrossPoints = new List<CrossPoint>();
            roads = Road.CheckCrossRoads(roads, width, height, out newCrossPoints);
            points.AddRange(newCrossPoints);

            Console.WriteLine("\nВсе доступные пункты: ");

            // все созданные точки 
            List<CrossPoint> show = new List<CrossPoint>();
            foreach (var item in roads)
                foreach (var item2 in item.GetCrossPoints())
                    if (!show.Contains(item2))
                        show.Add(item2);
            foreach (var item in show)
                Console.WriteLine(item.X + ";" + item.Y + " \t\t" + item.Type);

            // отдельный граф для вычисления путей по алгоритму Дейкстры
            var g = new Graph();

            // добпаление аершин
            foreach (var item in points)
            {
                g.AddVertex(item.X + "_" + item.Y);
            }

            // перебор дорог
            foreach (var item in roads)
            {
                // список точек на дороге
                var cp = item.GetCrossPoints();
                for (int i = 0; i < cp.Count - 1; i++)
                {
                    var from = cp[i];
                    var to = cp[i + 1];
                    int distance = (int)CrossPoint.Distance(from, to);

                    // добавление связи в одну и другую сторону
                    g.AddEdge(from.X + "_" + from.Y, to.X + "_" + to.Y, distance);
                    g.AddEdge(to.X + "_" + to.Y, from.X + "_" + from.Y, distance);

                }
            }

            Console.WriteLine("\nИщем путь к " + search);

            var dijkstra = new Dijkstra(g);
            List<string> shortPaths = new List<string>();

            // перебирая все точки
            foreach (var point in points)
            {
                // берём нужный тип
                if (point.Type == search)
                {
                    // получаем имя-координаты
                    string search = point.X + "_" + point.Y;
                    var path = dijkstra.FindShortestPath("0_0", search);
                    shortPaths.Add(path);
                }
            }

            Console.WriteLine("\nКороткие пути: ");
            if (shortPaths.Count > 0)
                foreach (var item in shortPaths)
                    Console.WriteLine(item);
            else
                Console.WriteLine("Коротких путей нету, так как нету нужного пункта назначения!");

            string[] str = { " -> " };
            double length;
            double minAnswer = double.MaxValue;
            string pathAnswer = "Отсутствует.";
            if (shortPaths.Count > 0)
            {
                foreach (var item in shortPaths)
                {
                    length = 0;
                    string[] coords = item.Split(str, StringSplitOptions.None);

                    // если в пути больше 1 точки (своей)
                    if (coords.Length > 1)
                    {
                        // перебор точек пути для суммирования длины пути
                        for (int i = 0; i < coords.Length - 1; i++)
                        {
                            string[] xy1 = coords[i].Split('_');
                            string[] xy2 = coords[i + 1].Split('_');
                            length += CrossPoint.Distance(new CrossPoint(int.Parse(xy1[0]), int.Parse(xy1[1])), new CrossPoint(int.Parse(xy2[0]), int.Parse(xy2[1])));
                        }
                    }
                    else
                    {
                        length = 0;
                    }

                    // если текущий путь меньше текущего минимального сохраняем ответ
                    if (length < minAnswer)
                    {
                        minAnswer = length;
                        pathAnswer = item;
                    }
                }

                Console.WriteLine("\nОптимальный маршрут: " + pathAnswer + "\nДлина: " + minAnswer);
            }

            Console.ReadKey();
        }
    }
}

/*
 *  A-------B 
 *  |       |
 *  D-------C
 *  
 *  A--E----B 
 *  |   \   |
 *  D----F--C
 *  
 *  короткий путь от A до C 
 *  AE - EF - FC
 */