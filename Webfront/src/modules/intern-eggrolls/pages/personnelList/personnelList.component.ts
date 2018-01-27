import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { NegStorage, NegAlert, NegTranslate,NegMultiTab } from '@newkit/core';

import { AnswerParticipatorService } from '../../services/AnswerParticipatorService'

import './personnelList.component.css';

@Component({
	selector: 'personnelList',
	templateUrl: 'personnelList.component.html'
})

export class PersonnelListComponent implements OnInit {
	public personnelList: any = {};
	public personnelListPara: any = {};
	constructor(
		private router: Router,
		private _negStorage: NegStorage,
		private negAlert: NegAlert,
		private _negTranslate: NegTranslate,
		private answerParticipatorService: AnswerParticipatorService,
		private negMultiTab: NegMultiTab
	) { }

	ngOnInit() {
		this.negMultiTab.setCurrentTabName('Egg Rolls')
		this.personnelListPara = this._negStorage.memory.get("personnelListPara");
		this._negStorage.memory.remove("personnelListPara");
		if (this.personnelListPara != undefined) {
			let topicID = this.personnelListPara.TopicID;
			let optionID = this.personnelListPara.OptionID;
			let questionnaireID = this.personnelListPara.QuestionnaireID;
			if (this.personnelList.data == undefined) {
				this.answerParticipatorService.OnGet(questionnaireID, topicID, optionID)
					.then(({ data }) => {
						if (data.Succeeded == true) {
							this.personnelList = data;
							var description:string = this._negTranslate.get('personnelList.main.description')
							description = description.replace('_1',topicID )
							description = description.replace('_2',optionID )
							document.getElementById('description').innerHTML = description
						}
						else {
							this.negAlert.error(this._negTranslate.get('personnelList.main.getFailed'))
						}
					},
					error => this.negAlert.error(this._negTranslate.get('personnelList.main.getFailed')))
			}
		}
		else {
			// this.router.navigate(['eggrolls/404'])
			this.negMultiTab.openPage('/eggrolls/404', null, false)
		}

		this._negTranslate.set('personnelList', {
			'en-us': {
				main: {
					shortname: 'Shortname',
					fullname: 'Fullname',
					department: 'Department',
					back: 'Back',
					getFailed: 'Get questionnaire failed, please ensure that the questionnaire exists or contact E.T. for help',
					description:"List of people who chose option _2 of topic _1"
				}
			},
			'zh-cn': {
				main: {
					shortname: '短名',
					fullname: '完整名',
					department: '部门',
					back: '返回',
					getFailed: '获取问卷失败，请确保该问卷未被删除或联系E.T.寻求帮助',
					description:"选择了题目_1选项_2的人员名单"
				}
			},
			'zh-tw': {
				main: {
					shortname: 'Shortname',
					fullname: 'Fullname',
					department: 'Department',
					back: 'Back',
					getFailed: '获取问卷失败，请确保该问卷未被删除或联系E.T.寻求帮助',
					description:"List of people who chose option _2 of topic _1"
				}
			}
		}
		)
	}

	public getPersonnelList() {
		if (this.personnelList != undefined) {
			return this.personnelList.Answers;
		}
	}

	public back() {
		this._negStorage.memory.set('sID', this.personnelListPara.QuestionnaireID);
		// this.router.navigate(['/eggrolls/statistics']);
		this.negMultiTab.openPage('/eggrolls/statistics', null, false)
	}
}