// 待导入的组件
import { CreateComponent } from './Create/Create.component'
import { EditComponent } from './Edit/Edit.component'
import { PreviewComponent } from './Preview/Preview.component'
import { DetailsComponent } from './Details/Details.component'
import { AnswersComponent } from './Answers/Answers.component'

import { ListComponent } from './list/list.component'
import { NavigationBarComponent } from './navigationBar/navigationBar.component'
import { StatisticsComponent } from './statistics/statistics.component'
import {PersonnelListComponent} from './personnelList/personnelList.component'
import {AnswerComponent} from './answer/answer.component'
import {NotFoundComponent} from './notFound/notFound.component'

// 导出单个组件，方便路由使用
export {
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
};

// 导出所有页面，方便在module中一次性注入
export const ALL_PAGES = [
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
];
