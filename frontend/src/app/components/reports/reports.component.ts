import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BrandingService } from '../../services/branding.service';

@Component({
  selector: 'app-reports',
  template: `
    <div class="reports-container" [dir]="branding.currentLang === 'ar' ? 'rtl' : 'ltr'">
      <div class="header" style="margin-bottom: 2rem;">
        <h1>📊 التقارير والإحصائيات</h1>
        <p>متابعة أداء الدعوات ونسب الاستجابة للفعاليات المختلفة.</p>
      </div>

      <div class="stats-grid" style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 2rem; margin-bottom: 3rem;">
        <div class="report-card">
          <h3>توزيع حالة الدعوات</h3>
          <div class="chart-mock" style="height: 200px; background: #f8fafc; border-radius: 12px; display: flex; align-items: center; justify-content: center; position: relative;">
             <div style="width: 150px; height: 150px; border-radius: 50%; border: 20px solid var(--secondary-color); border-top-color: #d1d5db;"></div>
             <div style="position: absolute; font-weight: 800; font-size: 1.5rem;">75%</div>
          </div>
          <div style="margin-top: 1rem; font-size: 0.9rem; color: var(--text-muted);">تم تأكيد 75% من إجمالي الدعوات المرسلة.</div>
        </div>

        <div class="report-card">
          <h3>أوقات الذروة للحضور</h3>
          <div style="display: flex; align-items: flex-end; gap: 8px; height: 150px; padding-bottom: 1rem; border-bottom: 2px solid #e2e8f0; margin-top: 1rem;">
             <div style="flex:1; background: var(--secondary-color); height: 30%;"></div>
             <div style="flex:1; background: var(--secondary-color); height: 60%;"></div>
             <div style="flex:1; background: var(--primary-color); height: 90%;"></div>
             <div style="flex:1; background: var(--secondary-color); height: 40%;"></div>
          </div>
          <p style="margin-top: 1rem; font-size: 0.8rem;">تحليل زمني لعمليات مسح الـ QR عند البوابة.</p>
        </div>
      </div>

      <div class="event-summary">
        <h3>ملخص الفعاليات النشطة</h3>
        <table style="width: 100%; margin-top: 1.5rem; text-align: right;">
          <thead>
            <tr style="border-bottom: 1px solid #e2e8f0;">
              <th>الفعالية</th>
              <th>المدعوين</th>
              <th>المؤكدين</th>
              <th>المعتذرين</th>
              <th>الإجراء</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let event of events" style="border-bottom: 1px solid #f1f5f9;">
              <td style="padding: 1rem;">{{ event.title }}</td>
              <td>{{ event.guests?.length || 0 }}</td>
              <td style="color: #10b981; font-weight: 700;">{{ getConfirmed(event) }}</td>
              <td style="color: #dc2626;">{{ getDeclined(event) }}</td>
              <td><button class="btn-export" (click)="export(event.id)">تصدير CSV</button></td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .reports-container { padding: 2rem; background: white; border-radius: 24px; box-shadow: 0 4px 20px rgba(0,0,0,0.05); }
    .report-card { background: #fff; padding: 1.5rem; border: 1px solid #e2e8f0; border-radius: 16px; transition: shadow 0.3s; }
    .report-card:hover { box-shadow: 0 10px 15px rgba(0,0,0,0.05); }
    .btn-export { background: #f1f5f9; color: var(--text-main); border: none; padding: 0.5rem 1rem; border-radius: 8px; cursor: pointer; font-weight: 700; transition: background 0.3s; }
    .btn-export:hover { background: #e2e8f0; }
  `]
})
export class ReportsComponent implements OnInit {
  events: any[] = [];
  constructor(private http: HttpClient, public branding: BrandingService) {}

  ngOnInit() {
    this.http.get<any[]>('http://localhost:5000/api/events').subscribe(data => this.events = data);
  }

  getConfirmed(event: any) {
    return event.guests?.filter((g: any) => g.status === 1).length || 0;
  }

  getDeclined(event: any) {
    return event.guests?.filter((g: any) => g.status === 2).length || 0;
  }

  export(id: number) {
    window.open(`http://localhost:5000/api/events/${id}/export-report`, '_blank');
  }
}
