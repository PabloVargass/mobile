import { Component, EventEmitter, Input, Output } from '@angular/core';
import { IonicModule } from '@ionic/angular';
import { CommonModule } from '@angular/common';
import { Camera, CameraResultType, CameraSource } from '@capacitor/camera';

@Component({
  selector: 'wf-evidence-uploader',
  standalone: true,
  imports: [IonicModule, CommonModule],
  templateUrl: './evidence-uploader.component.html'
})
export class EvidenceUploaderComponent {
  @Input() label = 'Foto';
  @Output() fileSelected = new EventEmitter<{ blob: Blob, dataUrl: string }>();
  preview?: string;
  busy = false;

  async pickFrom(source: 'camera' | 'gallery') {
    this.busy = true;
    try {
      const image = await Camera.getPhoto({
        quality: 70,
        resultType: CameraResultType.DataUrl,
        source: source === 'camera' ? CameraSource.Camera : CameraSource.Photos
      });
      const dataUrl = image.dataUrl!;
      const blob = this.dataUrlToBlob(dataUrl);
      this.preview = dataUrl;
      this.fileSelected.emit({ blob, dataUrl });
    } finally {
      this.busy = false;
    }
  }

  onFileInput(e: any) {
    const file: File = e.target.files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = () => {
      const dataUrl = String(reader.result);
      this.preview = dataUrl;
      this.fileSelected.emit({ blob: file, dataUrl });
    };
    reader.readAsDataURL(file);
  }

  private dataUrlToBlob(dataUrl: string): Blob {
    const arr = dataUrl.split(',');
    const head = arr[0] || '';
    const body = arr[1] || '';
    const m = head.match(/:(.*?);/);
    const mime = m?.[1] ?? 'image/jpeg';
    if (!body) return new Blob([], { type: mime });
    const bstr = atob(body);
    let n = bstr.length;
    const u8arr = new Uint8Array(n);
    while (n--) u8arr[n] = bstr.charCodeAt(n);
    return new Blob([u8arr], { type: mime });
  }
}
