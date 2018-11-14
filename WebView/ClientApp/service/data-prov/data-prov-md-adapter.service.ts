import { Injectable } from '@angular/core';
import { LoggerService } from "../logger.service";
import { IMetadatas, IMetadataTools } from "./data-prov.interface";
import { TransformProviderBuilder } from '../data-caster/data-caster.service'

// exclude interfaces : IMdProvider ITransProvider

const MODULE_NAME = 'John Galon';
const COMPONENT_NAME = 'DataProvMdAdapterService';

@Injectable()
export class DataAdapterService {

    constructor(private logger: LoggerService ) {
        this.log("Service activated... ");
    }

    public getBuilder<TTarget, TFrom>() {
        return new MetadataProviderBuilder<TTarget, TFrom>();
    }

    //Local log helper func
    log(msg: any) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
    error(msg: any) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
}


//Адаптация под метаданные
export class MetadataProviderBuilder<TTarget, TFrom> extends TransformProviderBuilder<IMetadatas, TTarget, TFrom, IMetadataTools>{
    constructor(){
        super(); // call to default constructor added implicitly     
        this.setTools(new BaseMetadataTools());
    }
}

// Попробуем тулзы
export class BaseMetadataTools implements IMetadataTools {

    ifNullOrUndef(val: any, valDef: any) {
        return (val === null || val === undefined) ?valDef: val;
    }

    valueOf(source: IMetadatas, key: string): any {
        return source[key];
    } 

    firstValueOf(source: IMetadatas, keys: string[]): any {
        var ret: any = null;

        for (var s in keys) {
            var v = this.valueOf(source, keys[s]);
            if (v) {
                ret = v;
                break;
            }
        }
        return ret;
    } 

    ifNull(val: any, valDef: any) {
        return (val === null) ? valDef : val;
    }

    valueOfFunc(source: IMetadatas, keyAcc: [string, (src: any) => any]) {

        var v = this.valueOf(source, keyAcc[0]);
        return (v == undefined || v == null ) ? v: keyAcc[1](this.valueOf(source, keyAcc[0]));
    }

    firstValueOfFunc(source: IMetadatas, keysAcc: [string, (src: any) => any][]) {
        var ret: any = null;
        for (var s in keysAcc) {
            var v = this.valueOfFunc(source, keysAcc[s]);
            if (v) {
                ret = v;
                break;
            }
        }
        return ret;
    }

}

