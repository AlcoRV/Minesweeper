import { UUID } from "crypto";

export interface GameInfoResponse{
    game_id: UUID,
    width: number,
    height: number,
    mines_count: number,
    completed: boolean,
    field: string[][]
}

export interface ErrorResponse{
    error: string
}