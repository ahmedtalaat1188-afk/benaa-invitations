import { Component, OnInit } from '@angular/core';
import { BrandingService } from './services/branding.service';

@Component({
  selector: 'app-root',
  template: `
    <div [dir]="branding.currentLang === 'ar' ? 'rtl' : 'ltr'">
      <header class="app-header">
        <img [src]="(branding.config$ | async)?.logoUrl" alt="School Logo" class="logo">
        <h1>{{ (branding.config$ | async)?.name }}</h1>
        <button (click)="toggleLanguage()">{{ branding.currentLang === 'ar' ? 'English' : 'العربية' }}</button>
      </header>
      
      <main class="content">
        <!-- Conditional Routing Simulation -->
        <div *ngIf="currentView === 'dashboard'">
          <app-dashboard></app-dashboard>
        </div>
        <div *ngIf="currentView === 'rsvp'">
          <app-rsvp-landing></app-rsvp-landing>
        </div>
        <div *ngIf="currentView === 'scanner'">
          <app-scanner></app-scanner>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .app-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem 2rem;
      background: var(--primary-color);
      color: white;
    }
    .logo { height: 50px; }
    .content { padding: 2rem; }
  `]
})
export class AppComponent implements OnInit {
  currentView: 'dashboard' | 'rsvp' | 'scanner' = 'dashboard';

  constructor(public branding: BrandingService) {
    const path = window.location.pathname;
    if (path.includes('rsvp')) this.currentView = 'rsvp';
    if (path.includes('scanner')) this.currentView = 'scanner';
  }

  ngOnInit() {
    this.branding.loadConfig().subscribe();
    this.branding.setLanguage(this.branding.currentLang);
  }

  toggleLanguage() {
    const newLang = this.branding.currentLang === 'ar' ? 'en' : 'ar';
    this.branding.setLanguage(newLang);
  }
}
