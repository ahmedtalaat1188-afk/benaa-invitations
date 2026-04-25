import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BrandingService } from '../../services/branding.service';

@Component({
  selector: 'app-scanner',
  templateUrl: './scanner.component.html',
  styleUrls: []
})
export class ScannerComponent implements OnInit {
  isScanning: boolean = true;
  scanResult: 'success' | 'error' | null = null;
  guestName: string = '';
  errorMessage: string = '';

  private apiUrl = 'http://localhost:5000/api';

  constructor(private http: HttpClient, public branding: BrandingService) {}

  ngOnInit() {
    // In a real app, we'd initialize the camera library here
  }

  // Simulate scanning a QR code
  onCodeScanned(token: string) {
    this.isScanning = false;
    this.http.get(`${this.apiUrl}/events/verify-scan/${token}`).subscribe({
      next: (res: any) => {
        this.scanResult = 'success';
        this.guestName = res.guestName;
        this.playAudio('success');
      },
      error: (err) => {
        this.scanResult = 'error';
        this.errorMessage = err.error?.message || 'كود غير صالح أو تم استخدامه مسبقاً';
        this.playAudio('error');
      }
    });
  }

  resetScanner() {
    this.isScanning = true;
    this.scanResult = null;
    this.guestName = '';
    this.errorMessage = '';
  }

  playAudio(type: 'success' | 'error') {
    // Simulation of audio feedback
    console.log(`Playing ${type} sound`);
  }
}
