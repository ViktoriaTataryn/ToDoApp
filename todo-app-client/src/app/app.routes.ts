import { Routes } from '@angular/router';

import { LoginComponent } from './components/login/login';
import { RegisterComponent } from './components/register/register';
import { TodosComponent } from './components/todos/todos';
import { CategoriesComponent } from './components/categories/categories';


export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'todos', component: TodosComponent },
  { path: 'categories', component: CategoriesComponent },
  { path: '', redirectTo: '/login', pathMatch: 'full' }, // Якщо просто localhost:4200, перекидає на логін
  { path: '**', redirectTo: '/login' } // Якщо ввели неіснуючу адресу, теж перекидає на логін
];
