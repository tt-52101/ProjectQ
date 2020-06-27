import { Injectable, Injector, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MenuService, SettingsService, TitleService, ALAIN_I18N_TOKEN } from '@delon/theme';
import { DA_SERVICE_TOKEN, ITokenService } from '@delon/auth';
import { ACLService } from '@delon/acl';
import { I18nService } from '../i18n';

import { NzIconService } from 'ng-zorro-antd/icon';
import { ICONS } from '../../../style-icons';
import { ICONS_AUTO } from '../../../style-icons-auto';
import { SessionService } from '../../shared/riven';
import { AppConsts } from '@shared';
import { SessionDto } from '../../service-proxies';

/**
 * Used for application startup
 * Generally used to get the basic data of the application, like: Menu Data, User Data, etc.
 */
@Injectable()
export class StartupService {
  constructor(
    iconSrv: NzIconService,
    private menuService: MenuService,
    @Inject(ALAIN_I18N_TOKEN) private i18n: I18nService,
    private settingService: SettingsService,
    private aclService: ACLService,
    private titleService: TitleService,
    @Inject(DA_SERVICE_TOKEN) private tokenService: ITokenService,
    private httpClient: HttpClient,
    private injector: Injector,
  ) {
    iconSrv.addIcon(...ICONS_AUTO, ...ICONS);
  }

  load(): Promise<any> {
    // only works with promises
    // https://github.com/angular/angular/issues/15088
    return new Promise((resolve, reject) => {
      this.getAppSettings(resolve, reject);
    });
  }


  /** 加载前端 appsettings.json 配置 */
  private getAppSettings(resolve: any, reject: any) {
    this.httpClient.get('assets/appsettings.json')
      .subscribe(
        (response) => {
          var result = response as any;
          AppConsts.remoteServiceUrl = result.remoteServiceUrl;
          AppConsts.appUrl = result.appUrl;

          this.getAppSession(resolve, reject);
        },
        (e) => {
          reject(e);
        });
  }

  /** 加载会话信息 */
  private getAppSession(resolve: any, reject: any) {
    const sessionSer = this.injector.get(SessionService);
    sessionSer.loadOrUpdateAppInfo((state, data: SessionDto | any) => {
      if (state) {
        if (resolve) {
          this.initAppInfo(data);

          this.initAclInfo(data);

          this.initUserInfo(data);

          this.initMenuInfo(data);


          resolve({});
        }
      } else {
        if (reject) {
          reject('init session info error' + data);
        }
      }
    });
  }

  /** 初始化应用信息 */
  private initAppInfo(input: SessionDto) {
    const app: any = {
      name: `ng-alain`,
      description: `Ng-zorro admin panel front-end framework`,
    };
    this.settingService.setApp(app);
    this.titleService.suffix = this.settingService.app.name;
  }

  /** 初始化权限信息 */
  private initAclInfo(input: SessionDto) {
    // 权限
    this.aclService.attachRole(input.auth.grantedClaims);
  }

  /** 初始化用户信息 */
  private initUserInfo(input: SessionDto) {
    const user: any = {
      name: 'Admin',
      avatar: './assets/tmp/img/avatar.jpg',
      email: 'cipchk@qq.com',
      token: '123456789',
    };
    this.settingService.setUser(user);
  }

  /** 初始化菜单信息 */
  private initMenuInfo(input: SessionDto) {
    const menus = JSON.parse(input.menu);
    this.menuService.add(menus);
  }
}
