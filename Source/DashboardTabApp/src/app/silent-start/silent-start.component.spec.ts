import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SilentStartComponent } from './silent-start.component';

describe('SilentStartComponent', () => {
  let component: SilentStartComponent;
  let fixture: ComponentFixture<SilentStartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SilentStartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SilentStartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
