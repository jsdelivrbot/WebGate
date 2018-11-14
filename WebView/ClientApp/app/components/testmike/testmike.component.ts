import { Component } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'testmike',
    templateUrl: './testmike.component.html',
})

export class TestmikeComponent {
    Name: string = 'eeeee';


    constructor(private http: Http) {

    }

    ngOnInit(): void {
        //console.log('$location', $location.$$absUrl);
        //console.log($browser.baseHref());
        this.http.get('http://webgate.nvavia.ru:8080/api/NvaSd/Incoming').subscribe(data => {
        //    console.log(data);
        });
    }

    
}
