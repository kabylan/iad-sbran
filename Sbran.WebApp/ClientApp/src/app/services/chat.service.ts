import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { mergeMap, map } from 'rxjs/operators';
import { APP_CONFIG, IAppConfig } from '../settings/app-config';
import * as signalR from "@aspnet/signalr";

@Injectable()
export class ChatService {


  profileService: string;

  constructor(
    @Inject(APP_CONFIG) private config: IAppConfig,
    private http: HttpClient) {
    this.profileService = `${this.config.icsApiEndpoint}api/v1/profile/search`;
  }

  public searchUsers(searchText: string): void {


    const headers = new HttpHeaders().set('Content-Type', 'application/json'); // x-www-form-urlencoded
    const options = { headers: headers };

    let data = {
      userName: searchText
    }

    this.http.get(this.profileService + `/${searchText}`).subscribe(responseData => console.log(responseData));
  }

}
