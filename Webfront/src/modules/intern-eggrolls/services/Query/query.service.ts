import {Injectable} from '@angular/core'
import { NegAjax } from '@newkit/core'
import {Questionnaire} from '../../components/Model/Questionnaire'

@Injectable()
export class QueryService{
    //API请求地址'http://scmisbiztalk01:8100/egg-rolls/v1/questionnaires'
    private _apiUrl = 'http://10.16.75.24:3000/egg-rolls/v1/questionnaires';
    private _apiUrl2 = 'http://10.16.75.24:3000/egg-rolls/v1';
    private _apiAnswerUrl = "http://10.16.75.24:3000/egg-rolls/v1/answers";

    constructor(private _negAjax : NegAjax){

    }

    getQuestionnaire(getPara:string,getHeader) : Promise<any>{
        let apiPara = "/questionnaire?questionnaireID=" + getPara;
        let strApiUrl = this._apiUrl2 + apiPara;
        return this._negAjax.get(strApiUrl, getHeader);
    }

    getQuestionnaireDetails(getPara:string, getHeader) : Promise<any>{
        let apiPara = "/questionnaire/participator?questionnaireID=" + getPara;
        let strApiUrl = this._apiUrl2 + apiPara;
        return this._negAjax.get(strApiUrl, getHeader);
    }

    getAnswersByShortName(getPara:string, urlPara:string, getHeader) : Promise<any>{
        let apiPara = "/answer-sheet?questionnaireID=" + urlPara + "&shortname=" + getPara;
        let strApiUrl = this._apiUrl2 + apiPara;
        return this._negAjax.get(strApiUrl, getHeader);
    }
}