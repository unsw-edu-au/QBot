import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserTutorialDialogComponent } from './user-tutorial-dialog.component';

describe('UserTutorialDialogComponent', () => {
  let component: UserTutorialDialogComponent;
  let fixture: ComponentFixture<UserTutorialDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserTutorialDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserTutorialDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
