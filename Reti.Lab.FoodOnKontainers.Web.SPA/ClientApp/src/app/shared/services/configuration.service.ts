import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {

  constructor(
    private http: HttpClient,
    @Inject('BASE_URL') private baseUrl: string
    ) {
      this.loadConfiguration();
    }

    public config: any;

    loadConfiguration() {
      this.http.get(`${this.baseUrl}api/SampleData/MicroservicesEndpoint`)
        .subscribe(resp => {
          this.config = resp;
        });
    }
}
