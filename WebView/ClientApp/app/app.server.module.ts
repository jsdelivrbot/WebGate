
import { NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';
import { AppModuleShared } from './app.shared.module';
import { AppComponent } from './components/app/app.component';

//import 'rxjs/add/Observable/take'
//import 'rxjs/operator/take'


@NgModule({
    bootstrap: [AppComponent],
    imports: [
        ServerModule,
        AppModuleShared
    ]
})

export class AppModule {
}
