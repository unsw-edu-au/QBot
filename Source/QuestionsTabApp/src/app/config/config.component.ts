import { OnInit, Component } from "@angular/core";
import * as microsoftTeams from '@microsoft/teams-js';
import { environment } from "src/environments/environment";

@Component({
    selector: 'config',
    templateUrl: './config.component.html'
})
export class ConfigComponent implements OnInit {

    upn: string = "";
    tid: string = "";
    gid: string = "";
    cname: string = "";
    selfURL: string = "";
    tabName = "";
    
    constructor() {
        this.selfURL = environment.selfUrl;

        if (!this.selfURL.endsWith('/')) {
            this.selfURL += "/";
        }
    }

    ngOnInit() {
        console.log('ngOnInit');
        // initialize teams
        microsoftTeams.initialize();
        // get context
        microsoftTeams.getContext((context: microsoftTeams.Context) => {
            console.log('getContext');
            this.upn = context.upn;
            this.tid = context.tid;
            this.gid = context.groupId;
            this.cname = context.channelName;

            microsoftTeams.settings.setValidityState(true);
        });

        microsoftTeams.settings.registerOnSaveHandler((saveEvent: microsoftTeams.settings.SaveEvent) => {
            // Calculate host dynamically to enable local debugging
            console.log(this.tabName)
            // Antares
            microsoftTeams.settings.setSettings({
                contentUrl: this.selfURL + "home?upn=" + this.upn + "&tid=" + this.tid + "&gid=" + this.gid + "&cname=" + this.cname,
                websiteUrl: this.selfURL + "home?upn=" + this.upn + "&tid=" + this.tid + "&gid=" + this.gid + "&cname=" + this.cname,
                suggestedDisplayName: this.tabName === "" ? "Questions" : this.tabName,
                removeUrl: this.selfURL + "remove",
                entityId: "1"
            });
            saveEvent.notifySuccess();
        });

    }
}
