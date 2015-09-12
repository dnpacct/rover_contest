using System;
using System.Collections.Generic;

namespace Rover
{
    public class State
    {
        public int x;
        public int y;
        public char direction;
    }
    public class Rover
    {
        private State state;
        private Camera camera = null;
        private char[,] field = null;
        public Rover()
        {
            camera = new Camera(15);
            state = new State();
        }
        public void ReadPosition()
        {
            string[] vals = Console.ReadLine().Split(' ', '\t');
            state.x = int.Parse(vals[0]);
            state.y = int.Parse(vals[1]);
            state.direction = vals[2][0];
        }
        public int EstimatePoint(int x, int y)
        {
            switch(field[y, x])
            {
                case '.':
                    return 1;
                case '*':
                    return 100;
                case '0':
                    return 300;
                case '!':
                    return 1000;
            }
            return 10000;
        }
        public char GetDirection(char nextState)
        {
            switch(state.direction)
            {
                case 'U':
                    switch(nextState)
                    {
                        case 'U':
                            return 'F';
                        case 'D':
                            return 'B';
                        case 'L':
                            return 'L';
                        case 'R':
                            return 'R';
                        default:
                            return 'F';
                    }
                case 'D':
                    switch (nextState)
                    {
                        case 'U':
                            return 'B';
                        case 'D':
                            return 'F';
                        case 'L':
                            return 'L';
                        case 'R':
                            return 'R';
                        default:
                            return 'F';
                    }
                case 'L':
                    switch(nextState)
                    {
                        case 'U':
                            return 'R';
                        case 'D':
                            return 'L';
                        case 'L':
                            return 'F';
                        case 'R':
                            return 'B';
                        default:
                            return 'F';
                    }
                case 'R':
                    switch (nextState)
                    {
                        case 'U':
                            return 'L';
                        case 'D':
                            return 'R';
                        case 'L':
                            return 'B';
                        case 'R':
                            return 'F';
                        default:
                            return 'F';
                    }
                default:
                    return 'F';
            }
        }
        public char Decide()
        {
            bool first = true;
            int test = 0;
            int[,] estim = new int[15, 15];
            char[,] res = new char[15, 15]; // вместо пути - направление движения в следующую точку
            string[,] tst = new string[15, 15];// весь путь для тестирования
            bool[,] visited = new bool[15, 15];
            ReadPosition();
            field = camera.View();
            int x = 7; // из центра поля обзора
            int y = 7;
            // алгоритм Дейкстры для текущего видимого поля
            do
            {
                if(!first)
                {
                    //найти следующую точку
                    x = -1;
                    y = -1;
                    test = int.MaxValue;
                    for(int i = 0; i < 15; i++)
                    {
                        for(int j = 0; j < 15; j++)
                        {
                            if(!visited[i, j] && estim[i, j] > 0 && estim[i, j] < test)
                            {
                                y = i;
                                x = j;
                                test = estim[i, j];
                            }
                        }
                    }
                    if (x < 0)
                        break;
                }
                visited[y, x] = true;
                if(x < 14 && !visited[y, x + 1])
                {
                    if (first)
                    {
                        estim[y, x + 1] = EstimatePoint(x + 1, y) +
                                         ((state.direction == 'L') ? EstimatePoint(x, y) : 0);
                        
                        res[y, x + 1] = GetDirection('R');
                        tst[y, x + 1] = "R";
                    }
                    else
                    {
                        test = estim[y, x] + EstimatePoint(x + 1, y);
                        if (test < estim[y, x + 1] || estim[y, x + 1] == 0)
                        {
                            estim[y, x + 1] = test;
                            res[y, x + 1] = res[y, x];
                            tst[y, x + 1] = tst[y, x] + "R";
                        }
                    }
                }
                if (x > 0 && !visited[y, x - 1])
                {
                    if (first)
                    {
                        estim[y, x - 1] = EstimatePoint(x - 1, y) +
                                         ((state.direction == 'R') ? EstimatePoint(x, y) : 0);
                        res[y, x - 1] = GetDirection('L');
                        tst[y, x - 1] = "L";
                    }
                    else
                    {
                        test = estim[y, x] + EstimatePoint(x - 1, y);
                        if (test < estim[y, x - 1] || estim[y, x - 1] == 0)
                        {
                            estim[y, x - 1] = test;
                            res[y, x - 1] = res[y, x];
                            tst[y, x - 1] = tst[y, x] + "L";
                        }
                    }
                }
                if (y < 14 && !visited[y + 1, x])
                {
                    if (first)
                    {
                        estim[y + 1, x] = EstimatePoint(x, y + 1) +
                                         ((state.direction == 'U') ? EstimatePoint(x, y) : 0);
                        res[y + 1, x] = GetDirection('D');
                        tst[y + 1, x] = "D";
                    }
                    else
                    {
                        test = estim[y, x] + EstimatePoint(x, y + 1);
                        if (test < estim[y + 1, x] || estim[y + 1, x] == 0)
                        {
                            estim[y + 1, x] = test;
                            res[y + 1, x] = res[y, x];
                            tst[y + 1, x] = tst[y, x] + "D";
                        }
                    }
                }
                if (y > 0 && !visited[y - 1, x])
                {
                    if (first)
                    {
                        estim[y - 1, x] = EstimatePoint(x, y - 1) +
                                         ((state.direction == 'D') ? EstimatePoint(x, y) : 0);
                        res[y - 1, x] = GetDirection('U');
                        tst[y - 1, x] = "U";
                    }
                    else
                    {
                        test = estim[y, x] + EstimatePoint(x, y - 1);
                        if (test < estim[y - 1, x] || estim[y - 1, x] == 0)
                        {
                            estim[y - 1, x] = test;
                            res[y - 1, x] = res[y, x];
                            tst[y - 1, x] = tst[y, x] + "U";
                        }
                    }
                }
                first = false;
            }
            while(true);
            int x0 = state.x;
            int y0 = state.y;
            //найти на видимо поле, ближайшую к [0, 0]
            if(x0 < -7)
            {
                x0 = -7;
            }
            else if (x0 > 7)
            {
                x0 = 7;
            }
            if(y0 < -7)
            {
                y0 = -7;
            }
            if(y0 > 7)
            {
                y0 = 7;
            }
            //Console.WriteLine(tst[7 + y0, 7 - x0]);
            return res[7 + y0, 7 - x0];
        }
    }
    public class Camera
    {
        public Camera(int scope)
        {
            this.scope = scope;
        }
        private int scope;
        public char[,] View()
        {
            char[,] field = new char[scope, scope];
            for (int i = 0; i < scope; i++)
            {
                string input = Console.ReadLine();
                for(int j = 0; j < input.Length && j < scope; j++)
                {
                    field[i, j] = input[j];
                }
            }
            return field;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {   
            Rover rover = new Rover();
            Console.WriteLine(rover.Decide());
        }
    }
}
