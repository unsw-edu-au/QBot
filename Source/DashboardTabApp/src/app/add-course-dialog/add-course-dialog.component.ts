import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-add-course-dialog',
    templateUrl: './add-course-dialog.component.html',
    styleUrls: ['./add-course-dialog.component.scss']
})
export class AddCourseDialogComponent implements OnInit {
    selectedCourse: any;

    constructor(public dialogRef: MatDialogRef<AddCourseDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {
    }

    ngOnInit() {
    }

    onOkClick(): void {
        this.dialogRef.close(this.selectedCourse);
    }

    onCancelClick(): void {
        this.dialogRef.close();
    }
}
