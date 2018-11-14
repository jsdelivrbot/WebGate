import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';



import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
import { CounterComponent } from './components/counter/counter.component';

import { TestmikeComponent } from './components/testmike/testmike.component';
import { SdIncomingComponent } from './components/sd-incoming/sd-incoming.component';
import { SdIncomingsComponent } from './components/sd-incomings/sd-incomings.component';

import { JnObserverComponent } from './components/jn-galon/jn-observer/jn-observer.component';
import { JnGridComponent } from './components/jn-galon/jn-grid/jn-grid.component';
import { JnGridRowComponent } from './components/jn-galon/jn-grid-row/jn-grid-row.component';
import { JnItemComponent } from './components/jn-galon/jn-item/jn-item.component';
import { JnItemFieldComponent } from './components/jn-galon/jn-item-field/jn-item-field.component';
import { jnFldNameReqPipe } from './components/jn-galon/jn-fld-req.pipe';

//import { MatButtonModule } from '@angular/material';


@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        CounterComponent,
        FetchDataComponent,
        HomeComponent,
        TestmikeComponent,
        SdIncomingComponent,
        SdIncomingsComponent,
        JnObserverComponent,
        JnGridComponent,
        JnGridRowComponent,
        JnItemComponent,
        JnItemFieldComponent,
        jnFldNameReqPipe


    ],
    //exports: [MatButtonModule],
    imports: [
        //MatButtonModule,
        CommonModule,
        HttpModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'ang-sandbox',  component: TestmikeComponent },
            { path: 'sd-incoming',  component: SdIncomingComponent },
            { path: 'sd-incomings', component: SdIncomingsComponent },
            { path: 'counter',      component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'john-galon', component: JnObserverComponent , data: { SvcSubSource:'NvaSd/Incoming' } },
            { path: '**', redirectTo: 'home' }
        ])
    ]
})

export class AppModuleShared {
}
