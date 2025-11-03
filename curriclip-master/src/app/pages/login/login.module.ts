import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';

import { LoginPageRoutingModule } from './login-routing.module';
import { LoginPage } from './login.page';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,             // 👈 lo necesitas para [(ngModel)]
    IonicModule,
    LoginPageRoutingModule,
    LoginPage                // 👈 importar el componente standalone
  ],
  declarations: []           // 👈 NO declararlo
})
export class LoginPageModule {}
