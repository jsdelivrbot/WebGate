
import { Component, Inject} from '@angular/core';
import { Http } from '@angular/http';
import { MdHelperService, IPresentable } from "../../../service/md-helper.service";


@Component({
    selector: 'sd-incomings',
    templateUrl: './sd-incomings.component.html',

    providers: [MdHelperService]
})

export class SdIncomingsComponent {
    public svcSubUrl: string = 'NvaSd/Incoming';
    public svcMetadataUrl: string  = "";
    public incomings: NVASD_Incoming[];
    public present: IPresentable = { name: this.svcSubUrl, description: "Представление сервиса: " + this.svcSubUrl } ;

   

    //constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
    //    http.get(baseUrl + 'api/SampleData/WeatherForecasts').subscribe(result => {
    //        this.forecasts = result.json() as WeatherForecast[];
    //    }, error => console.error(error));


    constructor(private http: Http, @Inject('BASE_SVC_URL') baseSvcUrl: string, @Inject('BASE_SVC_MD_SFX') baseSvcUrlMdSfx: string, helper: MdHelperService) {
        //, helper : MdHelper
        this.http.get(baseSvcUrl + this.svcSubUrl).subscribe(data => {
            this.incomings = data.json() as NVASD_Incoming[];
            this.svcMetadataUrl = baseSvcUrl + this.svcSubUrl + baseSvcUrlMdSfx
        }, error => console.error(error))

        //console.info(helper.getMetadataValue("DisplayName", this.svcSubUrl, ""));

        //helper.getPrmsMetadataValue("Description", this.svcSubUrl, "")
        //    .then(
        //        responce => console.info(responce),
        //        error => console.error(error)
        //);

        var d: IPresentable = { name: "a", description : "" };

        helper.getPresentable(this.svcSubUrl)
            .then(responce => this.present = responce as IPresentable ,
                  error => console.error(error)
        );
        console.log("IPresentable:" + d.name); 
    }

    ngOnInit(): void {
        console.log(this.incomings);
    }
}

interface NVASD_Incoming {
    ID: Number;
    EventDateTime: Date;
    CreatedDateTime: Date;
    ServiceDescID: Number;
    EventTypeID: string;
    EventTextID: string;
    EventText: string;
    SysUserId: string;
    SysSessionId: string;
}
    

