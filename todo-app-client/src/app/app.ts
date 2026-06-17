import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router'; 


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, 
    RouterOutlet,       //  Потрібно для <router-outlet>
    RouterLink,         //  Потрібно для routerLink="/todos"
    RouterLinkActive    //  Потрібно для routerLinkActive="active"
  ],
  templateUrl: './app.html', 
  styleUrl: './app.css'     
})
export class AppComponent {
  title = 'todo-app-client';


  constructor(private router: Router) {}

  
  logout(): void {
   
      localStorage.removeItem('token'); // Видаляємо JWT-токен
      this.router.navigate(['/login']); // Перенаправляємо на сторінку входу
    
  }

}
