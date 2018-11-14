import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppModuleShared } from './app.shared.module';
import { AppComponent } from './components/app/app.component';

import { DataAdapterService } from "../service/data-prov/data-prov-md-adapter.service";
import { DataProvService } from "../service/data-prov/data-prov.service";
import { LoggerService } from "../service/logger.service";


@NgModule({
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        AppModuleShared
    ],
    providers: [
        DataProvService,
        DataAdapterService,
        LoggerService,
        { provide: 'BASE_URL', useFactory: getBaseUrl },//getBaseUrl
        { provide: 'BASE_SVC_URL', useFactory: apiUrlFactory },
        { provide: 'BASE_SVC_MD_SFX', useFactory: apiUrlMetadataSfxFactory }
    ]
})
export class AppModule {
}

export function getBaseUrl() {
    return document.getElementsByTagName('base')[0].href;
}

export function apiUrlFactory() {
    return (window as any).svc_Config.apiSvcUrl;
}

export function apiUrlMetadataSfxFactory() {
    return (window as any).svc_Config.apiSvcMdSfx;
}