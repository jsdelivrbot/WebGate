import { Component, Input, SimpleChanges } from '@angular/core';
import { LoggerService } from "../../../../service/logger.service";
import { IPresentable } from "../../../../service/md-helper.service";

const SUB_SOURCE_PARAM_DATA_KEY = 'SvcSubSource';

const MODULE_NAME = 'John Galon';
const COMPONENT_NAME = 'GridRow';

@Component({
    selector: '[jn-grid-row]',
    templateUrl: './jn-grid-row.component.html',
    styleUrls: ['./jn-grid-row.component.css'] 
})

export class JnGridRowComponent {

    @Input() inputIsHeader: boolean = false; 

    // Row data input fields
    @Input() values: string[] | null = null; 
    @Input() valuesAsync: Promise<string[]> | null = null; 
    @Input() valuesJson: any | null = null;
    @Input() valuesJsonAsync: Promise<any> | null = null;
    @Input() presentableAsync: Promise<IPresentable[]> | null = null; 
    //-------------------------------------------------------------

    data: string[] | Promise<string[]> | null = null;
    isAsync: boolean = false;

    constructor( private logger: LoggerService) {
    } 

    ngOnChanges(changes: SimpleChanges) {

        if (changes["values"] != null) {
            this.data = this.values;
            this.isAsync = false;
            //this.log("Set sync data source from array...")
        }
        else if (changes["valuesAsync"] != null) {
            this.data = this.valuesAsync;
            this.isAsync = true;
            //this.log("Set async data source from array...")
        }
        else if (changes["valuesJson"] != null) {
            this.data = this.jsonToValuesArray(this.valuesJson);
            this.isAsync = false;
            //this.log("Set sync data source from JSON..." + this.valuesJson.id.toString());
        }
        else if (changes["presentableAsync"] != null) {
            var ps = this.presentableAsync as Promise<IPresentable[]>;

            //ps.then(x => console.log(x[0].name));

            if (ps != null) {
                this.data = ps.then(prs => prs.map(x => x.name)) ;
            }
            this.isAsync = true;
            //this.log("Set sync data source from JSON..." + this.valuesJson.id.toString());
        }
    }

    jsonToValuesArray(inData: any) {
        var r: string[] = [];
        for (var key in inData) {
            r.push(inData[key]);
        }
        return r;
    }
    
   
    log(msg: any) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
    error(msg: any) {
        this.logger.error(msg, MODULE_NAME, COMPONENT_NAME);
    }

}


