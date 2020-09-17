import { Order } from "../../models/order.model";
import { Router } from "@angular/router";
import { Component } from "@angular/core";

@Component({
  templateUrl: "checkoutDetails.component.html"
})
export class CheckoutDetailsComponent {
  constructor(private router: Router, public order: Order) {
    if (order.products.length == 0) {
      this.router.navigateByUrl("/cart");
    }
  }
}
