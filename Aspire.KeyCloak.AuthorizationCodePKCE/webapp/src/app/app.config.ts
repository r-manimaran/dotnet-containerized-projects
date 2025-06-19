import { APP_INITIALIZER, ApplicationConfig, importProvidersFrom, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { OAuthModule }  from 'angular-oauth2-oidc';
import { AppConfigService } from './services/app-config.service';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideHttpClient, withFetch } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes), provideClientHydration(withEventReplay()),
    provideHttpClient(withFetch()),
    importProvidersFrom(OAuthModule.forRoot({
      resourceServer: {
        allowedUrls:[],
        sendAccessToken: true,
      }
      
    })),
    {
      provide: APP_INITIALIZER,
      useFactory: (cfg: AppConfigService) => () => cfg.loadConfig(),
      deps: [AppConfigService],
      multi: true
    }
  ]
};


