using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMinesweeper
{
    public struct CellInfo
    {
        public readonly CellState State;

        public readonly int MinesAround;

        public CellInfo(CellState state, int mines)
        {
            State = state;
            MinesAround = mines;
        }
    }
}
