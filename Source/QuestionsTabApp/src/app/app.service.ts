import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Injectable, Inject } from "@angular/core";
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { User } from './models/User';
import { Question } from './models/Question';
import { environment } from 'src/environments/environment';
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

    getUserByUpn(upn: string): Observable<User> {
        return this.http.post(this.baseUrl + 'GetUserByUpn', JSON.stringify(upn), this.options)
            .pipe(map(res => res.json()));
    }

    getAllQuestions(tenantId: string): Observable<Question[]> {
        return this.http.post(this.baseUrl + 'GetAllQuestions', JSON.stringify(tenantId), this.options)
            .pipe(map(res => res.json()));
    }

    getQuestionsByGroup(groupId: string): Observable<Question[]> {
        return this.http.post(this.baseUrl + 'GetQuestionsByGroup', JSON.stringify(groupId), this.options)
            .pipe(map(res => res.json()));
    }
}
