import { Component, Injector, OnInit } from '@angular/core';
import { ModalComponentBase } from '@shared/common';
import { finalize } from 'rxjs/operators';
import { UserDto, UserServiceProxy } from '@service-proxies';

@Component({
  selector: 'create-or-edit-user',
  templateUrl: './create-or-edit-user.component.html',
  styleUrls: ['./create-or-edit-user.component.less'],
})
export class CreateOrEditUserComponent extends ModalComponentBase<string>
  implements OnInit {

  user = new UserDto();
  roles: string[] = [];

  constructor(
    injector: Injector,
    private userSer: UserServiceProxy,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.modalInput) {
      this.isEdit = true;
      this.loading = true;
      this.userSer.getEditById(this.modalInput)
        .pipe(finalize(() => {
          this.loading = false;
        }))
        .subscribe((res) => {
          if (!res) {
            return;
          }
          if (res.entityDto) {
            this.user = res.entityDto;
          }

          if (Array.isArray(res.roles)) {
            this.roles = res.roles;
          }

        });
    }
  }

  submitForm(event?: any) {
  }
}