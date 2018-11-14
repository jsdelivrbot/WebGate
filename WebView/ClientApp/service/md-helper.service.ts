//    _  _  _ _  ____
//   / \/ |( / |/ /_ \
//  /     / /   _/ __/
//  \/\/\_\/\/\_\___/
//  
//  02.2018 Data controller prototype
//  Цель: напихать функционала, затем оптимизровать 
//  03.2018 - класс свое отжил - редуцировался в data-prov объеденяющий функции дата и метадата провайдера.
//////////////////////////////////
import { Injectable, Inject } from '@angular/core';
import { Http } from '@angular/http';


@Injectable()
export class MdHelperService {

    constructor(private http: Http, @Inject('BASE_SVC_URL') private baseSvcUrl: string, @Inject('BASE_SVC_MD_SFX') private baseSvcUrlMdSfx: string) {
        console.log('wwww');
    }
    
    // Return Promised data from related location as String
    public getPrmsData(location: string)
    {
        var uri = this.baseSvcUrl + location;
        var ht = this.http.get(uri);

        var ret = new Promise(function (resolve, reject) {
            var d: string = "";
            ht.subscribe(
                data => { d = data.text(); }
                , error => reject(error)
                , () => resolve(d));
        });
        //ret.then(r => console.log(r));
        return ret;
    }


    // Return IPresentable  (name descr)
    // location: string  - related location
    // field: string - property(field) name,  null - self object  
    // defNameFunc: - fieldname to name func
    // defDescrFunc: - fieldname to descr func
    public getPresentable(location: string, field: string = "",
        defNameFunc: ((fld: string) => string) = (f => f),
        defDescrFunc: ((fld: string) => string) = (f => ""),
        )
    {
        var NameKey =  (field == "") ? "DisplayName" : "Display.Name";
        var DescrKey = (field == "") ? "Description" : "Display.Description";

        const promises  = [
            this.getPrmsMetadataValue(NameKey, location, field),
            this.getPrmsMetadataValue(DescrKey, location, field)
        ];

        return Promise.all(promises)
            .then(x => {
                var ret: IPresentable = {
                    name: (x.length > 0 && x[0] != null && x[0] !="" )  ? x[0].toString() : defNameFunc(field) ,
                    description: (x.length > 1 && x[1] != null && x[1] != "")  ? x[1].toString() : defDescrFunc(field)
                };
                return ret;
            });
    }

    // Return string metadata 
    // mdKey: string - Metadata key 
    // location: string  - related location
    // field: string - property(field) name,  null - self object  
    public getMetadataValue(mdKey: string, location: string, field: string): string {
        var uri = this.baseSvcUrl + location + this.baseSvcUrlMdSfx + ((field) ? ("/" + field) : ("")) + "?Name=" + mdKey;
        var md = "";
        this.http.get(uri).subscribe(data => {
            md = data.text();
        }, error => console.error(error), () => md)
        return md;
    } 

    // Return string metadata as Promise 
    // mdKey: string - Metadata key 
    // location: string  - related location
    // field: string - property(field) name,  null - self object  
    public getPrmsMetadataValue(mdKey: string, location: string, field: string) {
        var uri = this.baseSvcUrl + location + this.baseSvcUrlMdSfx + ((field != "") ? ("/" + field) : ("")) + "?Name=" + mdKey;
        var ht = this.http.get(uri);

        var ret = new Promise(function (resolve, reject) {
            var d: string = "";
            ht.subscribe(
                data => { d = data.text(); }
                , error => reject(error)
                , () => resolve(d));
        });   
        return ret;
    } 

    // Return all metadata as Promise 
    // location: string  - related location
    // field: string - property(field) name,  null - self object 
    public getMetadatas(location: string, field: string = "") 
    {
        var uri = this.baseSvcUrl + location + this.baseSvcUrlMdSfx + ((field != "") ? ("/" + field) : (""));
        var ht = this.http.get(uri);

        var ret = new Promise(function (resolve, reject) {
            var d: any ;
            ht.subscribe(
                data => { d = data.text(); }
                , error => reject(error)
                , () => resolve(d));
        });
        return ret;
    }

     
    // Get array of fields name as Promise 
    public getFields(location: string) {
        var ret =  this.getMetadatas(location)
            .then(response => {
                var data = JSON.parse(response.toString());
                var r: string[] = [];
                for (var key in data){
                    if (key[0] == '[') {  r.push(key.substring(1, key.length - 1));}
                }
                return r;
            });
        //ret.then(r => console.log(r));
        return ret;
    }

    // Get fields as IPresentable (async)
    public getFieldsPresentable(location: string) {
        return this.getFields(location).then(flds => {
            var promises: Promise<IPresentable>[] = [];
            for (var i in flds) {
                //this.getPresentable(location, i).then(x => console.log(x.name + x.description));
                promises.push(this.getPresentable(location, flds[i]));             
            }
            return Promise.all(promises);
        })
    }

    // Return fields count (async)
    public getFieldsCount(location: string) {
        var ret = this.getFields(location).then( );
        return ret;
    }
}




export interface IPresentable{
    name: string 
    description: string 
}

export interface IFieldMetadata extends IPresentable{
    visible: boolean
    mondatory: boolean
    defaultValue: any
}    

export interface IEntyty {
    name: string
    type: string
}