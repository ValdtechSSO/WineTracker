import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { forkJoin } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';

type WineType = 'Red' | 'White' | 'Rose' | 'Sparkling' | 'Fortified' | 'Orange' | 'Other';
type ReorderIntent = 'Undecided' | 'Yes' | 'No';

interface WineHistoryItem {
  consumptionId: string;
  wineId: string;
  producer: string;
  name: string;
  vintage: number | null;
  type: WineType;
  region: string | null;
  consumedOn: string;
  rating: number | null;
  notes: string | null;
  reorderIntent: ReorderIntent;
}

interface ReorderCandidate {
  wineId: string;
  producer: string;
  name: string;
  vintage: number | null;
  type: WineType;
  region: string | null;
  lastConsumedOn: string;
  lastRating: number | null;
  timesConsumed: number;
}

@Component({
  selector: 'app-root',
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatChipsModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressBarModule,
    MatSelectModule,
    MatSnackBarModule,
    MatTabsModule,
    MatToolbarModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class App {
  private readonly formBuilder = inject(FormBuilder);
  private readonly http = inject(HttpClient);
  private readonly snackBar = inject(MatSnackBar);

  protected readonly wineTypes: WineType[] = [
    'Red',
    'White',
    'Rose',
    'Sparkling',
    'Fortified',
    'Orange',
    'Other'
  ];
  protected readonly ratings = [1, 2, 3, 4, 5];
  protected readonly history = signal<WineHistoryItem[]>([]);
  protected readonly reorderCandidates = signal<ReorderCandidate[]>([]);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly distinctWineCount = computed(
    () => new Set(this.history().map(item => item.wineId)).size
  );

  protected readonly form = this.formBuilder.nonNullable.group({
    producer: ['', [Validators.required, Validators.maxLength(160)]],
    name: ['', [Validators.required, Validators.maxLength(160)]],
    vintage: this.formBuilder.control<number | null>(null, [Validators.min(1800), Validators.max(2200)]),
    type: this.formBuilder.nonNullable.control<WineType>('Red', Validators.required),
    region: ['', Validators.maxLength(160)],
    consumedOn: [this.today(), Validators.required],
    rating: this.formBuilder.control<number | null>(null, [Validators.min(1), Validators.max(5)]),
    notes: ['', Validators.maxLength(2000)],
    reorderIntent: this.formBuilder.nonNullable.control<ReorderIntent>('Undecided', Validators.required)
  });

  constructor() {
    this.refresh();
  }

  protected submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.http.post('/api/consumptions', this.form.getRawValue())
      .pipe(finalize(() => this.saving.set(false)))
      .subscribe({
        next: () => {
          this.snackBar.open('Wine added to your journal.', 'Close', { duration: 3500 });
          this.form.reset({
            producer: '',
            name: '',
            vintage: null,
            type: 'Red',
            region: '',
            consumedOn: this.today(),
            rating: null,
            notes: '',
            reorderIntent: 'Undecided'
          });
          this.refresh();
        },
        error: () => this.snackBar.open('The wine could not be saved. Check the API and try again.', 'Close')
      });
  }

  protected changeIntent(item: WineHistoryItem, reorderIntent: ReorderIntent): void {
    this.http.put(`/api/consumptions/${item.consumptionId}/reorder`, { reorderIntent })
      .subscribe({
        next: () => {
          this.history.update(history => history.map(entry =>
            entry.consumptionId === item.consumptionId ? { ...entry, reorderIntent } : entry));
          this.loadReorderCandidates();
        },
        error: () => this.snackBar.open('The reorder choice could not be updated.', 'Close')
      });
  }

  protected displayVintage(vintage: number | null): string {
    return vintage?.toString() ?? 'NV';
  }

  protected stars(rating: number | null): string {
    return rating === null ? 'Not rated' : `${rating}/5`;
  }

  private refresh(): void {
    this.loading.set(true);
    forkJoin({
      history: this.http.get<WineHistoryItem[]>('/api/consumptions'),
      reorderCandidates: this.http.get<ReorderCandidate[]>('/api/reorder-candidates')
    })
      .pipe(finalize(() => this.loading.set(false)))
      .subscribe({
        next: result => {
          this.history.set(result.history);
          this.reorderCandidates.set(result.reorderCandidates);
        },
        error: () => this.snackBar.open('WineTracker could not load your journal.', 'Close')
      });
  }

  private loadReorderCandidates(): void {
    this.http.get<ReorderCandidate[]>('/api/reorder-candidates').subscribe({
      next: candidates => this.reorderCandidates.set(candidates),
      error: () => this.snackBar.open('The order-again list could not be refreshed.', 'Close')
    });
  }

  private today(): string {
    const now = new Date();
    const localDate = new Date(now.getTime() - now.getTimezoneOffset() * 60_000);
    return localDate.toISOString().slice(0, 10);
  }
}
