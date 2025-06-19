import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { AppConfigService } from './services/app-config.service';
import { buildAuthConfig } from './auth/auth.config';
import { NgIf,NgFor } from '@angular/common';
import { WeatherForecast, WeatherForecastService } from './services/weather.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NgIf, NgFor],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected title = 'webapp';
  auth = inject(OAuthService);
  cfg = inject(AppConfigService);
  weatherService = inject(WeatherForecastService);
  weatherData: WeatherForecast[] = [];
  
  constructor()
    {
      const authConfig = buildAuthConfig(this.cfg);
      
      this.auth.configure(authConfig);
      
      // auth.setupAutomaticSilentRefresh();

      // (auth.resourceServer.setAllowedUrls as any )(
      //   [`${cfg.apiConfig}/api)`]
      // );
      this.auth.loadDiscoveryDocumentAndTryLogin();
    }

    ngOnInit() {
      if(this.isLoggedIn()){
        this.fetchWeatherData();
      }
    }
    isLoggedIn() {
      return this.auth.hasValidAccessToken();
    }
    
    login() {
      this.auth.initCodeFlow();
    }
    
    logout() {
      this.auth.logOut();    
    }
    fetchWeatherData() {
      console.log('Fetching weather data...');
      this.weatherService.getweatherForecast().subscribe({
        next: (data) => this.weatherData = data,
        error: (err) => console.error('Error fetching weather data:', err)
      });
    }
    
    getUserName(): string {
      const claims = this.auth.getIdentityClaims();
      if(claims) {
        return claims['preferred_username'] || claims['name'] || claims['email'] || 'User';
      }
      return 'User';
    }
}
