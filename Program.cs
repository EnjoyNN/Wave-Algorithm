using System;
using GetRequestNETStandart;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RealizeWaveAlgorithm
{
	class Program
	{
		static void Main(string[] args)
		{
			Program p = new Program();
			p.Start();
		}

		//-2 - значение стены
		//-1 не пройденной клетки

		//0 - проходимое в массиве, 1 - стена

		private void Start()
		{
			ReadMap();
			DrawMap();
		}

		int[,] Map;
		int MapWidht;
		int MapHeight;

		//int[,] WayMap;
		public void ReadMap()
		{
			MapWidht = 16;
			MapHeight = 9;
			Map = new int[,]{
				{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
				{1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
				{1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1},
				{1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1},
				{1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1},
				{1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
				{1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
				{1,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
				{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}};
			//WayMap = new int[10, 10];
		}


		public void DrawMap()
		{
			for (int y = 0; y < MapHeight; y++)
			{
				Console.WriteLine();
				for (int x = 0; x < MapWidht; x++)
					if (Map[y, x] == 1)
						Console.Write("+");
					else
						Console.Write(" ");
			}
			//Console.ReadLine();
			FindWave(1, 1, 3, 4);
		}

		public class Point
		{
			public int x;
			public int y;
			public Point(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		List<List<Point>> paths = new List<List<Point>>();

		public int[,] FindWave(int startX, int startY, int targetX, int targetY)
		{
			bool add = true;
			int[,] cMap = new int[MapHeight, MapWidht];
			int x, y, step = 0;
			for (y = 0; y < MapHeight; y++)
				for (x = 0; x < MapWidht; x++)
				{
					if (Map[y, x] == 1)
						cMap[y, x] = -2;//индикатор стены
					else
						cMap[y, x] = -1;//индикатор еще не ступали сюда
				}
			cMap[targetY, targetX] = 0;//Начинаем с финиша
			while (add == true)
			{
				add = false;
				for (y = 0; y < MapWidht; y++)
					for (x = 0; x < MapHeight; x++)
					{
						if (cMap[x, y] == step)
						{
							//Ставим значение шага+1 в соседние ячейки (если они проходимы)
							//шаг вверх
							if (y - 1 >= 0 && cMap[x - 1, y] != -2 && cMap[x - 1, y] == -1)
							{
								cMap[x - 1, y] = step + 1;
								addPathsToList(targetX, targetY, x, y, x - 1, y);
							}

							//шаг влево
							if (x - 1 >= 0 && cMap[x, y - 1] != -2 && cMap[x, y - 1] == -1)
							{
								cMap[x, y - 1] = step + 1;
								addPathsToList(targetX, targetY, x, y, x, y - 1);
							}

							//шаг вниз
							if (y + 1 < MapWidht && cMap[x + 1, y] != -2 && cMap[x + 1, y] == -1)
							{
								cMap[x + 1, y] = step + 1;
								addPathsToList(targetX, targetY, x, y, x + 1, y);
							}

							//шаг вправо
							if (x + 1 < MapHeight && cMap[x, y + 1] != -2 && cMap[x, y + 1] == -1)
							{
								cMap[x, y + 1] = step + 1;
								addPathsToList(targetX, targetY, x, y, x, y + 1);
							}
						}
					}
				step++;
				add = true;
				if (cMap[startY, startX] != -1)
				{//решение найдено

					//ищем пути до цели из тех путей, что собрали.
					List<List<Point>> availabePaths = new List<List<Point>>();
					for (int i = 0; i < paths.Count; i++)
					{
						Point end = new Point(startY, startX);
						if (paths[i][paths[i].Count - 1].x == end.x &&
						paths[i][paths[i].Count - 1].y == end.y)
						{
							availabePaths.Add(paths[i]);
						}
					}

					//ищем кратчайший(е) путь из тех путей, что ведут к цели.
					int shortestSteps = availabePaths[0].Count;
					for (int i = 0; i < availabePaths.Count; i++)
					{
						if (availabePaths[i].Count < shortestSteps)
							shortestSteps = availabePaths[i].Count;
					}
					List<List<Point>> shortestPaths = new List<List<Point>>();
					shortestPaths.AddRange(availabePaths.Where(path => path.Count == shortestSteps).ToList<List<Point>>());

					//если кратчайших путей до цели несколько с одинаковой длиной, то рандомно выбирает какой-то один
					List<Point> shortestPath = new List<Point>();
					Random rnd = new Random();
					int randomNumber = rnd.Next(1, shortestPaths.Count + 1);
					shortestPath.AddRange(shortestPaths[randomNumber - 1]);

					//переворачиваем путь, так как он от финиша и до начала
					shortestPath.Reverse();

					add = false;
				}
				if (step > MapWidht * MapHeight)
				{//решение не найдено
					add = false;
				}
			}
			//Отрисовываем карты
			for (y = 0; y < MapHeight; y++)
			{
				Console.WriteLine();
				for (x = 0; x < MapWidht; x++)
					if (cMap[y, x] == -1)
						Console.Write(" ");
					else
					if (cMap[y, x] == -2)
						Console.Write("#");
					else
					if (y == startY && x == startX)
						Console.Write("S");
					else
					if (y == targetY && x == targetX)
						Console.Write("F");
					else
					if (cMap[y, x] > -1)
						Console.Write("{0}", cMap[y, x]);

			}
			return cMap;
		}

		private void addPathsToList(int targetX, int targetY, int x, int y, int xNow, int yNow)
		{
			//меняем x и y (вместе с вычетаниями), отсчет с 0 если что

			//а здесь выстраиваем начало, если еще точек нет совсем. если стартовая точка равна старту (он же по идее не может прийти к старту откуда-то снова?) тоже меняем координаты тут (надеюсь не ошибся).
			if (y == targetX && x == targetY)
			{
				paths.Add(new List<Point> {
										new Point(y, x),
										new Point(yNow, xNow)
									});
			}

			//если находим уже путь, где в конце та точка, с который мы переходим сейчас куда-то, то добавляем к этому пути теперь точку с нашим переходом. Если таких путей будет несколько (то есть пришли к одной точке разными путями), то добавляем ко всем.
			for (int k = 0; k < paths.Count; k++)
			{
				Point start = new Point(y, x);
				if (paths[k][paths[k].Count - 1].x == start.x &&
				paths[k][paths[k].Count - 1].y == start.y)
				{
					List<Point> addPoints = new List<Point>();
					addPoints.AddRange(paths[k]);
					addPoints.Add(new Point(yNow, xNow));
					paths.Add(addPoints);
				}
			}
		}

		public void findMove(int[,] cMap, int targetX, int targetY, int startX, int startY)
		{

		}
	}
}