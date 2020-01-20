import { Component, OnInit, Inject } from '@angular/core';
import { User } from '../models/User';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { DomSanitizer } from '@angular/platform-browser';
//import { iconTypes, iconStyle } from 'msteams-ui-icons-core';

import * as microsoftTeams from '@microsoft/teams-js'
import { AppService } from '../app.service';
import { TeamGroupDetail } from '../models/TeamGroupDetail';

import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../auth.service';
import { faChalkboardTeacher, faUsers, faUserPlus, faCogs, faUsersCog, faUserFriends, faSignInAlt, faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import { environment } from 'src/environments/environment';


@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent implements OnInit {

    errorMessage: string = null;
    context: microsoftTeams.Context;

    isDemonstrator: boolean = false;
    isLecturer: boolean = false;
    isAdmin: boolean = false;

    groupIds: string[];
    teamGroupDetail: TeamGroupDetail[];

    faChalkboardTeacher = faChalkboardTeacher;
    faUsers = faUsers;
    faUserPlus = faUserPlus;
    faCogs = faCogs;
    faSignInAlt = faSignInAlt;
    faSignOutAlt = faSignOutAlt;
    faUsersCog = faUsersCog;
    faUserFriends = faUserFriends;

    constructor(
        private appService: AppService,
        private router: Router,
        public authService: AuthService,
        private domSanitizer: DomSanitizer,
        private spinnerService: Ng4LoadingSpinnerService) {
        
        
    }

    ngOnInit(): void {
        environment.upn = this.GetParam("upn");
        environment.tid = this.GetParam("tid");

        if (environment.upn && environment.tid) {
            this.spinnerService.show();
            this.authService.onAuthUpdate(this.onAuthUpdate.bind(this));
        }
    }

    onAuthUpdate() {
        this.appService.setHeaders();

        // Do any first-time initialisation (like bootstrapping the initial user)
        this.appService.initializeBotService().subscribe(
            () => {
                this.populateNav();
            },
            (error) => {
                this.showError("There was an error initializing the Bot service", error);
                this.spinnerService.hide();
            });
    }


    populateNav() {
        var upn = environment.upn;
        var tid = environment.tid;

        var userFinished: boolean = false;
        var groupFinished: boolean = false;

        this.appService.getUserAccess(upn)
            .subscribe(userAccess => {
                this.isAdmin = userAccess.isGlobalAdmin;
                this.isLecturer = userAccess.isLecturer;
                this.isDemonstrator = userAccess.isDemonstrator;
            
                // only hide spinner when both tasks are complete
                userFinished = true;
                if (groupFinished) {
                    this.spinnerService.hide();
                }
            },
            (error) => {
                this.showError("There was an error getting initial user access", error);
                this.spinnerService.hide();
            });

        // this needs to change when we implement a solution for provisioning the bot for different teams.
        // since there are very few teams, this is acceptable but it is in no way scalable.
        this.appService.getTeamGroupIdsWithQuestions(tid, upn)
            .subscribe(groupIds => {
                this.groupIds = groupIds.sort();

                var tgd = new Array<TeamGroupDetail>();

                this.groupIds.forEach(groupId => {
                    this.appService.getTeamGroupDetail(groupId).subscribe(group => {
                        group.safePhotoByteUrl = this.domSanitizer.bypassSecurityTrustUrl("data:image/png;base64," + group.photoByteUrl);
                        tgd.push(group);
                        if (tgd.length == groupIds.length) {
                            this.teamGroupDetail = tgd;
                            if (this.router.routerState.snapshot.url.indexOf('home') >= 0) {
                                this.router.navigate(['question/' + this.teamGroupDetail[0].id], { queryParams: { upn: environment.upn, tid: environment.tid } });
                            } else if (this.router.routerState.snapshot.url.indexOf('question') >= 0) {
                                let id = this.router.routerState.snapshot.url.split('/')[2].split('?')[0];
                                this.router.navigate(['question/' + id], { queryParams: { upn: environment.upn, tid: environment.tid } });
                            } else {
                                let root = this.router.routerState.snapshot.url.split('/')[1].split('?')[0];
                                this.router.navigate([root], { queryParams: { upn: environment.upn, tid: environment.tid } });
                            }

                            // only hide spinner when both tasks are complete
                            groupFinished = true;
                            if (userFinished) {
                                this.spinnerService.hide();
                            }
                        }
                    },
                    (error) => {
                        this.showError("There was an error getting group details", error);
                        this.spinnerService.hide();
                    });
                });
            },
            (error) => {
                this.showError("There was an error getting group details", error);
                this.spinnerService.hide();
            });
    }

    showError(errorMessage: string, error: any = null) {
        this.errorMessage = errorMessage + (error ? error : "");
    }

    GetParam(name): string {
        const results = new RegExp('[\\?&]' + name + '=([^&#]*)').exec(decodeURIComponent(window.location.href));
        if (!results) {
            return null;
        }
        return results[1] || null;
    }

    teamNav(teamId: string) {
        this.router.navigate(['question/' + teamId], { queryParams: { upn: environment.upn, tid: environment.tid } });

    }

    myStudentNav() {
        this.router.navigate(['mystudents/'], { queryParams: { upn: environment.upn, tid: environment.tid } });

    }

    courseAdminNav() {
        this.router.navigate(['courseadmin/'], { queryParams: { upn: environment.upn, tid: environment.tid } });

    }

    addStudentNav() {
        this.router.navigate(['app-useradmin/'], { queryParams: { upn: environment.upn, tid: environment.tid } });
    }

    tutorialAdminNav() {
        this.router.navigate(['tutorialadmin/'], { queryParams: { upn: environment.upn, tid: environment.tid } });

    }

    settingsNav() {
        this.router.navigate(['settings/'], { queryParams: { upn: environment.upn, tid: environment.tid } });

    }
}
