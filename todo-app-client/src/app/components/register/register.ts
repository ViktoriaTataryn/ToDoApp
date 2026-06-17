import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule,FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent {
  registerForm: FormGroup;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
  lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(5)]] 
    });
  }

onSubmit() {
  if (this.registerForm.valid) {
    this.errorMessage = '';
    this.successMessage = '';

   
    const newUser = {
      FirstName: this.registerForm.value.firstName.trim(),
      LastName: this.registerForm.value.lastName.trim(),
      Email: this.registerForm.value.email.trim(),
      PasswordHash: this.registerForm.value.password
    };

    

    this.authService.register(newUser).subscribe({
      next: () => {
        this.successMessage = 'Реєстрація успішна! Перенаправлення на вхід...';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        console.error('Повна помилка від сервера:', err);
        
        if (err.error && err.error.errors) {
          console.log('Помилки валідації .NET DTO:', err.error.errors);
          const firstKey = Object.keys(err.error.errors)[0];
          this.errorMessage = err.error.errors[firstKey][0];
        } else if (err.error && err.error.description) {
          this.errorMessage = err.error.description;
        } else {
          this.errorMessage = 'Помилка реєстрації. Перевірте введені дані.';
        }
      }
    });
  }
}
}

