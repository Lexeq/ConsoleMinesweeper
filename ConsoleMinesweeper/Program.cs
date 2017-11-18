using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMinesweeper
{
    class Program
    {
        private static char EmptyCell = ' ';
        private static char MarkedCell = '*';
        private static char ClosedCell = '#';

        //Key settings
        private const ConsoleKey MarkKey = ConsoleKey.W;
        private const ConsoleKey OpenKey = ConsoleKey.Enter;
        private const ConsoleKey LeftKey = ConsoleKey.LeftArrow;
        private const ConsoleKey UpKey = ConsoleKey.UpArrow;
        private const ConsoleKey RightKey = ConsoleKey.RightArrow;
        private const ConsoleKey DownKey = ConsoleKey.DownArrow;
        private const ConsoleKey YesKey = ConsoleKey.Y;

        private static Game _game;

        private static int cursorTop = 0;
        private static int cursorLeft = 0;

        public static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Нажмите любую клавишу для начала игры...");
                Console.ReadKey(true);
            } while (Start());
        }

        private static void Draw()
        {
            Console.Clear();
            for (int y = 0; y < _game.Height; y++, Console.WriteLine())
            {
                for (int x = 0; x < _game.Width; x++)
                {

                    if (_game[x, y].State == CellState.Opened)
                    {
                        if (_game[x, y].MinesAround == 0)
                            Console.Write(EmptyCell);
                        else
                            Console.Write(_game[x, y].MinesAround);
                    }
                    else if (_game[x, y].State == CellState.Marked)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(MarkedCell);
                        Console.ResetColor();
                    }
                    else Console.Write(ClosedCell);
                }
            }
            Console.WriteLine("W - пометить клетку, Enter - открыть");
            Console.WriteLine("Всего мин: {0} Отмечено {1}", _game.Mines, _game.Marked);
            Console.WriteLine("Осталось открыть ячеек: " + _game.NeedsOpen);
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }

        private static Game InitGame()
        {
            Console.Clear();

            Console.WriteLine("1 - Легкий");
            Console.WriteLine("2 - Средний");
            Console.WriteLine("3 - Тяжелый");
            Console.WriteLine("5 - Настроить");
            Console.Write("0 - Выход");

            while (true)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
                    return new Game(9, 9, 10);
                else if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2)
                    return new Game(16, 16, 40);
                else if (key == ConsoleKey.D3 || key == ConsoleKey.NumPad3)
                    return new Game(30, 16, 99);
                else if (key == ConsoleKey.D5 || key == ConsoleKey.NumPad5)
                {
                    var param = GetParameters();
                    return new Game(param.Item1, param.Item2, param.Item3);
                }
                else if (key == ConsoleKey.D0 || key == ConsoleKey.NumPad0)
                    Environment.Exit(0);
            }
        }

        private static Tuple<int, int, int> GetParameters()
        {
            int w, h, m;
            int wMax = 50;
            int hMax = 50;

            while (true)
            {
                Console.Clear();

                Console.Write("Введите ширину ({0} - {1}): ", 2, wMax);
                bool wFlag = int.TryParse(Console.ReadLine(), out w) && w > 1 && w <= wMax;

                Console.Write("Введите высоту ({0} - {1}): ", 2, hMax);
                bool hFlag = int.TryParse(Console.ReadLine(), out h) && h > 1 && h <= hMax;

                Console.Write("Введите количество мин ({0} - {1}): ", 1, w * h - 2);
                bool mFlag = int.TryParse(Console.ReadLine(), out m) && m > 0 && m <= w * h - 2;

                if (wFlag && hFlag && mFlag)
                    return new Tuple<int, int, int>(w, h, m);
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Недопустимые параметры!");
                    Console.ResetColor();
                    Console.ReadKey(true);
                }
            }
        }

        public static bool Start()
        {
            _game = InitGame();
            Draw();
            while (true)
            {

                if (_game.State == GameState.Lose)
                {
                    Console.Clear();
                    Console.WriteLine("Бах! Вы проиграли. Нажмите \"y\" чтобы сыграть еще раз или любую клавишу для выхода.");
                    var answer = Console.ReadKey(true);
                    return answer.Key == YesKey;
                }


                if (_game.State == GameState.Win)
                {
                    Console.Clear();
                    Console.WriteLine("Победа! Нажмите \"y\" чтобы сыграть еще раз или любую клавишу для выхода.");
                    var answer = Console.ReadKey(true);
                    return answer.Key == YesKey;
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Escape:
                        return false;

                    case LeftKey:
                        if (cursorLeft != 0)
                        {
                            cursorLeft--;
                        }

                        break;

                    case UpKey:
                        if (cursorTop != 0)
                        {
                            cursorTop--;
                        }
                        break;

                    case RightKey:
                        if (cursorLeft != _game.Width - 1)
                        {
                            cursorLeft++;
                        }
                        break;

                    case DownKey:
                        if (cursorTop != _game.Height - 1)
                        {
                            cursorTop++;
                        }
                        break;

                    case OpenKey:
                        if (_game[cursorLeft, cursorTop].State != CellState.Marked)
                        {
                            _game.OpenCell(cursorLeft, cursorTop);
                            Draw();
                        }
                        break;

                    case MarkKey:
                        _game.Mark(cursorLeft, cursorTop);
                        Draw();
                        break;

                    default:
                        break;
                }
                Console.SetCursorPosition(cursorLeft, cursorTop);
            }
        }
    }
}
