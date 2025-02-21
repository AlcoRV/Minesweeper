import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpService } from '../../core/services/http.service';
import { GameInfoResponse } from '../../core/models/Responses';
import { CommonModule } from '@angular/common';
import { GameTurnRequest } from '../../core/models/Requests';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-board',
  imports: [CommonModule],
  templateUrl: './board.component.html',
  styleUrl: './board.component.css'
})
export class BoardComponent implements OnInit, OnDestroy {
  boardData: GameInfoResponse | null = null;
  private boardDataSubscription: Subscription | null = null;

  constructor(private formDataService: HttpService) {}

  ngOnInit(): void {
    this.boardDataSubscription = this.formDataService.boardData$.subscribe((data) => {
      this.boardData = data;
      console.log('Board data loaded', data);
    });
  }

  makeTurn(row: number, col: number): void {
    if (!this.boardData) {
      console.error('Board data is not loaded yet!');
      return;
    }
    console.log('Making turn', row, col);

    const data: GameTurnRequest = {
      game_id: this.boardData.game_id,
      col: col,
      row: row
    };

    this.formDataService.makeTurn(data);
  }

  ngOnDestroy(): void {
    if (this.boardDataSubscription) {
      this.boardDataSubscription.unsubscribe();
    }
  }
}
