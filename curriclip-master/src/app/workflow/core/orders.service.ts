import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

export type OrderStatus = 'pending' | 'progress' | 'done';

export interface Order {
  id: number;
  code: string;
  status: OrderStatus;
  created_at: string;
  client?: { name?: string };
  company?: { name?: string };
  address?: string;
  description?: string;
}

@Injectable({ providedIn: 'root' })
export class OrdersService {
  private base = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /** 🔹 Obtener órdenes reales del empleado autenticado */
  list(status?: string): Observable<Order[]> {
    const token = sessionStorage.getItem('token');
    if (!token) {
      console.error('⚠️ No se encontró token en sessionStorage');
    }

    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    return this.http.get<Order[]>(`${this.base}/ordenes-trabajo/mine`, { headers });
  }

  /** 🔹 Detalle de una orden */
  detail(id: number): Observable<Order> {
    const token = sessionStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    return this.http.get<Order>(`${this.base}/ordenes-trabajo/${id}`, { headers });
  }

  /** 🔹 Subir evidencia (si aplica) */
  uploadEvidence(id: number, kind: 'before' | 'after', file: Blob, notes?: string): Observable<any> {
    const token = sessionStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    const fd = new FormData();
    fd.append('file', file);
    fd.append('kind', kind);
    if (notes) fd.append('notes', notes);

    return this.http.post(`${this.base}/ordenes-trabajo/${id}/evidence`, fd, { headers });
  }

  /** 🔹 Marcar orden como completada */
  markDone(id: number): Observable<any> {
    const token = sessionStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    return this.http.post(`${this.base}/ordenes-trabajo/${id}/done`, {}, { headers });
  }

  /** 🔹 Crear nueva orden (si aplica desde móvil) */
  create(data: any): Observable<any> {
    const token = sessionStorage.getItem('token');
    const headers = new HttpHeaders({
      Authorization: `Bearer ${token}`,
    });

    return this.http.post(`${this.base}/ordenes-trabajo`, data, { headers });
  }
}
