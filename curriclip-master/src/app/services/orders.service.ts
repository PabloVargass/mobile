import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

export type Status = 'pending' | 'progress' | 'done';

export interface EntityRef {
  id?: number;
  name: string;
}

export interface Order {
  id: number;
  folio: string;
  estado: string;
  fechaRegistro: string;
  fechaAgendada: string;
  fechaFinalizado?: string | null;
  horasTrabajo?: number;
  direccion?: string | null;
  cliente?: string | null;
  comuna?: { id: number; nombre: string } | null;
  region?: { id: number; nombre: string } | null;
}

@Injectable({ providedIn: 'root' })
export class OrdersService {
  private base = (environment.apiUrl || 'https://localhost:7226').replace(/\/$/, '');

  constructor(private http: HttpClient) {}

  /** 🔹 Obtener órdenes del empleado autenticado */
  list(): any {
    const token = sessionStorage.getItem('token') || localStorage.getItem('token');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.get<Order[]>(`${this.base}/ordenes-trabajo/mine`, { headers });
  }

  /** 🔹 Obtener detalle de una orden por ID */
  detail(id: number) {
    const token = sessionStorage.getItem('token') || localStorage.getItem('token');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.get<Order>(`${this.base}/ordenes-trabajo/${id}`, { headers });
  }

  /** 🔹 Cambiar estado dinámicamente (pendiente → en proceso → completada) */
  cambiarEstado(id: number, idEstado: number) {
    const token = sessionStorage.getItem('token') || localStorage.getItem('token');

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

   // 👇 usa "idEstado" con i minúscula (exactamente igual al backend)
  return this.http.patch(`${this.base}/ordenes-trabajo/${id}/estado`, { idEstado }, { headers });
}

  /** 🔹 Cambiar estado a “completada” directamente (si se requiere aparte) */
  markDone(id: number) {
    const token = sessionStorage.getItem('token') || localStorage.getItem('token');
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
    return this.http.patch(`${this.base}/ordenes-trabajo/${id}/estado`, { idEstado: 3 }, { headers });
  }
}
