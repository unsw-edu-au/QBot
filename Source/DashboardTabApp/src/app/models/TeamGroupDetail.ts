import { SafeUrl } from "@angular/platform-browser";

export class TeamGroupDetail {
    id: string = "";
    name: string = "";
    displayName: string = "";
    description: string = "";
    webUrl: string = "";
    photoByteUrl: string = "";
    safePhotoByteUrl: SafeUrl;
}
