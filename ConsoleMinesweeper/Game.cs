using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMinesweeper
{
    public class Game
    {
        private int _needsOpen;

        /// <summary>
        /// 0 - свободная клетка
        /// 1 - 8 - количество мин вокруг клетки
        /// 9 - мина
        /// </summary>
        private int[,] gameField;

        private CellState[,] cellStates;

        private Random rnd = new Random();

        public int Width { get; private set; }

        public int Height { get; private set; }

        public int Mines { get; private set; }

        public int NeedsOpen
        {
            get { return _needsOpen; }
            private set
            {
                _needsOpen = value;
                if (value == 0)
                    State = GameState.Win;
            }
        }

        public int Marked { get; private set; }

        public GameState State { get; private set; }

        public Game()
            : this(9, 9, 10)
        { }

        public Game(int width, int height, int mines)
        {
            if (width < 2)
                throw new ArgumentException("width");
            if (height < 2)
                throw new ArgumentException("heigth");
            if (mines < 1 || mines > width * height - 2)
                throw new ArgumentException("mines");

            Width = width;
            Height = height;
            Mines = mines;
            NeedsOpen = Width * Height - Mines;
            gameField = new int[Width, Height];
            cellStates = new CellState[Width, Height];
            State = GameState.Ready;
        }

        private void ForEachArround(int x, int y, Action<int, int> action)
        {
            if (x - 1 >= 0)
                action(x - 1, y);
            if (x + 1 < Width)
                action(x + 1, y);
            if (y - 1 >= 0)
                action(x, y - 1);
            if (y + 1 < Height)
                action(x, y + 1);

            if (x - 1 >= 0 && y - 1 >= 0)
                action(x - 1, y - 1);
            if (x - 1 >= 0 && y + 1 < Height)
                action(x - 1, y + 1);
            if (x + 1 < Width && y + 1 < Height)
                action(x + 1, y + 1);
            if (x + 1 < Width && y - 1 >= 0)
                action(x + 1, y - 1);
        }

        private void SetMinesCount()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (gameField[x, y] == 9)
                        continue;

                    int count = 0;
                    ForEachArround(x, y, (a, b) => {
                        if (gameField[a, b] == 9)
                            count++;
                    });
                    gameField[x, y] = count;
                }
            }
        }

        private void SetMines(int x, int y)
        {
            List<Point> pts = new List<Point>(Width * Height - 1);

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (i != x || j != y)
                    {
                        pts.Add(new Point(i, j));
                    }
                }
            }

            Random rnd = new Random();
            var mines = pts.OrderBy(m => rnd.Next()).Take(Mines);
            foreach (var item in mines)
            {
                gameField[item.X, item.Y] = 9;
            }
            SetMinesCount();
        }

        private int CountMarkedAround(int x, int y)
        {
            int count = 0;
            ForEachArround(x, y, (a, b) =>
                {
                    if (cellStates[a, b] == CellState.Marked)
                        count++;
                });
            return count;
        }

        private void OpenCells(int x, int y, bool[,] wasChecked)
        {
            wasChecked[x, y] = true;
            if (gameField[x, y] == 0)
            {
                ForEachArround(x, y, (a, b) =>
                    {
                        if (cellStates[a, b] == CellState.Closed && !wasChecked[a, b])
                            OpenCells(a, b, wasChecked);
                    });
            }
            cellStates[x, y] = CellState.Opened;
            NeedsOpen--;
        }

        private void Open(int x, int y)
        {
            if (gameField[x, y] == 9)
            {
                State = GameState.Lose;
                return;
            }

            if (cellStates[x, y] == CellState.Opened)
            {
                if (CountMarkedAround(x, y) == gameField[x, y])
                {
                    ForEachArround(x, y, (a, b) =>
                    {
                        if (cellStates[a, b] == CellState.Closed)
                            Open(a, b);
                    });
                }
            }
            else
            {
                bool[,] mask = new bool[Width, Height];
                OpenCells(x, y, mask);
            }
        }

        public void OpenCell(int x, int y)
        {
            if (State == GameState.Ready)
            {
                SetMines(x, y);
                State = GameState.Running;
            }

            if (State != GameState.Running)
                throw new InvalidOperationException("Game over");

            Open(x, y);
        }

        public void Mark(int x, int y)
        {
            if (cellStates[x, y] == CellState.Marked)
            {
                cellStates[x, y] = CellState.Closed;
                Marked--;
            }
            else if (cellStates[x, y] == CellState.Closed)
            {
                cellStates[x, y] = CellState.Marked;
                Marked++;
            }
        }

        public CellInfo this[int x, int y]
        {
            get
            {
                var state = cellStates[x, y];
                var mines = state == CellState.Opened ? gameField[x, y] : -1;
                return new CellInfo(state, mines);
            }
        }
    }
}
