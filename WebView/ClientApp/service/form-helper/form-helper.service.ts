import { Injectable } from '@angular/core';
import { IFieldMetadata } from "../data-prov/data-prov.interface";
import { LoggerService } from '../logger.service';
import { FormControl, FormGroup } from '@angular/forms';

// exclude interfaces : IMdProvider ITransProvider

const MODULE_NAME = 'John Galon';
const COMPONENT_NAME = 'FormHelper';

interface IFormGroup {
    [controlName: string]: any;
}

@Injectable()
export class FormHelperService {

    constructor(private logger: LoggerService) {
        this.log("Service activated... ");
    }

    // IFieldMetadata list to 
    toFormControls(from: IFieldMetadata[]) {
        var ret: IFormGroup = {};
        for (var fld in from) {
            if (from[fld].visible) {
                ret[from[fld].id] = this.toFormControl(from[fld]);
            }
        }
        //console.log(ret) 
        return ret;
    }
    
    // Some  IFieldMetadata info to FormControl
    toFormControl(from: IFieldMetadata) {
        var ctrl = new FormControl();

        //ctrl.disable(o ! from.visible);

        return ctrl;
    }
    
    //Local log helper func
    log(msg: any) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
    error(msg: any) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
}
