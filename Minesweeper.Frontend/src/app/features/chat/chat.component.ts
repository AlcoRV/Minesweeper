import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import * as signalR from '@microsoft/signalr'

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.css'
})
export class ChatComponent implements OnInit, OnDestroy {
  private hubConnection!: signalR.HubConnection;
  messages: { user: string; text: string }[] = [];
  user: string = 'User' + Math.floor(Math.random() * 1000);
  messageText: string = '';

  ngOnInit() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7007/chatHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connected to SignalR'))
      .catch(err => console.error('Error connecting to SignalR:', err));

    this.hubConnection.on('ReceiveMessage', (user: string, message: string) => {
      debugger;
      this.messages.push({ user, text: message });
    });
  }

  sendMessage() {
    if (this.messageText.trim()) {
      this.hubConnection.invoke('SendMessage', this.user, this.messageText)
        .catch(err => console.error(err));
      this.messageText = '';
    }
  }

  ngOnDestroy() {
    this.hubConnection.stop();
  }
}
