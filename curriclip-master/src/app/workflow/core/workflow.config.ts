// Estados válidos del flujo de trabajo
export type OrderStatus = 'pending' | 'progress' | 'done';

// Reglas de transición entre estados
type Need = 'before' | 'after';
interface Transition {
  from: OrderStatus;
  to: OrderStatus;
  action: 'iniciar' | 'completar';
  requires?: Need[];          // evidencias obligatorias
  roles?: string[];           // opcional: restringir por rol
}

export const WORKFLOW: Transition[] = [
  { from: 'pending',  to: 'progress', action: 'iniciar',  requires: ['before'] },
  { from: 'progress', to: 'done',     action: 'completar', requires: ['after'] },
];

// Helper de validación de transiciones
export function canTransition(
  current: OrderStatus,
  target: OrderStatus,
  hasBefore: boolean,
  hasAfter: boolean,
  role?: string
): { ok: boolean; reason?: string } {
  const rule = WORKFLOW.find(r => r.from === current && r.to === target);
  if (!rule) return { ok: false, reason: 'Transición no permitida' };
  if (rule.roles && role && !rule.roles.includes(role)) {
    return { ok: false, reason: 'Rol no autorizado' };
  }
  if (rule.requires?.includes('before') && !hasBefore) {
    return { ok: false, reason: 'Falta evidencia "Antes"' };
  }
  if (rule.requires?.includes('after') && !hasAfter) {
    return { ok: false, reason: 'Falta evidencia "Después"' };
  }
  return { ok: true };
}
