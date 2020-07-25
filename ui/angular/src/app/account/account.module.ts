import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccountRoutingModule } from './account-routing.module';
import { SharedModule } from '@shared';
import { LoginComponent } from './login';
import { I18nModule } from '@core/i18n';
import { RivenModule } from '@shared/riven';
import { TenantChangeComponent } from './tenant-change';


@NgModule({
  declarations: [
    LoginComponent,
    TenantChangeComponent
  ],
  imports: [
    CommonModule,
    I18nModule,
    AccountRoutingModule,
    SharedModule,
    RivenModule.forChild(),
  ],
})
export class AccountModule {
}
