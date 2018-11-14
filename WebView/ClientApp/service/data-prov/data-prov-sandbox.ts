

// Entity item (db-table-field) presentation
//export interface IEntyty {
//    name: string
//    type: string
//}


//Metadata provider
//export interface IMdProvider<TTarget>{
//    metadata?: TTarget
//}

//Metadata transform provider
//export interface ITransProvider<TSource,TTarget> {
//    getTransFunc() :( (src:TSource) => TTarget )  
//}

//Metadata transform provider for IMetadatas 
//export interface IMdTransProvider<TTarget> extends ITransProvider<IMetadatas, TTarget> {
//}



//    /*
//    ** Расширенные Функции трансформаторов
//    */

//    public getToIPresentMetadataExFunc() {
//        return (frm: IMetadatas, to: IPresentMetadata, tls: IMetadataTools) => {
//            to.name = tls.firstValueOf(frm, ["DisplayName", "Display.Name"]);
//            to.description = tls.firstValueOf(frm, ["Description", "Display.Description"]);
//            return to;
//        }
//    }

//    /*
//    ** Типизированные Metadata провайдеры
//    */
//    // провайдер  IMetadatas ->  IPresentMetadata
//    private getPresentProvider(mData: IMetadatas)  {
//        return this.mdProvider.getMetadataAdapterBuilder<IPresentMetadata>()
//            .setSource(mData)
//            .setFactoryFunc(() => {
//                var md: IPresentMetadata = { name: "", description: "" };
//                return md;
//            })
//            //.setFillFuncEx((frm, to, tls) => {
//            //    to.name = tls.firstValueOf(frm, ["DisplayName", "Display.Name"]);
//            //    to.description = tls.firstValueOf(frm, ["Description", "Display.Description"]);
//            //    return to;
//            //})
//            .setFillFuncEx(this.getToIPresentMetadataExFunc())
//            .build();
//     } 


//    //Return field list        
//    public getFields(location: string): Promise<string[]> {
//        return this.getAsyncMetadatas(location)
//            .then(mData =>
//                this.mdProvider.getMetadataAdapterBuilder<string[]>()
//                    .setSource(mData)
//                    .setFactoryFunc(() => [])
//                    .setFillFuncEx((frm, to, tls) => {
//                        for (var key in frm) {
//                            if (key[0] == '[') { to.push(key.substring(1, key.length - 1)); }
//                        }
//                        return to;
//                    })
//                    .build()
//                    .convert() 
//            );
//    }

//    // 30.03.18 
//    // Концепт следующий:
//    // Зачастую надо получить выборку элементов по полям неплохо бы сделать сие в общем виде
//    // типа:   [ target data container type ]  
//    // для этого должна быть функция: (field1 metadata -> target data container )   
//    // и мультипликатор по полям


//    // Принимает локейшн и функцию IMetadatas ->  TTarget
//    // и возвращает Promise< Array<TTarget> >
//    // Асинхронно выполняет функцию func для каждого поля пакует в массиив и промис 
//    public getFieldsMetadata<TTarget>(location: string, func: cvtMetadataFunc<TTarget>) {
//        return this.getFields(location).then(
//            flds => Promise.all(
//                flds.map(
//                    fldName => this.getAsyncMetadatas(location, fldName).then(md => func(md))
//                )
//            )
//        );
//    } 

//    // Презентаторы полей
//    public getFieldsPresentMetadata(location: string) {
//        return this.getFieldsMetadata(location, this.getToIPresentMetadataExFunc() )
//    }
//}

////  метадата конверт 
//interface cvtMetadataFunc<TTarget> {
//    (source: IMetadatas): TTarget;
//}



//////////////////////////////////////////////////////////////////////////////////////////////////////

// В реале же должен быть билдер собирающий функцию на основании сопоставления метаданных и свойств интерфейса
// Как это сделать я даже не представляю....
// В базовой реализации я должен связать свойства интерфейса с стринг ключами метаданных
// Попробуем:
// 28.03.18 Концепт вроде рабочий но говеный и медленный и магичный
// Надо по другому заходить сверху а не с низу релиз см. ниже:  
// вот это все ниже мертво - надо перенести 

//interface metadataMap {
//    ( key: string | string[] ) : any
//}

//interface metadataMap2 {
//    (propertyName: string ): any
//}


//// Коллекция правил 
//export class metadataMapCollection{
//    rules: { [propertyName: string]: string[]; } = {};

//    addRule(propertyName: string, metadataKey: string | string[]) {
//        var r = this.rules[propertyName];
//        var vl = (typeof metadataKey === "string") ? [metadataKey] : metadataKey;
//        this.rules[propertyName] = (this.rules[propertyName]) ? this.rules[propertyName].concat(vl) : vl;
//    }
//}

//// Билдер коллекци правил
//export class metadataMapCollectionBuilder {
//    collection: metadataMapCollection = new metadataMapCollection();
//    addRule(propertyName: string, metadataKey: string | string[]) {
//        this.collection.addRule(propertyName, metadataKey);
//        return this;
//    } 
//    build() {
//        return this.collection;
//    }
//}

//// Блин, таперяча нада взяти соурс в виде IMetadataSource, metadataMapCollection, получить таргет тип и захреначить 
//export class BaseMetadataMapConvertor<TTarget > implements IMdProvider<TTarget> {
//    source: IMetadatas | null = null;
//    mapCollection: metadataMapCollection | null = null;
//    metadata?: TTarget = this.convert()

//    private convert(): TTarget {
//        return <TTarget>this.convertToPrototype();
//    }

//    private convertToPrototype(): IMetadatas {
//        var ret: IMetadatas = {};
//        if (this.source && this.mapCollection && this.mapCollection.rules) {
//            for (var rkey in this.mapCollection.rules) {
//                var attrs: string[] = this.mapCollection.rules[rkey];
//                for (var skey in attrs) {
//                    var vl = this.source[skey];
//                    if (vl) {
//                        ret[rkey] = vl;
//                        break;
//                    } 
//                }   
//            }
//        }
//        return ret;
//    } 
//}

//export class MetadataProviderBuilder<TTarget> {
//    convertor: BaseMetadataMapConvertor<TTarget> = new BaseMetadataMapConvertor<TTarget>();
//    mapCollBuilder: metadataMapCollectionBuilder = new metadataMapCollectionBuilder();

//    addRule(propertyName: string, metadataKey: string | string[]) {
//        this.mapCollBuilder.addRule(propertyName, metadataKey);
//        return this;
//    }

//    setSource(src: IMetadatas) {
//        this.convertor.source = src;
//        return this;
//    }

//    build(): IMdProvider<TTarget> {

//        this.convertor()

//    }
//}

//// Класс для инекции 
//export class MetadataProviderFactory {
//    getBuilder<TTarget, TFrom>() {
//        return new MetadataProviderBuilder<TTarget, TFrom>();
//    }

//}

//// fill data from source function interface  with injecting tools-object
//interface filFuncEx<TSource, TTarget> {
//    (source: TSource, target: TTarget, tools:IMetadataTools ): TTarget;
//}

////-------------------------------------------------------------------------------------------------------------
//// 28.03.18  подход 2 (сверху) с передачей в лямбду функций акцессоров метаданных
//// Простейший билдер метадата адаптера
//// Источник - функция
//export class BaseDataProvMdAdapterBuilder<TSource, TTarget> {

//    adapter: BaseDataProvMdAdapter<TSource, TTarget> = new BaseDataProvMdAdapter<TSource, TTarget>(null, null);
//    setSource(src: TSource) {
//        this.adapter.source = src;
//        return this;
//    }
//    setFunc(func: cvtFunc<TSource, TTarget>) {
//        this.adapter.mainFunc = func;
//        return this;
//    }
//    build() {
//        return this.adapter;
//    }
//}


//// Base convert object implimentation
//export class BaseDataProvMdAdapter<TSource, TTarget> implements IMdProvider<TTarget>, ITransProvider<TSource, TTarget> {

//    public source: TSource | null = null;
//    public mainFunc: cvtFunc<TSource, TTarget> | null = null;

//    metadata?: TTarget = this.convert();

//    constructor(src: TSource | null, fnc: cvtFunc<TSource, TTarget> | null) {
//        this.source = src;
//        this.mainFunc = fnc;        
//    }

//    getTransFunc(){
//        if (this.mainFunc == null) {
//            throw Error("Convertation function is empty.");
//        }
//        return this.mainFunc;
//    }

//    convert(): TTarget {
//        var ret: any = undefined;


//        if (this.mainFunc && this.source) {
//            ret = this.mainFunc(this.source);
//        }
//        return ret;
//    } 
//}



////В основе BaseDataProvMdAdapter лежит функция конвертации из IMetadatas в таргет тип
//export class BaseMetadatasConvertFuncBuilder<TSource, TTarget> {

//    public crFunc: crtFunc<TTarget> | null = null;
//    public flFunc: filFunc<TSource, TTarget> | null = null;


//    constructor(createF: crtFunc<TTarget>  | null = null , fillF: filFunc<TSource, TTarget>| null = null ) {
//        this.crFunc = createF;
//        this.flFunc = fillF;
//    }

//    setFactoryFunc(cF: crtFunc<TTarget>) {
//        this.crFunc = cF;
//        return this;
//    }

//    setFillFunc(fF: filFunc<TSource, TTarget>) {
//        this.flFunc = fF;
//        return this;
//    }

//    build() {
//        if (!this.crFunc || !this.flFunc) {
//            throw new Error('Not impliment acessor functions...');
//        }
//        var cF = <crtFunc<TTarget>>this.crFunc;
//        var fF = <filFunc<TSource, TTarget>>this.flFunc;
//        var ret: cvtFunc<TSource, TTarget> = (x) => fF(x, cF());
//        return ret;
//    }
//}



//// Implement builder & adapter by IMetadataSource sourse
//export class DataProvMdAdapter<TTarget> extends BaseDataProvMdAdapter<IMetadatas, TTarget>{ }
//export class DataProvMdAdapterBuilder<TTarget> extends BaseDataProvMdAdapterBuilder<IMetadatas, TTarget>{ }

//// Билдер с тулзами для  IMetadatas конверт функции
//export class MetadatasConvertFuncBuilder<TTarget> extends BaseMetadatasConvertFuncBuilder<IMetadatas, TTarget>{

//    public tools: IMetadataTools = new BaseMetadataTools();
//    //public funcEx: filFuncEx<IMetadatas, TTarget> | null = null;

//    setTools(tls: IMetadataTools) {
//        this.tools = tls;
//        return this;
//    }

//    setFillFuncEx(fF: filFuncEx<IMetadatas, TTarget>) {
//        this.setFillFunc((s, t) => fF(s, t, this.tools));
//    }

//    //setFillFuncEx(fF: filFuncEx<IMetadatas, TTarget>) {
//    //    this.funcEx = fF;
//    //    return this;
//    //}

//    //build() {

//    //    // if (this.funcEx && this.crFunc) {
//    //    // Вота тута нада передать на родительскую setFillFunc
//    //    //var cF = <crtFunc<TTarget>>this.crFunc;
//    //    //var fF = <filFuncEx<IMetadatas, TTarget>>this.funcEx;
//    //    //var ret: cvtFunc<IMetadatas, TTarget> = (x) => fF(x, cF(), this.tools);
//    //    //return ret;
//    //    var s = (s, t) => this.funcEx(s, t, this.tools)

//    //    if (this.funcEx != null) {
//    //        this.setFillFunc((s, t) => this.funcEx(s, t, this.tools))
//    //    }
//    //    else { this.setFillFunc((s, t) => this.funcEx(s, t, this.tools)) }
//    //    return super.build();
//    //}
//}


////
//// Для билдера нужен инструментарий : 
//// 1. фабрика Target-типа
//// 2. Функция инициации Target-обекта из сорса
//// 3. Туулзы (передаватся в лямбду ?)


//// Попробуем тулзы
//export class BaseMetadataTools implements IMetadataTools {

//    valueOf(source: IMetadatas, key: string): any {
//        return source[key];
//    } 

//    firstValueOf(source: IMetadatas, keys: string[]): any {
//        var ret: any = null;

//        for (var s in keys) {
//            var v = this.valueOf(source, keys[s]);
//            if (v) {
//                ret = v;
//                break;
//            }
//        }
//        return ret;
//    } 
//}


//// Частная реализация 
//// Вот это первая рабочаяя лошадка должна быть.
//export class MdAdapterBuilder<TTarget> extends BaseDataProvMdAdapterBuilder<IMetadatas, TTarget>{

//    funcBuilder = new MetadatasConvertFuncBuilder<TTarget>();

//    setFactoryFunc(cF: crtFunc<TTarget>) {
//        this.funcBuilder.setFactoryFunc(cF);
//        return this;
//    }

//    setFillFunc(fF: filFunc<IMetadatas, TTarget>) {
//        this.funcBuilder.setFillFunc(fF);
//        return this;
//    }

//    setFillFuncEx(fF: filFuncEx<IMetadatas, TTarget>) {
//        this.funcBuilder.setFillFuncEx(fF);
//        return this;
//    }

//    build() {
//        if (!this.adapter.mainFunc) {
//            this.setFunc(this.funcBuilder.build());
//        }
//        return this.adapter;
//    }
//}


///----------------------------------------------------------------------------------------------------------
//// Internal transfer interfaces
////----------------------------------------------------------------------------------------------------------
//// convert function interface 
//interface cvtFunc<TSource, TTarget> {
//    (source: TSource): TTarget;
//}

//interface cvtExFunc<TSource, TTarget, ITools> {
//    (source: TSource, tools : ITools): TTarget;
//}

//// factory function interface 
//interface crtFunc<TTarget> {
//    (): TTarget;
//}

//// factory function from interface 
//interface crtExFunc<TFrom, TTarget> {
//    (fromData :TFrom): TTarget;
//}


//// fill data from source function interface 
//interface filFunc<TSource,TTarget> {
//    (source: TSource, target: TTarget): TTarget;
//}

//interface filExFunc<TSource, TTarget, ITools> {
//    (source: TSource, target: TTarget, tools: ITools): TTarget;
//}


//// 30.03.2018 Блять Третий ребилд ----------------------------------------------------------------------------------------------------------
//// Необходимо полностью уйти от контекста данных
//// Нужна фабрика предоставляющая 
//// чистую функцию 
//// подход 3 (сверху) без сорса
//export class BaseConvertFuncFactory<TSource, TTarget, TFrom, ITools  > {

//    private createFunc: crtFunc<TTarget> | null = null;
//    private createFuncEx: crtExFunc<TFrom, TTarget> | null = null;

//    private mainFunc: cvtFunc<TSource, TTarget> | null = null;
//    private mainExFunc: cvtExFunc<TSource, TTarget, ITools> | null = null;
//    private fillFunc: filFunc<TSource, TTarget> | null = null;
//    private fillExFunc: filExFunc<TSource, TTarget, ITools> | null = null;


//    private tools: ITools | null = null;

//    public setCreateFunc(fnc: crtFunc<TTarget>) {
//        this.createFunc = fnc;
//    }

//    public setCreateFuncEx(fnc: crtExFunc<TFrom, TTarget>) {
//        this.createFuncEx = fnc;
//    }

//    public setConvertFunc(fnc: cvtFunc<TSource, TTarget>) {
//        this.mainFunc = fnc;
//    }
//    public setConvertExFunc(fnc: cvtExFunc<TSource, TTarget, ITools>) {
//        this.mainExFunc = fnc;
//    }
//    public setFillFunc(fnc: filFunc<TSource, TTarget>) {
//        this.fillFunc = fnc;
//    }
//    public setFillExFunc(fnc: filExFunc<TSource, TTarget, ITools>) {
//        this.fillExFunc = fnc;
//    }
//    public setTools(tls:ITools) {
//        this.tools = tls;
//    }


//    public getMainFunc(initData: TFrom | null = null): cvtFunc<TSource, TTarget>  {
//        var ret = undefined;
//        if (this.mainFunc !== null) {
//            ret = this.mainFunc;
//        }
//        else if (this.mainExFunc !== null && this.tools !== null) {
//            var a = this.mainExFunc;
//            var t = this.tools;
//            ret = (x: TSource) => (a(x, t));
//        }
//        else if (initData != null && this.createFuncEx != null && this.fillFunc != null) {
//            var cfe = this.createFuncEx;
//            var ff = this.fillFunc;
//            ret = (x: TSource) => (ff(x, cfe(initData)));
//        }
//        else if (initData != null && this.createFuncEx != null && this.fillExFunc != null && this.tools !== null) {
//            var cfe = this.createFuncEx;
//            var fe = this.fillExFunc;
//            var t = this.tools;
//            ret = (x: TSource) => fe(x, cfe(initData), t);
//        }

//        else if ( this.createFunc != null && this.fillFunc != null) {
//            var cf = this.createFunc;
//            var ff = this.fillFunc;
//            ret = (x: TSource) => (ff(x, cf()));
//        }
//        else if ( this.createFunc != null && this.fillExFunc != null && this.tools !== null) {
//            var cf = this.createFunc;
//            var fe = this.fillExFunc;
//            var t = this.tools;
//            ret = (x: TSource) => fe(x, cf(), t);
//        }


//        else if (this.createFunc != null )
//        {
//            ret = (x: TSource) => cf();
//        }

//        else {
//            throw Error("Function not defined...")
//        }
//        return ret;
//    } 
// }

//// Трансформ провайдер
//export class TransformProvider<TSource, TTarget, TFrom, ITools> implements ITransformProvider<TSource, TTarget, TFrom>  {

//    private funcFactory = new BaseConvertFuncFactory<TSource, TTarget, TFrom,  ITools>();

//    constructor(fFactory: BaseConvertFuncFactory<TSource, TTarget, TFrom, ITools> | null = null) {
//        if (fFactory != null) {
//            this.funcFactory = fFactory;
//        }
//    }

//    getTransFunc(initData: TFrom | null): (src: TSource) => TTarget {
//        return this.funcFactory.getMainFunc(initData);
//    }

//    convert(source: TSource, initData: TFrom | null ): TTarget {
//        return this.getTransFunc(initData)(source);
//    }

//}

//// Трансформ провайдер Билдер
//export class TransformProviderBuilder<TSource, TTarget,  TFrom,  ITools>  {

//    private funcFactory = new BaseConvertFuncFactory<TSource, TTarget, TFrom, ITools>();

//    public setCreateFunc(fnc: crtFunc<TTarget>) {
//        this.funcFactory.setCreateFunc(fnc);
//        return this;
//    }

//    public setCreateFuncEx(fnc: crtExFunc<TFrom, TTarget>) {
//        this.funcFactory.setCreateFuncEx( fnc) ;
//        return this;
//    }

//    public setConvertFunc(fnc: cvtFunc<TSource, TTarget>) {
//        this.funcFactory.setConvertFunc(fnc);
//        return this;
//    }
//    public setConvertExFunc(fnc: cvtExFunc<TSource, TTarget, ITools>) {
//        this.funcFactory.setConvertExFunc(fnc);
//        return this;
//    }
//    public setFillFunc(fnc: filFunc<TSource, TTarget>) {
//        this.funcFactory.setFillFunc(fnc);
//        return this;
//    }
//    public setFillExFunc(fnc: filExFunc<TSource, TTarget, ITools>) {
//        this.funcFactory.setFillExFunc(fnc);
//        return this;
//    }
//    public setTools(tls: ITools) {
//        this.funcFactory.setTools(tls);
//        return this;
//    }

//    public Build(): ITransformProvider<TSource, TTarget, TFrom> {
//        return new TransformProvider<TSource, TTarget, TFrom , ITools>(this.funcFactory);
//    }
//}

//// Класс для инекции 
//export class TransformProviderFactory{
//    getBuilder<TSource, TTarget, TFrom, ITools>(){
//        return new TransformProviderBuilder<TSource, TTarget, TFrom, ITools>();
//    }
//}

