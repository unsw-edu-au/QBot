import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { Http, HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { ConfigComponent } from './config/config.component';
import { HomeComponent } from './home/home.component';
import { AppService } from './app.service';

import { Tabs } from './tabcontrol/tabs';
import { Tab } from './tabcontrol/tab';

import { HttpClientModule } from '@angular/common/http';
import { AuthService } from './auth.service';
import { SilentStartComponent } from './silent-start/silent-start.component';
import { SilentEndComponent } from './silent-end/silent-end.component';



const appRoutes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'config', component: ConfigComponent },
    { path: 'app-silent-start', component: SilentStartComponent },
    { path: 'app-silent-end', component: SilentEndComponent },
    { path: '**', component: HomeComponent }
];

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        ConfigComponent,
        SilentStartComponent, SilentEndComponent,
        Tabs, Tab
    ],
    imports: [
        BrowserModule, FormsModule, HttpModule,
        RouterModule.forRoot(appRoutes), HttpClientModule
    ],
    providers: [{ provide: 'BASE_URL', useFactory: getBaseUrl }, AppService, AuthService],
    bootstrap: [AppComponent]
})
export class AppModule { }

export function getBaseUrl() {
    return document.getElementsByTagName('base')[0].href;
}
