import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DecryptComponentComponent } from './decrypt-component.component';

describe('DecryptComponentComponent', () => {
  let component: DecryptComponentComponent;
  let fixture: ComponentFixture<DecryptComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DecryptComponentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DecryptComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
