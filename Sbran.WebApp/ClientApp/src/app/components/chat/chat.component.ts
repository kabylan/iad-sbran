import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ChatService } from '../../services/chat.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent {

  errorMessage: string;

  constructor(
    private router: Router,
    private authService: ChatService) {
  }

  login(login: string, password: string) {
    this.authService.login(login, password).subscribe(
      accountDetailsResult => {
        this.router.navigateByUrl(`/profile/${accountDetailsResult.profileId}/employee/${accountDetailsResult.employeeId}`);
      },
      error => {
        console.log(error);
        this.errorMessage = error.error_description;
      }
    );
  }
}
