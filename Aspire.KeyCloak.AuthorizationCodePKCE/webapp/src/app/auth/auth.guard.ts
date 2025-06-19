import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { OAuthService } from "angular-oauth2-oidc";

@Injectable({providedIn:'root'})
export class AuthGuard implements CanActivate {

    constructor(private auth: OAuthService, private router: Router) { }

    canActivate(): boolean {
        if(this.auth.hasValidAccessToken()){
            return true;
        }
        this.auth.initCodeFlow();
        return false;
    }
}