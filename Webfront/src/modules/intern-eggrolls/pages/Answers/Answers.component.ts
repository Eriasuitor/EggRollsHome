import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { NegAlert,NegAuth } from '@newkit/core';
import { Questionnaire } from '../../components/Model/Questionnaire';
import { Topic } from '../../components/Model/Topic'
import { Option } from '../../components/Model/Option'
import { Answer } from '../../components/Model/Answer'
import { QueryService } from '../../services';
import { MyService } from '../../services';

@Component({
	selector: 'Answers',
	templateUrl: 'Answers.component.html',
	styles: [require('./Answers.component.css').toString()],
})

export class AnswersComponent implements OnInit {

	public formControl: Questionnaire;
	public strQuestionnaireID:string;
	public questionnaire:Questionnaire
	public answers = []

	constructor(
		private _negAuth: NegAuth,
		private _negAlert : NegAlert,
		private _activatedRoute : ActivatedRoute,
		private _route : Router,
		private _queryService : QueryService,
		private _service : MyService,
	){
		
	}

	ngOnInit() {
		this.questionnaire = new Questionnaire()
		this.formControl = new Questionnaire(this._negAuth.userId,this._negAuth.user.FullName);
		//获取查询参数
		this.formControl.shortName = this._activatedRoute.snapshot.queryParams['shortname'];
		//获取路由参数
		this.formControl.questionnaireID = this._activatedRoute.snapshot.params['questionnaireid'];

		this.getTheQuestionnaire(this.formControl.questionnaireID);
	}

	public gobackDetails(){
		this._route.navigate(['/intern-eggrolls/details', this.formControl.questionnaireID]);
	}

	public attachAnswer(){
		if(this.questionnaire.QuestionnaireID == undefined){
			setTimeout(this.attachAnswer, 100);
		}
		else{
			this.questionnaire.Topics.forEach(topic => {
				if(topic.Type == "Text"){
					for(var i = 0; i < this.answers.length; i++){
						if(this.answers[i].TopicID == topic.TopicID){
							topic.Answer = this.answers[i].Ans
						}
					}
				}
				else{
					topic.Options.forEach(option => {
						for(var i = 0; i < this.answers.length; i++){
							if(this.answers[i].TopicID == topic.TopicID && this.answers[i].Ans == option.OptionID){
								option.isAnswer = true
								break
							}
						}
					});
				}
			});
		}
	}
	
	public getTheQuestionnaire(tempID:string){
		let getPara = tempID;
		let getHeader = {useCustomErrorHandler: true};
		this._service.getQuestionnaire(getPara).then(({data})=>{
			if (data.Succeeded){
				this.questionnaire = data.Questionnaire
				document.getElementById("back").style.backgroundImage = "url(" + this.questionnaire.BackgroundImageUrl + ")";
				document.getElementById("back").style.backgroundSize = "cover";
				document.getElementById("divDescription2").innerHTML = this.questionnaire.Description;
			}else{
				this._negAlert.error(("Failed to get the questionnaire"));
			}
		},
		error=>{ })
		this._queryService.getAnswersByShortName(this.formControl.shortName, getPara,getHeader).then(({data}) => {
			if (data.Succeeded){
				this.answers = data.AnswerList
				this.attachAnswer()
			}else{
				this._negAlert.error("Failed to get the answers");
			}
		},
		error => {});
	}
}