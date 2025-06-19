import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { OAuthService } from "angular-oauth2-oidc";
import { Observable } from "rxjs";
import { ApiBaseService } from "./api-base.service";


export interface WeatherForecast {
    date: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;

}

@Injectable({providedIn:'root'})
export class WeatherForecastService extends ApiBaseService {
    private apiUrl ='http://localhost:5196/api/weatherforecast';

    constructor(http: HttpClient, oauthService: OAuthService) {
        super(http, oauthService);
     }

    getweatherForecast(): Observable<WeatherForecast[]>{
        
        return this.get<WeatherForecast[]>('weatherforecast');
    }
}