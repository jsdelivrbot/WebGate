
export interface ITransformProvider<TSource, TTarget, TFrom> {
    getTransFunc(initData: TFrom | null): ((src: TSource) => TTarget);
    convert(source: TSource, initData: TFrom | null): TTarget;
}