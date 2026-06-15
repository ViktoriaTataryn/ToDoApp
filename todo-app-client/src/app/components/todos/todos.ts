import { Component, OnInit, ChangeDetectorRef } from '@angular/core'; // 🔥 Додали ChangeDetectorRef
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TodoService, TodoItem } from '../../services/todo';
import { CategoryService } from '../../services/category'; // 🔥 Додали імпорт сервісу категорій

@Component({
  selector: 'app-todos',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './todos.html',
  styleUrl: './todos.css'
})
export class TodosComponent implements OnInit {
  todos: TodoItem[] = [];
  categories: any[] = []; // Масив для категорій
  todoForm: FormGroup;
  errorMessage: string = '';
  isModalOpen: boolean = false;

  // 🔥 Впроваджуємо всі 3 потрібні сервіси через конструктор
  constructor(
    private todoService: TodoService, 
    private categoryService: CategoryService, // 🔥 Правильно підключили сервіс категорій
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef // 🔥 Підключили cdr
  ) {
    // Створюємо форму для додавання нового завдання
    this.todoForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      isImportant: [false],
      dueDate: [''],
      categoryId: [''] // Додали поле для категорії у форму
    });
  }

  ngOnInit(): void {
    console.log('Компонент завантажено, завантажую завдання та категорії...');
    this.loadTodos();
    this.loadCategories();
  }
sortTodos(): void {
    // 1. Спочатку сортуємо поточний масив
    this.todos.sort((a, b) => {
      if (a.isCompleted === b.isCompleted) {
        // Якщо статус виконання однаковий, сортуємо за важливістю (зірочкою)
        return (b.isImportant ? 1 : 0) - (a.isImportant ? 1 : 0);
      }
      return a.isCompleted ? 1 : -1;
    });

  
    this.todos = [...this.todos];
  }
  // Завантажуємо таски з бекенду
  loadTodos(): void {
    this.todoService.getTodos().subscribe({
      next: (data) => {
        this.todos = [...data]; 
        this.sortTodos();
        console.log('Завдання успішно отримані:', this.todos);
        this.cdr.detectChanges(); // Тепер працює без помилок
      },
      error: (err) => console.error('Помилка при завантаженні завдань:', err)
    });
  }

  // Завантажуємо категорії з бекенду
  loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (data: any[]) => {
        this.categories = data;
         this.sortTodos();
        this.cdr.detectChanges();
       
      },
      error: (err) => console.error('Помилка при завантаженні категорій:', err)
    });
  }

  openModal(): void {
    this.isModalOpen = true;
    this.cdr.detectChanges();
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.todoForm.reset({ title: '', description: '', isImportant: false, dueDate: '', categoryId: '' });
    this.cdr.detectChanges();
  }

  // Додавання нового завдання
  onSubmit(): void {
    if (this.todoForm.valid) {
      const newTodo: TodoItem = {
        title: this.todoForm.value.title,
        description: this.todoForm.value.description || '',
        isCompleted: false,
        isImportant: this.todoForm.value.isImportant,
        dueDate: this.todoForm.value.dueDate ? new Date(this.todoForm.value.dueDate).toISOString() : undefined,
        categoryId: this.todoForm.value.categoryId ? Number(this.todoForm.value.categoryId) : undefined
      };

      this.todoService.createTodo(newTodo).subscribe({
        next: (createdTodo) => {
          this.todos.push(createdTodo); // Додаємо в список на екрані
          this.closeModal(); // Закриваємо модалку та чистимо форму
          this.cdr.detectChanges();
        },
        error: (err) => console.error('Помилка створення:', err)
      });
    }
  }

  // Зміна статусу виконання (галочка)
 toggleComplete(todo: TodoItem): void {
    if (todo.id === undefined) return;
    
    // 1. Чітко фіксуємо нове значення у змінну
    const newStatus = !todo.isCompleted;
    const updatedTodo = { ...todo, isCompleted: newStatus };

    this.todoService.updateTodo(todo.id, updatedTodo).subscribe({
      next: () => {
        // 2. Присвоюємо локально саме те значення, яке відправили на сервер
        todo.isCompleted = newStatus; 
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Помилка оновлення статусу виконання:', err)
    });
  }

  // // Зміна статусу важливості (зірочка)
  // toggleImportant(todo: TodoItem): void {
  //   if (todo.id === undefined) return;

  //   const updatedTodo = { ...todo, isImportant: !todo.isImportant };

  //   this.todoService.updateTodo(todo.id, updatedTodo).subscribe({
  //     next: () => {
  //       todo.isImportant = !todo.isImportant;
  //       this.cdr.detectChanges();
  //     },
  //     error: (err) => console.error('Помилка при оновленні статусу важливості:', err)
  //   });
  // }
  // Зміна статусу важливості (зірочка)
  toggleImportant(todo: TodoItem): void {
    if (todo.id === undefined) return;

    const newStatus = !todo.isImportant; // 1. Перевертаємо саме важливість
    const updatedTodo = { ...todo, isImportant: newStatus };

    this.todoService.updateTodo(todo.id, updatedTodo).subscribe({
      next: (responseTodo) => {
        // 🔥 ПЕРЕВІР ОЦЕЙ РЯДОК:
        // Тут має бути тільки isImportant! Якщо випадково було написано isCompleted, 
        // то Angular на мить закреслював таску на екрані.
        todo.isImportant = newStatus; 
        
        // Пересортировуємо, бо важливі таски мають піднятися вгору всередині своєї групи
        this.sortTodos(); 
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Помилка оновлення статусу важливості:', err)
    });
  }

  // Видалення завдання
  deleteTodo(id: number | undefined): void {
    if (id === undefined) return;

    if (confirm('Ви впевнені, що хочете видалити це завдання?')) {
      this.todoService.deleteTodo(id).subscribe({
        next: () => {
          this.todos = this.todos.filter(t => t.id !== id);
          this.cdr.detectChanges();
        },
        error: (err) => console.error('Помилка видалення:', err)
      });
    }
  }
 
}