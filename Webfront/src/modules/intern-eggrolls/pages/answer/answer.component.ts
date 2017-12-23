import { Component, OnInit } from '@angular/core';
import { Location, NgStyle } from '@angular/common';
import { NegAuth, NegAlert } from '@newkit/core';

import { MyService } from '../../services';

import "./answer.component.css";

@Component({
	selector: 'answer',
	templateUrl: 'answer.component.html'
})
export class AnswerComponent implements OnInit {

	public state: any = { skip: 0, take: 5, sort: [] };
	public questionnaire: any = { Title: "", Description: "", IsRealName: false, DueDate: "", Topics: [] };
	public postJson: any = { QuestionnaireID: 0, ShortName: null, FullName: null, Department: null, Topics: [] };
	public thanks = false;
	public submitting = false;
	public thanksInf = { title: null, inf: null, end: null };
	public doc: any;
	public iCheck: any = {};
	public selectionTimer: any = {};
	public static selectionOpa: any = {};
	private questionnaireID;
	public count = 0;

	constructor(
		private _service: MyService,
		private _negAuth: NegAuth,
		private negAlert: NegAlert
	) { }

	ngOnInit() {
		var addressUrl = location.search.slice(1);
		var searchParams = new URLSearchParams(addressUrl);
		this.questionnaireID = searchParams.get('questionnaireID');
		let ret = this._service.getQuestionnaire(this.questionnaireID)
		ret.then(({ data }) => {
			if (data.Questionnaire.Status == 'Ended') {
				this.blurBack();
				this.thanksInf.title = "Sorry !";
				this.thanksInf.inf = "Deadline has arrived";
				this.thanksInf.end = "So you can not fill in this questionnaire";
				this.submitting = true;
				this.thanks = true;
				setTimeout(this.showThanks, 600);
			}
			else if (data.Questionnaire.Status == 'Processing') {
				this.isNewUser();
				this.questionnaire = data.Questionnaire;
				this.init();
			}
			else {
				this.blurBack();
				this.thanksInf.title = "Sorry !";
				this.thanksInf.inf = "404 Not Found";
				this.thanksInf.end = "Can not find this questionnaire";
				this.submitting = true;
				this.thanks = true;
				setTimeout(this.showThanks, 600);
			}
		}, error => {
			this.questionnaire = undefined
			this.blurBack();
			this.thanksInf.title = "Sorry !";
			this.thanksInf.inf = "404 Not Found";
			this.thanksInf.end = "Can not find this questionnaire";
			this.submitting = true;
			this.thanks = true;
			setTimeout(this.showThanks, 600);
		});
	}
	public isNewUser() {
		let ret = this._service.getAnswer(this.questionnaireID, this._negAuth.user.UserName)
		ret.then(({ data }) => {
			if(data.Succeeded){
				this.blurBack();
				this.thanksInf.title = "Sorry !";
				this.thanksInf.inf = "Inaccessible";
				this.thanksInf.end = "Because you have submitted this questionnaire";
				this.submitting = true;
				this.thanks = true;
				setTimeout(this.showThanks, 600);
			}
		}, error => { });
	}
	public init() {
		document.getElementById("back").style.backgroundImage = "url(" + this.questionnaire.BackgroundImageUrl + ")";
		document.getElementById("back").style.backgroundSize = "cover";
		document.getElementById("description").innerHTML = this.questionnaire.Description;
		// document.getElementById("backBoard").style.backgroundImage = "url(" + this.questionnaire.BackgroundImageUrl + ")";
	}
	public getQuestionnaire() {
		if (this.questionnaire != undefined) {
			return this.questionnaire;
		}
	}
	public doSubmit() {
		this.postJson = { QuestionnaireID: 0, ShortName: null, FullName: null, Department: null, Topics: [] };
		this.postJson.QuestionnaireID = this.questionnaire.QuestionnaireID;
		this.postJson.ShortName = this._negAuth.user.UserName;
		this.postJson.FullName = this._negAuth.user.FullName
		this.postJson.Department = this._negAuth.user.Department;
		var allRight = true;
		for (var i = 0; i < this.questionnaire.Topics.length; i++) {
			switch (this.questionnaire.Topics[i].Type) {
				case 'Radio':
					var topicRadio = { TopicID: this.questionnaire.Topics[i].TopicID, Type: 'Radio', Answers: [] };
					var options = document.getElementsByName(this.questionnaire.Topics[i].TopicID);
					var forkWrong = true;
					for (var j = 0; j < options.length; j++) {
						if (options[j].checked) {
							topicRadio.Answers.push({ Type: 'Radio', TopicID: topicRadio.TopicID, Ans: options[j].value });
							this.postJson.Topics.push(topicRadio);
							forkWrong = false;
							break;
						}
					}
					if (this.questionnaire.Topics[i].IsRequired && forkWrong) {
						document.getElementById(this.questionnaire.Topics[i].TopicID).style.color = "red";
						allRight = false;
					}
					break;
				case 'Checkbox':
					var topicCheckbox = { TopicID: this.questionnaire.Topics[i].TopicID, Type: 'Checkbox', Answers: [] };
					var options = document.getElementsByName(this.questionnaire.Topics[i].TopicID);
					var forkWrong = true;
					var totSelected = 0;
					for (var j = 0; j < options.length; j++) {
						if (options[j].checked) {
							topicCheckbox.Answers.push({ Type: 'Checkbox', TopicID: topicCheckbox.TopicID, Ans: options[j].value });
							forkWrong = false;
							totSelected++;
						}
					}
					if (this.questionnaire.Topics[i].IsRequired && (forkWrong || this.questionnaire.Topics[i].Limited > 0 && this.questionnaire.Topics[i].Limited != totSelected)) {
						document.getElementById(this.questionnaire.Topics[i].TopicID).style.color = "red";
						allRight = false;
					}
					else {
						this.postJson.Topics.push(topicCheckbox);
					}
					break;
				case 'Text':
					var topicText = { TopicID: this.questionnaire.Topics[i].TopicID, Type: 'Text', Answers: [] };
					var options = document.getElementsByName(this.questionnaire.Topics[i].TopicID);
					topicText.Answers.push({ Type: 'Text', TopicID: topicText.TopicID, Ans: options[0].value });
					if (this.questionnaire.Topics[i].IsRequired && (topicText.Answers[0].Ans == null || topicText.Answers[0].Ans == "")) {
						document.getElementById(this.questionnaire.Topics[i].TopicID).style.color = "red";
						allRight = false;
					}
					else {
						this.postJson.Topics.push(topicText);
					}
					break;
				default:
					break;
			}
		}
		if (allRight) {
			this.count += 1;
			this.blurBack();
			if (this.count == 1) {
				let ret = this._service.postAnswer(this.postJson)
				ret.then(({ data }) => {
					this.thanksInf.title = "Thanks !";
					this.thanksInf.inf = "For your participation";
					this.thanksInf.end = "And your answer has been submitted successfully";
					if (this.count != 1) {
						this.thanksInf.end += ". You click " + this.count + " times, you are great.";
					}
				}, error => {
					this.thanksInf.title = "Sorry !";
					this.thanksInf.inf = "Submitted failed";
					this.thanksInf.end = "The reason is that the deadline has expired or you have submitted";
				})
				this.submitting = true;
				this.thanks = true;
				setTimeout(this.showThanks, 500);
			}
		}
	}
	public blurBack() {
		var blur = 0;
		let inter1 = setInterval(function () {
			if (blur <= 30) {
				document.getElementById("back").style.filter = "blur(" + blur + "px)";
				document.getElementById("mainTain").style.filter = "blur(" + blur + "px)";
				blur += 1;
			}
			else {
				clearInterval(inter1);
			}
		}, 10);
	}
	public showThanks() {
		var iTop = (window.screen.availHeight - 450) / 2;
		var iLeft = (window.screen.availWidth - 450) / 2;
		var thanksBoard = document.getElementById("thanksBoard");
		var thanks = document.getElementById("thanks");
		var opacity = 0;
		thanksBoard.style.top = "0";
		thanksBoard.style.bottom = "0";
		thanks.style.top = iTop + "px";
		thanks.style.left = iLeft + "px";
		let inter2 = setInterval(function () {
			if (opacity <= 100) {
				thanksBoard.style.opacity = opacity / 100 + '';
				opacity += 2;
			}
			else {
				clearInterval(inter2);
			}
		}, 10);
	}
	public reduction(topicID) {
		document.getElementById(topicID).style.color = "black";
		document.getElementById(topicID + "tips").style.color = "red";
	}
	public checkSelected(topicID, optionID) {
		document.getElementById('i' + topicID + optionID).style.display = 'inline';
		if (this.questionnaire.Topics[topicID - 1].Limited != 0) {
			var options = document.getElementsByName(topicID);
			var totSelected = 0;
			for (var j = 0; j < options.length; j++) {
				if (options[j].checked) {
					totSelected++;
				}
			}
			if (totSelected > this.questionnaire.Topics[topicID - 1].Limited) {
				document.getElementById(topicID + optionID).checked = false;
			}
		}
		if (document.getElementById(topicID + optionID).checked) {
			document.getElementById('i' + topicID + optionID).style.display = 'inline';
		}
		else {
			document.getElementById('i' + topicID + optionID).style.display = 'none';
		}
	}
	public selected(topicID, optionID) {
		if (document.getElementById(topicID + optionID).checked) {
			document.getElementById('i' + topicID + optionID).style.display = 'inline';
		}
		else {
			document.getElementById('i' + topicID + optionID).style.display = 'none';
		}
		for (var j = 0; j < this.questionnaire.Topics[topicID - 1].Options.length; j++) {
			if (this.questionnaire.Topics[topicID - 1].Options[j].OptionID != optionID) {
				document.getElementById('i' + topicID + this.questionnaire.Topics[topicID - 1].Options[j].OptionID).style.display = 'none';
				document.getElementById(topicID + this.questionnaire.Topics[topicID - 1].Options[j].OptionID).checked = false;
			}
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
	public getStanderdDate(dateGet) {
		return dateGet.substr(0, 6) == '/Date(' ? this.dmft(this.date(dateGet)) : dateGet;
	}
	public dmft(d) { return d.getFullYear() + '-' + this.pad(d.getMonth() + 1) + '-' + this.pad(d.getDate()) + ' ' + this.pad(d.getHours()) + ':' + this.pad(d.getMinutes()); }
	public date(s) { return new Date(parseFloat(/Date\(([^)]+)\)/.exec(s)[1])); }
	public pad(d) { return d < 10 ? '0' + d : d; }
}