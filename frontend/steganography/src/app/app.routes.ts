import { Routes } from '@angular/router';
import { LandingPageComponent } from './shared/components/landing-page/landing-page.component';
import { UploadPageComponent } from './shared/components/upload-page/upload-page.component';

export const routes: Routes = [
    {
        path: '',
        component: LandingPageComponent
    },
    {
        path: 'upload/:type',
        component: UploadPageComponent
    },
    { 
        path: '**', 
        component: LandingPageComponent 
    }
];
