import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UploadService {
  private readonly apiUrl = 'http://localhost:5149/api';

  constructor(private _http: HttpClient) { }

  encrypt(file: File, message: string): Observable<Blob> {
    if (!file || !message || !message.length) throw new Error('Invalid encrypt arguments.');

    const formData = new FormData();
    formData.append('file', file);
    formData.append('message', message);

    return this._http.post(`${this.apiUrl}/encrypt`, formData, { responseType: 'blob' });
  }

  decrypt(file: File): Observable<string> {
    if (!file) throw new Error('Invalid decrypt argument.');

    const formData = new FormData();
    formData.append('file', file);

    return this._http.post(`${this.apiUrl}/decrypt`, formData, { responseType: 'text' });
  }
}
