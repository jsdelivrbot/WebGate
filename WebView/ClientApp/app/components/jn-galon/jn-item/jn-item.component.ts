import { Component, Input } from '@angular/core';  //, OnInit
import { FormGroup, FormArray, FormControl } from '@angular/forms';
import { DataProvService } from "../../../../service/data-prov/data-prov.service";
import { FormHelperService } from '../../../../service/form-helper/form-helper.service';
import { LoggerService } from "../../../../service/logger.service";
import { IPresentMetadata, IFieldMetadata } from "../../../../service/data-prov/data-prov.interface";
import { Observable } from 'rxjs/Observable';
//import { Component, Input } from '@angular/material';  //, OnInit

//import 'rxjs/operator/take'


const MODULE_NAME = 'John Galon';
const COMPONENT_NAME = 'Item';

@Component({
    selector:   'jn-item',
    templateUrl:'./jn-item.component.html',
    styleUrls: ['./jn-item.component.css'], 
    providers: [DataProvService, FormHelperService, LoggerService],
})



//implements OnInit
export class JnItemComponent  {
    
    @Input() svcSubUrl: string = "";                // Лукашин основоного ресурса
    @Input() key: any = null;                       // Item key, if empty - new item;   (Новый или редактирование  1804 тока новый  )

    isNewItem: boolean = (this.key === null);           

    //180418 Большой ребилд 
    caption$: Observable<string> | null = null;                      // на obs           
    fieldsPresentors$: Observable<IFieldMetadata[]> | null = null;   // пресентор полей 

    formGroup: FormGroup | null = null;                              // основной набор контролов  

    // вот эти поля архитектурно лишние но как без них я пока не знаю...
    refDataObs: { [location: string]: Observable<any>; } = {};       // Словарь - кэш наборов данных форегинов как обсеров  v2 (todo для динамиков)
    refControls: string[] = [];                                      // Зависимые контролы... бля 

    // invokers: Observable<>


    
    //formGui: Observable<FormGroup> | null = null;
    //isLoaded: boolean = false;
       
    /// это старое классическое
    fieldsPres: Promise<IFieldMetadata[]> | null = null;        // сет метаданных полей формы
    form: FormGroup | null = null;                              // основной набор контролов

    refData: { [location: string]: Promise<any>; } = {};        // Словарь - кэш наборов данных форегинов как промисов   
    
       
    constructor(
        private mdHelper: DataProvService,
        private frmHelper: FormHelperService,
        private logger: LoggerService
    ) {
    }


    // тригер получения вход параметра
    private ngOnChanges(changes: any) {
        this.log("onChange SubURL:" + this.svcSubUrl);
        this.reset();
    }

    private reset() {
        if (this.svcSubUrl != "") {
            this.caption$ = this.mdHelper.getPresentableObs(this.svcSubUrl).map(
                r => (this.isNewItem ? "Новое " : "Редактирование ") + "'" + r.name + "'"
            );

            this.formInit();
        }
    }

    //Инициализация параметры полученны
    private formInit() {

        this.fieldsPresentors$ = this.mdHelper.getFieldsMetadataObs(this.svcSubUrl);

        // FormGroup почему то не удалось на темплете развернуть из обертки поэтому являем его
        this.fieldsPresentors$.subscribe(
            x => {
                this.formGroup = new FormGroup(this.frmHelper.toFormControls(x));
                //this.valueChangesSubscibe(this.formGroup.controls["ServiceDescID"].valueChanges);
                //console.log(x);
            }
        )

        //this.formGui = this.fieldsPresentors
        //    .map(x => new FormGroup(this.frmHelper.toFormControls(x)));

        //this.formGui.subscribe(
        //    x => { this.isLoaded = true; }
        //)

       //this.fieldsPres = this.mdHelper.getFieldsMetadata(this.svcSubUrl)
       //    .then(x => {
       //        this.form = new FormGroup(
       //            this.frmHelper.toFormControls(x)
       //        );
       //        //this.valueChangesSubscibe(this.form.valueChanges);
       //        return x;                  
       //    })
    }


    // Оформление подписки на изменения значений контролов
    // 1804 для изменений по зависимым лукапам (форегинам)
    private valueChangesSubscibe(controlChangeValues: Observable<any>) {
        controlChangeValues.subscribe(x => console.log(x));
    }
    
    getForeignValues(foreginKeyLocator: string) {
        // тута просто легкий кеш (без него по цыкле не ходит)
        // кешруем со значениями параметров .... или перекеширываем при их изенении - так делать пока не будем
        // console.log(this.mdHelper.getLocationMacros(r));
        var exLocator = this.buildStateRefLocation(foreginKeyLocator);

        var ret = this.refDataObs[exLocator];
        if (!ret) {
            console.log(exLocator);
            ret = this.mdHelper.getLookupData(exLocator);
            this.refDataObs[exLocator] = ret;
            this.log("Loaded ref data from :'" + exLocator + "'.")
            console.log(ret);
        }
        return ret;
    }
        
    // Формирует локатор со значениями контролов ( фильтра включены ! )
    private buildStateRefLocation(foreginKeyLocator: string) {
        // Выбираем макросы (пока просто имена полей)
        return this.mdHelper.buildLocationWithMacrosValues(
            foreginKeyLocator,
            this.getControlValuesByNames(
                this.mdHelper.getLocationMacros(foreginKeyLocator)
            )
        );
    }
    
    // значения контролов по именам (values)
    private getControlValuesByNames(names: string[]) {
        var ret: any[] = [];
        if (this.formGroup != null) {
            var f = this.formGroup; 
            return names.map(x => (f.controls[x].value == null) ? 1 : f.controls[x].value);
        }
        return [];
    }

    
    //----------------------------------------------------------------------
    //Call from template function

    // Концепт динамик фильтра на форегин:
    // 170318 сначала просто фильтр 
    tptRegisterForeginControls(foreginKeyLocator: string, ctrlName: string) {
        // тут бы все ничего но нужно оформить подписку (первичную)
        // на изменение значений root-bhvr контролов 
        // то есть если foreginKeyLocator содержит макросы связанные со значениями активных контролов, 
        // то я должен подписаться на изменения значений этих контролов и переделывать ctrlName-контрол,
        // при этом пока концепт такой - если это лукап и он пустой то его хайдим если не пустой то переделываем и шовим
        // классик релиз был бы такой (цитата):
                    // Нужен словарь для отслеживания изменений контролов 
                    // Самый простой вар прямой  [изменяемый]: { зависимый 1,  зависимый 2 ... }
                    // Но лучше такой ( [зависимый 1]: {изменяемый 1, изменяемый 2 } )
        // React : по идее я должен иметь Обсервабл изменений от него пучковать нужные мне и завязывать на  них рефреш...

        

        if (this.formGroup != null && this.refControls.find(x => x == ctrlName) == undefined) {
            var fg = this.formGroup;
            this.refControls.push(ctrlName);
            this.mdHelper.getLocationMacros(foreginKeyLocator)
                .map(cname => {
                    var cnt = fg.controls[cname];
                    if (cnt !== null) {
                        this.valueChangesSubscibe(cnt.valueChanges);
                    }
                });
        }
        //else { this.error(" formGroup obj not initialize. It is bad...") }


        return this.getForeignValues(foreginKeyLocator);
    }

    tptOnSubmit() {
        console.log(this.form);
        if (this.form != null) {
            this.mdHelper.insertNewItem(this.svcSubUrl, this.form.value);
        }
    }

    //------------------------------------------------------------
    //Standart helper funcs
    private log(msg: string) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
    private error(msg: string) {
        this.logger.log(msg, MODULE_NAME, COMPONENT_NAME);
    }
}


