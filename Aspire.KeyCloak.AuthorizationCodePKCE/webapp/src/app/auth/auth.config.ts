
import { AuthConfig }  from 'angular-oauth2-oidc';
import { AppConfigService } from '../services/app-config.service';

export function buildAuthConfig(cfg: AppConfigService): AuthConfig {
  const o = cfg.oidcConfig;

  return {
    issuer: o.issuer,
    clientId: o.client_id,
    responseType: o.response_type,
    redirectUri: o.redirectUri,
    scope: o.scopes,
    disablePKCE: false, 
    strictDiscoveryDocumentValidation:o.strictDiscoveryDocumentValidation,
    showDebugInformation: true
  };
}