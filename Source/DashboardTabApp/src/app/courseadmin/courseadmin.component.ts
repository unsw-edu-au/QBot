import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AppService } from '../app.service';
import { Course } from '../models/Course';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ViewEncapsulation, ViewChild } from '@angular/core';
import { MatTableDataSource, MatDialog } from '@angular/material';
import { environment } from 'src/environments/environment';
import { AddCourseDialogComponent } from '../add-course-dialog/add-course-dialog.component';
import { GenericDialogComponent } from '../generic-dialog/generic-dialog.component';
import { DeleteCourseDialogComponent } from '../delete-course-dialog/delete-course-dialog.component';
import { CourseProvisioningInfoDialogComponent } from '../course-provisioning-info-dialog/course-provisioning-info-dialog.component';
import { Router, ActivatedRoute } from '@angular/router';
import { window } from 'rxjs/operators';


@Component({
    selector: 'courseadmin',
    templateUrl: './courseadmin.component.html',
    styleUrls: ['./courseadmin.component.css'],
    encapsulation: ViewEncapsulation.None
})


export class CourseadminComponent implements OnInit {
    @ViewChild('table', { static: false }) table: any;
    appService: AppService;
    courses: Course[];
    displayedColumns = ['courseName', 'predictiveQnAServiceHost', 'predictiveQnAKnowledgeBaseId', 'predictiveQnAEndpointKey'
        , 'predictiveQnAHttpEndpoint', 'predictiveQnAHttpKey', 'predictiveQnAKnowledgeBaseName', 'predictiveQnAConfidenceThreshold', 'deleteButton'];
    dataSource;

    public formGroup: FormGroup;

    private editedRowIndex: number;
    router: Router;

    constructor(appService: AppService, public dialog: MatDialog, router: Router) {
        this.appService = appService;
        this.router = router;
    }

    ngOnInit() {
        this.appService.getCourses()
            .subscribe(courses => {
                this.courses = courses;
                this.dataSource = new MatTableDataSource(this.courses);
            });
    }

    editCourse(course: any) {
        if (course.id == 0) {
            alert("Please save this course first before editing.");
        } else {
            this.router.navigate(['usertutorialsetup/'], { queryParams: course} );

        }
        console.log(course)
    }

    addRow() {

        this.appService.getOwnedTeams(environment.tid, environment.upn).subscribe(ownedTeams => {
            var teams: any = ownedTeams;
            teams.value.forEach((t: any, index, array) => {
                this.courses.forEach(c => {
                    if (t.displayName == c.name) {
                        array.splice(index, 1);
                    }
                });
            });
            //check against teams already in the table
            const dialogRef = this.dialog.open(AddCourseDialogComponent, {
                width: '250px',
                data: teams.value
            });

            dialogRef.afterClosed().subscribe(res => {
                console.log("Dialog result: ", res)
                if (res) {
                    var newCourse: Course = {
                        id: 0,
                        name: res.displayName,
                        predictiveQnAServiceHost: "",
                        predictiveQnAKnowledgeBaseId: "",
                        predictiveQnAEndpointKey: "",
                        predictiveQnAHttpEndpoint: "",
                        predictiveQnAHttpKey: "",
                        predictiveQnAKnowledgeBaseName: "",
                        predictiveQnAConfidenceThreshold: "",
                        deployedURL: "",
                        groupId: res.id,
                        edited: false
                    }
                    this.courses.push(newCourse);
                    this.dataSource = new MatTableDataSource(this.courses);
                }
            });
        });

    }

    deleteRow(row: number) {
        var courseToDeleteID = this.courses[row].id;
        var courseName = this.dataSource.data[row].name;
        const dialogRef = this.dialog.open(DeleteCourseDialogComponent, {
            width: '250px',
            data: { courseName: courseName, courseToDeleteID: courseToDeleteID }
        });
        dialogRef.afterClosed().subscribe(res => {
            console.log("Dialog result: ", res)
            if (res) {
                if (res > 0) {
                    this.appService.deleteCourse(res)
                        .subscribe(courses => {
                            this.courses = courses;
                            this.courses.splice(row, 1);
                            this.dataSource = new MatTableDataSource(this.courses);
                        });
                } else {
                    this.courses.splice(row, 1);
                    this.dataSource = new MatTableDataSource(this.courses);
                }
            }
        });
    }

    saveRow(course: Course) {
        this.appService.saveCourse(course)
            .subscribe(courses => {
                if (courses != null) {// api returns null if an exception is caught
                    this.openDialog("Success", "Courses successfully saved.")

                } else {
                    this.openDialog("Failure", "Courses were not saved, try again.")
                }
                this.courses = courses;
                this.dataSource = new MatTableDataSource(this.courses);

            });
    }

    openDialog(outcome: string, message: string): void {
        const dialogRef = this.dialog.open(GenericDialogComponent, {
            width: '250px',
            data: { outcome: outcome, message: message }
        });
    }
}
