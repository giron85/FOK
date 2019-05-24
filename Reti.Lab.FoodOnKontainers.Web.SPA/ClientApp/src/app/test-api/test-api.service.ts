import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ConfigurationService } from '../shared/services/configuration.service';

@Injectable({
  providedIn: 'root'
})
export class TestApiService {

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string,
    private configSvc: ConfigurationService
    ) { }

    private getApiUrl(apiKey: string) {
      return `${this.configSvc.config['gatewayApiClient']}/api/gateway/${apiKey}/values`;
      // return `${this.configSvc.config[`${apiKey}ApiClient`]}/api/values`;
    }

    getBasket() {
      return this.http.get(this.getApiUrl('baskets'))
        .pipe(map((resp: Response) => {
          return resp;
      }));
    }

    getDeliveries() {
      return this.http.get(this.getApiUrl('deliveries'))
        .pipe(map((resp: Response) => {
          return resp;
      }));
    }

    getOrders() {
      return this.http.get(this.getApiUrl('orders'))
        .pipe(map((resp: Response) => {
          return resp;
      }));
    }

    getPayments() {
      return this.http.get(this.getApiUrl('payments'))
        .pipe(map((resp: Response) => {
          return resp;
      }));
    }

    getRestaurants() {
      return this.http.get(this.getApiUrl('restaurants'))
        .pipe(map((resp: Response) => {
          return resp;
      }));
    }

    getReviews() {
      return this.http.get(this.getApiUrl('reviews'))
        .pipe(map((resp: Response) => {
          return resp;
      }));
    }

    getUsers() {
      return this.http.get(this.getApiUrl('users'))
        .pipe(map((resp: Response) => {
          return resp;
      }));
    }
}
