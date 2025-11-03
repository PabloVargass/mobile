import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-olvide',
  templateUrl: './olvide.page.html',
  styleUrls: ['./olvide.page.scss'],
  standalone: true,
  imports: [CommonModule, IonicModule, FormsModule],
})
export class OlvidePage {
  email = '';

  enviarCorreo() {
    console.log('Enviar correo de recuperación a:', this.email);
  }
}
