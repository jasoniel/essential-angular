import { Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Repository } from "../models/repository";

export class AuthenticationService {
  /**
   *
   */
  constructor(private repo: Repository, private router: Router) {}

  authenticated: boolean = false;
  name: string;
  password: string;
  callbackUrl: string;

  login(): Observable<boolean> {
    this.authenticated = false;

    return this.repo.login(this.name, this.password).pipe(
      map(response => {
        if (response) {
          this.authenticated = true;
          this.password = null;
          this.router.navigateByUrl(this.callbackUrl || "/admin/overview");
        }
        return this.authenticated;
      }),
      catchError(e => {
        this.authenticated = false;
        return of<boolean>(false);
      })
    );
  }

  logout() {
    this.authenticated = false;
    this.repo.logout();
    this.router.navigateByUrl("/admin/login");
  }
}
