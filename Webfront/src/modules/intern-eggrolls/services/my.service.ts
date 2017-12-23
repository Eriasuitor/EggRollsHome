import { Injectable } from '@angular/core';
import { NegAjax } from '@newkit/core';

@Injectable()
export class MyService {

    // private api_url = 'http://wcmis366';
    private api_url = 'http://10.16.75.24:3000/egg-rolls/v1';
    // private api_url = 'http://10.16.85.94:3000';

    private reqOptions = {
        hideLoading: true, // 默认false
        useCustomErrorHandler: true // 默认false
    };


    constructor(
        private negAjax: NegAjax
    ) { }

    getPagedQuestionnaires(searchObj): Promise<any> {
        let _api_url = this.api_url + '/questionnaire/search?';
        searchObj.Title = searchObj.Title.replace(/\%/g, "%25");
        // searchObj.Title = searchObj.Title.replace(/\ /g,"%20");
        searchObj.Title = searchObj.Title.replace(/\#/g, "%23");
        searchObj.Title = searchObj.Title.replace(/\//g, "%2F");
        searchObj.Title = searchObj.Title.replace(/\?/g, "%3F");
        searchObj.Title = searchObj.Title.replace(/\&/g, "%26");
        searchObj.Title = searchObj.Title.replace(/\=/g, "%3D");
        searchObj.Title = searchObj.Title.replace(/\+/g, "%2B");
        _api_url += "shortname=" + searchObj.ShortName + "&title=" + searchObj.Title + "&pageindex=" + searchObj.PageIndex + "&pagesize=" + searchObj.PageSize;
        return this.negAjax.get(_api_url, this.reqOptions);
    }
    getStatistics(QuestionnaireID): Promise<any> {
        let _api_url = this.api_url + '/answer-sheet/statistics?questionnaireID=' + QuestionnaireID;
        return this.negAjax.get(_api_url, this.reqOptions);
    }
    getDetail(questionnaireID, topicID): Promise<any> {
        let _api_url = this.api_url + '/answer-sheet/answer/participator?questionnaireID=' + questionnaireID + "&topicID=" + topicID;
        return this.negAjax.get(_api_url);
    }
    getPersonnelList(questionnaireID, topicID, optionID): Promise<any> {
        let _api_url = this.api_url + '/answer-sheet/answer/participator?questionnaireID=' + questionnaireID + "&topicID=" + topicID + "&optionID=" + optionID;
        return this.negAjax.get(_api_url);
    }
    getQuestionnaire(questionnaireID): Promise<any> {
        let _api_url = this.api_url + '/questionnaire?questionnaireID=' + questionnaireID;
        return this.negAjax.get(_api_url, this.reqOptions);
    }
    doDelete(questionnaireID): Promise<any> {
        let _api_url = this.api_url + '/questionnaire?questionnaireID=' + questionnaireID;
        return this.negAjax.delete(_api_url, this.reqOptions);
    }
    postAnswer(postJson): Promise<any> {
        let _api_url = this.api_url + '/answer-sheet';
        return this.negAjax.post(_api_url, postJson, this.reqOptions);
    }
    getAnswer(questionnaireID, ShortName) {
        let _api_url = this.api_url + '/answer-sheet?questionnaireID=' + questionnaireID + "&shortname=" + ShortName;
        return this.negAjax.get(_api_url, this.reqOptions);
    }
}