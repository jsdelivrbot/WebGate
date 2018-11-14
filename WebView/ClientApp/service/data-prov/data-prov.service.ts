//    _  _  _ _  ____
//   / \/ |( / |/ /_ \
//  /     / /   _/ __/
//  \/\/\_\/\/\_\___/
//  
//  28.03.2018 Data controller prototype beta
//  Цель: систематизация md-helpera, полный от него отказ  
//  ...по хорошему надо с промисов на обсерваблы все перевести...
//  10.04.2018 Implementing create item.. тодо
// 
//////////////////////////////////


import { Injectable, Inject } from '@angular/core';
import { Http } from '@angular/http'; 
import { IMetadatas, IPresentMetadata, IMetadataTools, IFieldMetadata, IDatas, ILookUpMetadata, ILookUpData } from "./data-prov.interface";
import { DataAdapterService} from "./data-prov-md-adapter.service";
import { LoggerService } from "../logger.service";



import 'rxjs/add/operator/map'
import 'rxjs/add/operator/mergeAll'
import 'rxjs/add/operator/merge'
import 'rxjs/add/operator/mergeMap'
import 'rxjs/add/operator/concatMap'
import 'rxjs/add/operator/concat'
import 'rxjs/add/operator/toArray'
import 'rxjs/add/operator/filter'

import 'rxjs/add/observable/from'
//import 'rxjs/add/Observable/take'

import { Observable } from 'rxjs/observable';


// exclude interfaces  IMdProvider

const FK_MACRO_BEGIN = "{"; 
const FK_MACRO_END = "}";

const MODULE_NAME    = 'John Galon';
const COMPONENT_NAME = 'DataProvider';


@Injectable()
export class DataProvService {

    constructor(private http: Http,
        private logger: LoggerService,
        private dataAdapter: DataAdapterService,
        @Inject('BASE_SVC_URL') private baseSvcUrl: string,
        @Inject('BASE_SVC_MD_SFX') private baseSvcUrlMdSfx: string
    ) {
        this.log("Service activated... ");
    }

    // local log helper func
    log(msg: string) { 
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
    error(msg: string) {
        this.logger.error(msg, MODULE_NAME, COMPONENT_NAME);
    }

    // Uri prepare tools -----------------------------------------------
    // 060418 
    // В связи с использованием референсов на внешние сервисы необходимо 
    // парсить локатион
    private prepareLocation(location: string, isCutTail: boolean = false) {
        function cutTail(l: string) {
            return l.indexOf("?") >= 0 ? l.substring(0, l.indexOf("?")) : l;
        }
        location = (isCutTail) ? cutTail(location) : location;
        var ret = "";
        if (location.startsWith("http://"))     { throw new Error("Not implement"); }
        else if (location.startsWith("../"))    { throw new Error("Not implement"); }
        else if (location.startsWith("./"))     { ret = this.baseSvcUrl + location.substr(2);}
        else                                    { ret = this.baseSvcUrl + location; }
        return ret;
    }
    


    // Выделяет из Лукашина макросы
    // helper func
    public getLocationMacros(location: string) {
        var recFun = (s: string, r: string[]) => {
            var bp = s.indexOf(FK_MACRO_BEGIN)
            if (bp > 0 && s.length > (bp+1) ) {
                var ss = s.substring(bp+1);
                var ep = ss.indexOf(FK_MACRO_END);
                if (ep > 0) {
                    r.push(ss.substring(0, ep));
                    r = recFun(ss.substring(ep), r);
                }
            }
            return r;
        }

        return recFun(location, []);
    }

    // Заполняет макросы Лукашина валуями
    public buildLocationWithMacrosValues(location: string, values: any[]) {
        //console.log(location);
        //console.log(values);
        var mcs = this.getLocationMacros(location);
        if (mcs.length > values.length) {
            throw Error("Can't aplay values '" + values.toString() + "' on Uri '" + location+"'" )
        }
        var i = 0;
        for (let v of mcs) {
            location = location.replace(FK_MACRO_BEGIN + v + FK_MACRO_END, values[i]);
            i++;
        }
        return location;
    }


    // Uri for service data
    private buildDataUri(location: string, field: string | null = null) {
        return this.prepareLocation(location) + ((typeof field === "string" && field != "") ? ("/" + field) : (""));
    }
    // Uri for service metadata
    private buildMetadataUri(location: string, field: string | null = null) {
        return this.prepareLocation(location,true) + this.baseSvcUrlMdSfx + ((typeof field === "string" && field != "") ? ("/" + field) : (""));
    }
    // End Uri prepare tools --------------------------------------------

    
    //Backend acess points ---------------------------------------------- 
    //BASE Data Service Acess by URI
    private getAsyncDataFromUri(uri: string) {
        var ht = this.http.get(uri);
        var ret = new Promise(function (resolve, reject) {
            var d: any;
            ht.subscribe(
                data => { d = data.text(); }
                , error => reject(error)
                , () => resolve(d));
        });
        return ret;
    }

    // BASE Observable release v2
    private getDataFromUri(uri: string) {
        return this.http.get(uri)
            .map(resp => resp.text());
    }
    //--------------------------------------------------------------------

    // PROMISE ACCESSORS --------------------------------------------------
    //
    // Return all data as Promise  (LOW LEVEL)
    // location: string  - related location
    // field: string - property(field) name,  null - self object 
    public getAsyncDatasLowLevel(location: string, field: string | null = null) {
        //var uri = this.prepareLocation(location) + ((typeof field === "string" && field != "") ? ("/" + field) : (""));
        return this.getAsyncDataFromUri(
            this.buildDataUri(location, field)
        );
    }

    // Return all metadata as Promise  (LOW LEVEL)
    // location: string  - related location
    // field: string - property(field) name,  null - self object 
    public getAsyncMetadatasLowLevel(location: string, field: string | null = null) {
       // var uri = this.prepareLocation(location) + this.baseSvcUrlMdSfx + ((typeof field === "string" && field != "") ? ("/" + field) : (""));
        return this.getAsyncDataFromUri(
            this.buildMetadataUri(location, field)
        );
    }
    
    // promise Data as Json 
    public getAsyncDatasJson(location: string, field: string | null = null) {
        return this.getAsyncDatasLowLevel(location, field).then(x => JSON.parse(x.toString()));
    }

    // Return metadatas casting to IMetadatas
    // location: string  - related location
    // field: string - subobject (property, field | other ) name,  null - root object 
    public getAsyncMetadatas(location: string, field: string | null = null) {
        return this.getAsyncMetadatasLowLevel(location, field).then(x => this.toMetadataSource(x.toString()));
    }

    // 130418 Возможно нужно убрати !!!
    // to IMetadataSource converter
    private toMetadataSource(sourceData: string) {
        var ret: IMetadatas = {};
        if (sourceData !== "" && sourceData != null && sourceData != undefined) {
            var data = JSON.parse(sourceData);
            for (var key in data) {
                ret[key] = data[key];
            }
        }
        return ret;
    }

    // OBSERVABLE ACCESSORS --------------------------------------------------
    // Return all data as Observable  (LOW LEVEL)
    // location: string  - related location
    // field: string - property(field) name,  null - self object 
    public getDataLowLevel(location: string, field: string | null = null) {
        //var uri = this.prepareLocation(location) + ((typeof field === "string" && field != "") ? ("/" + field) : (""));
        return this.getDataFromUri(
            this.buildDataUri(location, field)
        );
    }
    // Return all metadata as Observable  (LOW LEVEL)
    // location: string  - related location
    // field: string - property(field) name,  null - self object 
    public getMetadataLowLevel(location: string, field: string | null = null) {
        // var uri = this.prepareLocation(location) + this.baseSvcUrlMdSfx + ((typeof field === "string" && field != "") ? ("/" + field) : (""));
        return this.getDataFromUri(
            this.buildMetadataUri(location, field)
        );
    }

    // Observable Data as Json
    public getDatasJson(location: string, field: string | null = null) {
        return this.getDataLowLevel(location, field)
            .map(x => JSON.parse(x));
    }

    // горбылечек as iterable кастинг
    public getDataAsList(location: string, field: string | null = null){
        return this.getDatasJson(location, field)
            .map(x => {
                var r = <any[]>x;
                return (r === null) ? [] : r;
            })
    }

    // Observable Metadatas as IMetadata
    public getMetadatas(location: string, field: string | null = null) {
        return this.getMetadataLowLevel(location, field)
            .map(x => this.toMetadataSource(x)); // toString()
    }
    

    /* ----------------------------------------------------------------
    ** Item ubdate region    
    ** TODO 
    */
    public insertNewItem(location: string, data: IDatas) {
        var uri = this.prepareLocation(location);
        var ht = this.http.post(uri, data);

        var ret = new Promise(function (resolve, reject) {
            var d: any;
            ht.subscribe(
                data => { d = data.text(); console.log(d); }
                , error => reject(error)
                , () => resolve(d));
        });
        return ret;
    }




    //----------------------------------------------------------------------------------------------------------------------
    // Базовые конверторы 

    // Специфическая штука
    // Вот to Root Metadatas to field list as string[]
    private toFieldsList(){
        return this.dataAdapter.getBuilder<string[], any>()
            .setCreateFunc(() => [])
            .setFillFunc((frm, to) => {
                for (var key in frm) {
                    if (key[0] == '[') { to.push(key.substring(1, key.length - 1)); }
                }
                return to;
            })
            .Build()
    }


    //Вот IPresentMetadata
    private toPresentProvider( ) {
        return this.dataAdapter.getBuilder<IPresentMetadata, string >()
            .setCreateFuncEx((dv:string ) => {
                var md: IPresentMetadata = {
                    name: dv,
                    description: dv
                };
                return md;
            })
            .setFillExFunc((frm, to, tls) => {
                var v = tls.firstValueOf(frm, ["DisplayName", "Display.Name"])
                to.name = (v != null) ? v : to.name;
                v = tls.firstValueOf(frm, ["Description", "Display.Description"])
                to.description = (v != null) ? v : to.name;
                
                return to;
            })
            .Build();
    }

    // Вот IFieldMetadata
    private toFieldMetadataProvider() {
        return this.dataAdapter.getBuilder<IFieldMetadata, string>()
            .setCreateFuncEx((dv: string) => {
                var md: IFieldMetadata = {
                    id: dv,
                    foreignKey: "",
                    type: "string",
                    name: dv,
                    description: dv,
                    visible: true,
                    required: false,
                    defaultValue: undefined,
                    length: undefined
                };
                return md;
            })
            .setFillExFunc((frm, to, tls) => {
                to.name = tls.ifNull(tls.firstValueOf(frm, ["DisplayName", "Display.Name"]), to.name);
                to.description = tls.ifNull(tls.firstValueOf(frm, ["Description", "Display.Description"]), to.description);
                to.foreignKey = tls.ifNull(tls.firstValueOf(frm, ["ForeignKey", "ForeignKey.Name"]), to.foreignKey);
                to.required = tls.ifNull(tls.firstValueOfFunc(frm, [
                    ["Required", (x => x)],
                    ["Required.AllowEmptyStrings", (x => !x)]
                ]), to.required);
                to.visible = tls.ifNullOrUndef(   tls.valueOfFunc(frm, ["Editable.AllowEdit", (x => x)]), to.visible);  // нецелевое использование 
                //to.visible = tls.ifNull(tls.firstValueOf(frm, ["DisplayName", "Display.Name"]), to.name);
                //to.defaultValue = tls.ifNull(tls.firstValueOf(frm, ["Description", "Display.Description"]), to.description);
                //console.log(to.name);
                //console.log(to);
                return to;
            })
            .Build();
    }

    // Вот ILookUpMetadata
    // Iface for Lookup reference lists 
    private toLookUpMetadataProvider() {
        return this.dataAdapter.getBuilder<ILookUpMetadata, string>()
            .setCreateFunc(() => {
                var md: ILookUpMetadata = {
                    id: "",
                    labelFld: "",
                    sortFld: "" 
                };
                return md;
            })
            .setFillExFunc((frm, to, tls) => {
                to.id = tls.ifNull(tls.firstValueOf(frm, ["Key", "KEY"]), to.id);
                to.labelFld = tls.ifNull(tls.firstValueOf(frm, ["DisplayColumn" ]), to.labelFld);
                to.sortFld = tls.ifNull(tls.firstValueOf(frm, ["DisplayColumn.SortColumn"]), to.sortFld);
                return to;
            })
            .Build();
    }

    
    //----------------------------------------------------------------------------------------------------------------------
    // прикладные

    // Get IPresentMetadata
    public getPresentable(location: string, field: string | null = null): Promise<IPresentMetadata>  {
        return this.getAsyncMetadatas(location, field).then(md => this.toPresentProvider().convert(md, (field == null ?location:field) ));
    }
    public getPresentableObs(location: string, field: string | null = null): Observable<IPresentMetadata> {
        return this.getMetadatas(location, field)
            .map(md => this.toPresentProvider().convert(md, (field == null ? location : field)));
    }

    // Get IFieldMetadata
    public getFieldMetadata(location: string, field: string | null = null): Promise<IFieldMetadata> {
        return this.getAsyncMetadatas(location, field).then(md => this.toFieldMetadataProvider().convert(md, (field == null ? location : field)));
    }
    public getFieldMetadataObs(location: string, field: string | null = null): Observable<IFieldMetadata> {
        return this.getMetadatas(location, field)
            .map(md => this.toFieldMetadataProvider().convert(md, (field == null ? location : field)));
    }

    
    // Get field list as string[]
    public getFields(location: string): Promise<string[]> {
        return this.getAsyncMetadatas(location).then(md => this.toFieldsList().convert(md, null));
    }
    public getFieldsObs(location: string): Observable<string[]> {
        return this.getMetadatas(location).map(md => this.toFieldsList().convert(md, null));
    }
    //-----

    // Презентаторы полей
    public getFieldsPresentMetadata(location: string) {
         return this.getFields(location).then(flds => {
             var promises: Promise<IPresentMetadata>[] = [];
             for (var i in flds) {
                 promises.push(this.getPresentable(location, flds[i]));
             }
             return Promise.all(promises);
         });
    }

    // тоже через обсервыблы
    // 160318 TODO check
    public getFieldsPresentMetadataObs(location: string) {
        return this.getFieldsObs(location)
            .map(flds => flds.map(f => this.getPresentableObs(location, f).mergeAll()));
    }
    

    // IFieldMetadata полей
    public getFieldsMetadata(location: string) {
        return this.getFields(location).then(flds => {
            var promises: Promise<IFieldMetadata>[] = [];
            for (var i in flds) {
                promises.push(this.getFieldMetadata(location, flds[i]));
            }
            return Promise.all(promises);
        });
    }

    // тоже через обсервыблы - 
    // 160318 TODO check
    public getFieldsMetadataObs(location: string) {
        return this.getFieldsObs(location)
            .mergeMap( flds => Observable.from(
                    flds.map(f => this.getFieldMetadataObs(location, f))
                ).mergeAll().toArray()
            )
    }

   

    //---------------------------------------------
    //160318 Далее наверно промисов уже не будет...

    // ILookUpMetadata
    public getLookUpMetadata(location: string) {
        return this.getMetadatas(location)
            .map(md => this.toLookUpMetadataProvider().convert(md, null));
    }

    // Лукапы, референсные пока. вощем два варика либо возвращаем их в оригинальных структурах и на компоненте разбираем
    // либо что выглядит более перспективно ( в части лукапов по крайней мере ) приводим их к единой структуре

    // Получает лукап по Лукашину в виде ILookUpData (К ЕДИНОЙ СТРУКТУРЕ)
    public getLookupData(location: string) {
        // 1 берем данные по Лукашину
        // 2 берем метаданные по лукап функциям Лукашина
        // 3 трансформируем в интерфейс лукапа

        return this.getLookUpMetadata(location)
            .mergeMap(lmds =>
                this.getDataAsList(location)
                    .map(dts => dts.map(item => {
                        var ret: ILookUpData = {
                            id: item[this.fieldNameBung(lmds.id)],
                            value: item[this.fieldNameBung((lmds.labelFld == null || lmds.labelFld == undefined || lmds.labelFld == "" )? lmds.id : lmds.labelFld )]
                        }
                        //console.log(ret);
                        return ret;
                    }))
            )
    }

    // 170418 Чета я не понимаю, при выборке данных посредством entyty  
    // с регистром лэйблов происходят чудеса
    // нужна затычка до выяснения обстоятельств такого поведения. :( 
    // Преобразует оригинальное (сикульно-ентитивое) имя поля в формат приходящий с сервиса
    public fieldNameBung(origName: string) {
        var recFun = (s: string, r: string, canToLow: boolean) => {
            if (s.length > 0) {
                var c = s[0];
                var lw = (c.toUpperCase() == c);
                r += (lw && canToLow) ? c.toLowerCase() : c;
                r = recFun(s.substring(1), r, lw )
            }
            return r;
        }
        return recFun(origName, "", true )
    }
}
