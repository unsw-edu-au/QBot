import { Component, OnInit } from '@angular/core';
/// <reference path='../../../node_modules/@types/adal/index.d.ts' />
import 'expose-loader?AuthenticationContext!../../../node_modules/adal-angular/lib/adal.js'
import * as AuthenticationContext from 'adal-angular/lib/adal'
//let createAuthContextFn: adal.AuthenticationContextStatic = AuthenticationContext;
import * as microsoftTeams from '@microsoft/teams-js'
import { environment } from 'src/environments/environment';
@Component({
    selector: 'app-silent-start',
    templateUrl: './silent-start.component.html',
    styleUrls: ['./silent-start.component.scss']
})
export class SilentStartComponent implements OnInit {
    
    constructor() {
        microsoftTeams.initialize();
        // Get the tab context, and use the information to navigate to Azure AD login page
        microsoftTeams.getContext(function (context) {
            // ADAL.js configuration
            // Setup extra query parameters for ADAL
            // - openid and profile scope adds profile information to the id_token
            // - login_hint provides the expected user name
            let config = {
                clientId: environment.authConfig.clientId,
                tenant: environment.authConfig.tenantId,
                // redirectUri must be in the list of redirect URLs for the Azure AD app
                redirectUri: window.location.origin + environment.authConfig.redirectUri,
                cacheLocation: environment.authConfig.cacheLocation,
                navigateToLoginRequestUrl: environment.authConfig.navigateToLoginRequestUrl,
                //popUp: true,
                extraQueryParameters: environment.authConfig.extraQueryParameters,
                displayCall: null
            };

            if (context.upn) {
                config.extraQueryParameters = "scope=openid+profile&login_hint=" + encodeURIComponent(context.upn);
            } else {
                config.extraQueryParameters = "scope=openid+profile";
            }

            // Use a custom displayCall function to add extra query parameters to the url before navigating to it

            config.displayCall = function (urlNavigate) {
                if (urlNavigate) {
                    if (config.extraQueryParameters) {
                        urlNavigate += "&" + config.extraQueryParameters;
                    }
                    window.location.replace(urlNavigate);
                }
            }
            // Navigate to the AzureAD login page        
            let authContext = new AuthenticationContext(config);
            authContext.login();
        });
    }

    ngOnInit() {
    }

}
