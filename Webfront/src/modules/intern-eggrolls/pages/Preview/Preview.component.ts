import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { NegAlert, NegStorage, NegAuth } from '@newkit/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Questionnaire } from '../../components/Model/Questionnaire';

import { Topic } from '../../components/Model/Topic';
import { Option } from '../../components/Model/Option';
import { QueryService } from '../../services';


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


	setBackgroundImage: string;
	setBackgroundSize: string;
	public selectionTimer: any = {};

	constructor(
		private ref: ChangeDetectorRef,
		private _negAlert: NegAlert,
		private _negStorage: NegStorage,
		private _activatedRoute: ActivatedRoute,
		private _queryService: QueryService,
		private _negAuth: NegAuth,
		private _route: Router,
	) {
		//初始化调查问卷Model
		this.formControl = new Questionnaire(this._negAuth.userId, this._negAuth.user.FullName);
	}
	ngOnInit() {
		//判断是否从编辑或新建页面跳转到预览界面，如果不是，则从主页传递参数到预览界面，需要查询
		if (this._negStorage.memory.get("PreQuestionnaire") == undefined) {
			//获取路由参数
			this.formControl.questionnaireID = this._activatedRoute.snapshot.params['questionnaireid'];
			if (this.formControl.questionnaireID != undefined) {
				this.strOperating = "index";
				this.getTheQuestionnaire(this.formControl.questionnaireID);
			} else {
				this.strOperating = "index";
				this._negAlert.error("预览界面初始化失败！！");
				this._route.navigate(['/intern-eggrolls/404']);
			}

			//判断是否从编辑或新建页面跳转到预览界面，如果是，则从memory中获取
		} else {
			let strQuestionnaire = JSON.parse(this._negStorage.memory.get("PreQuestionnaire")[0]);
			this.strOperating = this._negStorage.memory.get("PreQuestionnaire")[1];
			if (strQuestionnaire.QuestionnaireID != undefined) {
				this.formControl.questionnaireID = strQuestionnaire.QuestionnaireID;
			} else {
				this.queryParaID = this._negStorage.memory.get("PreQuestionnaire")[2];
			}
			this.getQuestionnaireFromSession(strQuestionnaire);
		}
	}

	//从Memory中读取数据，绑定到页面Model
	public getQuestionnaireFromSession(strQuestionnaire) {

		document.getElementById("back").style.backgroundImage = "url(" + strQuestionnaire.BackgroundImageUrl + ")";
		document.getElementById("back").style.backgroundSize = "cover";
		document.getElementById("divDescription2").innerHTML = strQuestionnaire.Description;

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
		let divDescription = document.getElementById('1');
		// divDescription.innerHTML = this.formControl.description;

		let backgroundSet = document.getElementsByClassName('ntk-Preview');

		this.setBackgroundImage = "url(" + this.formControl.backgroundImageUrl + ")";
		this.setBackgroundSize = "cover";

		this._negStorage.memory.remove("PreQuestionnaire");
		this._negStorage.memory.remove("EditQuestionnaire");
		this._negStorage.memory.remove("CreateQuestionnaire");
	}

	//get请求调查问卷：查询调查问卷的service方法调用
	public getTheQuestionnaire(tempID: string) {
		let getPara = tempID;
		let getHeader = { useCustomErrorHandler: true };
		this._queryService.getQuestionnaire(getPara, getHeader).then(({ data }) => {
			if (data.Questionnaire != null) {
				this.getQuestionnaireFromSession(data.Questionnaire);
			} else {
				this._negAlert.error("编辑查询失败！！！");
			}

		},
			error => this._negAlert.error("编辑界面初始化失败！"));
	}

	//返回按钮监听事件
	public gobackCreateOrEdit() {
		switch (this.strOperating) {
			case "edit":
				this._negStorage.memory.set("EditQuestionnaire", JSON.stringify(this.formControl));
				this._route.navigate(['/intern-eggrolls/' + this.strOperating, this.formControl.questionnaireID]);
				break;
			case "create":
				this._negStorage.memory.set("CreateQuestionnaire", JSON.stringify(this.formControl));
				this._route.navigate(['/intern-eggrolls/' + this.strOperating]);
				break;
			case "copy":
				this._negStorage.memory.set("EditQuestionnaire", JSON.stringify(this.formControl));
				this._route.navigate(['/intern-eggrolls/edit'], { queryParams: { 'id': this.queryParaID } });
				break;
			case "index":
				this._route.navigate(['/intern-eggrolls/index']);
				break;
			default:
				this._negAlert.error("返回失败！！");
				break;
		}
	}



	public mouseIn(topicID, optionID) {
		if (document.getElementById('d' + topicID + optionID).style.backgroundColor == "") {
			document.getElementById('d' + topicID + optionID).style.backgroundColor = 'rgba(0,0,0,0)'
		}
		clearInterval(this.selectionTimer[topicID + optionID]);
		let temTimer = setInterval(function () {
			var bgClr = document.getElementById('d' + topicID + optionID).style.backgroundColor;
			var opacity = parseFloat(bgClr.substring(bgClr.lastIndexOf(',') + 1, bgClr.lastIndexOf(')'))) * 100;
			if (opacity < 80) {
				opacity += 2;
				document.getElementById('d' + topicID + optionID).style.backgroundColor = 'rgba(255,255,255,' + opacity / 100 + ' )';
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
			var bgClr = document.getElementById('d' + topicID + optionID).style.backgroundColor;
			var opacity = parseFloat(bgClr.substring(bgClr.lastIndexOf(',') + 1, bgClr.lastIndexOf(')'))) * 100;
			if (opacity != 0) {
				opacity -= 2;
				document.getElementById('d' + topicID + optionID).style.backgroundColor = 'rgba(255,255,255,' + opacity / 100 + ' )';
			}
			else {
				clearInterval(temTimer);
			}
		}, 5);
		this.selectionTimer[topicID + optionID] = temTimer;
	}
}