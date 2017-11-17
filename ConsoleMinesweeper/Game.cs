using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMinesweeper
{
    public class Game
    {
        public int _needsOpen;
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

        /// <summary>
        /// 0 - свободная клетка
        /// 1 - 8 - количество мин вокруг клетки
        /// 9 - мина
        /// </summary>
        private int[,] gameField;

        private CellState[,] cellStates;

        private Random rnd = new Random();

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

        private void SetMinesCount()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (gameField[x, y] == 9)
                        continue;

                    int count = 0;

                    bool up = x - 1 >= 0;
                    bool down = x + 1 < Width;
                    bool left = y - 1 >= 0;
                    bool right = y + 1 < Height;

                    if (up && gameField[x - 1, y] == 9)
                        count++;

                    if (down && gameField[x + 1, y] == 9)
                        count++;

                    if (left && gameField[x, y - 1] == 9)
                        count++;

                    if (right && gameField[x, y + 1] == 9)
                        count++;

                    if (up && left && gameField[x - 1, y - 1] == 9)
                        count++;

                    if (up && right && gameField[x - 1, y + 1] == 9)
                        count++;

                    if (down && right && gameField[x + 1, y + 1] == 9)
                        count++;

                    if (down && left && gameField[x + 1, y - 1] == 9)
                        count++;

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
            if (x - 1 >= 0 && cellStates[x - 1, y] == CellState.Marked)
                count++;
            if (x + 1 < Width && cellStates[x + 1, y] == CellState.Marked)
                count++;
            if (y - 1 >= 0 && cellStates[x, y - 1] == CellState.Marked)
                count++;
            if (y + 1 < Height && cellStates[x, y + 1] == CellState.Marked)
                count++;

            if (x - 1 >= 0 && y - 1 >= 0 && cellStates[x - 1, y - 1] == CellState.Marked)
                count++;
            if (x + 1 < Width && y + 1 < Height && cellStates[x + 1, y + 1] == CellState.Marked)
                count++;
            if (x - 1 >= 0 && y + 1 < Height && cellStates[x - 1, y + 1] == CellState.Marked)
                count++;
            if (x + 1 < Width && y - 1 >= 0 && cellStates[x + 1, y - 1] == CellState.Marked)
                count++;
            return count;
        }

        private void OpenCells(int x, int y, bool[,] wasChecked)
        {
            wasChecked[x, y] = true;
            if (gameField[x, y] == 0)
            {
                if (x + 1 < Width && !wasChecked[x + 1, y] && cellStates[x + 1, y] == 0)
                    OpenCells(x + 1, y, wasChecked);
                if (x - 1 >= 0 && !wasChecked[x - 1, y] && cellStates[x - 1, y] == 0)
                    OpenCells(x - 1, y, wasChecked);
                if (y + 1 < Height && !wasChecked[x, y + 1] && cellStates[x, y + 1] == 0)
                    OpenCells(x, y + 1, wasChecked);
                if (y - 1 >= 0 && !wasChecked[x, y - 1] && cellStates[x, y - 1] == 0)
                    OpenCells(x, y - 1, wasChecked);

                if (x + 1 < Width && y - 1 >= 0 && !wasChecked[x + 1, y - 1] && cellStates[x + 1, y - 1] == 0)
                    OpenCells(x + 1, y - 1, wasChecked);
                if (x + 1 < Width && y + 1 < Height && !wasChecked[x + 1, y + 1] && cellStates[x + 1, y + 1] == 0)
                    OpenCells(x + 1, y + 1, wasChecked);
                if (x - 1 >= 0 && y - 1 >= 0 && !wasChecked[x - 1, y - 1] && cellStates[x - 1, y - 1] == 0)
                    OpenCells(x - 1, y - 1, wasChecked);
                if (x - 1 >= 0 && y + 1 < Height && !wasChecked[x - 1, y + 1] && cellStates[x - 1, y + 1] == 0)
                    OpenCells(x - 1, y + 1, wasChecked);
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
                    if (x - 1 >= 0 && cellStates[x - 1, y] == CellState.Closed)
                        Open(x - 1, y);
                    if (x + 1 < Width && cellStates[x + 1, y] == CellState.Closed)
                        Open(x + 1, y);
                    if (y - 1 >= 0 && cellStates[x, y - 1] == CellState.Closed)
                        Open(x, y - 1);
                    if (y + 1 < Height && cellStates[x, y + 1] == CellState.Closed)
                        Open(x, y + 1);

                    if (x - 1 >= 0 && y - 1 >= 0 && cellStates[x - 1, y - 1] == CellState.Closed)
                        Open(x - 1, y - 1);
                    if (x + 1 < Width && y + 1 < Height && cellStates[x + 1, y + 1] == CellState.Closed)
                        Open(x + 1, y + 1);
                    if (x - 1 >= 0 && y + 1 < Height && cellStates[x - 1, y + 1] == CellState.Closed)
                        Open(x - 1, y + 1);
                    if (x + 1 < Width && y - 1 >= 0 && cellStates[x + 1, y - 1] == CellState.Closed)
                        Open(x + 1, y - 1);
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
            if(cellStates[x,y] == CellState.Marked)
            {
                cellStates[x, y] = CellState.Closed;
                Marked--;
            }
            else if(cellStates[x,y] == CellState.Closed)
            {
                cellStates[x,y] = CellState.Marked;
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
