import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';

import { NegAjax, NegAlert, NegAuth, NegStorage, NegTranslate, NegMultiTab } from '@newkit/core';

import {AnswerParticipatorService} from '../../services/AnswerParticipatorService'
import {AnswerSheetStatisticsService} from '../../services/AnswerSheetStatisticsService'

import {Environment} from '../../config/Environment'

import './statistics.component.css';
import { Questionnaire } from '../../components/Model/Questionnaire';
import { AnswerSheetStatistics } from '../../components/Model/AnswerSheetStatistics';
import { AnswerStatistics } from '../../components/Model/AnswerStatistics';

@Component({
	selector: 'statistics',
	templateUrl: 'statistics.component.html'
})

export class StatisticsComponent implements OnInit {

	public statistics:AnswerSheetStatistics= new AnswerSheetStatistics;
	public topicBy: any = {};
	public detail: any = {};
	public questionnaireID: number;
	public url = null;
	public environmentUrl:string

	constructor(
		private router: Router,
		private negAjax: NegAjax,
		private negAlert: NegAlert,
		private _negAuth: NegAuth,
		private _negStorage: NegStorage,
		private _negTranslate:NegTranslate,
		private answerParticipatorService:AnswerParticipatorService,
		private answerSheetStatisticsService:AnswerSheetStatisticsService,
		private _environment:Environment,
		private negMultiTab: NegMultiTab
	) {
		this.environmentUrl = _environment.getEnvironmentUrl()
	 }

	ngOnInit() {
		this.negMultiTab.setCurrentTabName('Egg Rolls')
		if(this._negStorage.memory.get("sID") == undefined){
			// this.router.navigate(['/eggrolls/404'])
			this.negMultiTab.openPage('/eggrolls/404', null, false)
		}
		else{
			this.questionnaireID = this._negStorage.memory.get("sID");
			this._negStorage.memory.remove("sID");
			let ret = this.answerSheetStatisticsService.OnGet(this.questionnaireID)
			ret.then(({ data }) => {
				if(data.Succeeded == true){
					this.statistics = data
					document.getElementById("description").innerHTML = this.statistics.Description;
				}
				else{
					this.negAlert.error(this._negTranslate.get('statistics.statistics.getFailed'))
				}
			}, error => this.negAlert.error(this._negTranslate.get('statistics.statistics.getFailed')))
		}
		this._negTranslate.set('statistics', {
			'en-us': {
				questionnaire: {
					realName: 'Real name',
					anonymous:'Anonymous',
					deadline: 'Deadline',
					export:'Export Result',
					link:'Link'
				},
				topic: {
					multiChoice: 'Multiple choice',
					limit: 'Limit',
					limitTip1: ', have to choose ',
					limitTip2: ' options',
					requiredTip: ' *',				
				},
				statistics: {
					byPerson:'By person',
					byDepartment:'By department',
					option:'Option',
					number:'Number',
					percentage:'Percentage',
					detail:'Detail',
					personList:'Personnel list',
					allDepartments : 'All Departments',
					shortname : 'Shortname',
					fullname :'Fullname',
					department : 'Department',
					answer : 'Answer',
					noAnswer:'No one answered this question',
					getFailed:'Get message failed, please ensure that the questionnaire exists or contact E.T. for help'
				}
			},
			'zh-cn': {
				questionnaire: {
					realName: '实名',
					anonymous:'匿名',
					deadline: '截止时间',
					export:'导出结果',
					link:'链接'
				},
				topic: {
					multiChoice: '多选',
					limit: '限选 ',
					limitTip1: '， 必选 ',
					limitTip2: ' 个',
					requiredTip: ' *',
				},
				statistics: {
					byPerson:'个人统计',
					byDepartment:'部门统计',
					option:'选项',
					number:'选择人数',
					percentage:'百分比',
					detail:'详情',
					personList:'人员列表',
					allDepartments : '所有部门',
					shortname : '短名',
					fullname :'完整名',
					department : '部门',
					answer : '答案',
					noAnswer:'无人作答此题',
					getFailed:'获取信息失败，请确保问卷未被删除或联系E.T.寻求帮助'
				}
			},
			'zh-tw': {
				questionnaire: {
					realName: '实名',
					anonymous:'匿名',
					deadline: '截止时间：',
					export:'Export the result',
					link:'链接'
				},
				topic: {
					limit: 'Limit',
					required: 'Required',
					limitTip1: ', have to choose ',
					limitTip2: ' options',
					requiredTip: ' *',
				},
				statistics: {
					byPerson:'按个人',
					byDepartment:'按部门',
					option:'选项',
					number:'选择人数',
					percentage:'百分比',
					detail:'详情',
					personList:'人员列表',
					allDepartments : '所有部门',
					shortname : '短名',
					fullname :'完整名',
					department : '部门',
					answer : '回答',
					noAnswer:'无人作答此题',
					getFailed:'获取信息失败，请确保问卷未被删除或联系E.T.寻求帮助'
				}
			}
		});
	}

	public byPerson(topicID) {
		this.topicBy[topicID] = false;
	}

	public byDepartment(topicID) {
		this.topicBy[topicID] = true;
	}

	public topicsBy(topicID) {
		return this.topicBy[topicID];
	}

	public showDetail(topicID) {
		this.topicBy[topicID] = !this.topicBy[topicID];
		if (this.detail[topicID] == undefined) {
			var ret = this.answerParticipatorService.OnGet(this.questionnaireID, topicID)
			ret.then(({ data }) => {
				if(data.Succeeded == true){
					this.detail[topicID] = data;
				}
				else{
					this.negAlert.error(this._negTranslate.get('statistics.statistics.getFailed'))
				}
			}, error => {
				this.negAlert.error(this._negTranslate.get('statistics.statistics.getFailed'))
			})
		}
	}

	public getDetail(topicID) {
		console.log(this.detail[topicID])
		if (this.detail[topicID] != undefined && this.detail[topicID].Answers.length != 0) {
			return this.detail[topicID].Answers;
		}
		return undefined;
	}

	public getPersonnelList(topicID, optionID) {
		var personnelListPara = {QuestionnaireID:0, TopicID:0,OptionID:0};
		personnelListPara.QuestionnaireID = this.questionnaireID;
		personnelListPara.TopicID = topicID;
		personnelListPara.OptionID = optionID;
		this._negStorage.memory.set('personnelListPara', personnelListPara);
		// this.router.navigate(['/eggrolls/personnelList']);
		this.negMultiTab.openPage('/eggrolls/personnelList', null, false)
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

	public download() {
		var excel = '<table align=3Dleft"><tr><td colspan = 11>' + this.statistics.Title + '</td></tr><tr></tr>';
		for (var i = 0; i < this.statistics.Topics.length; i++) {
			if (this.statistics.Topics[i].Type != "Text") {
				excel += "<tr><td colspan = "
					+ (11) + ">"
					+ this.statistics.Topics[i].TopicID + "." + this.statistics.Topics[i].TopicTitle
					+ (this.statistics.Topics[i].Type == "Checkbox" ? ' Multiple choice ' : '')
					+ (this.statistics.Topics[i].Type != "Checkbox" || this.statistics.Topics[i].Limited == 0 || this.statistics.Topics[i].Limited == undefined ? '' : ", have to choose " + this.statistics.Topics[i].Limited + ' options')
					+ (this.statistics.Topics[i].IsRequired ? ' *' : '')
					+ "</td></tr><tr></tr>";
				excel += "<tr><td>Option</td><td>Number</td><td>Percentage</td><td></td>";
				for (var j = 0; j < this.statistics.Topics[i].Options[0].Departments.length; j++) {
					excel += "<td>" + this.statistics.Topics[i].Options[0].Departments[j].Department + "</td>"
				}
				excel += "</tr>";
				for (var j = 0; j < this.statistics.Topics[i].Options.length; j++) {
					excel += "<tr><td>" + this.statistics.Topics[i].Options[j].OptionID + "." + this.statistics.Topics[i].Options[j].OptionTitle + "</td>";
					excel += "<td>" + this.statistics.Topics[i].Options[j].ChosenNumber + "</td>";
					excel += "<td>" + this.statistics.Topics[i].Options[j].Percentage + "</td>";
					excel += "<td></td>";

					for (var k = 0; k < this.statistics.Topics[i].Options[j].Departments.length; k++) {

						excel += "<td>" + this.statistics.Topics[i].Options[j].Departments[k].ChosenNumber +
							"(" + this.statistics.Topics[i].Options[j].Departments[k].Percentage + ")" + "</td>";
					}
					excel += "</tr>"
				}
			}
			else {
				excel += "<tr><td colspan = "
					+ (11) + ">"
					+ this.statistics.Topics[i].TopicID + "." + this.statistics.Topics[i].TopicTitle
					+ "</td></tr>";
			}
			excel += "<tr></tr>";
		}
		var FileName = "[Statistics]" + this.statistics.Title;
		excel += "</table>";
		var excelFile = "<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:x='urn:schemas-microsoft-com:office:excel' xmlns='http://www.w3.org/TR/REC-html40'>";
		excelFile += '<meta http-equiv="content-type" content="application/vnd.ms-excel; charset=UTF-8">';
		excelFile += '<meta http-equiv="content-type" content="application/vnd.ms-excel';
		excelFile += '; charset=UTF-8">';
		excelFile += "<head>";
		excelFile += "<!--[if gte mso 9]>";
		excelFile += "<xml>";
		excelFile += "<x:ExcelWorkbook>";
		excelFile += "<x:ExcelWorksheets>";
		excelFile += "<x:ExcelWorksheet>";
		excelFile += "<x:Name>";
		excelFile += "{worksheet}";
		excelFile += "</x:Name>";
		excelFile += "<x:WorksheetOptions>";
		excelFile += "<x:DisplayGridlines/>";
		excelFile += "</x:WorksheetOptions>";
		excelFile += "</x:ExcelWorksheet>";
		excelFile += "</x:ExcelWorksheets>";
		excelFile += "</x:ExcelWorkbook>";
		excelFile += "</xml>";
		excelFile += "<![endif]-->";
		excelFile += "</head>";
		excelFile += "<body>";
		excelFile += excel;
		excelFile += "</body>";
		excelFile += "</html>";
		var uri = 'data:application/vnd.ms-excel;charset=utf-8,' + encodeURIComponent(excelFile);
		var link = document.createElement("a");
		link.href = uri;
		link.download = FileName + ".xls";
		document.body.appendChild(link);
		link.click();
		document.body.removeChild(link);
	}
}