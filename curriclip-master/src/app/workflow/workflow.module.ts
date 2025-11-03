import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { WorkflowRoutingModule } from './workflow-routing.module';

import { OrdersPage } from './pages/orders/orders.page';
import { OrderDetailPage } from './pages/order-detail/order-detail.page';
import { NewOrderPage } from './pages/new-order/new-order.page';
import { EvidenceUploaderComponent } from './components/evidence-uploader/evidence-uploader.component';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    WorkflowRoutingModule,
    OrdersPage,
    OrderDetailPage,
    NewOrderPage,
    EvidenceUploaderComponent
  ]
})
export class WorkflowModule {}
