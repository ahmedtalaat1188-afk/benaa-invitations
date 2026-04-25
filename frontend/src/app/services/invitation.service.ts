import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InvitationService {
  private apiUrl = 'http://localhost:5000/api'; // Standard .NET port

  constructor(private http: HttpClient) {}

  // Events API
  getEvents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/events`);
  }

  // RSVP Logic (WhatsApp Generator)
  generateWhatsAppLink(guest: any, event: any): string {
    const baseUrl = window.location.origin;
    const confirmUrl = `${baseUrl}/rsvp/confirm/${guest.token}`;
    const declineUrl = `${baseUrl}/rsvp/decline/${guest.token}`;
    
    const message = `مرحباً ${guest.nameAr}، 
يتشرف ${event.title} بدعوتكم لحضور الفعالية المقامة في ${event.eventDate}.

للموافقة وتأكيد الحضور:
${confirmUrl}

للاعتذار:
${declineUrl}

نسعد بتواجدكم!`;

    const encodedMessage = encodeURIComponent(message);
    const phone = guest.phone.startsWith('966') ? guest.phone : `966${guest.phone}`;
    
    return `https://wa.me/${phone}?text=${encodedMessage}`;
  }

  confirmRSVP(token: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/rsvp/confirm/${token}`);
  }

  declineRSVP(token: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/rsvp/decline/${token}`);
  }

  // AI Design (Mock)
  generateAIDesign(prompt: string, style: string): Observable<string> {
    // In a real app, this would call OpenAI DALL-E or similar
    console.log(`Generating design for: ${prompt} in style: ${style}`);
    return of('assets/images/ai-generated-mock.png');
  }

  // Real Scanner Verification
  verifyToken(token: string): Observable<any> {
    // Calls the [HttpGet("verify-scan/{token}")] endpoint in EventsController
    return this.http.get(`${this.apiUrl}/events/verify-scan/${token}`);
  }
}
