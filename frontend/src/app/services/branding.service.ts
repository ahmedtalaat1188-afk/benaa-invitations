import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

export interface BrandingConfig {
  name: string;
  primaryColor: string;
  secondaryColor: string;
  logoUrl: string;
}

@Injectable({
  providedIn: 'root'
})
export class BrandingService {
  private get apiUrl(): string {
    // Dynamically detect API host or use environment
    return `${window.location.origin.replace(':4200', ':5000')}/api`;
  }
  
  private configSubject = new BehaviorSubject<BrandingConfig | null>(null);
  config$ = this.configSubject.asObservable();

  constructor(private http: HttpClient) {}

  loadConfig(): Observable<BrandingConfig> {
    return this.http.get<BrandingConfig>(`${this.apiUrl}/branding/config`).pipe(
      tap(config => {
        this.configSubject.next(config);
        this.applyBranding(config);
      })
    );
  }

  applyBranding(config: BrandingConfig) {
    const root = document.documentElement;
    root.style.setProperty('--primary', config.primaryColor);
    root.style.setProperty('--secondary', config.secondaryColor);
    root.style.setProperty('--brand-logo', `url(${config.logoUrl})`);
    
    // For legacy support if needed
    root.style.setProperty('--primary-color', config.primaryColor);
    root.style.setProperty('--secondary-color', config.secondaryColor);

    document.title = `${config.name} | Benaa`;
  }

  setLanguage(lang: 'ar' | 'en') {
    document.documentElement.lang = lang;
    document.documentElement.dir = lang === 'ar' ? 'rtl' : 'ltr';
    localStorage.setItem('lang', lang);
  }

  get currentLang(): 'ar' | 'en' {
    return (localStorage.getItem('lang') as 'ar' | 'en') || 'ar';
  }
}
