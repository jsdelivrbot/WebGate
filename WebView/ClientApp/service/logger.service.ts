import { Injectable } from '@angular/core';

@Injectable()
export class LoggerService implements ILogger {

    public log(message: any, module: string = "", submodule: string = "") {

        console.log(this.wrapMsg(module, submodule) + " " + (  message == null?"null":message.toString() ) )
    }

    public error(message: any, module: string = "", submodule: string = "") {
        console.error(this.wrapMsg(module, submodule) + " " + (message == null ? "null" : message.toString()))
    }


    wrapMsg(module: string = "", submodule: string = "") {
        return '[' + module + ':' + submodule + '] ';
    }
}


export interface ILogger{

    log(message: any, module: string, submodule: string): void;
    error(message: any, module: string, submodule: string): void;

}