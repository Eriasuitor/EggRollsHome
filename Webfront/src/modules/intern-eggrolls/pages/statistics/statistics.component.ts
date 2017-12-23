import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { NegAjax, NegAlert, NegAuth, NegStorage } from '@newkit/core';

import { MyService } from '../../services';

import './statistics.component.css';
import { start } from 'repl';

@Component({
	selector: 'statistics',
	templateUrl: 'statistics.component.html'
})

export class StatisticsComponent implements OnInit {

	public statistics = {DueDate:""};
	public topicBy: any = {};
	public detail: any = {};
	public questionnaireID: number;
	public url = null;

	constructor(
		private router: Router,
		private negAjax: NegAjax,
		private negAlert: NegAlert,
		private _service: MyService,
		private _negAuth: NegAuth,
		private _negStorage: NegStorage
	) { }

	ngOnInit() {
		this.questionnaireID = this._negStorage.memory.get("sID");
		this._negStorage.memory.remove("sID");
		let ret = this._service.getStatistics(this.questionnaireID)
		ret.then(({ data }) => {
			this.statistics = data.AnswerSheet
			document.getElementById("description").innerHTML = this.statistics.Description;
		}, error => this.statistics = undefined);
	}

	public download() {
		var excel = '<table><tr><td colspan = 11>' + this.statistics.Title + '</td></tr><tr></tr>';
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
				for (var j = 0; j < this.statistics.Topics[i].Options[0].DepartmentUnits.length; j++) {
					excel += "<td>" + this.statistics.Topics[i].Options[0].DepartmentUnits[j].Department + "</td>"
				}
				excel += "</tr>";
				for (var j = 0; j < this.statistics.Topics[i].Options.length; j++) {
					excel += "<tr><td>" + this.statistics.Topics[i].Options[j].OptionID + "." + this.statistics.Topics[i].Options[j].OptionTitle + "</td>";
					excel += "<td>" + this.statistics.Topics[i].Options[j].ChosenNumber + "</td>";
					excel += "<td>" + this.statistics.Topics[i].Options[j].PersonalUnits + "</td>";
					excel += "<td></td>";

					for (var k = 0; k < this.statistics.Topics[i].Options[j].DepartmentUnits.length; k++) {

						excel += "<td>" + this.statistics.Topics[i].Options[j].DepartmentUnits[k].ChosenNumber +
							"(" + this.statistics.Topics[i].Options[j].DepartmentUnits[k].Percentage + ")" + "</td>";

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
			var ret = this._service.getDetail(this.questionnaireID, topicID)
			ret.then(({ data }) => {
				this.detail[topicID] = data;
			}, error => {
				this.negAlert.error("Get detail failed.");
				this.detail[topicID] = undefined;
			})
		}
	}
	public getDetail(topicID) {
		if (this.detail[topicID] != undefined) {
			return this.detail[topicID].Answers;
		}
		return undefined;
	}
	public getPersonnelList(topicID, optionID) {
		// var paras = 'topicID=' + topicID + '&optionID=' + optionID + '&questionnaireID=' + this.questionnaireID;
		// var iTop = (window.screen.availHeight - 30 - 700) / 2;
		// var iLeft = (window.screen.availWidth - 10 - 800) / 2;
		// window.open("/test-module/personnelList?" + paras,'width=800,height=700,top=' + iTop + ',left=' + iLeft + ',toolbar=no,menubar=no,scrollbars=no,resizable=no,location=no,status=no');
		var personnelListPara = {QuestionnaireID:0, TopicID:0,OptionID:0};
		personnelListPara.QuestionnaireID = this.questionnaireID;
		personnelListPara.TopicID = topicID;
		personnelListPara.OptionID = optionID;
		this._negStorage.memory.set('personnelListPara', personnelListPara);
		// window.open("/intern-eggrolls/personnelList?" + paras, '_blank', 'width=800,height=700,top=' + iTop + ',left=' + iLeft);
		this.router.navigate(['/intern-eggrolls/personnelList']);
	}
	public getStanderdDate(dateGet){
		return dateGet.substr(0,6) == '/Date(' ? this.dmft(this.date(dateGet)) : dateGet;
	}
	public dmft(d) { return d.getFullYear() + '-' + this.pad(d.getMonth() + 1) + '-' + this.pad(d.getDate()) + ' ' + this.pad(d.getHours()) + ':' +  this.pad(d.getMinutes()); }
	public date(s) { return new Date(parseFloat(/Date\(([^)]+)\)/.exec(s)[1])); }
	public pad(d) { return d < 10 ? '0'+d : d; }
}