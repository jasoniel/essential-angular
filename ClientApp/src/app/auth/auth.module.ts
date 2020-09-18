import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { AuthenticationComponent } from "./authentication.component";
import { AuthenticationGuard } from "./authentication.guard";
import { AuthenticationService } from "./authentication.service";

@NgModule({
  imports: [RouterModule, FormsModule, CommonModule],
  declarations: [AuthenticationComponent],
  providers: [AuthenticationService, AuthenticationGuard],
  exports: [AuthenticationComponent]
})
export class AuthModule {}
