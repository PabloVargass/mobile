# Workflow Feature Module (drop‑in)
Reutiliza tu **app base (LinkedIn/TikTok)** y agrega este módulo de **Órdenes de Trabajo** sin rehacer todo.

## Cómo integrarlo
1) Copia **la carpeta `src/app/workflow/`** dentro de tu proyecto existente (Ionic + Angular).
2) En `app-routing.module.ts`, agrega una ruta lazy:
```ts
{ path: 'wf', loadChildren: () => import('./workflow/workflow.module').then(m => m.WorkflowModule) }
```
3) Verifica que ya tengas `HttpClientModule` en `app.module.ts` y que exista `src/environments/environment.ts` con `API_BASE_URL`.
4) Instala Capacitor plugins si aún no están:
```bash
npm i @capacitor/camera @capacitor/filesystem @capacitor/network
```
5) Navega a **/wf** para ver la lista de órdenes. Rutas:
- `/wf` → Lista
- `/wf/orders/:id` → Detalle + Evidencias (Antes/Después)
- `/wf/new` → Crear nueva OT (opcional)

> Este módulo NO toca tu login/feed existente. Puedes proteger `/wf` con tu propio `AuthGuard` si ya lo tienes.
