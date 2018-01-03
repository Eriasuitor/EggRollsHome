import {Injectable} from '@angular/core'
import { NegAjax } from '@newkit/core'
import {Questionnaire} from '../../components/Model/Questionnaire'

@Injectable()
export class UpdateService{
    ///API请求地址'http://wcmis366/questionnaires/{questionnaireid}'
    private _apiUrl = 'http://10.16.75.24:3000/egg-rolls/v1/questionnaire';
    //private _apiPara = '';

    constructor(private _negAjax : NegAjax){

    }

    putQuestionnaire(putObj:Questionnaire,putHeader) : Promise<any>{
        let strQuestionnaire: string = JSON.stringify(putObj);
        console.log(putObj)
        return this._negAjax.put(this._apiUrl, strQuestionnaire, putHeader);
    }
}