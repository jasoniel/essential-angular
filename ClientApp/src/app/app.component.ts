import { Component } from '@angular/core';
import { Repository } from './models/repository'
import { Product } from './models/product.model'


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(private repo: Repository) { }

  get products(): Product[] {
    return this.repo.products;
  }
}
