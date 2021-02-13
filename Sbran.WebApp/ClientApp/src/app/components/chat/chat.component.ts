import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ChatService } from '../../services/chat.service';
import { AuthService } from '../../services/auth.service';
import * as signalR from '@aspnet/signalr';

declare var $: any;

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss'],
  providers: [AuthService]
})

export class ChatComponent implements OnInit {

  errorMessage: string;
  accessToken: string;
  private hubConnection: signalR.HubConnection;
  profileId: string;
  employeeId: string;


  constructor(
    private router: Router,
    private authService: AuthService) {
  }

  ngOnInit(): void {

    let user = JSON.parse(localStorage.getItem('user'));
    this.profileId = user.profileId;
    this.employeeId = user.employeeId;

    this.accessToken = this.authService.getToken();

    console.log($.fn.jquery);

    this.hubConnection = new signalR.HubConnectionBuilder()
      //.withUrl("https://localhost:44343/chattinghub", {  })
      .configureLogging(signalR.LogLevel.Debug)
      .withUrl("https://localhost:44343/chattinghub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
        accessTokenFactory: () => this.accessToken
      })
      .build();


    this.hubConnection.on("Receive", function (message, userName) {

      // создаем элемент <b> для имени пользователя
      let userNameElem = document.createElement("b");
      userNameElem.appendChild(document.createTextNode(userName + ": "));

      // создает элемент <p> для сообщения пользователя
      let elem = document.createElement("p");
      elem.appendChild(userNameElem);
      elem.appendChild(document.createTextNode(message));

      var firstElem = document.getElementById("chatroom").firstChild;
      document.getElementById("chatroom").insertBefore(elem, firstElem);
    });



    this.hubConnection.on("Notify", function (message) {

      // создает элемент <p> для сообщения пользователя
      let elem = document.createElement("p");
      elem.appendChild(document.createTextNode(message));

      var firstElem = document.getElementById("chatroom").firstChild;
      document.getElementById("chatroom").insertBefore(elem, firstElem);
    });


    // начинаем соединение с хабом
    this.hubConnection.start();
  }

  public sendMessage(message: string, receiver: string): void {
    // отправка сообщения на сервер
    this.hubConnection.invoke("Send", message, receiver);
  }
}
