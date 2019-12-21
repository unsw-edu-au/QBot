import { Component, OnInit } from '@angular/core';
/// <reference path='../../../node_modules/@types/adal/index.d.ts' />
import 'expose-loader?AuthenticationContext!../../../node_modules/adal-angular/lib/adal.js'
import * as AuthenticationContext from 'adal-angular/lib/adal'
//let createAuthContextFn: adal.AuthenticationContextStatic = AuthenticationContext;
import * as microsoftTeams from '@microsoft/teams-js'
import { environment } from 'src/environments/environment';
@Component({
    selector: 'app-silent-end',
    templateUrl: './silent-end.component.html',
    styleUrls: ['./silent-end.component.scss']
})
export class SilentEndComponent implements OnInit {


    constructor() {
        microsoftTeams.initialize(function () {
            let config = {
                clientId: environment.authConfig.clientId,
                tenant: environment.authConfig.tenantId,
                // redirectUri must be in the list of redirect URLs for the Azure AD app
                redirectUri: window.location.origin + environment.authConfig.redirectUri,
                cacheLocation: environment.authConfig.cacheLocation,
                navigateToLoginRequestUrl: environment.authConfig.navigateToLoginRequestUrl,
            };
            let authContext = new AuthenticationContext(config);
            if (authContext.isCallback(window.location.hash)) {
                authContext.handleWindowCallback();
                // Only call notifySuccess or notifyFailure if this page is in the authentication popup
                if (window.opener) {
                    if (authContext.getCachedUser()) {
                        microsoftTeams.authentication.notifySuccess();
                    } else {
                        microsoftTeams.authentication.notifyFailure(authContext.getLoginError());
                    }
                }
            }
        });
    }


    ngOnInit() {
    }

}
