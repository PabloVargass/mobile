import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7226'; // ajusta si tu backend usa otro puerto
  private usuarioActual: any = null;

  constructor(private http: HttpClient, private router: Router) {}

  // 🔐 LOGIN con API .NET
  login(correo: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, { correo, password });
  }

  // ✅ Guarda los datos de sesión
  guardarSesion(usuario: any) {
    this.usuarioActual = usuario;
    sessionStorage.setItem('usuario', JSON.stringify(usuario));
    if (usuario.token) {
      sessionStorage.setItem('token', usuario.token);
    }
  }

  // ✅ Obtiene el usuario autenticado
  obtenerUsuario() {
    if (this.usuarioActual) return this.usuarioActual;

    const data = sessionStorage.getItem('usuario');
    return data ? JSON.parse(data) : null;
  }

  // ✅ Cierra sesión
  cerrarSesion() {
    this.usuarioActual = null;
    sessionStorage.clear();
    this.router.navigate(['/login']);
  }

  // ✅ Verifica si hay sesión activa
  estaAutenticado(): boolean {
    const token = sessionStorage.getItem('token');
    return !!token;
  }

  // (Opcional) para otros componentes
  getToken(): string | null {
    return sessionStorage.getItem('token');
  }
}
