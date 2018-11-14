import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'sd-incoming',
    templateUrl: './sd-incoming.component.html',
})

export class SdIncomingComponent {
    
    public svcSubUrl: string = 'NvaSd/Incoming';
    //public incoming: NVASD_Incoming;


    constructor(id: Number, private http: Http,  @Inject('BASE_SVC_URL') baseSvcUrl: string, @Inject('BASE_SVC_MD_SFX') baseSvcUrlMdSfx: string) {
        this.http.get(baseSvcUrl + this.svcSubUrl + '/' + id.toString() ).subscribe(data => {
            //this.incoming = data.json() as NVASD_Incoming;
        }, error => console.error(error))
    }


}


