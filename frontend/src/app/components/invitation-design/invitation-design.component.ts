import { Component, Input, OnInit } from '@angular/core';
import { BrandingService } from '../../services/branding.service';

@Component({
  selector: 'app-invitation-design',
  template: `
    <div class="invitation-card" [ngClass]="style" [dir]="'rtl'" style="width: 100%; max-width: 500px; min-height: 700px; padding: 3rem; border-radius: 30px; position: relative; overflow: hidden; background: #fff; box-shadow: 0 40px 100px rgba(0,0,0,0.1); margin: 0 auto;">
      
      <!-- Design Decorative Elements -->
      <div class="deco deco-top"></div>
      <div class="deco deco-bottom"></div>

      <!-- School Logo & Branding -->
      <div style="text-align: center; margin-bottom: 3rem; position: relative; z-index: 5;">
        <img [src]="(branding.config$ | async)?.logoUrl" height="100" style="margin-bottom: 1rem;">
        <h2 style="font-size: 1.2rem; color: var(--primary-color);">{{ schoolName }}</h2>
        <div style="width: 60px; height: 3px; background: var(--secondary-color); margin: 1rem auto;"></div>
      </div>

      <!-- Invitation Content -->
      <div class="content" style="text-align: center; position: relative; z-index: 5;">
        <div style="font-size: 1.1rem; color: #1e293b; margin-bottom: 1rem;">نتشرف بدعوتكم لحضور</div>
        <h1 style="font-size: 2.2rem; font-weight: 900; color: #000; margin-bottom: 2rem;">{{ event?.title }}</h1>
        
        <div style="background: rgba(0,0,0,0.03); padding: 2rem; border-radius: 20px; border: 1px dashed rgba(0,0,0,0.1); margin-bottom: 3rem;">
           <div style="font-size: 1.2rem; font-weight: 700;">{{ guestName }}</div>
           <p style="font-size: 0.9rem; color: #64748b; margin-top: 1rem;">يسعدنا جداً حضوركم ومشاركتكم لنا هذه اللحظات المميزة.</p>
        </div>

        <!-- Event Details -->
        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; text-align: right; margin-bottom: 3rem;">
          <div>
             <span style="font-size: 0.8rem; color: #94a3b8;">📅 التاريخ</span>
             <div style="font-weight: 700;">{{ event?.eventDate | date:'fullDate' }}</div>
          </div>
          <div>
             <span style="font-size: 0.8rem; color: #94a3b8;">📍 الموقع</span>
             <div style="font-weight: 700;">{{ event?.location }}</div>
          </div>
        </div>

        <div class="qr-placeholder" style="margin-top: 3rem;">
           <div style="width: 120px; height: 120px; background: #fff; padding: 10px; border: 1px solid #e2e8f0; border-radius: 12px; margin: 0 auto; display: flex; align-items: center; justify-content: center;">
              <span style="color: #cbd5e1; font-size: 0.8rem;">رابط الـ QR</span>
           </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .invitation-card { transition: all 0.5s ease; border: 12px solid transparent; }
    .deco { position: absolute; width: 300px; height: 300px; border-radius: 50%; opacity: 0.05; filter: blur(60px); z-index: 1; }
    .deco-top { top: -100px; right: -100px; background: var(--primary-color); }
    .deco-bottom { bottom: -100px; left: -100px; background: var(--secondary-color); }

    /* AI Styles */
    .classic { border-color: rgba(99,102,241,0.1); background: linear-gradient(135deg, #fff 0%, #f5f3ff 100%); }
    .elegant { border-color: #f59e0b; font-family: 'Amiri', serif; }
    .modern { border-radius: 0; box-shadow: 0 0 0 20px #fff, 0 0 0 21px #eee; }
  `]
})
export class InvitationDesignComponent implements OnInit {
  @Input() event: any;
  @Input() guestName: string = 'أحمد طارق';
  @Input() style: 'classic' | 'elegant' | 'modern' = 'classic';
  schoolName: string = '';

  constructor(public branding: BrandingService) {}

  ngOnInit() {
    this.branding.config$.subscribe(cfg => this.schoolName = cfg?.name || '');
  }
}
