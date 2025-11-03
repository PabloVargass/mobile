import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment'; // 👈 importa el environment

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  // ✅ URL base tomada del environment (según tu entorno)
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // 🔐 LOGIN
  login(credentials: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/login`, credentials);
  }

  // 👤 OBTENER DATOS DEL USUARIO LOGUEADO (si usas token)
  getUsuario(): Observable<any> {
    return this.http.get(`${this.baseUrl}/me`);
  }

  // 👨‍💼 EMPLEADOS
  getEmpleados(): Observable<any> {
    return this.http.get(`${this.baseUrl}/Empleado`);
  }

  crearEmpleado(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Empleado`, data);
  }

  // 📍 COMUNAS
  getComunas(): Observable<any> {
    return this.http.get(`${this.baseUrl}/Comuna`);
  }

  // 🌎 REGIONES
  getRegiones(): Observable<any> {
    return this.http.get(`${this.baseUrl}/Region`);
  }

  // 🔓 LOGOUT (si lo implementas)
  logout(): Observable<any> {
    return this.http.post(`${this.baseUrl}/logout`, {});
  }
}
