import { Component, OnInit } from '@angular/core';
import { IonicModule } from '@ionic/angular';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { EvidenceUploaderComponent } from '../../components/evidence-uploader/evidence-uploader.component';
import { OrdersService } from '../../core/orders.service';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [IonicModule, CommonModule, FormsModule, EvidenceUploaderComponent],
  templateUrl: './order-detail.page.html'
})
export class OrderDetailPage implements OnInit {
  id!: number;
  data?: any;
  notesBefore = '';
  notesAfter = '';
  uploading = false;
  before?: Blob;
  after?: Blob;

  constructor(private route: ActivatedRoute, private api: OrdersService) {}

ngOnInit() {
  this.id = Number(this.route.snapshot.paramMap.get('id'));
  this.api.detail(this.id).subscribe((d: any) => this.data = d);
}

onBefore(e: { blob: Blob, dataUrl: string }) { this.before = e.blob; }
onAfter(e:  { blob: Blob, dataUrl: string }) { this.after  = e.blob; }

upload(kind: 'before'|'after') {
  const file = kind === 'before' ? this.before : this.after;
  const notes = kind === 'before' ? this.notesBefore : this.notesAfter;
  if (!file) return;

  this.uploading = true;
  this.api.uploadEvidence(this.id, kind, file, notes).subscribe({
    next: () => {},
    complete: () => this.uploading = false
  });
}

markDone() {
  this.api.markDone(this.id).subscribe(() => {
    // toast/navegar si quieres
  });
}

  }
