import { Injectable } from '@angular/core';
import * as microsoftTeams from '@microsoft/teams-js';
import * as authContext from 'adal-angular/lib/adal';
import { environment } from './../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
    context;
    config;

    token: string;

    showSignInButton: boolean = false;
    loggedIn: boolean = false;

    private loadNav: () => void;
    onAuthUpdate(fn: () => void) {
        this.loadNav = fn;
    }

    constructor() {
        this.config = {
            clientId: environment.authConfig.clientId,
            tenant: environment.authConfig.tenantId,
            // redirectUri must be in the list of redirect URLs for the Azure AD app
            redirectUri: window.location.origin + environment.authConfig.redirectUri,
            cacheLocation: environment.authConfig.cacheLocation,
            navigateToLoginRequestUrl: environment.authConfig.navigateToLoginRequestUrl,
            popUp: environment.authConfig.popUp,
            extraQueryParameters: environment.authConfig.extraQueryParameters,
            postLogoutRedirectUri: window.location.origin
        };
    }

    getToken() {
        console.log("getToken");

        let context = new authContext(this.config);
        // try get cached token
        let token = context.getCachedToken(this.config.clientId);
        if (token) {
            // cache token success
            this.token = token;
            this.loggedIn = true;
            this.showSignInButton = false;

            console.log("token saved");
            this.loadNav();
        } else {
            console.log("no cachedToken");
            // No token, or token is expired
            var callback = (function (err1, idToken) {
                if (err1) {
                    // token renew fail
                    this.showSignInButton = true;
                    this.loggedIn = false;

                    console.log("renew fail: " + err1);
                } else {
                    // token renew success
                    this.token = idToken;
                    this.loggedIn = true;
                    this.showSignInButton = false;

                    console.log("renew success");
                    this.loadNav();
                }
            }).bind(this);
            context._renewIdToken(callback);
        }
    }

    signOut() {
        let context = new authContext(this.config);
        context.logOut();
    }

    signIn() {
        // try get token from AAD
        let context = new authContext(this.config);

        var signInSuccessCallback = (function (result) {
            // AuthenticationContext is a singleton
            let config = {
                clientId: this.config.clientId,
                tenant: this.config.tenantId,
                // redirectUri must be in the list of redirect URLs for the Azure AD app
                redirectUri: window.location.origin + this.config.redirectUri,
                cacheLocation: this.config.cacheLocation,
                navigateToLoginRequestUrl: this.config.navigateToLoginRequestUrl,
                extraQueryParameters: this.config.extraQueryParameters,
            };
            let context = new authContext(config);
            let idToken = context.getCachedToken(config.clientId);
            if (idToken) {
                this.showSignInButton = false;
                this.loggedIn = true;
                this.token = idToken;

                console.log("auth success");
                this.loadNav();
            } else {
                this.loggedIn = false;
                this.showSignInButton = true;
                console.log("auth success but no token");
            };
        }).bind(this);

        var signInFailCallback = (function (reason) {
            this.loggedIn = false;
            this.showSignInButton = true;
            console.log("Login failed: " + reason);

            if (reason === "CancelledByUser" || reason === "FailedToOpenWindow") {
                console.log("Login was blocked by popup blocker or canceled by user.");
            }
        }).bind(this);

        var msTeamsAuthenticate = (function () {
            microsoftTeams.authentication.authenticate({
                url: window.location.origin + environment.authConfig.popUpUri,
                width: environment.authConfig.popUpWidth,
                height: environment.authConfig.popUpHeight,
                successCallback: signInSuccessCallback,
                failureCallback: signInFailCallback
            });
        });

        var acquireTokencallback = (function (err, idToken) {
            // no token
            if (err) {
                console.log("acquire token failed: " + err);
                //context.login();

                microsoftTeams.initialize(msTeamsAuthenticate);
                
            } else {
                this.showSignInButton = false;
                this.loggedIn = true;
                this.token = idToken;

                console.log("acquire token success");
                this.loadNav();
            }
        }).bind(this);
        context.acquireToken(environment.authConfig.instance, acquireTokencallback);
    }

    onRenewSuccess(idToken) {
        // token renew success
        this.token = idToken;
        this.loggedIn = true;
        this.showSignInButton = false;
        console.log("renew success");
    }

    onRenewFailure(err1) {
        // token renew fail
        this.showSignInButton = true;
        this.loggedIn = false;

        console.log("renew fail" + err1);
    }
}
