import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {
  CreateComponent,
  EditComponent,
  PreviewComponent,
  DetailsComponent,
  AnswersComponent,
  ListComponent,
  NavigationBarComponent,
  StatisticsComponent,
  PersonnelListComponent,
  AnswerComponent,
  NotFoundComponent
} from './pages';

// 配置路由
const appRoutes: Routes = [
  { path: 'index', component: ListComponent },

  { path: 'create', component: CreateComponent },
  { path: 'edit/:questionnaireid', component: EditComponent },
  { path: 'edit', component: EditComponent },
  { path: 'preview/:questionnaireid', component: PreviewComponent },
  { path: 'preview', component: PreviewComponent },
  { path: 'details/:questionnaireid', component: DetailsComponent },
  { path: 'answers/:questionnaireid', component: AnswersComponent },

  { path: 'statistics', component: StatisticsComponent },
  { path: 'personnelList', component: PersonnelListComponent },
  { path: 'questionnaire', component: AnswerComponent },
  { path: 'abc', component: NavigationBarComponent },

  { path: '', pathMatch: 'full', redirectTo: '/intern-eggrolls/index' },
  { path: '404', component: NotFoundComponent },
  { path: '**', component: NotFoundComponent }

];

export const routing: ModuleWithProviders = RouterModule.forChild(appRoutes);
