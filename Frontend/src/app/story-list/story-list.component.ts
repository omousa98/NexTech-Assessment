import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';

interface Story {
  id: number;
  title: string;
  url?: string;
}

interface StoryResponse {
  stories: Story[];
  totalCount: number;
}

@Component({
  selector: 'app-story-list',
  templateUrl: './story-list.component.html',
  styleUrls: ['./story-list.component.css']
})
export class StoryListComponent implements OnInit {
  stories: Story[] = [];
  searchTerm: string = '';
  pageNumber: number = 1;
  pageSize: number = 20;
  totalStories: number = 0;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadStories();
  }

  loadStories(): void {
    let params = new HttpParams()
      .set('pageNumber', this.pageNumber.toString())
      .set('pageSize', this.pageSize.toString());

    if (this.searchTerm.trim()) {
      params = params.set('search', this.searchTerm.trim());
    }

    this.http.get<StoryResponse>('http://localhost:5000/api/stories', { params })
      .subscribe({
        next: data => {
          console.log('Stories data received:', data);
          this.stories = data.stories;
          this.totalStories = data.totalCount;
        },
        error: err => {
          console.error('Error loading stories:', err);
        }
      });
  }

  onSearch(): void {
    this.pageNumber = 1;
    this.loadStories();
  }

  onPageChange(newPage: number): void {
    this.pageNumber = newPage;
    this.loadStories();
  }

  get totalPages(): number {
    return Math.ceil(this.totalStories / this.pageSize);
  }

  pages(): number[] {
    return Array(this.totalPages).fill(0).map((x, i) => i + 1);
  }
}
