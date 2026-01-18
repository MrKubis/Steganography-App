import { Component, signal } from '@angular/core';
import { UploadStrategyComponent } from '../strategies/upload-strategy.model';
import { UploaderComponent } from '../uploader/uploader.component';
import { UploadService } from '../../../services/upload.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-decrypt-component',
  imports: [UploaderComponent, RouterLink],
  templateUrl: './decrypt-component.component.html',
  styleUrl: './decrypt-component.component.scss'
})
export class DecryptComponentComponent implements UploadStrategyComponent {
  isUploading: boolean = false;
  decryptedMessage = signal<string | null>(null);

  constructor(private _uploadService: UploadService) {}

  handleFileUpload(data: { file: File; message?: string }): void {
    if (!data.file) return;

    this.isUploading = true;

    this._uploadService.decrypt(data.file).subscribe({
      next: response => {
        this.decryptedMessage.set(response ?? null);
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

  clearDecryptedMessage(): void {
    this.decryptedMessage.set(null);
  }
}
