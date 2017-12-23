import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NegAlert,NegAuth } from '@newkit/core';
import { Questionnaire } from '../../components/Model/Questionnaire';
import { Topic } from '../../components/Model/Topic'
import { Option } from '../../components/Model/Option'
import { Answer } from '../../components/Model/Answer'
import { QueryService } from '../../services';

@Component({
	selector: 'Answers',
	templateUrl: 'Answers.component.html',
	styles: [require('./Answers.component.css').toString()],
})

export class AnswersComponent implements OnInit {

	formControl: Questionnaire;
	strQuestionnaireID:string;

	constructor(
		private _negAuth: NegAuth,
		private _negAlert : NegAlert,
		private _activatedRoute : ActivatedRoute,
		private _route : Router,
		private _queryService : QueryService,
	){
		this.formControl = new Questionnaire(this._negAuth.userId,this._negAuth.user.FullName);
	}

	ngOnInit() {
		//获取查询参数
		this.formControl.shortName = this._activatedRoute.snapshot.queryParams['shortname'];
		//获取路由参数
		this.formControl.questionnaireID = this._activatedRoute.snapshot.params['questionnaireid'];

		this.getTheQuestionnaire(this.formControl.questionnaireID);
	}

	public gobackDetails(){
		this._route.navigate(['/intern-eggrolls/details', this.formControl.questionnaireID]);
	}

	public initEditView(tempData){

		document.getElementById("back").style.backgroundImage = "url(" + tempData.BackgroundImageUrl + ")";
		document.getElementById("back").style.backgroundSize = "cover";
		document.getElementById("divDescription2").innerHTML = tempData.Description;

		//this.formControl.shortName = tempData.ShortName.toString();
		this.formControl.fullName = tempData.FullName.toString();
		this.formControl.status = tempData.Status;
		this.formControl.title = tempData.Title.toString();
		this.formControl.description = tempData.Description.toString();
		this.formControl.backgroundImageUrl = tempData.BackgroundImageUrl.toString();
		this.formControl.isRealName = tempData.IsRealName;
		this.formControl.dueDate = new Date(tempData.DueDate);
		for (let i = 0; i < tempData.Topics.length; i++){
			let tempTopic = new Topic(tempData.Topics[i].TopicID,tempData.Topics[i].Type,tempData.Topics[i].IsRequired,tempData.Topics[i].limited,undefined,tempData.Topics[i].TopicTitle);
			tempTopic.answer = "";
			if (tempData.Topics[i].Type == "Text"){
				tempTopic.answer = tempData.Topics[i].Answers[0].Ans;
			}
			for (let j = 0; j < tempData.Topics[i].Options.length; j++){
				let tempOption = new Option(tempData.Topics[i].Options[j].OptionID,tempData.Topics[i].Options[j].OptionTitle);
				for (let k=0; k< tempData.Topics[i].Answers.length;k++){
					if (tempData.Topics[i].Answers[k].Ans == tempData.Topics[i].Options[j].OptionID){
						tempOption.isAnswer = true;
					}
					//let tempAnswer = new Answer(tempData.Topics[i].Answers[k].ans);
					//tempTopic.answers.push(tempAnswer);
				}
				tempTopic.options.push(tempOption);
			}
			this.formControl.topics.push(tempTopic);
		}
		let divDescription = document.getElementById('divDescription2');
		divDescription.innerHTML = this.formControl.description;

	}
	
	public getTheQuestionnaire(tempID:string){
		let getPara = tempID;
		let getHeader = {useCustomErrorHandler: true};
		this._queryService.getAnswersByShortName(this.formControl.shortName, getPara,getHeader).then(({data}) => {
			if (data.Succeeded){
				this.initEditView(data.AnswerSheet);
			}else{
				this._negAlert.error(("获取答卷信息失败"));
			}
		},
		error => this._negAlert.error("获取答卷信息失败"));
	}
}