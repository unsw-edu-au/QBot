import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SilentEndComponent } from './silent-end.component';

describe('SilentEndComponent', () => {
  let component: SilentEndComponent;
  let fixture: ComponentFixture<SilentEndComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SilentEndComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SilentEndComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
