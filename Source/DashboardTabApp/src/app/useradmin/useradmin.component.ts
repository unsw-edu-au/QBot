import { Component, OnInit, ViewChild, ElementRef, ViewEncapsulation } from '@angular/core';
import { Student } from '../models/Student';
import { AppService } from '../app.service';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatTableDataSource } from '@angular/material';
import { GenericDialogComponent } from '../generic-dialog/generic-dialog.component';
import { Course } from '../models/Course';
import { UserTutorialDialogComponent } from '../user-tutorial-dialog/user-tutorial-dialog.component';
import { TutorialGroup } from '../models/TutorialGroup';

@Component({
    selector: 'app-useradmin',
    templateUrl: './useradmin.component.html',
    styleUrls: ['./useradmin.component.css'],
    encapsulation: ViewEncapsulation.None
})
export class UserAdmin implements OnInit {
    @ViewChild('studentFile', { static: false }) studentFile: ElementRef;
    studentsToUpload: any;
    studentsToAdd: Student[];
    appService: AppService;
    selectedCourse: any;
    selectedCourseName: any;
    courseList: Course[];
    roleList: any;
    displayedColumns = ['userName', 'firstName', 'lastName', 'role'
        , 'tutorialGroups', 'actions'];
    dataSource;
    tutorials: TutorialGroup[];
    userList: any[];

    constructor(appService: AppService, public dialog: MatDialog) {
        this.appService = appService;
        this.appService.getRoles().subscribe(roles => {
            this.roleList = roles;
            console.log(this.roleList);
        });
    }

    ngOnInit() {
        this.appService.getCourses()
            .subscribe(courses => {
                this.courseList = courses;
            });
    }

    handleFileInput(input: HTMLInputElement) {
        const files = input.files;
        if (files && files.length) {
            const fileToRead = files[0];
            const fileReader = new FileReader();
            fileReader.onload = () => {
                const textFromFileLoaded = <string>fileReader.result;
                const lines = textFromFileLoaded.split('\n');
                var count = 1;
                this.studentsToAdd = [];
                lines.forEach(element => {
                    if (count == 1) {// the reader doesn't give a line count so I just skip the first one
                        count++;
                    } else {
                        var line = element.split(',');
                        var student: Student = {
                            studentID: line[0],
                            firstName: line[1],
                            lastName: line[2],
                            username: line[3],
                            email: line[4],
                            role: line[5],
                            courseName: this.selectedCourseName,
                            tutorialGroupID: (line[6]) ? line[6].trim() : ''
                        };
                        if (student.firstName != undefined)// skip the last line
                            this.studentsToAdd.push(student);
                    }
                });
                if (this.studentsToAdd.length > 0) {
                    //call app service to add students
                    this.appService.addStudents(this.studentsToAdd).subscribe(students => {
                        if (students != null) {// api returns null if an exception is caught
                            this.openDialog("Success", "Students successfully added/updated.")

                        } else {
                            this.openDialog("Failure", "Students were not added/updated. Please check your .csv file and try again.")
                        }
                    })
                }
            };
            fileReader.readAsText(fileToRead, "UTF-8");
            this.studentFile.nativeElement.value = "";
        }
    }

    updateUserTable(course) {
        console.log(course);
        if (this.selectedCourse == undefined || course.name != this.selectedCourse.name) {
            this.selectedCourse = course;
            this.appService.getUserCourseRoleMappingsByCourse(course.id)
                .subscribe(users => {
                    console.log(users);
                    this.userList = users;
                    this.dataSource = new MatTableDataSource(users);
                    this.appService.getTutorialsByCourse(course.id)
                        .subscribe(tutorials => {
                            console.log(tutorials);
                            this.tutorials = tutorials;
                        });
                });
        }
    }


    manageUserTutorial(user) {
        if (user.id == 0) {
            alert("Please save this user first before editing.");
        } else {
            const dialogRef = this.dialog.open(UserTutorialDialogComponent, {
                width: '500px',
                height: '700px',
                data: { user: user, tutorials: this.tutorials }
            });
            dialogRef.afterClosed().subscribe(res => {
                if (res) {
                    var index = this.userList.findIndex(user => user.id == res.id);
                    if (index > -1) {
                        var newTutString = "";
                        if (res.tutorialGroups.length > 0) {
                            for (var i = 0; i < res.tutorialGroups.length; i++) {
                                newTutString += res.tutorialGroups[i].code + ", ";
                            }
                            newTutString = newTutString.substring(0, newTutString.length - 2);
                        }
                        res.tutorialGroupsString = newTutString;
                        this.userList[index] = res;
                        this.dataSource = new MatTableDataSource(this.userList);
                    }
                    console.log(res);
                }
            });
        }
        console.log(user)
    }

    refreshUsers() {
        this.appService.refreshUsers(this.selectedCourse).subscribe(users => {
            console.log(users)
            if (users) {
                this.dataSource = new MatTableDataSource(users);
            }
        });
    }

    deleteRow(user: any, index: number) {
        this.appService.deleteUserCourseRoleMapping(user)
            .subscribe(users => {
                this.userList = users;
                this.userList.splice(index, 1);
                this.dataSource = new MatTableDataSource(this.userList);
            });
    }


    saveRow(user: any, index: number) {
        this.appService.saveUserCourseRoleMapping(user)
            .subscribe(users => {
                if (users != null) {// api returns null if an exception is caught
                    this.openDialog("Success", "User successfully saved.")

                } else {
                    this.openDialog("Failure", "User was not saved, try again.")
                }
                
                this.refreshUsers();

            });
    }
    openDialog(outcome: string, message: string): void {
        const dialogRef = this.dialog.open(GenericDialogComponent, {
            width: '250px',
            data: { outcome: outcome, message: message }
        });
    }

}
