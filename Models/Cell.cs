namespace Minesweeper.Models
{
    public struct Cell(int value)
    {
        public int Value = value;
        public char View = ' ';
    }
}
