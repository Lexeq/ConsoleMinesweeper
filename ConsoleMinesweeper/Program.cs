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
                            cursorLeft -= 1;
                        }

                        break;

                    case UpKey:
                        if (cursorTop != 0)
                        {
                            cursorTop -= 1;
                        }
                        break;

                    case RightKey:
                        if (cursorLeft != _game.Width - 1)
                        {
                            cursorLeft += 1;
                        }
                        break;

                    case DownKey:
                        if (cursorTop != _game.Height - 1)
                        {
                            cursorTop += 1;
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
