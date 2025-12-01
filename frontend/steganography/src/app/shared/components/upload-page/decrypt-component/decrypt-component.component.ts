import { Component } from '@angular/core';
import { UploadStrategyComponent } from '../strategies/upload-strategy.model';
import { UploaderComponent } from '../uploader/uploader.component';

@Component({
  selector: 'app-decrypt-component',
  imports: [UploaderComponent],
  templateUrl: './decrypt-component.component.html',
  styleUrl: './decrypt-component.component.scss'
})
export class DecryptComponentComponent implements UploadStrategyComponent {
  handleFileUpload(event: { file: File; message?: string }): void {
    return; // TODO
  }
}
