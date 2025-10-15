import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EncryptComponentComponent } from './encrypt-component.component';

describe('EncryptComponentComponent', () => {
  let component: EncryptComponentComponent;
  let fixture: ComponentFixture<EncryptComponentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EncryptComponentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EncryptComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
