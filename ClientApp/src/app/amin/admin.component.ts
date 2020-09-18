import { Repository } from "../models/repository";
import { Component } from "@angular/core";

@Component({
  templateUrl: "admin.component.html"
})
export class AdminComponent {
  constructor(private repo: Repository) {
    repo.filter.reset();
    repo.filter.related = true;
    this.repo.getProducts();
    this.repo.getSuppliers();
    this.repo.getSuppliers();
    this.repo.getOrders();
  }
}
