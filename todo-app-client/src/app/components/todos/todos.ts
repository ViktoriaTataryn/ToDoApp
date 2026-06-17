import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TodoService, TodoItem } from '../../services/todo';
import { CategoryService } from '../../services/category';
import { FormsModule } from '@angular/forms';

export type TodoFilter = 'all' | 'important' | 'planned' | 'completed'|number;

@Component({
  selector: 'app-todos',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,FormsModule],
  templateUrl: './todos.html',
  styleUrl: './todos.css'
})
export class TodosComponent implements OnInit {
  todos: TodoItem[] = [];
  categories: any[] = []; 
  todoForm: FormGroup;
  errorMessage: string = '';
  isModalOpen: boolean = false;
  currentFilter: TodoFilter = 'all';
  // Змінна для збереження тексту пошуку категорій
categorySearchQuery: string = '';

  constructor(
    private todoService: TodoService, 
    private categoryService: CategoryService, 
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef 
  ) {
    this.todoForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      isImportant: [false],
      dueDate: [''],
      categoryId: [''] 
    });
  }

  ngOnInit(): void {
    console.log('Компонент завантажено, завантажую завдання та категорії...');
    this.loadTodos();
    this.loadCategories();
  }

  //  Оновлене безпечне сортування
  sortTodos(): void {
    // Сортуємо тільки через створення нової копії, щоб Angular чітко бачив зміни стану
    this.todos = [...this.todos].sort((a, b) => {
      if (a.isCompleted === b.isCompleted) {
        return (b.isImportant ? 1 : 0) - (a.isImportant ? 1 : 0);
      }
      return a.isCompleted ? 1 : -1;
    });
  }

  loadTodos(): void {
    this.todoService.getTodos().subscribe({
      next: (data) => {
        this.todos = [...data]; 
        this.sortTodos();
        console.log('Завдання успішно отримані:', this.todos);
        this.cdr.detectChanges(); 
      },
      error: (err) => console.error('Помилка при завантаженні завдань:', err)
    });
  }

get filteredTodos(): TodoItem[] {
  // Якщо фільтр не вибрано або він системний ("Усі"), повертаємо весь список
  if (!this.currentFilter || this.currentFilter === 'all') {
    return this.todos;
  }

  // Перевірка інших системних фільтрів (якщо вони є)
  if (this.currentFilter === 'important') {
    return this.todos.filter(todo => todo.isImportant || (todo as any).IsImportant);
  }
  if (this.currentFilter === 'planned') {
    return this.todos.filter(todo => todo.dueDate || (todo as any).DueDate);
  }

  return this.todos.filter(todo => {
  
    const todoCatId = todo.categoryId ?? (todo as any).CategoryId;
    
    if (!todoCatId) return false;
    

    return todoCatId.toString() === this.currentFilter.toString();
  });
}
  getCategoryCount(categoryId: number): number {
  if (!this.todos) return 0;
  // Фільтруємо наш головний масив завдань за ID категорії та повертаємо його довжину
  return this.todos.filter(todo => todo.categoryId === categoryId).length;
}

  changeFilter(filter: TodoFilter): void {
    this.currentFilter = filter;
    this.cdr.detectChanges(); 
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe({
      next: (data: any[]) => {
        this.categories = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Помилка при завантаженні категорій:', err)
    });
  }
  isCategoryModalOpen: boolean = false;
categoryErrorMessage: string = '';
openAddCategoryModal(): void {
  this.isCategoryModalOpen = true;
  this.categoryErrorMessage = ''; // Скидаємо помилки
  this.cdr.detectChanges();
}

// 3. Метод закриття модалки
closeCategoryModal(): void {
  this.isCategoryModalOpen = false;
  this.categoryErrorMessage = '';
  this.cdr.detectChanges();
}

// 4. Метод відправки форми на бекенд
submitCategory(name: string): void {
  if (!name || name.trim().length < 2) {
    this.categoryErrorMessage = 'Назва повинна містити мін. 2 символи';
    this.cdr.detectChanges();
    return;
  }

  this.categoryErrorMessage = '';
  
 
  const newCategory = {
    title: name.trim() 
  };

  console.log('Відправка правильної моделі на бекенд:', newCategory);

  this.categoryService.createCategory(newCategory).subscribe({
    next: (createdCategory) => {
      console.log('Категорію успішно створено бекендом:', createdCategory);
      
     
     this.categories = [...this.categories, createdCategory];
      
      
      this.closeCategoryModal();
    },
    error: (err) => {
      console.error('Помилка при створенні категорії:', err);
      this.categoryErrorMessage = `Помилка сервера (${err.status}). Перевірте дані.`;
      this.cdr.detectChanges();
    }
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

  // Геттер для відфільтрованих категорій у сайдбарі
get filteredCategories(): any[] {
  if (!this.categorySearchQuery || this.categorySearchQuery.trim() === '') {
    return this.categories;
  }
  
  const query = this.categorySearchQuery.toLowerCase().trim();
  
  return this.categories.filter(cat => {
    const title = cat.title || cat.Title || '';
    return title.toLowerCase().includes(query);
  });
}

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
         
          if ((createdTodo as any).CategoryId && !createdTodo.categoryId) {
      createdTodo.categoryId = Number((createdTodo as any).CategoryId);
    }
    

    if (!createdTodo.categoryId && this.todoForm.value.categoryId) {
      createdTodo.categoryId = Number(this.todoForm.value.categoryId);
    }

    this.todos = [...this.todos, createdTodo]; 
    
    this.sortTodos(); 
    this.closeModal(); 
    this.cdr.detectChanges();
        },
        error: (err) => console.error('Помилка створення:', err)
      });
    }
  }

 
  toggleComplete(todo: TodoItem): void {
    if (todo.id === undefined) return;
    
    const newStatus = !todo.isCompleted;
    const updatedTodo = { ...todo, isCompleted: newStatus };

    this.todoService.updateTodo(todo.id, updatedTodo).subscribe({
      next: () => {
        todo.isCompleted = newStatus; 
        this.sortTodos(); 
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Помилка оновлення статусу виконання:', err)
    });
  }


  toggleImportant(todo: TodoItem): void {
    if (todo.id === undefined) return;

    const newStatus = !todo.isImportant; 
    const updatedTodo = { ...todo, isImportant: newStatus };

    this.todoService.updateTodo(todo.id, updatedTodo).subscribe({
      next: () => {
        todo.isImportant = newStatus; 
        this.sortTodos(); 
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Помилка оновлення статусу важливості:', err)
    });
  }

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
  // Геттер для динамічного заголовка сторінки
get currentFilterTitle(): string {
  if (!this.currentFilter || this.currentFilter === 'all') {
    return 'Усі завдання';
  }
  if (this.currentFilter === 'important') {
    return 'Важливі завдання';
  }
  if (this.currentFilter === 'planned') {
    return 'Заплановані завдання';
  }

  // Якщо фільтр — це ID категорії, шукаємо цю категорію в масиві, щоб узяти її Title
  const activeCategory = this.categories.find(
    cat => (cat.id || cat.Id)?.toString() === this.currentFilter.toString()
  );

  return activeCategory ? (activeCategory.title || activeCategory.Title) : 'Завдання';
}
}