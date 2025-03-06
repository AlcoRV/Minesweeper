import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CreationFormComponent } from './features/creation.form/creation.form.component';
import { BoardComponent } from './features/board/board.component';
import { ChatComponent } from "./features/chat/chat.component";

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CreationFormComponent, BoardComponent, ChatComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Minesweeper.Frontend';
}
