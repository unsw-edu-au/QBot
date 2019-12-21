import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
@Component({
    selector: 'app-user-tutorial-dialog',
    templateUrl: './user-tutorial-dialog.component.html',
    styleUrls: ['./user-tutorial-dialog.component.scss']
})
export class UserTutorialDialogComponent implements OnInit {
    user: any;
    tutorials: any[] = [];
    userTutorials: any[] = [];
    selectedTutorials: any;
    tutorialForm: FormGroup;

    constructor(public dialogRef: MatDialogRef<UserTutorialDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: any, private formBuilder: FormBuilder) {
        this.user = data.user;
        this.tutorials = data.tutorials;
        this.userTutorials = this.user.tutorialGroups;
        this.selectedTutorials = this.userTutorials;
        this.tutorialForm = this.formBuilder.group({
            tutorials: new FormControl(this.userTutorials)
        });
    }

    ngOnInit() {
    }

    onOkClick(): void {
        this.user.tutorialGroups = this.selectedTutorials;
        this.dialogRef.close(this.user);
    }

    onCancelClick(): void {
        this.dialogRef.close();
    }

}
