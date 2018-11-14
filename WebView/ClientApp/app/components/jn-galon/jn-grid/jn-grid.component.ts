import { Component, Input } from '@angular/core';
//import { MdHelperService, IPresentable } from "../../../../service/md-helper.service";
import { LoggerService } from "../../../../service/logger.service";

import { DataProvService } from "../../../../service/data-prov/data-prov.service";
import { IPresentMetadata } from "../../../../service/data-prov/data-prov.interface";


const MODULE_NAME = 'John Galon';
const COMPONENT_NAME = 'Grid';

@Component({
    selector:     'jn-grid',
    templateUrl:  './jn-grid.component.html',
    styleUrls:   ['./jn-grid.component.css'] 
})

export class JnGridComponent {
    
    @Input() svcSubUrl: string = ""; 
    
    rows:     Promise<{}>       | null = null;
    cols:     Promise<string[]> | null = null;
    colsPres: Promise<IPresentMetadata[]> | null = null;

    constructor(private mdHelper: DataProvService, private logger: LoggerService) {
        
    }

    ngOnChanges(changes: any) {
        this.log("onChange SubURL:" + this.svcSubUrl );
        this.reset();
    }

    reset() {
        if (this.svcSubUrl != "") {
            this.log("Reseting... Load data...");
            //this.cols = this.mdHelper.getFields(this.svcSubUrl).then(x => { console.log(x); return x;});
            this.cols = this.mdHelper.getFields(this.svcSubUrl);
            this.rows = this.mdHelper.getAsyncDatasJson(this.svcSubUrl);
            //this.rows = this.mdHelper.getPrmsData(this.svcSubUrl).then(x => JSON.parse(x.toString()));
            this.colsPres = this.mdHelper.getFieldsPresentMetadata(this.svcSubUrl);
        }
    }

    log(msg: string) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
    error(msg: string) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
}


