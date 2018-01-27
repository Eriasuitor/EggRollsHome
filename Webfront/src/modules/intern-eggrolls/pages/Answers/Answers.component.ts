import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NegAlert,NegAuth,NegTranslate,NegStorage, NegMultiTab } from '@newkit/core';
import { Questionnaire } from '../../components/Model/Questionnaire';
import { Topic } from '../../components/Model/Topic'
import { Option } from '../../components/Model/Option'
import { Answer } from '../../components/Model/Answer'

import {AnswerSheetService} from '../../services/AnswerSheetService'
import {QuestionnaireService} from '../../services/QuestionnaireService'

@Component({
	selector: 'Answers',
	templateUrl: 'Answers.component.html',
	styles: [require('./Answers.component.css').toString()],
})

export class AnswersComponent implements OnInit {

	public formControl: Questionnaire;
	public strQuestionnaireID:string;
	public questionnaire:Questionnaire = new Questionnaire()
	public answers = []
	public self:AnswersComponent

	constructor(
		private _negAuth: NegAuth,
		private _negAlert : NegAlert,
		private _activatedRoute : ActivatedRoute,
		private _route : Router,
		private _negTranslate:NegTranslate,
		private _negStorage:NegStorage,
		private answerSheetService:AnswerSheetService,
		private questionnaireService:QuestionnaireService,
		private negMultiTab:NegMultiTab
	){}

	ngOnInit() {
		self = this
		this.questionnaire = new Questionnaire()
		this.formControl = new Questionnaire(this._negAuth.userId,this._negAuth.user.FullName);
		this.formControl.shortName = this._negStorage.memory.get('detaiSetSID')
		this.formControl.questionnaireID = this._negStorage.memory.get('detaiSetQID')
		this._negStorage.memory.remove('detaiSetQID')
		this._negStorage.memory.remove('detaiSetSID')
		if(this.formControl.shortName == undefined || this.formControl.questionnaireID == undefined){
			// this._route.navigate(['/eggrolls/404'])
			this.negMultiTab.openPage('/eggrolls/404', null, false)
		}
		else{
			this.getTheQuestionnaire(this.formControl.questionnaireID);
		}

		this._negTranslate.set('answer', {
			'en-us': {
				questionnaire: {
					realName: 'Real name',
					anonymous:'Anonymous',
					deadline: 'Deadline',
					link:'Link'
				},
				topic: {
					multiChoice: 'Multiple choice',
					limit: 'Limit',
					limitTip1: ', have to choose ',
					limitTip2: ' options',
					requiredTip: ' *'		
				},
				answer: {
					back:'Back',
					noAnswer:'Get answer sheet failed, please ensure that the answer sheet exists or contact E.T. for help'
				}
			},
			'zh-cn': {
				questionnaire: {
					realName: '实名',
					anonymous:'匿名',
					deadline: '截止时间',
					link:'链接'
				},
				topic: {
					multiChoice: '多选',
					limit: '限选 ',
					limitTip1: '， 必选 ',
					limitTip2: ' 个',
					requiredTip: ' *'
				},
				answer: {
					back:'返回',
					noAnswer:'获取答卷时出错，请确保该答卷存在或联系E.T.寻求帮助'
				}
			},
			'zh-tw': {
				questionnaire: {
					realName: '实名',
					anonymous:'匿名',
					deadline: '截止时间：',
					link:'链接'
				},
				topic: {
					limit: 'Limit',
					required: 'Required',
					limitTip1: ', have to choose ',
					limitTip2: ' options',
					requiredTip: ' *'
				},
				answer: {
					back:'Back',
					noAnswer:'Get answer sheet failed, please ensure that the answer sheet exists or contact E.T. for help'
				}
			}
		});
	}

	public gobackDetails(){
		this._negStorage.memory.set('dID', this.formControl.QuestionnaireID);
		if (document.getElementsByClassName("nk-main-content")[0] != undefined) {
			document.getElementsByClassName("nk-main-content")[0].style.backgroundImage = null;
		}
		// this._route.navigate(['/eggrolls/details']);
		this.negMultiTab.openPage('/eggrolls/details', null, false)		
	}

	public attachAnswer(){
		if(self.questionnaire == undefined || self.questionnaire.QuestionnaireID == undefined || self.questionnaire.QuestionnaireID == 0){
			setTimeout(self.attachAnswer, 100)
		}
		else{
			self.questionnaire.Topics.forEach(topic => {
				if(topic.Type == "Text"){
					for(var i = 0; i < self.answers.length; i++){
						if(self.answers[i].TopicID == topic.TopicID){
							topic.Answer = self.answers[i].Ans
						}
					}
				}
				else{
					topic.Options.forEach(option => {
						for(var i = 0; i < self.answers.length; i++){
							if(self.answers[i].TopicID == topic.TopicID && self.answers[i].Ans == option.OptionID){
								option.isAnswer = true
								break
							}
						}
					});
				}
			});
		}
	}
	
	public getTheQuestionnaire(tempID:number){
		let getPara = tempID;
		let getHeader = {useCustomErrorHandler: true};
		this.questionnaireService.OnGet(getPara).then(({data})=>{
			if (data.Succeeded){
				this.questionnaire = data.Questionnaire
				if (document.getElementsByClassName("nk-main-content")[0] == undefined) {
					document.getElementById("backImg").style.backgroundImage = "url(" + this.questionnaire.BackgroundImageUrl + ")";
					document.getElementById("backImg").style.backgroundSize = "cover";
				}
				else {
					document.getElementsByClassName("nk-main-content")[0].style.backgroundImage = "url(" + this.questionnaire.BackgroundImageUrl + ")";
					document.getElementsByClassName("nk-main-content")[0].style.backgroundSize = "cover";
				}
				document.getElementById("divDescription2").innerHTML = this.questionnaire.Description;
			}else{
				this._negAlert.error(this._negTranslate.get('answer.answer.noAnswer'))
			}
		},
		error=>{
			this._negAlert.error(this._negTranslate.get('answer.answer.noAnswer'))
		 })
		this.answerSheetService.OnGet(getPara, this.formControl.shortName).then(({data}) => {
			if (data.Succeeded){
				this.answers = data.AnswerList
				this.attachAnswer()
			}else{
				this._negAlert.error(this._negTranslate.get('answer.answer.noAnswer'))
			}
		},
		error => {
			this._negAlert.error(this._negTranslate.get('answer.answer.noAnswer'))
		});
	}

	public getStanderdDate(dateGet){
		return dateGet.substr(0,6) == '/Date(' ? this.dmft(this.date(dateGet)) : dateGet;
	}

	public dmft(d) { 
		return d.getFullYear() + '-' + this.pad(d.getMonth() + 1) + '-' + this.pad(d.getDate()) + ' ' + this.pad(d.getHours()) + ':' +  this.pad(d.getMinutes()); 
	}

	public date(s) { 
		return new Date(parseFloat(/Date\(([^)]+)\)/.exec(s)[1])); 
	}
	
	public pad(d) { 
		return d < 10 ? '0'+d : d; 
	}
}