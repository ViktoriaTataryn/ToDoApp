import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryService } from '../../services/category';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './categories.html',
  styleUrls: ['./categories.css']
})
export class CategoriesComponent implements OnInit {
  categories: any[] = [];
  categoryForm: FormGroup;

  constructor(private categoryService: CategoryService, private fb: FormBuilder) {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]]
    });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.categoryService.getCategories().subscribe(data => {
      this.categories = data;
    });
  }

  addCategory(): void {
    if (this.categoryForm.valid) {
      this.categoryService.createCategory(this.categoryForm.value).subscribe(() => {
        this.loadCategories(); // Оновлюємо список
        this.categoryForm.reset();
      });
    }
  }

  deleteCategory(id: number): void {
    if (confirm('Видалити категорію? Усі задачі в ній можуть втратити прив\'язку.')) {
      this.categoryService.deleteCategory(id).subscribe(() => {
        this.loadCategories();
      });
    }
  }
}
