namespace Minesweeper.Models
{
    public class Board
    {
        public int MinesCount { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public Cell[][] Cells { get; set; }
        public bool Completed { get; set; }

        public Board(int height, int width, int minesCount)
        {
            Height = height;
            Width = width;
            MinesCount = minesCount;

            Cells = Enumerable.Range(0, height)
                .Select(_ => Enumerable.Range(0, width)
                .Select(_ => new Cell(0)).ToArray())
                .ToArray();
        }
    }
}
