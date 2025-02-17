using System.Text.Json.Serialization;

namespace Minesweeper.Models
{
    public record NewGameRequest(int Width, int Height, [property: JsonPropertyName("mines_count")] int MinesCount);
    public record GameTurnRequest([property: JsonPropertyName("game_id")] Guid GameId, int Col, int Row);
    public record GameInfoResponse([property: JsonPropertyName("game_id")] Guid GameId, int Width, int Height, [property: JsonPropertyName("mines_count")] int MinesCount, bool Completed, char[][] Field);
    public record ErrorResponse(string Error);
}
