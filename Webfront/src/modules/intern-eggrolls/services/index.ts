import {CreateService} from './Create/create.service';
import {UpdateService} from './Update/update.service';
import {QueryService} from './Query/query.service'
import { MyService } from './my.service';
export {
    CreateService,
    UpdateService,
    QueryService,
    MyService
};
export const ALL_SERVICES = [
    CreateService,
    UpdateService,
    QueryService,
    MyService
];
