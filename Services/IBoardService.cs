using Minesweeper.Models;
using Minesweeper.Models.Enum;

namespace Minesweeper.Services
{
    public interface IBoardService
    {
        public Board CreateBoard(int height, int width, int countMines);

        public GameStatus OpenCell(Board board, int col, int row);
    }
}
