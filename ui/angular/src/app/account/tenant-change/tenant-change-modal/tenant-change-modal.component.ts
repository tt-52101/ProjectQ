import { Component, Injector, OnInit } from '@angular/core';
import { ModalComponentBase } from '@shared/common';
import { IsTenantAvailableInput, TenantAvailabilityState, TenantServiceProxy } from '@service-proxies';
import { finalize } from 'rxjs/operators';
import { SettingsService } from '@delon/theme';
import { RequestHelper } from '@shared/riven/helper';

@Component({
  selector: 'tenant-change-modal.component',
  templateUrl: './tenant-change-modal.component.html',
  styleUrls: ['./tenant-change-modal.component.less'],
})
export class TenantChangeModalComponent extends ModalComponentBase implements OnInit {

  input = new IsTenantAvailableInput();

  beforeTenantName: string;

  constructor(
    injector: Injector,
    public settingsSer: SettingsService,
    public tenantSer: TenantServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.title = this.l('label.change-tenant');
    this.beforeTenantName = this.settingsSer.getData(RequestHelper.multiTenancy.key);
    this.input.tenantName = this.beforeTenantName;
  }

  submitForm(event?: any) {

    if (this.input.tenantName === this.beforeTenantName) {
      this.close();
      return;
    }

    if (!this.input.tenantName || this.input.tenantName === '') {
      this.settingsSer.setData(RequestHelper.multiTenancy.key, '');
      this.success();
      return;
    }

    this.loading = true;

    this.tenantSer.isTenantAvailable(this.input)
      .pipe(finalize(() => {
        this.loading = false;
      }))
      .subscribe((res) => {
        switch (res.state) {
          case TenantAvailabilityState.Available:
            this.settingsSer.setData(RequestHelper.multiTenancy.key, this.input.tenantName);
            this.success();
            break;
          case TenantAvailabilityState.InActive:
            this.message.warn(this.l('message.tenant.in-active', this.input.tenantName));
            break;
          case TenantAvailabilityState.NotFound:
            this.message.warn(this.l('message.tenant.not-found', this.input.tenantName));
            break;
        }
      });
  }

}
