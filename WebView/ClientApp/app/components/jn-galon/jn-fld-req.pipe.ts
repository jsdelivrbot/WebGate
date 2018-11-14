import { Pipe, PipeTransform }  from "@angular/core";
import { IFieldMetadata }       from "../../../service/data-prov/data-prov.interface";

@Pipe({
    name: 'jnFldNameReq'
})

export class jnFldNameReqPipe implements PipeTransform {
    transform(value: IFieldMetadata) {
        return value.name + (value.required ? " * " : "");
    }
}