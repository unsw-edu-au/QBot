import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';


@Component({
  selector: 'app-delete-course-dialog',
  templateUrl: './delete-course-dialog.component.html',
  styleUrls: ['./delete-course-dialog.component.scss']
})
export class DeleteCourseDialogComponent implements OnInit {
    courseData: any;

    constructor(public dialogRef: MatDialogRef<DeleteCourseDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {
        this.courseData = data;
    }

    ngOnInit() {
    }

    onYesClick(): void {
        this.dialogRef.close(this.courseData.courseToDeleteID);
    }

    onNoClick(): void {
        this.dialogRef.close();
    }
}
