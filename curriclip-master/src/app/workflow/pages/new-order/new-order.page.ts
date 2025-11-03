import { Component } from '@angular/core';
import { IonicModule } from '@ionic/angular';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { OrdersService } from '../../core/orders.service';

@Component({
  selector: 'app-new-order',
  standalone: true,
  imports: [IonicModule, CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './new-order.page.html'
})
export class NewOrderPage {
  loading = false;
  form = this.fb.group({
    code: ['', Validators.required],
    operator: [''],
    company: [''],
    client_id: [''],
    description: ['']
  });

  constructor(private fb: FormBuilder, private api: OrdersService, private router: Router) {}

  submit() {
    if (this.form.invalid) return;
    this.loading = true;
    this.api.create(this.form.value).subscribe({
      next: (res: any) => this.router.navigate(['/wf']),
      complete: () => this.loading = false
    });
  }
}
