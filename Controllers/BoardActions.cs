using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Minesweeper.Models;
using Minesweeper.Models.Enum;
using Minesweeper.Services;

namespace Minesweeper.Controllers
{
    [ApiController]
    [Route("api/")]
    public class BoardActions : ControllerBase
    {
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
            var gameId = Guid.NewGuid();

            var board = _boardService.CreateBoard(request.Height, request.Width, request.MinesCount);

            _cache.Set(gameId, board, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

            return Ok(CreateResponse(gameId, board));
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
            var gameId = request.GameId;

            if (!_cache.TryGetValue(request.GameId, out Board? board) || board is null)
                return NotFound(new ErrorResponse($"Игра с Id={request.GameId} не найдена"));

            if(board.Completed) return BadRequest(new ErrorResponse("Игра завершена"));

            var status = _boardService.OpenCell(board, request.Col, request.Row);
            board.Completed = status != GameStatus.NotOver;

            _cache.Set(gameId, board, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

            return Ok(CreateResponse(gameId, board));
        }
    }
}
