import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BrandingService } from '../../services/branding.service';

@Component({
  selector: 'app-rsvp-landing',
  templateUrl: './rsvp-landing.component.html',
  styleUrls: []
})
export class RsvpLandingComponent implements OnInit {
  token: string | null = null;
  guestData: any = null;
  status: 'pending' | 'confirmed' | 'declined' | 'loading' | 'error' = 'loading';
  schoolName: string = '';

  private apiUrl = 'http://localhost:5000/api';

  constructor(
    private route: ActivatedRoute, 
    private http: HttpClient,
    public branding: BrandingService
  ) {}

  ngOnInit() {
    // In a real router setup, we'd use this.route.snapshot.paramMap.get('token')
    // For this simulation, we'll check the URL manually or use a mock
    const urlParams = new URLSearchParams(window.location.search);
    this.token = urlParams.get('token') || 'MOCK_TOKEN';

    this.loadGuestData();
    this.branding.config$.subscribe(cfg => this.schoolName = cfg?.name || '');
  }

  loadGuestData() {
    if (!this.token) {
        this.status = 'error';
        return;
    }

    // Mocking the get guest data call
    // In real app: this.http.get(`${this.apiUrl}/rsvp/guest/${this.token}`)
    setTimeout(() => {
        this.guestData = { 
            name: 'أحمد محمد طارق', 
            eventTitle: 'حفل تكريم المتميزين',
            eventDate: '2026-05-15',
            location: 'القاعة الرئيسية بالمجمع'
        };
        this.status = 'pending';
    }, 1000);
  }

  confirm() {
    this.status = 'loading';
    this.http.get(`${this.apiUrl}/rsvp/confirm/${this.token}`).subscribe({
      next: (res: any) => {
        this.status = 'confirmed';
      },
      error: () => {
        this.status = 'error';
      }
    });
  }

  decline() {
    this.status = 'loading';
    this.http.get(`${this.apiUrl}/rsvp/decline/${this.token}`).subscribe({
      next: () => {
        this.status = 'declined';
      },
      error: () => {
        this.status = 'error';
      }
    });
  }

  getQRCodeUrl() {
    return `${this.apiUrl}/events/${this.guestData?.eventId}/qr/${this.token}`;
  }

  sendToWhatsApp() {
    const text = encodeURIComponent(`هذه هي تذكرة حضوري لفعالية ${this.guestData?.eventTitle}. نراكم هناك!`);
    window.open(`https://wa.me/?text=${text}`, '_blank');
  }
}
