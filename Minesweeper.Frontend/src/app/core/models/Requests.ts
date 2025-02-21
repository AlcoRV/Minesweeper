import { UUID } from "crypto"

export interface NewGameRequest{
    width: number,
    height: number,
    mines_count: number
}

export interface GameTurnRequest{
    game_id: UUID,
    col: number,
    row: number
}