import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { NegAlert, NegStorage, NegAuth,NegTranslate, NegMultiTab } from '@newkit/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Questionnaire } from '../../components/Model/Questionnaire';

import { Topic } from '../../components/Model/Topic';
import { Option } from '../../components/Model/Option';

import {QuestionnaireService} from '../../services/QuestionnaireService'

import {Environment} from '../../config/Environment'

@Component({
	selector: 'Preview',
	templateUrl: 'Preview.component.html',
	styles: [require('./Preview.component.css').toString()],
})

export class PreviewComponent implements OnInit {

	//调查问卷Model的实体类，用于维护页面所有数据
	public formControl: Questionnaire;

	//记录跳转到预览界面的操作
	strOperating: string;

	//查询参数：问卷ID
	queryParaID: string;
	environmentUrl:string;

	public selectionTimer: any = {};
	public opacity:any={}
	self:PreviewComponent

	constructor(
		private ref: ChangeDetectorRef,
		private _negAlert: NegAlert,
		private _negStorage: NegStorage,
		private _activatedRoute: ActivatedRoute,
		private _negAuth: NegAuth,
		private _route: Router,
		private _negTranslate:NegTranslate,
		private questionnaireService:QuestionnaireService,
		private _environment:Environment,
		private negMultiTab: NegMultiTab
	) {
		this.formControl = new Questionnaire(this._negAuth.userId, this._negAuth.user.FullName)
		this.environmentUrl = this._environment.getEnvironmentUrl()
	}

	ngOnInit() {
		this.negMultiTab.setCurrentTabName('Egg Rolls')
		self = this
		if (this._negStorage.memory.get("pID") != undefined) {
			this.formControl.questionnaireID = this._negStorage.memory.get('pID')
			this._negStorage.memory.remove('pID')
			this.strOperating = "index";
			this.getTheQuestionnaire(this.formControl.questionnaireID);
		} 
		else if(this._negStorage.memory.get("PreQuestionnaire") != undefined){
			let strQuestionnaire = JSON.parse(this._negStorage.memory.get("PreQuestionnaire")[0]);
			this.strOperating = this._negStorage.memory.get("PreQuestionnaire")[1];
			console.log(this.strOperating)
			if (strQuestionnaire.QuestionnaireID != undefined) {
				this.formControl.questionnaireID = strQuestionnaire.QuestionnaireID;
			} else {
				this.queryParaID = this._negStorage.memory.get("PreQuestionnaire")[2];
			}
			this.formControl = strQuestionnaire
			if (document.getElementsByClassName("nk-main-content")[0] == undefined) {
				document.getElementById("backImg").style.backgroundImage = "url(" + this.formControl.BackgroundImageUrl + ")"
				document.getElementById("backImg").style.backgroundSize = "cover";
			}
			else {
				document.getElementsByClassName("nk-main-content")[0].style.backgroundImage = "url(" + this.formControl.BackgroundImageUrl + ")";
				document.getElementsByClassName("nk-main-content")[0].style.backgroundSize = "cover";
			}
			document.getElementById("divDescription2").innerHTML = this.formControl.Description;
		}
		else{
			// this._route.navigate(['/eggrolls/404'])
			this.negMultiTab.openPage('/eggrolls/404', null, false)
		}
		this._negTranslate.set('preview', {
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
					getFailed:'Get questionnaire failed, please ensure that the questionnaire exists or contact E.T. for help'
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
					getFailed:'获取问卷失败，请确保该问卷未被删除或联系E.T.寻求帮助'
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
					requiredTip: ' *',
				},
				answer: {
					back:'Back',
					getFailed:'获取问卷失败，请确保该问卷未被删除或联系E.T.寻求帮助'
				}
			}
		});
	}

	public getQuestionnaireFromSession(strQuestionnaire) {
		this.formControl.shortName = strQuestionnaire.ShortName.toString();
		this.formControl.status = strQuestionnaire.Status;
		this.formControl.title = strQuestionnaire.Title.toString();
		this.formControl.description = strQuestionnaire.Description.toString();
		this.formControl.backgroundImageUrl = strQuestionnaire.BackgroundImageUrl.toString();
		this.formControl.isRealName = strQuestionnaire.IsRealName;
		if (strQuestionnaire.DueDate != null) {
			this.formControl.dueDate = new Date(strQuestionnaire.DueDate);
		} else {
			this.formControl.dueDate = null;
		}

		for (let i = 0; i < strQuestionnaire.Topics.length; i++) {
			let tempTopic = new Topic(strQuestionnaire.Topics[i].TopicID, strQuestionnaire.Topics[i].Type, strQuestionnaire.Topics[i].IsRequired, strQuestionnaire.Topics[i].Limited, undefined, strQuestionnaire.Topics[i].TopicTitle);
			if (tempTopic.Type != "Text")
				for (let j = 0; j < strQuestionnaire.Topics[i].Options.length; j++) {
					let tempOption = new Option(strQuestionnaire.Topics[i].Options[j].OptionID, strQuestionnaire.Topics[i].Options[j].OptionTitle);
					tempTopic.options.push(tempOption);
				}
			this.formControl.topics.push(tempTopic);
		}

		this._negStorage.memory.remove("PreQuestionnaire");
		this._negStorage.memory.remove("EditQuestionnaire");
		this._negStorage.memory.remove("CreateQuestionnaire");
	}

	public getTheQuestionnaire(tempID: number) {
		let getPara = tempID;
		let getHeader = { useCustomErrorHandler: true };
		this.questionnaireService.OnGet(getPara).then(({ data }) => {
			if (data.Succeeded == true) {
				this.formControl = data.Questionnaire
				if (document.getElementsByClassName("nk-main-content")[0] == undefined) {
					document.getElementById("backImg").style.backgroundImage = "url(" + this.formControl.BackgroundImageUrl + ")";
					document.getElementById("backImg").style.backgroundSize = "cover";
				}
				else {
					document.getElementsByClassName("nk-main-content")[0].style.backgroundImage = "url(" + this.formControl.BackgroundImageUrl + ")";
					document.getElementsByClassName("nk-main-content")[0].style.backgroundSize = "cover";
				}
				document.getElementById("divDescription2").innerHTML = this.formControl.Description;
			} else {
				this._negAlert.error(this._negTranslate.get('preview.answer.getFailed'))
			}
		},
			error => {
				this._negAlert.error(this._negTranslate.get('preview.answer.getFailed'))
			});
	}

	public gobackCreateOrEdit() {
		if (document.getElementsByClassName("nk-main-content")[0] != undefined) {
			document.getElementsByClassName("nk-main-content")[0].style.backgroundImage = null;
		}
		switch (this.strOperating) {
			case "create":
				this._negStorage.memory.set("CreateQuestionnaire", JSON.stringify(this.formControl))
				// this._route.navigate(['/eggrolls/' + this.strOperating]);
				this.negMultiTab.openPage('/eggrolls/' + this.strOperating, null, false)
				break;
			case "index":
				// this._route.navigate(['/eggrolls/index']);
				this.negMultiTab.openPage('/eggrolls/index', null, false)
				break;
			default:
				this._negAlert.error("Failed");
				break;
		}
	}

	public mouseIn(topicID, optionID) {
		if(this.opacity[topicID + optionID] == undefined){
			this.opacity[topicID + optionID] = 0
		}
		clearInterval(this.selectionTimer[topicID + optionID]);
		let temTimer = setInterval(function () {
			if (self.opacity[topicID + optionID] < 73) {
				self.opacity[topicID + optionID] += 1;
				document.getElementById('d' + topicID + optionID).style.backgroundColor = 'rgba(255,255,255,' + self.opacity[topicID + optionID]/100 + ' )';
			}
			else {
				clearInterval(temTimer);
			}
		}, 0);
		this.selectionTimer[topicID + optionID] = temTimer;
	}

	public mouseOut(topicID, optionID) {
		clearInterval(this.selectionTimer[topicID + optionID]);
		let temTimer = setInterval(function () {
			if (self.opacity[topicID + optionID] > 0) {
				self.opacity[topicID + optionID] -= 1;			
				document.getElementById('d' + topicID + optionID).style.backgroundColor = 'rgba(255,255,255,' + self.opacity[topicID + optionID] / 100 + ' )';
			}
			else {
				clearInterval(temTimer);
			}
		}, 0);
		this.selectionTimer[topicID + optionID] = temTimer;
	}

	public getStanderdDate(dateGet){
		return dateGet.substr(0,6) == '/Date(' ? this.dmft(this.date(dateGet)) : this.dmft(new Date(dateGet));
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