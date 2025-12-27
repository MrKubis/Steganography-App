import { AfterViewInit, Component,  Type, ViewChild, ViewContainerRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UploadStrategyFactory } from './strategies/upload.strategy';
import { UploadStrategyComponent } from './strategies/upload-strategy.model';

@Component({
  selector: 'app-upload-page',
  imports: [],
  templateUrl: './upload-page.component.html',
  styleUrl: './upload-page.component.scss'
})
export class UploadPageComponent implements AfterViewInit {
  @ViewChild('uploadComponentOutlet', { read: ViewContainerRef, static: true }) uploadComponentOutletRef!: ViewContainerRef;

  constructor(private _route: ActivatedRoute, private _router: Router) {}

  ngAfterViewInit(): void {
    const uploadType = this._route.snapshot.paramMap.get('type');

    // We allow only these two types for now:
    if (!uploadType || !['encrypt', 'decrypt'].includes(uploadType)) {
      this._router.navigate(['']);
      return;
    };

    const uploadComponent = UploadStrategyFactory.getStrategyComponent(uploadType);
    
    if (uploadComponent && this.uploadComponentOutletRef) {
      this.uploadComponentOutletRef.clear();
      this.uploadComponentOutletRef.createComponent<UploadStrategyComponent>(uploadComponent);
    }
  }
}
