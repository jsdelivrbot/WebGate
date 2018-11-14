import { Component, Input } from '@angular/core';
import { DataProvService } from "../../../../service/data-prov/data-prov.service";
import { IFieldMetadata } from "../../../../service/data-prov/data-prov.interface";
import { LoggerService } from "../../../../service/logger.service";

const MODULE_NAME = 'John Galon';
const COMPONENT_NAME = 'Item-field';

@Component({
    selector:    'jn-item-field',
    templateUrl: './jn-item-field.component.html',
    styleUrls: ['./jn-item-field.component.css'], 
    providers: [DataProvService, LoggerService]
})

export class JnItemFieldComponent {
    @Input() svcSubUrl: string = ""; 
    @Input() metadata: IFieldMetadata | null = null;

    name: string = "";

    constructor(private mdHelper: DataProvService, private logger: LoggerService) {
        
    }

    ngOnChanges(changes: any) {
        console.log(this.metadata);
        if (changes["metadata"] != null) {
            console.log(this.name);
            this.name = (this.metadata != null) ? this.metadata.description : "";
            console.log(this.name);
        }            
    }

    reset() {
        if (this.metadata !== null) {
            console.log(this.metadata);

        }
    }

    log(msg: string) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
    error(msg: string) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
}


