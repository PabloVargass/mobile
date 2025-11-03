export interface Client { id: number; name: string; }
export type OrderStatus = 'pending'|'progress'|'done';

export interface Order {
  id: number | string;
  code: string;
  client?: Client;
  status: OrderStatus;
  created_at?: string;
  assigned_user_id?: number;
  company?: { name: string };
  description?: string;
  address?: string;
}
