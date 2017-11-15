using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMinesweeper
{
    public enum GameState { Ready, Running, Win, Lose }

    public enum CellState { Closed, Opened, Marked }
}
