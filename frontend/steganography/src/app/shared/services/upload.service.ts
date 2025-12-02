import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

type ApiEncryptDecryptResponse = { file: File, message: string }

@Injectable({
  providedIn: 'root'
})
export class UploadService {
  private readonly apiUrl = 'http://localhost:5149/api';

  constructor(private _http: HttpClient) { }

  encrypt(file: File, message: string): Observable<ApiEncryptDecryptResponse> {
    if (!file || !message || !message.length) throw new Error('Invalid encrypt arguments.');

    const formData = new FormData();
    formData.append('file', file);
    formData.append('message', message);

    return this._http.post<ApiEncryptDecryptResponse>(`${this.apiUrl}/encrypt`, formData);
  }

  decrypt(file: File): Observable<ApiEncryptDecryptResponse> {
    if (!file) throw new Error('Invalid decrypt argument.');

    const formData = new FormData();
    formData.append('file', file);

    return this._http.post<ApiEncryptDecryptResponse>(`${this.apiUrl}/decrypt`, formData);
  }
}
