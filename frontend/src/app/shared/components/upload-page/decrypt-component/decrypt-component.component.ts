import { Component } from '@angular/core';
import { UploadStrategyComponent } from '../strategies/upload-strategy.model';
import { UploaderComponent } from '../uploader/uploader.component';
import { UploadService } from '../../../services/upload.service';

@Component({
  selector: 'app-decrypt-component',
  imports: [UploaderComponent],
  templateUrl: './decrypt-component.component.html',
  styleUrl: './decrypt-component.component.scss'
})
export class DecryptComponentComponent implements UploadStrategyComponent {
  isUploading: boolean = false;

  constructor(private _uploadService: UploadService) {}

  handleFileUpload(data: { file: File; message?: string }): void {
    if (!data.file) return;

    this.isUploading = true;

    this._uploadService.decrypt(data.file).subscribe({
      next: response => {
        console.log(response);
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
