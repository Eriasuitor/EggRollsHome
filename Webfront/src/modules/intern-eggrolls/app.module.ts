import { NgModule } from '@angular/core';
import { NkCoreModule } from '@newkit/core';

import { routing } from './app.routing';
import { ALL_PAGES } from './pages';
import { ALL_COMPONENTS } from './components';
import { ALL_PIPES } from './pipes';
import { ALL_SERVICES } from './services';

@NgModule({
  imports: [
    NkCoreModule,
    routing
  ],
  declarations: [
    ...ALL_COMPONENTS,
    ...ALL_PAGES,
    ...ALL_PIPES
  ],
  providers: [
    ...ALL_SERVICES
  ],
})
export class AppModule {

}
