import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantChangeComponent } from './tenant-change.component';

describe('TenantChangeComponent', () => {
  let component: TenantChangeComponent;
  let fixture: ComponentFixture<TenantChangeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TenantChangeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantChangeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
