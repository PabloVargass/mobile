import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
  standalone: true,
  imports: [CommonModule, IonicModule, FormsModule]
})
export class LoginPage {
  email = '';
  password = '';
  loading = false;
  error = '';

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    this.loading = true;
    this.error = '';

    if (!this.email || !this.password) {
      this.error = 'Por favor ingresa correo y contraseña';
      this.loading = false;
      return;
    }

    this.auth.login(this.email, this.password).subscribe({
  next: (res) => {
    console.log('✅ Login exitoso:', res);

    // Guarda usuario y token
    this.auth.guardarSesion(res);

    // Redirige correctamente
    this.router.navigateByUrl('/wf', { replaceUrl: true });
  },
  error: (err) => {
    console.error('❌ Error de login:', err);
    this.error = 'Error de conexión con el servidor';
    this.loading = false;
  },
  complete: () => {
    this.loading = false;
  }
});

  }

  irARecuperar() {
    this.router.navigateByUrl('/pages/olvide'); // Ajusta si la ruta es diferente
  }
}
