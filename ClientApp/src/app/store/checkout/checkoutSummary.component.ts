import { Router } from "@angular/router";
import { Order } from "src/app/models/order.model";
import { Component } from "@angular/core";

@Component({
  templateUrl: "checkoutSummary.component.html"
})
export class CheckoutSummaryComponent {
  constructor(private router: Router, public order: Order) {
    if (
      order.payment.cardNumber == null ||
      order.payment.cardSecurityCode == null ||
      order.payment.cardExpiry == null
    ) {
      console.table(order);
      router.navigateByUrl("/checkout/step2");
    }
  }

  submitOrder() {
    this.order.submit();
    this.router.navigateByUrl("/checkout/confirmation");
  }
}
