import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BrandingService } from '../../services/branding.service';

@Component({
  selector: 'app-user-management',
  template: `
    <div class="user-management-container" [dir]="branding.currentLang === 'ar' ? 'rtl' : 'ltr'">
      <header style="margin-bottom: 2rem; display: flex; justify-content: space-between; align-items: center;">
        <div>
          <h1>👮 إدارة المستخدمين</h1>
          <p style="color: #64748b;">إدارة حسابات الموظفين والمنظمين للمدرسة.</p>
        </div>
        <button class="btn btn-primary" (click)="showCreateModal = true">+ إضافة مستخدم جديد</button>
      </header>

      <div class="glass-card" style="padding: 0; overflow: hidden;">
        <table style="width: 100%; border-collapse: collapse; text-align: right;">
          <thead>
            <tr style="background: #f8fafc; border-bottom: 2px solid #e2e8f0;">
              <th style="padding: 1.2rem;">الاسم</th>
              <th style="padding: 1.2rem;">البريد الإلكتروني</th>
              <th style="padding: 1.2rem;">الدور (Role)</th>
              <th style="padding: 1.2rem;">الحالة</th>
              <th style="padding: 1.2rem;">الإجراءات</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let user of users" style="border-bottom: 1px solid #f1f5f9;">
              <td style="padding: 1.2rem; font-weight: 700;">{{ user.name }}</td>
              <td style="padding: 1.2rem;">{{ user.email }}</td>
              <td style="padding: 1.2rem;"><span class="badge">{{ user.role }}</span></td>
              <td style="padding: 1.2rem;">
                  <span [style.color]="user.isActive ? '#10b981' : '#ef4444'" style="font-weight: 700;">
                    ● {{ user.isActive ? 'نشط' : 'غير نشط' }}
                  </span>
              </td>
              <td style="padding: 1.2rem;">
                 <button class="btn-delete" (click)="deleteUser(user.id)">Hذف</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Create User Modal -->
      <div *ngIf="showCreateModal" class="modal-overlay">
        <div class="modal">
          <h3>إضافة مستخدم جديد</h3>
          <div class="form-group">
            <label>الاسم الكامل</label>
            <input type="text" [(ngModel)]="newUser.name" class="form-input">
          </div>
          <div class="form-group">
            <label>البريد الإلكتروني</label>
            <input type="email" [(ngModel)]="newUser.email" class="form-input">
          </div>
          <div class="form-group">
            <label>الدور</label>
            <select [(ngModel)]="newUser.role" class="form-input">
              <option value="Admin">مدير (Admin)</option>
              <option value="Scanner">منظم بوابات (Scanner)</option>
              <option value="Viewer">مشاهد (Viewer)</option>
            </select>
          </div>
          <div class="modal-actions">
            <button class="btn" (click)="showCreateModal = false">إلغاء</button>
            <button class="btn btn-primary" (click)="saveUser()">تأكيد الإضافة</button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .user-management-container { padding: 1rem; }
    .badge { background: #fef3c7; color: #92400e; padding: 4px 10px; border-radius: 20px; font-size: 0.8rem; font-weight: 800; }
    .btn-delete { background: #fee2e2; color: #dc2626; border: none; padding: 0.4rem 1rem; border-radius: 8px; cursor: pointer; }
    .modal-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 2000; }
    .modal { background: #fff; padding: 2.5rem; border-radius: 24px; width: 400px; box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1); }
    .form-input { width: 100%; padding: 0.8rem; border-radius: 10px; border: 1px solid #e2e8f0; margin-top: 5px; }
  `]
})
export class UserManagementComponent implements OnInit {
  users: any[] = [];
  showCreateModal = false;
  newUser = { name: '', email: '', role: 'Viewer' };

  private apiUrl = 'http://localhost:5000/api/users';

  constructor(private http: HttpClient, public branding: BrandingService) {}

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.http.get<any[]>(this.apiUrl).subscribe(data => this.users = data);
  }

  saveUser() {
    this.http.post(this.apiUrl, this.newUser).subscribe(() => {
      this.loadUsers();
      this.showCreateModal = false;
      this.newUser = { name: '', email: '', role: 'Viewer' };
    });
  }

  deleteUser(id: number) {
    if (confirm('هل أنت متأكد من حذف هذا المستخدم؟')) {
      this.http.delete(`${this.apiUrl}/${id}`).subscribe(() => this.loadUsers());
    }
  }
}
