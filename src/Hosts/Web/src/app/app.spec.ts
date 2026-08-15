import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { App } from './app';

describe('App', () => {
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [provideHttpClient(), provideHttpClientTesting()]
    }).compileComponents();

    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('shows the journal summary returned by the API', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();

    http.expectOne('/api/consumptions').flush([
      {
        consumptionId: 'consumption-1',
        wineId: 'wine-1',
        producer: 'Example Estate',
        name: 'First Pour',
        vintage: 2022,
        type: 'Red',
        region: 'Rioja',
        consumedOn: '2026-08-15',
        rating: 4,
        notes: 'Fresh and balanced',
        reorderIntent: 'Yes'
      }
    ]);
    http.expectOne('/api/reorder-candidates').flush([
      {
        wineId: 'wine-1',
        producer: 'Example Estate',
        name: 'First Pour',
        vintage: 2022,
        type: 'Red',
        region: 'Rioja',
        lastConsumedOn: '2026-08-15',
        lastRating: 4,
        timesConsumed: 1
      }
    ]);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Drink thoughtfully');
    expect(compiled.querySelector('.summary')?.textContent).toContain('1tastings');
  });
});
