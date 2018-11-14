import { ITransformProvider } from "./data-caster.interface";


//----------------------------------------------------------------------------------------------------------
// Internal transfer interfaces
//----------------------------------------------------------------------------------------------------------
// convert function interface 
interface cvtFunc<TSource, TTarget> {
    (source: TSource): TTarget;
}

interface cvtExFunc<TSource, TTarget, ITools> {
    (source: TSource, tools: ITools): TTarget;
}

// factory function interface 
interface crtFunc<TTarget> {
    (): TTarget;
}

// factory function from interface 
interface crtExFunc<TFrom, TTarget> {
    (fromData: TFrom): TTarget;
}


// fill data from source function interface 
interface filFunc<TSource, TTarget> {
    (source: TSource, target: TTarget): TTarget;
}

interface filExFunc<TSource, TTarget, ITools> {
    (source: TSource, target: TTarget, tools: ITools): TTarget;
}



// 30.03.2018 Блять Третий ребилд ----------------------------------------------------------------------------------------------------------
// Необходимо полностью уйти от контекста данных
// Нужна фабрика предоставляющая 
// чистую функцию 
// подход 3 (сверху) без сорса
export class BaseConvertFuncFactory<TSource, TTarget, TFrom, ITools> {

    private createFunc: crtFunc<TTarget> | null = null;
    private createFuncEx: crtExFunc<TFrom, TTarget> | null = null;

    private mainFunc: cvtFunc<TSource, TTarget> | null = null;
    private mainExFunc: cvtExFunc<TSource, TTarget, ITools> | null = null;
    private fillFunc: filFunc<TSource, TTarget> | null = null;
    private fillExFunc: filExFunc<TSource, TTarget, ITools> | null = null;


    private tools: ITools | null = null;

    public setCreateFunc(fnc: crtFunc<TTarget>) {
        this.createFunc = fnc;
    }

    public setCreateFuncEx(fnc: crtExFunc<TFrom, TTarget>) {
        this.createFuncEx = fnc;
    }

    public setConvertFunc(fnc: cvtFunc<TSource, TTarget>) {
        this.mainFunc = fnc;
    }
    public setConvertExFunc(fnc: cvtExFunc<TSource, TTarget, ITools>) {
        this.mainExFunc = fnc;
    }
    public setFillFunc(fnc: filFunc<TSource, TTarget>) {
        this.fillFunc = fnc;
    }
    public setFillExFunc(fnc: filExFunc<TSource, TTarget, ITools>) {
        this.fillExFunc = fnc;
    }
    public setTools(tls: ITools) {
        this.tools = tls;
    }


    public getMainFunc(initData: TFrom | null = null): cvtFunc<TSource, TTarget> {
        var ret = undefined;

        if (this.mainFunc !== null) {
            ret = this.mainFunc;
            return ret;
        }
        else if (this.mainExFunc !== null && this.tools !== null) {
            var a = this.mainExFunc;
            var t = this.tools;
            ret = (x: TSource) => (a(x, t));
            return ret;
        }
        else if (initData != null && this.createFuncEx != null && this.fillFunc != null) {
            var cfe = this.createFuncEx;
            var ff = this.fillFunc;
            ret = (x: TSource) => (ff(x, cfe(initData)));
            return ret;
        }
        else if (initData != null && this.createFuncEx != null && this.fillExFunc != null && this.tools !== null) {
            var cfe = this.createFuncEx;
            var fe = this.fillExFunc;
            var t = this.tools;
            ret = (x: TSource) => fe(x, cfe(initData), t);
            return ret;
        }

        else if (this.createFunc != null && this.fillFunc != null) {
            var cf = this.createFunc;
            var ff = this.fillFunc;
            ret = (x: TSource) => (ff(x, cf()));
            return ret;
        }

        else if (this.createFunc != null && this.fillExFunc != null && this.tools !== null) {
            var cf = this.createFunc;
            var fe = this.fillExFunc;
            var t = this.tools;
            ret = (x: TSource) => fe(x, cf(), t);
            return ret;
        }

        else if (this.createFunc != null) {
            return this.createFunc;
        }

        else {
            throw Error("Function not defined...")
        }

        


        //else if (this.createFuncEx != null && initData !== null) {
        //    var cfe = this.createFuncEx;
        //    ret = (x: TSource) => cfe( initData );
        //}


    }
}

// Трансформ провайдер
export class TransformProvider<TSource, TTarget, TFrom, ITools> implements ITransformProvider<TSource, TTarget, TFrom>  {

    private funcFactory = new BaseConvertFuncFactory<TSource, TTarget, TFrom, ITools>();

    constructor(fFactory: BaseConvertFuncFactory<TSource, TTarget, TFrom, ITools> | null = null) {
        if (fFactory != null) {
            this.funcFactory = fFactory;
        }
    }

    getTransFunc(initData: TFrom | null): (src: TSource) => TTarget {
        return this.funcFactory.getMainFunc(initData);
    }

    convert(source: TSource, initData: TFrom | null): TTarget {
        return this.getTransFunc(initData)(source);
    }

}

// Трансформ провайдер Билдер
export class TransformProviderBuilder<TSource, TTarget, TFrom, ITools>  {

    private funcFactory = new BaseConvertFuncFactory<TSource, TTarget, TFrom, ITools>();

    public setCreateFunc(fnc: crtFunc<TTarget>) {
        this.funcFactory.setCreateFunc(fnc);
        return this;
    }

    public setCreateFuncEx(fnc: crtExFunc<TFrom, TTarget>) {
        this.funcFactory.setCreateFuncEx(fnc);
        return this;
    }

    public setConvertFunc(fnc: cvtFunc<TSource, TTarget>) {
        this.funcFactory.setConvertFunc(fnc);
        return this;
    }
    public setConvertExFunc(fnc: cvtExFunc<TSource, TTarget, ITools>) {
        this.funcFactory.setConvertExFunc(fnc);
        return this;
    }
    public setFillFunc(fnc: filFunc<TSource, TTarget>) {
        this.funcFactory.setFillFunc(fnc);
        return this;
    }
    public setFillExFunc(fnc: filExFunc<TSource, TTarget, ITools>) {
        this.funcFactory.setFillExFunc(fnc);
        return this;
    }
    public setTools(tls: ITools) {
        this.funcFactory.setTools(tls);
        return this;
    }

    public Build(): ITransformProvider<TSource, TTarget, TFrom> {
        return new TransformProvider<TSource, TTarget, TFrom, ITools>(this.funcFactory);
    }
}
