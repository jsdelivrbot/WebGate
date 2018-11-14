import { Component } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'sd-incomings',
    templateUrl: './sd-incoming-add.component.html',
    
})

export class SdIncomingsComponent {
    public name: string = 'eeeee';
    public incomings: SdIncomings[];


    //constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
    //    http.get(baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
    //        this.forecasts = result.json() as WeatherForecast[];
    //    }, error => console.error(error));


    constructor(private http: Http) {
        this.http.get('http://webgate.nvavia.ru:8080/api/NvaSd/Incoming').subscribe(data => {
            this.incomings = data.json() as SdIncomings[];
        }, error => console.error(error))
    }

    public incrementCounter() {
        console.log(this.incomings);

    }

    ngOnInit(): void {
        console.log(this.incomings);
    }
}

interface SdIncomings {
    eventDateTime: string;
    serviceDescID: number;
}
    

