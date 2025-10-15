import { Component } from '@angular/core';
import { LandingPageComponent } from "./shared/components/landing-page/landing-page.component";

@Component({
  selector: 'app-root',
  imports: [LandingPageComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent { }
