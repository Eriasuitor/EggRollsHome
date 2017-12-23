import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { NegAlert, NegStorage, NegAuth } from '@newkit/core';

import { QueryService, UpdateService } from '../../services';
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
		private _queryService: QueryService,
		private _activatedRoute: ActivatedRoute,
		private _negAuth: NegAuth,
	) {

	}

	ngOnInit() {
		//获取查询参数
		//this.questionnaireTitle = this._activatedRoute.snapshot.queryParams['title'];

		//获取路由参数
		this.questionnaireID = this._activatedRoute.snapshot.params['questionnaireid'];

		this.getTheQuestionnaireDetails(this.questionnaireID);
	}

	//获取问卷详情
	public getTheQuestionnaireDetails(tempID: string) {
		let getPara = tempID;
		let getHeader = { useCustomErrorHandler: true };
		this._queryService.getQuestionnaireDetails(getPara, getHeader).then(({ data }) => {
			this.initDetailsView(data);
		},
			error => {
				if (error.status == 404) {
					this._negAlert.warn("该问卷还没有答题者！");
				} else {
					this._negAlert.error("请求错误！");
				}
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
}