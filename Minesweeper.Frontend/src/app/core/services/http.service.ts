import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, Observable } from 'rxjs';
import { ErrorResponse, GameInfoResponse } from '../models/Responses';
import { GameTurnRequest, NewGameRequest } from '../models/Requests';

@Injectable({
  providedIn: 'root'
})
export class HttpService {
  private newGameUrl = 'https://localhost:7007/api/new'
  private gameTurnUrl = 'https://localhost:7007/api/turn'
  private boardDataSubject = new BehaviorSubject<GameInfoResponse | null>(null);
  boardData$ : Observable<GameInfoResponse | null> = this.boardDataSubject.asObservable();

  constructor(private http: HttpClient) { }

  generateBoard(data: NewGameRequest): void{
    this.http.post<GameInfoResponse>(this.newGameUrl, data).pipe(
      catchError(error => {
        console.log('generateBoard');
        this.boardDataSubject.next(null);
        return [];
      })
    ).subscribe(response => this.boardDataSubject.next(response));
  }

  makeTurn(data: GameTurnRequest): void{
    this.http.post<GameInfoResponse>(this.gameTurnUrl, data).pipe(
      catchError(error => {
        console.log('makeTurn');
        this.boardDataSubject.next(null);
        return [];
      })
    ).subscribe(response => this.boardDataSubject.next(response));
  }

  private handleError(error: any): Observable<ErrorResponse> {
    console.error('Ошибка при запросе:', error);
    return new Observable<ErrorResponse>((observer) => {
      observer.next({ error: 'Произошла ошибка при обработке запроса' });
      observer.complete();
    });
  }
}
