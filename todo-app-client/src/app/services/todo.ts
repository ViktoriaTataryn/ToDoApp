import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface TodoItem {
  id?: number;
  title: string;
  description?: string;
  isCompleted: boolean;
  isImportant: boolean;
  dueDate?: string; // У TypeScript DateTime передається як рядок (ISO string)
  createdAt?: string;
  categoryId?: number;
}

@Injectable({
  providedIn: 'root'
})
export class TodoService {
  // 🔥 ЗАМІНИ ПОРТ на свій правильний порт .NET бекенду!
  private apiUrl = 'https://localhost:7197/api/tasks'; 

  constructor(private http: HttpClient) { }

  // Допоміжний метод для створення заголовків із токеном
  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  // Отримати всі завдання користувача
  getTodos(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(this.apiUrl, { headers: this.getHeaders() });
  }

  // Створити нове завдання
  createTodo(todo: TodoItem): Observable<TodoItem> {
    return this.http.post<TodoItem>(this.apiUrl, todo, { headers: this.getHeaders() });
  }

  // Оновити статус завдання (виконано/не виконано)
  updateTodo(id: number, todo: TodoItem): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, todo, { headers: this.getHeaders() });
  }

  // Видалити завдання
  deleteTodo(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  }
}
