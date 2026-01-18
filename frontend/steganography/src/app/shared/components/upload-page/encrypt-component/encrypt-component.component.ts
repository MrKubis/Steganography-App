import { Component } from '@angular/core';
import { UploadStrategyComponent } from '../strategies/upload-strategy.model';
import { UploaderComponent } from "../uploader/uploader.component";
import { UploadService } from '../../../services/upload.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-encrypt-component',
  imports: [UploaderComponent, RouterLink],
  templateUrl: './encrypt-component.component.html',
  styleUrl: './encrypt-component.component.scss'
})
export class EncryptComponentComponent implements UploadStrategyComponent {
  isUploading: boolean = false;

  constructor(private _uploadService: UploadService) {}

  handleFileUpload(data: { file: File; message?: string }): void {
    if (!data.message || !data.message.length) return;

    this.isUploading = true;

    this._uploadService.encrypt(data.file, data.message).subscribe({
      next: response => {
        const url = window.URL.createObjectURL(response);
        const a = document.createElement('a');
        a.href = url;
        a.download = `encrypted-${new Date().toISOString().replace(/:/g, '-')}.png`;
        a.click();
      },
      error: () => {
        this.isUploading = false;
      },
      complete: () => {
        this.isUploading = false;
      }
    });

    return;
  }
}
