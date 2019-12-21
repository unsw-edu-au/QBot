import { OnInit, Component } from "@angular/core";
import * as microsoftTeams from '@microsoft/teams-js';

@Component({
    selector: 'config',
    templateUrl: './config.component.html'
})
export class ConfigComponent implements OnInit {

    constructor() { }

    ngOnInit() {
        console.log('ngOnInit');
        // initialize teams
        microsoftTeams.initialize();
        // get context
        microsoftTeams.getContext((context: microsoftTeams.Context) => {
            console.log('getContext');
            this.setValidityState(true);
        });

        microsoftTeams.settings.registerOnSaveHandler((saveEvent: microsoftTeams.settings.SaveEvent) => {
            // Calculate host dynamically to enable local debugging
            let host = "https://" + window.location.host;

            microsoftTeams.settings.setSettings({
                contentUrl: host + "/#/home",
                suggestedDisplayName: "Questions",
                removeUrl: host + "/remove",
                entityId: "1"
            });

            saveEvent.notifySuccess();
        });

    }

    public setValidityState(val: boolean) {
        microsoftTeams.settings.setValidityState(val);
    }

}
