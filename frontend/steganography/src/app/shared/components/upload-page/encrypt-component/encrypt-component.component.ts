import { Component } from '@angular/core';
import { UploadStrategyComponent } from '../strategies/upload-strategy.model';
import { UploaderComponent } from "../uploader/uploader.component";

@Component({
  selector: 'app-encrypt-component',
  imports: [UploaderComponent],
  templateUrl: './encrypt-component.component.html',
  styleUrl: './encrypt-component.component.scss'
})
export class EncryptComponentComponent implements UploadStrategyComponent {
  handleFileUpload(event: { file: File; message?: string }): void {
    return; // TODO
  }
}
