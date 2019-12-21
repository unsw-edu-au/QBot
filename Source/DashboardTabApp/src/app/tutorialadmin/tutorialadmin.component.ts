import { Component, OnInit } from '@angular/core';
import { Course } from '../models/Course';
import { UserCourseRole } from '../models/UserCourseRoleModel';
import { AppService } from '../app.service';
import { MatTableDataSource, MatDialog } from '@angular/material';
import { Tutorial } from '../models/Tutorial';
import { TutorialGroup } from '../models/TutorialGroup';
import { GenericDialogComponent } from '../generic-dialog/generic-dialog.component';

@Component({
    selector: 'tutorialadmin',
    templateUrl: './tutorialadmin.component.html',
    styleUrls: ['./tutorialadmin.component.css']
})
export class TutorialAdminComponent implements OnInit {
    userCourseRoleMappings: UserCourseRole[];
    courseList: Course[];
    appService: AppService;
    displayedColumns = ['tutorialcode', 'name', 'actions'];
    tutorials: TutorialGroup[];
    dataSource;
    selectedCourseName: any;
    selectedCourse: any;

    constructor(appService: AppService, public dialog: MatDialog) {
        this.appService = appService;

    }

    ngOnInit() {
        this.appService.getCourses()
            .subscribe(courses => {
                this.courseList = courses;
            });
    }

    updateTutorialMappingTable(course) {
        console.log(course);
        this.appService.getTutorialsByCourse(course.id)
            .subscribe(tutorials => {
                console.log(tutorials);
                this.selectedCourse = course;
                this.tutorials = tutorials;
                this.dataSource = new MatTableDataSource(this.tutorials);
            });
    }
    addRow() {
        var newTutorial: TutorialGroup = {
            id: 0,
            courseId: this.selectedCourse.id,
            code: "",
            name: ""
        }
        this.tutorials.push(newTutorial);
        this.dataSource = new MatTableDataSource(this.tutorials);

    }

    deleteRow(tutorial: TutorialGroup, row: number) {
        var tutorialToDelete = tutorial;
        
        //const dialogRef = this.dialog.open(DeleteCourseDialogComponent, {
        //    width: '250px',
        //    data: { courseName: courseName, courseToDeleteID: tutorialToDelete }
        //});
        // dialogRef.afterClosed().subscribe(res => {
        //    console.log("Dialog result: ", res)
        //    if (res) {
        //if (res > 0) {
        this.appService.deleteTutorial(tutorialToDelete)
            .subscribe(tutorials => {
                this.tutorials = tutorials;
                this.tutorials.splice(row, 1);
                this.dataSource = new MatTableDataSource(this.tutorials);
            });
        // } else {
        //  this.courses.splice(row, 1);
        //  this.dataSource = new MatTableDataSource(this.courses);
        // }
        //   }
        // });
    }

    saveRow(row: number) {
        //this.editService.save(course, isNew);
        var tutorial = this.tutorials[row];
        this.appService.saveTutorial(tutorial)
            .subscribe(tutorials => {
                if (tutorials != null) {// api returns null if an exception is caught
                    this.openDialog("Success", "Tutorials successfully saved.")

                } else {
                    this.openDialog("Failure", "Tutorials were not saved, try again.")
                }
                this.tutorials = tutorials;
                this.dataSource = new MatTableDataSource(this.tutorials);

            });
    }

    openDialog(outcome: string, message: string): void {
        const dialogRef = this.dialog.open(GenericDialogComponent, {
            width: '250px',
            data: { outcome: outcome, message: message }
        });
    }

}
