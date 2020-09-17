import { Order } from "src/app/models/order.model";
import { Router } from "@angular/router";
import { Component } from "@angular/core";

@Component({
  templateUrl: "orderConfirmation.component.html"
})
export class OrderConfirmationComponent {
  constructor(private router: Router, private order: Order) {
    if (!order.submited) {
      router.navigateByUrl("/checkout/step3");
    }
  }
}
