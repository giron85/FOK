import { Component } from '@angular/core';
import { catchError } from 'rxjs/operators';

import { TestApiService } from './test-api.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-test-api',
  templateUrl: './test-api.component.html',
  styleUrls: ['./test-api.component.css']
})
export class TestApiComponent {

  public basket: any;
  public deliveries: any;
  public orders: any;
  public payments: any;
  public restaurants: any;
  public reviews: any;
  public users: any;

  constructor(
    private svc: TestApiService
  ) { }

  getBasket() {
    this.svc.getBasket()
      .subscribe(data => {
        this.basket = data;
      });
  }

  getDeliveries() {
    this.svc.getDeliveries()
      .subscribe(data => {
        this.deliveries = data;
      });
  }

  getOrders() {
    this.svc.getOrders()
      .subscribe(data => {
        this.orders = data;
      });
  }

  getPayments() {
    this.svc.getPayments()
      .subscribe(data => {
        this.payments = data;
      });
  }

  getRestaurants() {
    this.svc.getRestaurants()
      .subscribe(data => {
        this.restaurants = data;
      });
  }

  getReviews() {
    this.svc.getReviews()
      .subscribe(data => {
        this.reviews = data;
      });
  }

  getUsers() {
    this.svc.getUsers()
      .subscribe(data => {
        this.users = data;
      });
  }
}
