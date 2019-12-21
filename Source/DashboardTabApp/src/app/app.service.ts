import { Http, Headers, RequestOptions } from '@angular/http';
import { Injectable, Inject } from "@angular/core";
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { User } from './models/User';
import { Question } from './models/Question';
import { TeamGroupDetail } from './models/TeamGroupDetail';
import { TeamChannel } from './models/TeamChannel';
import { Course } from './models/Course';
import { Student } from './models/Student';
import { environment } from './../environments/environment';
import { UserCourseRole } from './models/UserCourseRoleModel';
import { TutorialGroup } from './models/TutorialGroup';
import { AuthService } from './auth.service';


@Injectable()
export class AppService {
    http: Http;
    baseUrl: string;
    options: RequestOptions;

    constructor(http: Http,
        @Inject('BASE_URL') baseUrl: string,
        private authService: AuthService) {
        this.http = http;
        this.baseUrl = environment.apiBaseUrl;

        this.setHeaders();
    }

    setHeaders() {
        let headers = new Headers({
            'Content-Type': 'application/json',
            'X-Frame-Options': 'SAMEORIGIN',
            'Authorization': 'Bearer ' + this.authService.token
            
        });

        this.options = new RequestOptions({ headers: headers });
    }


    initializeBotService(): Observable<any> {
        return this.http.post(this.baseUrl + 'InitializeBotService', "", this.options);
    }

    getUserAccess(upn: string): Observable<any> {
        return this.http.get(this.baseUrl + 'GetUserAccess?upn=' + upn, this.options)
            .pipe(map(res => res.json()));
    }

    getUserByUpn(upn: string): Observable<User> {
        return this.http.post(this.baseUrl + 'GetUserByUpn', JSON.stringify(upn), this.options)
            .pipe(map(res => res.json()));
    }

    getAllQuestions(tenantId: string): Observable<Question[]> {
        return this.http.post(this.baseUrl + 'GetAllQuestions', JSON.stringify(tenantId), this.options)
            .pipe(map(res => res.json()));
    }

    getTeamGroupIdsWithQuestions(tenantId: string, upn: string): Observable<string[]> {
        return this.http.post(this.baseUrl + 'GetTeamGroupIdsWithQuestions', JSON.stringify({ tenantId: tenantId, upn: upn }), this.options)
            .pipe(map(res => res.json()));
    }

    getOwnedTeams(tenantId: string, upn: string): Observable<string[]> {
        return this.http.post(this.baseUrl + 'GetOwnedTeams', JSON.stringify({ tenantId: tenantId, upn: upn }), this.options)
            .pipe(map(res => res.json()));
    }

    getTeamGroupDetail(groupId: string): Observable<TeamGroupDetail> {
        return this.http.post(this.baseUrl + 'GetTeamGroupDetail', JSON.stringify(groupId), this.options)
            .pipe(map(res => res.json()));
    }

    getTeamChannels(groupId: string): Observable<TeamChannel[]> {
        return this.http.post(this.baseUrl + 'GetTeamChannels', JSON.stringify(groupId), this.options)
            .pipe(map(res => res.json()));
    }

    getQuestionsByGroup(groupId: string): Observable<Question[]> {
        return this.http.post(this.baseUrl + 'GetQuestionsByGroup', JSON.stringify(groupId), this.options)
            .pipe(map(res => res.json()));
    }

    getQuestionsByTutorial(tenantId: string, code: string): Observable<Question[]> {
        return this.http.post(this.baseUrl + 'GetQuestionsByTutorial', JSON.stringify({ tenantId: tenantId, code: code }), this.options)
            .pipe(map(res => res.json()));
    }

    getCourses(): Observable<Course[]> {
        return this.http.get(this.baseUrl + 'GetCourses', this.options)
            .pipe(map(res => res.json()));
    }

    saveCourse(course: Course): Observable<Course[]> {
        return this.http.post(this.baseUrl + 'SaveCourse', course, this.options)// to actually just call Bot api
            .pipe(map(res => res.json()));
    }

    deleteCourse(id: number): Observable<Course[]> {
        return this.http.post(this.baseUrl + 'DeleteCourse', id, this.options)
            .pipe(map(res => res.json()));
    }

    addStudents(students: Student[]): Observable<any> {
        return this.http.post(this.baseUrl + 'AddStudents', students, this.options)// to actually just call Bot api
            .pipe(map(res => res.json()));
    }

    getPredictiveQnAThreshold(): Observable<string> {
        return this.http.get(this.baseUrl + 'GetPredictiveQnAThreshold', this.options)
            .pipe(map(res => res.json()));
    }

    setPredictiveQnAThreshold(threshold: string): Observable<boolean> {
        return this.http.post(this.baseUrl + 'SetPredictiveQnAThreshold', JSON.stringify(threshold), this.options)
            .pipe(map(res => res.json()));
    }

    searchAndGetTimeUrl(phrase: string): Observable<string> {
        return this.http.post(this.baseUrl + 'SearchAndGetTimeUrl', JSON.stringify(phrase), this.options)
            .pipe(map(res => res.json()));
    }

    getUserCourseRoleMappingsByCourse(courseId: number): Observable<UserCourseRole[]> {
        return this.http.post(this.baseUrl + 'GetUserCourseRoleMappingsByCourse', JSON.stringify(courseId), this.options)
            .pipe(map(res => res.json()));
    }

    deleteUserCourseRoleMapping(user: any): Observable<UserCourseRole[]> {
        return this.http.post(this.baseUrl + 'DeleteUserCourseRoleMapping', JSON.stringify(user), this.options)
            .pipe(map(res => res.json()));
    }

    saveUserCourseRoleMapping(user: any): Observable<UserCourseRole[]> {
        return this.http.post(this.baseUrl + 'SaveUserCourseRoleMapping', JSON.stringify(user), this.options)
            .pipe(map(res => res.json()));
    }

    refreshUsers(course: Course): Observable<UserCourseRole[]> {
        return this.http.post(this.baseUrl + 'RefreshUsers', JSON.stringify(course), this.options)
            .pipe(map(res => res.json()));
    }

    getRoles(): Observable<any[]> {
        return this.http.get(this.baseUrl + 'GetRoles', this.options)
            .pipe(map(res => res.json()));
    }

    getTutorialsByCourse(courseId: number): Observable<TutorialGroup[]> {
        return this.http.post(this.baseUrl + 'GetTutorialsByCourse', JSON.stringify(courseId), this.options)
            .pipe(map(res => res.json()));
    }

    saveTutorial(tutorial: TutorialGroup): Observable<TutorialGroup[]> {
        return this.http.post(this.baseUrl + 'SaveTutorialGroup', tutorial, this.options)
            .pipe(map(res => res.json()));
    }

    deleteTutorial(tutorial: TutorialGroup): Observable<TutorialGroup[]> {
        return this.http.post(this.baseUrl + 'DeleteTutorialGroup', tutorial, this.options)
            .pipe(map(res => res.json()));
    }
}
