import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// Інтерфейс для відповіді від сервера (аналог DTO в C#)
export interface AuthResponse {
  token: string;
}

@Injectable({
  providedIn: 'root' // Сервіс буде доступний у всьому додатку
})
export class AuthService {
  // 🔥 ЗАМІНИ ПОРТ (7001) на той, на якому реально запускається твій .NET Swagger!
  private apiUrl = 'https://localhost:7197/api/auth'; 

  constructor(private http: HttpClient) { }

  // Метод для логіну
  login(credentials: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials);
  }

  // Метод для реєстрації
  register(userData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userData);
  }
}
