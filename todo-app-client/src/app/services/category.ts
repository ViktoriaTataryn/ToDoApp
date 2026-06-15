import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

// services/category.ts
@Injectable({ providedIn: 'root' })
export class CategoryService {
  private apiUrl = 'https://localhost:7001/api/categories'; // Зміни на свій URL

  constructor(private http: HttpClient) {}

  getCategories() { return this.http.get<any[]>(this.apiUrl, { headers: this.getAuthHeaders() }); }
  createCategory(cat: { name: string }) { return this.http.post(this.apiUrl, cat, { headers: this.getAuthHeaders() }); }
  deleteCategory(id: number) { return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() }); }

  private getAuthHeaders() {
    return new HttpHeaders({ 'Authorization': `Bearer ${localStorage.getItem('token')}` });
  }
}
