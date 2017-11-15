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

        private static Game _game;

        private static int cursorTop = 0;
        private static int cursorLeft = 0;



        public static void Main(string[] args)
        {
            do
            {
                _game = new Game(15, 15, 15);
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
            Console.WriteLine("Всего мин: " + _game.Mines);
            Console.WriteLine("Осталось открыть ячеек: " + _game.NeedsOpen);
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }

        public static bool Start()
        {
            Draw();
            while (true)
            {

                if (_game.State == GameState.Lose)
                {
                    Console.Clear();
                    Console.WriteLine("Бах! Вы проиграли. Нажмите \"y\" чтобы сыграть еще раз или любую клавишу для выхода.");
                    var answer = Console.ReadKey();
                    return answer.Key == ConsoleKey.Y;
                }


                if (_game.State == GameState.Win)
                {
                    Console.Clear();
                    Console.WriteLine("Победа! Нажмите \"y\" чтобы сыграть еще раз или любую клавишу для выхода.");
                    var answer = Console.ReadKey();
                    return answer.Key == ConsoleKey.Y;
                }

                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Escape:
                        return false;

                    case ConsoleKey.LeftArrow:
                        if (cursorLeft != 0)
                        {
                            cursorLeft -= 1;
                            //             Console.SetCursorPosition(cursorLeft, cursorTop);
                        }

                        break;

                    case ConsoleKey.UpArrow:
                        if (cursorTop != 0)
                        {
                            cursorTop -= 1;
                            //        Console.SetCursorPosition(cursorLeft, cursorTop);
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (cursorLeft != _game.Width - 1)
                        {
                            cursorLeft += 1;
                            //         Console.SetCursorPosition(cursorLeft, cursorTop);
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (cursorTop != _game.Height - 1)
                        {
                            cursorTop += 1;
                            //         Console.SetCursorPosition(cursorLeft, cursorTop);
                        }
                        break;

                    case ConsoleKey.Enter:
                        if (_game[cursorLeft, cursorTop].State != CellState.Marked)
                        {
                            _game.OpenCell(cursorLeft, cursorTop);
                            Draw();
                        }
                        break;

                    case ConsoleKey.W:
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
