import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './services/auth.guard';


const routes: Routes = [
  { path: '', redirectTo: 'pages/login', pathMatch: 'full' },

  { path: 'pages/login',
    loadChildren: () =>
      import('./pages/login/login.module').then(m => m.LoginPageModule),
  },
  { path: 'wf',
    canActivate: [AuthGuard],
    loadChildren: () => import('./workflow/workflow.module').then(m => m.WorkflowModule),
  },

  { path: 'pages/upload',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./pages/upload/upload.module').then(m => m.UploadPageModule),
  },

  { path: 'pages/profile',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./pages/profile/profile.module').then(m => m.ProfilePageModule),
  },

  { path: 'pages/messages',
    canActivate: [AuthGuard],
    loadChildren: () =>
      import('./pages/messages/messages.module').then(m => m.MessagesPageModule),
  },

   // 🔹 Nueva ruta: página de recuperación de contraseña
{
  path: 'pages/olvide',
  loadComponent: () =>
    import('src/app/pages/olvide/olvide.page').then(m => m.OlvidePage),
},

  { path: '**', redirectTo: 'pages/login' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
