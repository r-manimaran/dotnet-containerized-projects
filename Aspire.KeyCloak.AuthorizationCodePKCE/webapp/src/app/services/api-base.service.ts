import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { OAuthService } from "angular-oauth2-oidc";
import { Observable } from "rxjs";

@Injectable({providedIn: 'root'})
export class ApiBaseService {
    
    protected baseUrl: string = 'http://localhost:5196/api';

    constructor(protected http: HttpClient, protected oauthService: OAuthService) {}

    protected getAuthHeaders(): HttpHeaders {
        return new HttpHeaders({
            'Authorization': 'Bearer ' + this.oauthService.getAccessToken()
        });    
    }
    
    protected get<T>(endpoint: string): Observable<T> {
        return this.http.get<T>(`${this.baseUrl}/${endpoint}`,{
            headers: this.getAuthHeaders()
        });
    }

}