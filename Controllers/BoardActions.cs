using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Minesweeper.Models;
using Minesweeper.Models.Enum;
using Minesweeper.Services;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Minesweeper.Controllers
{
    [ApiController]
    [Route("api/")]
    public class BoardActions : ControllerBase
    {
        private static Random _random = new();

        private readonly IMemoryCache _cache;
        private readonly IBoardService _boardService;

        public BoardActions(IMemoryCache cache, IBoardService boardService)
        {
            _cache = cache;
            _boardService = boardService;
        }

        [HttpPost("new")]
        public IActionResult New(NewGameRequest request)
        {
            try
            {
                var gameId = Guid.NewGuid();

                var board = _boardService.CreateBoard(request.Height, request.Width, request.MinesCount);

                _cache.Set(gameId, board, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                GameInfoResponse response = CreateResponse(gameId, board);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        private static GameInfoResponse CreateResponse(Guid gameId, Board board)
        {
            return new GameInfoResponse(
                gameId,
                board.Width,
                board.Height,
                board.MinesCount,
                board.Completed,
                board.Cells.Select(r => r.Select(c => c.View).ToArray()).ToArray());
        }

        [HttpPost("turn")]
        public IActionResult Turn(GameTurnRequest request)
        {
            try
            {
                var gameId = request.GameId;
                var board = _cache.Get<Board>(gameId) ?? throw new Exception($"Игра с Id={request.GameId} не существует");
                if(board.Completed) throw new Exception($"Игры с Id={request.GameId} завершена");

                var status = _boardService.OpenCell(board, request.Col, request.Row);
                board.Completed = status != GameStatus.NotOver;

                _cache.Set(gameId, board, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

                var response = CreateResponse(gameId, board);

                return Ok(response);
            }
            catch(Exception ex) 
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }


        public record NewGameRequest(int Width, int Height, [property: JsonPropertyName("mines_count")] int MinesCount);
        public record GameTurnRequest([property: JsonPropertyName("game_id")] Guid GameId, int Col, int Row);
        public record GameInfoResponse([property: JsonPropertyName("game_id")] Guid GameId, int Width, int Height, [property: JsonPropertyName("mines_count")] int MinesCount, bool Completed, char[][] Field);
        public record GameInfoResponse1(Guid GameId, int Width, int Height, int MinesCount, bool Completed, string[] Field);
        public record ErrorResponse(string Error);
    }
}
