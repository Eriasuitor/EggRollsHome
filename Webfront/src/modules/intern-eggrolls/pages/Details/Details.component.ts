import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NegAlert, NegStorage, NegAuth, NegTranslate, NegMultiTab } from '@newkit/core';

import { QuestionnaireParticipatorService } from '../../services/QuestionnaireParticipatorService'

import { AnswersDetails } from './AnswersDetails'

@Component({
	selector: 'ntk-Details',
	templateUrl: 'Details.component.html'
})

export class DetailsComponent implements OnInit {
	//回答详情Model数组
	answerDetails: AnswersDetails[] = [];
	//问卷ID
	questionnaireID: string = "";
	//问卷标题--弃用
	//questionnaireTitle:string = "";

	constructor(
		private _negAlert: NegAlert,
		private _activatedRoute: ActivatedRoute,
		private _negAuth: NegAuth,
		private _negTranslate: NegTranslate,
		private _negStorage: NegStorage,
		private _router: Router,
		private questionnaireParticipatorService: QuestionnaireParticipatorService,
		private negMultiTab: NegMultiTab
	) {

	}

	ngOnInit() {
		this.negMultiTab.setCurrentTabName('Egg Rolls')
		//获取查询参数
		//this.questionnaireTitle = this._activatedRoute.snapshot.queryParams['title'];

		//获取路由参数
		this.questionnaireID = this._negStorage.memory.get('dID');
		if (this.questionnaireID == undefined) {
			// this._router.navigate(['/eggrolls/404'])
			this.negMultiTab.openPage('/eggrolls/404', null, false)
		}
		else {
			this._negStorage.memory.remove('dID');
			this.getTheQuestionnaireDetails(this.questionnaireID);
		}

		this._negTranslate.set('details', {
			'en-us': {
				main: {
					description: 'Details of the person who answered this questionnaire',
					index: 'Index',
					shortname: 'Shortname',
					fullname: 'Fullname',
					department: 'Department',
					details: 'details',
					clickToView: 'Click to view',
					getFailed: 'Get questionnaire failed, please ensure that the questionnaire exists or contact E.T. for help',
					noAnswer:'No answer sheet was found'
				}
			},
			'zh-cn': {
				main: {
					description: '参与此问卷的所有人员的答卷',
					index: '序号',
					shortname: '短名',
					fullname: '完整名',
					department: '部门',
					details: '详情',
					clickToView: '点击查看答卷',
					getFailed: '获取问卷失败，请确保该问卷未被删除或联系E.T.寻求帮助',
					noAnswer:'未找到任何答卷'					
				}
			},
			'zh-tw': {
				main: {
					description: 'Details of the person who answered this questionnaire',
					index: 'Index',
					shortname: 'Shortname',
					fullname: 'Fullname',
					department: 'Department',
					details: 'details',
					clickToView: 'Click to view',
					getFailed: '获取问卷失败，请确保该问卷未被删除或联系E.T.寻求帮助',
					noAnswer:'No answer sheet was found'					
				}
			}
		}
		);
	}

	//获取问卷详情
	public getTheQuestionnaireDetails(tempID: string) {
		let getPara = tempID;
		let getHeader = { useCustomErrorHandler: true };
		this.questionnaireParticipatorService.OnGet(getPara).then(({ data }) => {
			if (data.Succeeded == true) {
				this.initDetailsView(data);
			}
			else {
				this._negAlert.error(this._negTranslate.get('details.main.getFailed'))
			}
		},
			error => {
				this._negAlert.error(this._negTranslate.get('details.main.getFailed'))
			})
	}

	//初始化页面
	public initDetailsView(data) {
		let answers = data['Participators'];
		for (let i = 0; i < answers.length; i++) {
			let answerItem = new AnswersDetails(answers[i].ShortName, answers[i].FullName, answers[i].Department);
			this.answerDetails.push(answerItem);
		}
	}
	public goTo(questionnaireID, shortName) {
		this._negStorage.memory.set('detaiSetQID', questionnaireID)
		this._negStorage.memory.set('detaiSetSID', shortName)
		// this._router.navigate(['/eggrolls/answers'])
		this.negMultiTab.openPage('/eggrolls/answers', null, false)
	}
}