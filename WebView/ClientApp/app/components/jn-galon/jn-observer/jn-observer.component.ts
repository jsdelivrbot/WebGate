import { Component} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DataProvService } from "../../../../service/data-prov/data-prov.service";
import { DataAdapterService } from "../../../../service/data-prov/data-prov-md-adapter.service";
import { LoggerService } from "../../../../service/logger.service";
import { IPresentMetadata } from "../../../../service/data-prov/data-prov.interface";


const SUB_SOURCE_PARAM_DATA_KEY = 'SvcSubSource';
const MODULE_NAME = 'John Galon';
const COMPONENT_NAME = 'Оbserver';

@Component({
    selector: 'jn-observer',
    templateUrl: './jn-observer.component.html',
    providers: [DataProvService, LoggerService, DataAdapterService ] 
})

export class JnObserverComponent {


    public svcSubUrl: string = '';
    public data: any; 
    public presenter: IPresentMetadata = { name: this.svcSubUrl, description: "Представление сервиса: " + this.svcSubUrl };;

    constructor(private route: ActivatedRoute, private mdHelper: DataProvService , private logger: LoggerService) {
    }

    ngOnInit() {
        this.route.data.subscribe(x => this.data = x)
        this.svcSubUrl = this.data[SUB_SOURCE_PARAM_DATA_KEY];
        this.presenter = { name: this.svcSubUrl, description: "Представление сервиса: " + this.svcSubUrl };
          
        this.log('Set new source - ' + this.svcSubUrl);

        //console.log(responce);
        this.mdHelper.getPresentable(this.svcSubUrl)
            .then(responce => {  this.presenter = responce as IPresentMetadata },
             error => this.error(error )
            );

        //this.mdHelper.getPresentMetadataAsync(this.svcSubUrl)
        //    .then(responce => console.log(!responce || responce === undefined  ? "хуя" : responce.toString()) 
        //    ,   error => this.error(error )
        //    );

        
        // разремить
        //this.mdHelper.getPresentable(this.svcSubUrl)
        //    .then(responce => this.presenter = responce as IPresentable,
        //    error => this.error(error )
        //    );
        

        //console.log(this.mdHelper.getAsyncMetadatas()) 

    }

    ngOnDestroy() {
        //this.route.data.unsubscribe();
    }

    log(msg: string) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
    error(msg: string) {
        this.logger.error(msg, MODULE_NAME, COMPONENT_NAME);
    }


}


