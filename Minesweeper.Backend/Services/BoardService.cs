using Minesweeper.Models;
using Minesweeper.Models.Enum;

namespace Minesweeper.Services
{
    public class BoardService : IBoardService
    {
        public Board CreateBoard(int height, int width, int countMines)
        {
            if (height < 2 || height > 30) throw new ArgumentException($"Height должно быть в пределах от 2 до 30");

            if (width < 2 || width > 30) throw new ArgumentException($"Width должно быть в пределах от 2 до 30");

            var totalCellsCount = height * width;
            if (countMines > totalCellsCount - 1 || countMines < 1) throw new ArgumentException($"CountMines должно быть в пределах от 1 до {totalCellsCount - 1}");

            var board = new Board(height, width, countMines);
            var cells = board.Cells;

            var minesPositions = PutMines(cells, countMines);

            FillCellValues(cells, minesPositions);

            return board;
        }

        private static IEnumerable<(int y, int x)> PutMines(Cell[][] cells, int countMines)
        {
            int height = cells.Length, width = cells[0].Length;

            return height * width / 2 > countMines
                ? PutMinesByLoop(cells, countMines)
                : PutMinesByList(cells, countMines);
        }

        private static IEnumerable<(int y, int x)> PutMinesByLoop(Cell[][] cells, int countMines)
        {
            var minesPositions = new List<(int y, int x)>(countMines);
            int height = cells.Length, width = cells[0].Length;
            for (int i = 0; i < countMines; i++)
            {
                int y = Random.Shared.Next(height), x = Random.Shared.Next(width);

                while(cells[y][x].Value == -1)
                {
                    y = Random.Shared.Next(height);
                    x = Random.Shared.Next(width);
                }

                cells[y][x].Value = -1;
                minesPositions.Add((y, x));
            }

            return minesPositions;
        }

        private static IEnumerable<(int y, int x)> PutMinesByList(Cell[][] cells, int countMines)
        {
            int height = cells.Length, width = cells[0].Length;

            var minesPositions = Enumerable.Range(0, height)
                .SelectMany(y => Enumerable.Range(0, width), (y, x) => (y, x))
                .OrderBy(it => Random.Shared.Next())
                .Take(countMines)
                .ToList();

            foreach (var (y, x) in minesPositions)
            {
                cells[y][x].Value = -1;
            }

            return minesPositions;
        }

        private static void FillCellValues(Cell[][] cells, IEnumerable<(int y, int x)> minesPositions)
        {
            int height = cells.Length, width = cells[0].Length;

            foreach (var (y, x) in minesPositions)
            {
                IEnumerable<(int y, int x)> adjacentCells = GetAdjacentCells(cells, y, x, cell => cell.Value != -1);

                foreach (var c in adjacentCells)
                {
                    cells[c.y][c.x].Value++;
                }
            }
        }

        private static IEnumerable<(int y, int x)> GetAdjacentCells(Cell[][] cells, int y, int x, Func<Cell, bool> condition)
        {
            int height = cells.Length, width = cells[0].Length;

            return Enumerable.Range(-1, 3)
                .SelectMany(dy => Enumerable.Range(-1, 3), (dy, dx) => (y: y + dy, x: x + dx))
                .Where(it => it.y >= 0 && it.y < height && it.x >= 0 && it.x < width)
                .Where(it => condition(cells[it.y][it.x]));
        }

        public GameStatus OpenCell(Board board, int col, int row)
        {
            if (row < 0 || row > board.Height - 1) throw new ArgumentException("Выбранная строка находится за пределами доски");

            if (col < 0 || col > board.Width - 1) throw new ArgumentException("Выбранный столбец находится за пределами доски");

            if (board.Cells[row][col].View != ' ') throw new ArgumentException("Ячейка уже была проверена");

            if (board.Cells[row][col].Value == -1)
            {
                board.Cells[row][col].View = 'X';
                return GameStatus.Lose;
            }

            if (board.Cells[row][col].Value != 0)
            {
                board.Cells[row][col].View = (char)('0' + board.Cells[row][col].Value);
            }
            else
            {
                var emptyCellsPositions = new Queue<(int y, int x)>();
                emptyCellsPositions.Enqueue((row, col));

                while (emptyCellsPositions.Count > 0)
                {
                    var pos = emptyCellsPositions.Dequeue();
                    board.Cells[pos.y][pos.x].View = (char)('0' + board.Cells[pos.y][pos.x].Value);

                    if (board.Cells[pos.y][pos.x].Value != 0) continue;

                    var adjacentCells = GetAdjacentCells(board.Cells, pos.y, pos.x, cell => cell.View == ' ');

                    foreach (var c in adjacentCells)
                    {
                        emptyCellsPositions.Enqueue(c);
                    }
                }
            }

            var status = !RemainOnlyMines(board.Cells)
                ? GameStatus.NotOver
                : GameStatus.Win;

            if (status == GameStatus.Win) { OpenMines(board.Cells); }

            return status;
        }

        private static bool RemainOnlyMines(Cell[][] cells) => cells.SelectMany(r => r).Where(c => c.View == ' ').All(c => c.Value == -1);
        

        private static void OpenMines(Cell[][] cells)
        {
            int height = cells.Length, width = cells[0].Length;

            var minesPositions = Enumerable.Range(0, height)
                .SelectMany(y => Enumerable.Range(0, width), (y, x) => (y, x))
                .Where(c => cells[c.y][c.x].Value == -1);

            foreach (var c in minesPositions)
            {
                cells[c.y][c.x].View = 'M';
            }
        }
    }
}
