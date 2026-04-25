import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StudentsService } from '../../services/students.service';
import { BrandingService } from '../../services/branding.service';

@Component({
  selector: 'app-student-management',
  template: `
    <div class="management-container" [dir]="branding.currentLang === 'ar' ? 'rtl' : 'ltr'">
      <header style="margin-bottom: 2rem; display: flex; justify-content: space-between; align-items: center;">
        <div>
          <h1>👥 إدارة الطلاب وأولياء الأمور</h1>
          <p style="color: #64748b;">قائمة شاملة لكافة المسجلين في النظام التعليمي.</p>
        </div>
        <div class="actions">
          <button class="btn btn-primary" (click)="loadStudents()">🔄 تحديث البيانات</button>
        </div>
      </header>

      <!-- Filters -->
      <div class="filters-row" style="display: flex; gap: 1rem; margin-bottom: 2rem; background: #fff; padding: 1.5rem; border-radius: 12px; border: 1px solid #e2e8f0;">
        <input type="text" placeholder="بحث بالاسم أو الهوية..." style="flex: 2; padding: 0.8rem; border-radius: 8px; border: 1px solid #cbd5e1;">
        <select style="flex: 1; padding: 0.8rem; border-radius: 8px; border: 1px solid #cbd5e1;">
          <option value="">-- كل الصفوف --</option>
          <option *ngFor="let g of (grades$ | async)" [value]="g">{{ g }}</option>
        </select>
        <select style="flex: 1; padding: 0.8rem; border-radius: 8px; border: 1px solid #cbd5e1;">
          <option value="">-- كل الأقسام --</option>
          <option *ngFor="let s of (sections$ | async)" [value]="s">{{ s }}</option>
        </select>
      </div>

      <!-- Students Table -->
      <div class="glass-card" style="padding: 0; overflow: hidden;">
        <table style="width: 100%; border-collapse: collapse; text-align: right;">
          <thead>
            <tr style="background: #f8fafc; border-bottom: 2px solid #e2e8f0;">
              <th style="padding: 1.2rem;">اسم الطالب (AR)</th>
              <th style="padding: 1.2rem;">اسم ولي الأمر</th>
              <th style="padding: 1.2rem;">رقم الهاتف</th>
              <th style="padding: 1.2rem;">المرحلة / الصف</th>
              <th style="padding: 1.2rem;">القسم / المدرسة</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let student of students" style="border-bottom: 1px solid #f1f5f9;">
              <td style="padding: 1.2rem; font-weight: 700;">{{ student.nameAr }}</td>
              <td style="padding: 1.2rem;">{{ student.parentName }}</td>
              <td style="padding: 1.2rem; font-family: monospace;">{{ student.phone }}</td>
              <td style="padding: 1.2rem;"><span class="badge">{{ student.grade }}</span></td>
              <td style="padding: 1.2rem; color: #64748b;">{{ student.section }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .management-container { padding: 1rem; }
    .badge { background: #f1f5f9; padding: 4px 10px; border-radius: 20px; font-size: 0.85rem; font-weight: 600; color: var(--primary-color); }
    .btn { padding: 0.8rem 1.5rem; border-radius: 10px; border: none; font-weight: 700; cursor: pointer; transition: all 0.3s; }
    .btn-primary { background: var(--secondary-color); color: white; }
  `]
})
export class StudentManagementComponent implements OnInit {
  students: any[] = [];
  grades$: any;
  sections$: any;

  constructor(
    private http: HttpClient,
    public branding: BrandingService,
    private studentService: StudentsService // Assuming we've made the backend StudentService reachable via API
  ) {
    this.grades$ = this.studentService.getGradesAsync(); // Need to adapt to return Observable if using API
    this.sections$ = this.studentService.getSectionsAsync();
  }

  ngOnInit() {
    this.loadStudents();
  }

  loadStudents() {
    // Calling backend GetStudents
    this.http.get<any[]>('http://localhost:5000/api/students').subscribe(data => this.students = data);
  }
}
