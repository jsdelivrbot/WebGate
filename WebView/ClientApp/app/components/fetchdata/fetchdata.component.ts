import { Component, Inject } from '@angular/core'; 
import { Http } from '@angular/http';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})

export class FetchDataComponent {
    public forecasts: WeatherForecast[];
    public test: string;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string, @Inject('BASE_URL2') baseUrl2: string) {
        http.get(baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
            this.forecasts = result.json() as WeatherForecast[];
            this.test = baseUrl2;
        }, error => console.error(error));
    }
}

interface WeatherForecast {
    dateFormatted: string;
    temperatureC: number;
    temperatureF: number;
    summary: string;
}
