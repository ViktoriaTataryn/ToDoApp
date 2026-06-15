import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router'; // 🔥 Додали імпорти з роутера

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, 
    RouterOutlet,       // 🔥 Потрібно для <router-outlet>
    RouterLink,         // 🔥 Потрібно для routerLink="/todos"
    RouterLinkActive    // 🔥 Потрібно для routerLinkActive="active"
  ],
  templateUrl: './app.html', // або ./app.html залежно від того, як у тебе названо
  styleUrl: './app.css'     // або ./app.css
})
export class AppComponent {
  title = 'todo-app-client';

  constructor(private router: Router) {}

  // 🔥 Метод для кнопки-аватара, який ми щойно створили
  logout(): void {
   
      localStorage.removeItem('token'); // Видаляємо JWT-токен
      this.router.navigate(['/login']); // Перенаправляємо на сторінку входу
    
  }
}
