import { Component, Inject, OnInit } from '@angular/core';
import { window } from 'rxjs/operators';
//import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog'; // in case we need to inject data to this page later on

@Component({
    selector: 'app-course-provisioning-info-dialog',
    templateUrl: './course-provisioning-info-dialog.component.html',
    styleUrls: ['./course-provisioning-info-dialog.component.css']
})
export class CourseProvisioningInfoDialogComponent implements OnInit {

    constructor() { }

    ngOnInit() { }


    onOKClick(): void {
        close();
    }

}
