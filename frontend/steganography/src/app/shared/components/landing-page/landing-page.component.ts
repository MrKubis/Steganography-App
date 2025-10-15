import { Component } from '@angular/core';

@Component({
  selector: 'app-landing-page',
  imports: [],
  templateUrl: './landing-page.component.html',
  styleUrls: ['./landing-page.component.scss']
})
export class LandingPageComponent {
  runPasswordAnimation(event: Event): void {
    const container = event.currentTarget as HTMLElement;
    const placeholderElement = container.querySelector('.placeholder') as HTMLParagraphElement;
    const label = container.dataset['label']?.toUpperCase();

    if (!placeholderElement || !label) return;

    const activeInterval = (container as any)._intervalId as number | undefined;
    if (activeInterval) clearInterval(activeInterval);

    const placeholderLength = label.length;
    let counter = 0;
    placeholderElement.innerText = '';

    const interval = setInterval(() => {
      placeholderElement.innerText += '*';
      counter++;

      if (counter >= placeholderLength) {
        clearInterval(interval);
        (container as any)._intervalId = undefined;
      }
    }, 100);

    (container as any)._intervalId = interval;
  }

  resetPasswordAnimation(event: Event): void {
    const container = event.currentTarget as HTMLElement;
    const placeholderElement = container.querySelector('.placeholder') as HTMLParagraphElement;
    const label = container.dataset['label']?.toUpperCase();

    if (!placeholderElement || !label) return;

    const activeInterval = (container as any)._intervalId as number | undefined;
    if (activeInterval) {
      clearInterval(activeInterval);
      (container as any)._intervalId = undefined;
    }

    placeholderElement.innerText = label;
  }
}
