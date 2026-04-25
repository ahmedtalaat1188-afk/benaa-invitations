import { Component, Input, OnInit } from '@angular/core';
import { InvitationService } from '../../services/invitation.service';

@Component({
  selector: 'app-guest-list',
  template: `
    <div class="guest-list-container">
      <div class="header">
        <h2>إدارة المدعوين: {{ event?.title }}</h2>
        <div class="actions">
          <button class="btn-import">استيراد من Excel</button>
          <button class="btn-add">إضافة يدويًا</button>
        </div>
      </div>

      <div class="table-card">
        <table dir="rtl">
          <thead>
            <tr>
              <th>الاسم</th>
              <th>الجوال</th>
              <th>المرحلة</th>
              <th>الحالة</th>
              <th>الإجراءات</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let guest of event?.guests">
              <td>{{ guest.name }}</td>
              <td>{{ guest.phone }}</td>
              <td>{{ guest.grade }} / {{ guest.section }}</td>
              <td>
                <span class="status-badge" [class]="getStatusClass(guest.status)">
                  {{ getStatusLabel(guest.status) }}
                </span>
              </td>
              <td>
                <button class="btn-wa" (click)="sendWhatsApp(guest)">
                  <img src="assets/images/wa-icon.png" alt="WA" width="16"> دعوة
                </button>
                <button class="btn-edit">تعديل</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .guest-list-container { 
      background: #f8fafc; padding: 2rem; border-radius: 12px; margin-top: 2rem;
      border: 1px solid #e2e8f0;
    }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; }
    .table-card { background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1); }
    table { width: 100%; border-collapse: collapse; text-align: right; }
    th { background: #f1f5f9; padding: 1rem; color: #475569; font-weight: 600; font-size: 0.9rem; }
    td { padding: 1rem; border-bottom: 1px solid #f1f5f9; color: #334155; font-size: 0.9rem; }
    
    .status-badge { padding: 0.25rem 0.75rem; border-radius: 9999px; font-size: 0.8rem; font-weight: 500; }
    .status-pending { background: #fee2e2; color: #991b1b; }
    .status-confirmed { background: #dcfce7; color: #166534; }
    .status-declined { background: #f1f5f9; color: #475569; }

    .btn-wa { 
      background: #25d366; color: white; border: none; padding: 0.4rem 0.8rem; 
      border-radius: 6px; cursor: pointer; display: inline-flex; align-items: center; gap: 0.5rem;
    }
    .btn-edit { background: none; border: 1px solid #e2e8f0; margin-right: 0.5rem; padding: 0.4rem 0.8rem; border-radius: 6px; }
    .btn-import { background: #1e293b; color: white; border: none; padding: 0.6rem 1.2rem; border-radius: 6px; margin-right: 1rem; }
    .btn-add { background: var(--primary-color); color: white; border: none; padding: 0.6rem 1.2rem; border-radius: 6px; }
  `]
})
export class GuestListComponent implements OnInit {
  @Input() event: any;

  constructor(private invitationService: InvitationService) {}

  ngOnInit() {}

  getStatusLabel(status: number) {
    if (status === 1) return 'مؤكد';
    if (status === 2) return 'معتذر';
    return 'بانتظار الرد';
  }

  getStatusClass(status: number) {
    if (status === 1) return 'status-confirmed';
    if (status === 2) return 'status-declined';
    return 'status-pending';
  }

  sendWhatsApp(guest: any) {
    // Generate WhatsApp URL with pre-filled message
    const url = this.invitationService.generateWhatsAppLink(
      guest.name, 
      guest.phone, 
      this.event.title, 
      guest.secureToken
    );
    window.open(url, '_blank');
  }
}
