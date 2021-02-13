import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { mergeMap, map } from 'rxjs/operators';
import { APP_CONFIG, IAppConfig } from '../settings/app-config';
import * as signalR from "@aspnet/signalr";

@Injectable()
export class ChatService {

  private hubConnection: signalR.HubConnection


}
