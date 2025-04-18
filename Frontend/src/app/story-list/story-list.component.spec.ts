import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { StoryListComponent } from './story-list.component';

describe('StoryListComponent', () => {
  let component: StoryListComponent;
  let fixture: ComponentFixture<StoryListComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StoryListComponent ],
      imports: [ HttpClientTestingModule, FormsModule ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StoryListComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load stories on init', () => {
    const mockStories = [
      { id: 1, title: 'Story 1', url: 'http://example.com/1' },
      { id: 2, title: 'Story 2', url: null }
    ];

    component.ngOnInit();

    const req = httpMock.expectOne(req => req.url.includes('/api/stories'));
    expect(req.request.method).toBe('GET');
    req.flush(mockStories);

    expect(component.stories.length).toBe(2);
    expect(component.stories[0].title).toBe('Story 1');
  });

  it('should search stories', () => {
    component.searchTerm = 'test';
    component.onSearch();

    const req = httpMock.expectOne(req => req.url.includes('/api/stories') && req.params.get('search') === 'test');
    expect(req.request.method).toBe('GET');
    req.flush([]);

    expect(component.pageNumber).toBe(1);
  });

  it('should change page', () => {
    component.onPageChange(2);

    const req = httpMock.expectOne(req => req.url.includes('/api/stories') && req.params.get('pageNumber') === '2');
    expect(req.request.method).toBe('GET');
    req.flush([]);

    expect(component.pageNumber).toBe(2);
  });
});
