import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseProvisioningInfoDialogComponent } from './course-provisioning-info-dialog.component';

describe('CourseProvisioningInfoDialogComponent', () => {
  let component: CourseProvisioningInfoDialogComponent;
  let fixture: ComponentFixture<CourseProvisioningInfoDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CourseProvisioningInfoDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseProvisioningInfoDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
