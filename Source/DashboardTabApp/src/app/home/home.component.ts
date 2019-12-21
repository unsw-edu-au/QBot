
/// <reference path='../../../node_modules/@types/adal/index.d.ts' />
import { Component } from '@angular/core';
import { AuthService } from '../auth.service';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent {
    isIframe: boolean;
    title = 'app';

    constructor(public authService: AuthService) {
        this.isIframe = window !== window.parent && !window.opener;

        //alert("home");
        authService.getToken();

    }

    public SignIn(event: any) {
        this.authService.signIn();
    }
}
