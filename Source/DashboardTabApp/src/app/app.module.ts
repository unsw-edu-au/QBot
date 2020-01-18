import { NgModule, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { Http, HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { AppService } from './app.service';

import { QuestionComponent } from './question/question.component';
import { NavMenuComponent } from './navmenu/navmenu.component';
import { HomeComponent } from './home/home.component';
import { ConfigComponent } from './config/config.component';
import { MyStudentsComponent } from './mystudents/mystudents.component';

import { Tabs } from './tabcontrol/tabs';
import { Tab } from './tabcontrol/tab';

import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner';
import { CourseadminComponent } from './courseadmin/courseadmin.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClient, HttpClientModule, HttpClientJsonpModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { UserAdmin } from './useradmin/useradmin.component';
import { GenericDialogComponent } from './generic-dialog/generic-dialog.component';

import { MatDialogModule, MatTableModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule, MatSelectModule, MatListModule } from '@angular/material';
import { CdkTableModule } from '@angular/cdk/table';

import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AddCourseDialogComponent } from './add-course-dialog/add-course-dialog.component';
import { DeleteCourseDialogComponent } from './delete-course-dialog/delete-course-dialog.component';
import { CourseProvisioningInfoDialogComponent } from './course-provisioning-info-dialog/course-provisioning-info-dialog.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { TutorialAdminComponent } from './tutorialadmin/tutorialadmin.component';
import { UserTutorialDialogComponent } from './user-tutorial-dialog/user-tutorial-dialog.component';

/*Authentication*/
import { MsalModule, MsalInterceptor } from '@azure/msal-angular';
import { SilentStartComponent } from './silent-start/silent-start.component';
import { SilentEndComponent } from './silent-end/silent-end.component';
import { AuthService } from './auth.service';

const appRoutes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'question/:id', component: QuestionComponent, runGuardsAndResolvers: 'always' },
    { path: 'config', component: ConfigComponent },
    { path: 'mystudents', component: MyStudentsComponent },
    { path: 'courseadmin', component: CourseadminComponent },
    { path: 'app-silent-start', component: SilentStartComponent },
    { path: 'app-silent-end', component: SilentEndComponent },


    { path: 'app-useradmin', component: UserAdmin },
    { path: 'tutorialadmin', component: TutorialAdminComponent },
    { path: 'courseinfo', component: CourseProvisioningInfoDialogComponent },
    { path: '**', component: HomeComponent }
];

@NgModule({
    declarations: [
        AppComponent,
        HomeComponent,
        NavMenuComponent,
        QuestionComponent,
        ConfigComponent,
        MyStudentsComponent,
        Tabs,
        Tab,
        CourseadminComponent,
        UserAdmin,
        GenericDialogComponent,
        AddCourseDialogComponent,
        DeleteCourseDialogComponent,
        CourseProvisioningInfoDialogComponent,
        TutorialAdminComponent,
        UserTutorialDialogComponent,
        SilentStartComponent,
        SilentEndComponent
    ],
    imports: [BrowserModule, FormsModule, HttpModule, Ng4LoadingSpinnerModule.forRoot(),
        RouterModule.forRoot(appRoutes, {  onSameUrlNavigation: 'reload' }),
        BrowserAnimationsModule,
        HttpClientModule,
        HttpClientJsonpModule,
        ReactiveFormsModule,
        MatDialogModule,
        FontAwesomeModule,
        MatTableModule,
        MatFormFieldModule,
        MatInputModule,
        CdkTableModule,
        MatButtonModule,
        MatIconModule,
        MatSelectModule,
        MatListModule,
        DragDropModule
    ],
    entryComponents: [GenericDialogComponent, AddCourseDialogComponent, DeleteCourseDialogComponent, UserTutorialDialogComponent],
    providers: [{ provide: 'BASE_URL', useFactory: getBaseUrl }, AppService, AuthService],
    bootstrap: [AppComponent]
})
export class AppModule { }

export function getBaseUrl() {
    return document.getElementsByTagName('base')[0].href;
}
