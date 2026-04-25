import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { InvitationService } from '../../services/invitation.service';
import { BrandingService } from '../../services/branding.service';

@Component({
  selector: 'app-dashboard',
  template: `
    <div class="dashboard-shell" [dir]="branding.currentLang === 'ar' ? 'rtl' : 'ltr'">
      
      <!-- Top Navigation -->
      <nav class="top-nav">
        <div class="nav-brand">
           <img [src]="(branding.config$ | async)?.logoUrl" height="50">
           <span style="font-weight: 800; font-size: 1.2rem; color: var(--primary-color);">{{ branding.currentLang === 'ar' ? 'منصة بناء - الدعوات' : 'Benaa - Invitations' }}</span>
        </div>
        <div class="nav-links">
           <button class="nav-item" [class.active]="activeTab === 'dashboard'" (click)="activeTab = 'dashboard'">🏠 {{ branding.currentLang === 'ar' ? 'الرئيسية' : 'Dashboard' }}</button>
           <button *ngIf="userRole === 'Admin'" class="nav-item" [class.active]="activeTab === 'reports'" (click)="activeTab = 'reports'">📊 {{ branding.currentLang === 'ar' ? 'التقارير' : 'Reports' }}</button>
           <button *ngIf="userRole === 'Admin'" class="nav-item" [class.active]="activeTab === 'students'" (click)="activeTab = 'students'">👥 {{ branding.currentLang === 'ar' ? 'الطلاب' : 'Students' }}</button>
           <button *ngIf="userRole === 'Admin'" class="nav-item" [class.active]="activeTab === 'users'" (click)="activeTab = 'users'">👮 {{ branding.currentLang === 'ar' ? 'المستخدمين' : 'Users' }}</button>
        </div>
        <div style="display: flex; gap: 1rem;">
           <button *ngIf="userRole === 'Admin'" class="btn-create" (click)="activeTab = 'events'" style="background: var(--secondary-color);">+ {{ branding.currentLang === 'ar' ? 'إنشاء فعالية' : 'Create Event' }}</button>
           <button class="btn-outline" (click)="switchRole('Admin')">🔄 خروج</button>
        </div>
      </nav>

      <div class="main-view animate-fade" [ngSwitch]="activeTab">
        
        <!-- DASHBOARD TAB -->
        <div *ngSwitchCase="'dashboard'">
           <div class="stats-row">
             <div class="stat-card"><h3>الفعاليات النشطة</h3><p class="value">{{ events.length }}</p></div>
             <div class="stat-card primary"><h3>إجمالي المدعوين</h3><p class="value">{{ totalGuests }}</p></div>
             <div class="stat-card success"><h3>تم التأكيد</h3><p class="value">{{ confirmedCount }}</p></div>
           </div>

           <div class="events-section">
              <h2 style="margin-bottom: 2rem;">قائمة الفعاليات النشطة</h2>
              <div class="event-grid">
                <div *ngFor="let event of events" class="event-card" (click)="openDesignPreview(event)">
                  <div class="event-date">📅 {{ event.eventDate | date:'mediumDate' }}</div>
                  <h3>{{ event.title }}</h3>
                  <div class="event-footer">
                    <span>{{ event.guests?.length || 0 }} مدعو</span>
                    <button class="btn-manage" (click)="openImportModal(event.id); $event.stopPropagation()">إدارة</button>
                  </div>
                </div>
              </div>
           </div>
        </div>

        <!-- REPORTS TAB -->
        <div *ngSwitchCase="'reports'">
           <app-reports></app-reports>
        </div>

        <!-- STUDENTS TAB -->
        <div *ngSwitchCase="'students'">
           <app-student-management></app-student-management>
        </div>

        <!-- USERS TAB -->
        <div *ngSwitchCase="'users'">
           <app-user-management></app-user-management>
        </div>

        <!-- NEW EVENT / EVENTS TAB -->
        <div *ngSwitchCase="'events'">
           <div class="glass-card" style="max-width: 700px; margin: 0 auto; padding: 3rem;">
              <h2>خطوات إنشاء فعالية ذكية</h2>
              <p style="color: var(--text-muted); margin-bottom: 2rem;">ارفع تصميمك المخصص أو اختر من قوالبنا الجاهزة.</p>
              
              <div class="form-group">
                 <label>1. عنوان الفعالية</label>
                 <input type="text" class="form-input" placeholder="اكتب عنوان جذاب">
              </div>

              <!-- Unified Design Section -->
              <div class="design-section" style="margin-top: 2rem;">
                  <label>2. طريقة التصميم</label>
                  <div style="display: flex; gap: 1rem; margin-top: 1rem;">
                      <button class="btn" [class.btn-primary]="!isCustomTemplate" (click)="isCustomTemplate = false">القوالب الجاهزة (AI)</button>
                      <button class="btn" [class.btn-primary]="isCustomTemplate" (click)="isCustomTemplate = true">رفع تصميم خاص (Custom)</button>
                  </div>

                  <div *ngIf="isCustomTemplate" style="margin-top: 2rem; border: 2px dashed #cbd5e1; padding: 2rem; border-radius: 20px; text-align: center;">
                      <input type="file" (change)="onFileSelected($event)" accept="image/*" id="fileLogo" class="hidden">
                      <label for="fileLogo" class="btn btn-outline" style="cursor: pointer;">📤 اختر صورة التصميم</label>
                      <div *ngIf="selectedFile" style="margin-top: 1.5rem; position: relative; border: 1px solid #eee; display: inline-block;">
                           <p style="font-size: 0.8rem; color: #64748b; margin-bottom: 1rem;">انقر على المكان الذي تود ظهور اسم الضيف فيه:</p>
                           <img [src]="'/assets/mock-template.jpg'" style="max-width: 100%; border-radius: 10px; cursor: crosshair;" (click)="setCoordinates($event)">
                           <div *ngIf="nameX > 0" [style.left.px]="nameX" [style.top.px]="nameY" style="position: absolute; background: var(--secondary-color); color: white; padding: 4px 8px; border-radius: 4px; font-size: 10px;">[ اسم الضيف ]</div>
                      </div>
                  </div>
              </div>

              <div style="margin-top: 3rem;">
                 <button class="btn-create" style="width: 100%;" (click)="activeTab = 'dashboard'">إطلاق الفعالية ومتابعة المدعوين 🚀</button>
              </div>
           </div>
        </div>
      </div>

      <!-- Import Modal -->
      <div *ngIf="showImportModal" class="modal-overlay">
        <!-- Existing Import Modal Content -->
      </div>

      <!-- Preview Invitation Modal -->
      <div *ngIf="selectedEventForPreview" class="modal-overlay" (click)="selectedEventForPreview = null">
        <div (click)="$event.stopPropagation()" style="transform: scale(0.85); transform-origin: center;">
           <app-invitation-design [event]="selectedEventForPreview"></app-invitation-design>
        </div>
      </div>

    </div>
  `,
  styles: [`
    .dashboard-shell { min-height: 100vh; background: #f8fafc; font-family: 'Inter', 'Amiri', sans-serif; }
    .top-nav { background: #fff; padding: 1rem 5%; display: flex; align-items: center; justify-content: space-between; position: sticky; top: 0; z-index: 100; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.05); }
    .nav-links { display: flex; gap: 1.5rem; }
    .nav-item { background: transparent; border: none; font-weight: 700; color: #64748b; cursor: pointer; padding: 0.6rem 1.2rem; border-radius: 12px; transition: all 0.3s; }
    .nav-item.active { color: var(--primary-color); background: rgba(99,102,241,0.05); }
    .main-view { padding: 3rem 5%; }
    
    .stats-row { display: grid; grid-template-columns: repeat(3, 1fr); gap: 2rem; margin-bottom: 4rem; }
    .stat-card { background: white; padding: 2rem; border-radius: 20px; box-shadow: 0 10px 15px -3px rgba(0,0,0,0.02); text-align: center; }
    .stat-card .value { font-size: 2.5rem; font-weight: 900; color: var(--primary-color); }
    .stat-card.primary { border-top: 6px solid var(--primary-color); }
    .stat-card.success { border-top: 6px solid #10b981; }

    .event-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); gap: 2rem; }
    .event-card { background: white; padding: 2rem; border-radius: 24px; border: 1px solid #f1f5f9; cursor: pointer; transition: 0.3s; }
    .event-card:hover { transform: translateY(-8px); box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1); }
    .btn-create { background: var(--secondary-color); color: white; border: none; padding: 0.8rem 1.5rem; border-radius: 12px; font-weight: 700; cursor: pointer; }
    .btn-outline { background: #f8fafc; border: 1px solid #e2e8f0; color: var(--primary-color); padding: 0.6rem 1.2rem; border-radius: 10px; font-weight: 700; cursor: pointer; }
    .hidden { display: none; }
    .modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.4); backdrop-filter: blur(8px); display: flex; align-items: center; justify-content: center; z-index: 2000; }
  `]
})
export class DashboardComponent implements OnInit {
  events: any[] = [];
  selectedEventForPreview: any = null;
  totalGuests = 0;
  confirmedCount = 0;
  activeTab: 'dashboard' | 'reports' | 'events' | 'students' | 'users' = 'dashboard';

  isCustomTemplate = false;
  selectedFile: File | null = null;
  nameX = 0;
  nameY = 0;

  grades: string[] = ['المستوى الأول', 'المستوى الثاني', 'الصف الأول', 'الصف الخامس'];
  sections: string[] = ['القسم الدولي - بنات', 'القسم العام - بنين'];
  selectedGrade: string = '';
  selectedSection: string = '';
  showImportModal = false;
  activeEventId: number | null = null;

  userRole: 'Admin' | 'Scanner' = 'Admin'; 

  private apiUrl = 'http://localhost:5000/api';

  constructor(
    private invitationService: InvitationService,
    private http: HttpClient,
    public branding: BrandingService
  ) {}

  ngOnInit() {
    this.loadData();
    // Simulate role check from auth service
    const savedRole = localStorage.getItem('userRole') as any;
    if (savedRole) this.userRole = savedRole;
  }

  switchRole(role: 'Admin' | 'Scanner') {
    this.userRole = role;
    localStorage.setItem('userRole', role);
    this.activeTab = (role === 'Scanner') ? 'dashboard' : 'dashboard'; 
  }

  loadData() {
    this.invitationService.getEvents().subscribe(events => {
      this.events = events;
      this.calculateStats();
    });
  }

  calculateStats() {
    this.totalGuests = this.events.reduce((sum, e) => sum + (e.guests?.length || 0), 0);
    this.confirmedCount = this.events.reduce((sum, e) => 
      sum + (e.guests?.filter((g: any) => g.status === 1).length || 0), 0);
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
    this.isCustomTemplate = true;
  }

  setCoordinates(event: MouseEvent) {
    const rect = (event.target as HTMLElement).getBoundingClientRect();
    this.nameX = Math.round(event.clientX - rect.left);
    this.nameY = Math.round(event.clientY - rect.top);
    console.log(`Coords: X=${this.nameX}, Y=${this.nameY}`);
  }

  openDesignPreview(event: any) {
    this.selectedEventForPreview = event;
  }

  openImportModal(id: number) {
    this.activeEventId = id;
    this.showImportModal = true;
  }

  closeImportModal() {
    this.showImportModal = false;
    this.activeEventId = null;
  }

  executeImport() {
    if (!this.activeEventId || !this.selectedGrade) return;

    this.http.post(`${this.apiUrl}/events/${this.activeEventId}/import-students`, null, {
        params: { grade: this.selectedGrade, section: this.selectedSection }
    }).subscribe({
      next: (res: any) => {
        alert('تم استيراد الطلاب بنجاح');
        this.closeImportModal();
        this.loadData();
      },
      error: (err) => alert('خطأ في الاستيراد')
    });
  }
}
