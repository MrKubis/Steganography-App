import { NgClass } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-uploader',
    standalone: true,
    imports: [NgClass, FormsModule],
    templateUrl: './uploader.component.html',
    styleUrls: ['./uploader.component.scss']
})
export class UploaderComponent {
    @Input() isMessageRequired: boolean = true;
    @Output() upload = new EventEmitter<{file: File, message?: string}>();

    selectedFile: File | null = null;
    filePreview: string | ArrayBuffer | null = null;
    message: string = '';

    onFileSelected(event: any): void {
        const file: File = event.target.files[0];
        if (file) {
            this.selectedFile = file;

            const reader = new FileReader();
            reader.onload = e => this.filePreview = reader.result;
            reader.readAsDataURL(file);
        }
    }

    onUpload(): void {

        if (this.selectedFile) {
            if (this.isMessageRequired && !this.message) return;

            this.upload.emit({
                file: this.selectedFile,
                message: this.isMessageRequired ? this.message : undefined
            });
        }
    }
}
