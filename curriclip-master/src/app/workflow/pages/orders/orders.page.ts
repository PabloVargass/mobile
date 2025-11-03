import { Component, OnInit } from '@angular/core';
import { IonicModule, ToastController } from '@ionic/angular';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { OrdersService } from 'src/app/services/orders.service';
import { AuthService } from 'src/app/services/auth.service';

type OrderStatus = 'pending' | 'progress' | 'done';

interface Order {
  id: number;
  code: string;
  status: OrderStatus;
  created_at: string;
  client?: { name?: string };
  company?: { name?: string };
  address?: string;
  description?: string;
  hours?: number;
  fechaAgendada?: string;
  fechaFinalizado?: string;
}

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [IonicModule, CommonModule, FormsModule, RouterModule, DatePipe],
  templateUrl: './orders.page.html',
  styleUrls: ['./orders.page.css']
})
export class OrdersPage implements OnInit {
  usuario: any = null;
  q = '';
  status: '' | OrderStatus = '';
  loading = false;
  data: Order[] = [];
  filtered: Order[] = [];

  constructor(
    private api: OrdersService,
    private router: Router,
    private auth: AuthService,
    private toastCtrl: ToastController
  ) {}

  ngOnInit() {
    this.usuario = this.auth.obtenerUsuario();
    if (!this.usuario) {
      this.auth.cerrarSesion();
      return;
    }
    this.load();
  }

  logout() {
    this.auth.cerrarSesion();
  }

  /** 🔹 Cargar órdenes del empleado autenticado */
  load() {
    this.loading = true;
    this.api.list().subscribe({
      next: (rows: any[]) => {
        console.log('✅ Datos recibidos del backend:', rows);
        this.data = rows.map(o => ({
          id: o.id,
          code: o.folio?.toString() || '-',
          status:
            o.estado === 'AGENDADO'
              ? 'pending'
              : o.estado === 'EN PROCESO'
              ? 'progress'
              : o.estado === 'REALIZADO'
              ? 'done'
              : 'pending',
          created_at: o.fechaRegistro
            ? new Date(o.fechaRegistro).toISOString().split('T')[0]
            : '',
          client: { name: o.cliente || 'Sin cliente' },
          company: { name: o.region?.nombre || 'Sin región' },
          address: o.direccion || 'Sin dirección',
          description: o.observaciones || 'Sin observación',
          hours: o.horasTrabajo || 0,
          fechaAgendada: o.fechaAgendada
            ? new Date(o.fechaAgendada).toLocaleString()
            : 'No registrada',
          fechaFinalizado: o.fechaFinalizado
            ? new Date(o.fechaFinalizado).toLocaleString()
            : 'No registrada'
        }));
        this.apply();
      },
      error: (err:any) => console.error('❌ Error al obtener órdenes:', err),
      complete: () => (this.loading = false)
    });
  }

  /** 🔹 Cambiar estado al presionar el botón */
  cambiarEstado(o: any) {
    let nuevoEstado: any;
    if (o.status === 'pending') nuevoEstado = 'progress';
    else if (o.status === 'progress') nuevoEstado = 'done';
    else return;

    const idEstado = nuevoEstado === 'pending' ? 1 : nuevoEstado === 'progress' ? 2 : 3;

    this.api.cambiarEstado(o.id, idEstado).subscribe({
      next: async () => {
        o.status = nuevoEstado;
        const toast = await this.toastCtrl.create({
          message:
            nuevoEstado === 'progress'
              ? '✅ Orden marcada como En Proceso'
              : '🎉 Orden completada exitosamente',
          duration: 2000,
          color: nuevoEstado === 'progress' ? 'tertiary' : 'success',
          position: 'top'
        });
        await toast.present();
      },
      error: async (err: any) => {
        console.error('❌ Error al cambiar estado:', err);
        const toast = await this.toastCtrl.create({
          message: 'Error al cambiar el estado de la orden',
          duration: 2000,
          color: 'danger',
          position: 'top'
        });
        await toast.present();
      }
    });
  }

  /** 🔹 Filtro de búsqueda local + estado */
  apply() {
    const q = this.q.trim().toLowerCase();

    this.filtered = this.data.filter(o => {
      const matchesSearch =
        !q ||
        (
          o.code +
          ' ' +
          (o.client?.name || '') +
          ' ' +
          (o.company?.name || '') +
          ' ' +
          (o.address || '')
        )
          .toLowerCase()
          .includes(q);

      const matchesStatus = !this.status || this.status === o.status;
      return matchesSearch && matchesStatus;
    });
  }

  /** 🔹 Colores según estado */
  chipColor(s: OrderStatus) {
    return s === 'pending'
      ? 'warning'
      : s === 'progress'
      ? 'tertiary'
      : 'success';
  }

  /** 🔹 Texto legible del estado */
  label(s: OrderStatus) {
    return s === 'pending'
      ? 'Pendiente'
      : s === 'progress'
      ? 'En progreso'
      : 'Completada';
  }

  /** 🔹 Navegar al detalle */
  go(o: Order) {
    this.router.navigate(['/wf', 'detail', o.id]);
  }
}
