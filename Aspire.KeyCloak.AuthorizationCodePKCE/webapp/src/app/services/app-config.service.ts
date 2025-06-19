import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { first, firstValueFrom } from "rxjs";

@Injectable({providedIn:'root'})
export class AppConfigService {
     private config: any = {
        oidc: {
            issuer: "http://localhost:8080/realms/maransys",
            client_id: "angular-app",
            redirectUri: "http://localhost:4200",
            scopes: "openid profile email",
            response_type: "code",
            usePkce: true,
            strictDiscoveryDocumentValidation: false
        },
        api: {
            baseUrl: "http://localhost:5196"
        }
    };

    constructor(private http: HttpClient) {}

    loadConfig(): Promise<void> {
        // Skip HTTP request during SSR to avoid timeout
        if (typeof window === 'undefined') {
            return Promise.resolve();
        }
         return firstValueFrom(this.http.get('/assets/config.json'))
            .then(cfg => { this.config = cfg })
            .catch(err => {
                console.warn('Failed to load config, using defaults', err);
                // Continue with default config
            });           
    }

    get oidcConfig() {
        return this.config.oidc;   
    }
    
    get apiConfig(){
        return this.config.api;
    }
}