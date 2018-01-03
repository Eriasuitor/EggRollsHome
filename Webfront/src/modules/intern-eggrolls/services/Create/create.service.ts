import {Injectable} from '@angular/core'
import { NegAjax } from '@newkit/core'
import {Questionnaire} from '../../components/Model/Questionnaire'
import { Email } from '../../components/Model/Email'
@Injectable()
export class CreateService{
    //API请求地址'http://scmisbiztalk01:8100/egg-rolls/v1/questionnaires'
    private _apiUrl = 'http://10.16.75.24:3000/egg-rolls/v1/questionnaire';
    private _apiEmail = 'http://10.16.75.24:3000/egg-rolls/v1/email';
    constructor(private _negAjax : NegAjax){

    }

    postQuestionnaire(postObj:Questionnaire,postHeader) : Promise<any>{
        let strQuestionnaire: string = JSON.stringify(postObj);
        console.log(postObj)
        console.log(strQuestionnaire)
        return this._negAjax.post(this._apiUrl, postObj, postHeader);
    }

    postEmail(postObj:Email, postHeader) : Promise<any>{
        let strEmail : string = JSON.stringify(postObj);
       console.log("******createEmail*******");
        console.log(strEmail);
        console.log("*****************");
        return this._negAjax.post(this._apiEmail, strEmail, postHeader);
    }
}