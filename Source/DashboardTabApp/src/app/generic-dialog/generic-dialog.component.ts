import { Component, Inject, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-generic-dialog',
    templateUrl: './generic-dialog.component.html',
    styleUrls: ['./generic-dialog.component.css']
})
export class GenericDialogComponent implements OnInit {

    constructor(
        public dialogRef: MatDialogRef<GenericDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any) { }

    ngOnInit() { }


    onNoClick(): void {
        this.dialogRef.close();
    }
}
