
// Base presentation level metadata
export interface IPresentMetadata {
    name: string
    description: string
}

// Item (db-table-field) Metadata
export interface IFieldMetadata extends IPresentMetadata {
    id: string
    foreignKey: string
    type: string
    visible: boolean
    required: boolean
    defaultValue: any
    length?: number
}

// Lookup reference list metadata
export interface ILookUpMetadata {
    id: string
    labelFld: string
    sortFld:string
}

// Lookup data
export interface ILookUpData {
    id: any
    value: string
}


//Universal metadata type
export interface IMetadatas{
     [propertyName: string]: any; 
}

//Universal data type
export interface IDatas {
    [name: string]: any;
}


//------------
//export interface IMdSetter {
//    
//}


//Tools set. For injection to lambda convert function
export interface IMetadataTools {
    //value of key
    valueOf(source: IMetadatas, key: string): any;
    // first not empty value of keys
    firstValueOf(source: IMetadatas, keys: string[]): any;

    //value of key with transform
    valueOfFunc(source: IMetadatas, keyAcc: [string, (src: any) => any]): any;
    //first not empty value of keys with transform
    firstValueOfFunc(source: IMetadatas, keysAcc: [string, (src: any) => any][]): any;

    ifNull(val: any, valDef: any): any;
    ifNullOrUndef(val: any, valDef: any): any;
}




