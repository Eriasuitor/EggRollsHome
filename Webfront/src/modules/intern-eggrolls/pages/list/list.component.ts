import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';

import { NegAjax, NegAlert, NegAuth, NegStorage, NegTranslate } from '@newkit/core';

import { MyService } from '../../services';

import './my.component.styl';
import './list.component.css';
import { QuestionnaireSearch } from '../../components/Model/QuestionnaireSearch';

@Component({
	selector: 'list',
	templateUrl: 'list.component.html'
})

export class ListComponent implements OnInit {

	public state: any = { skip: 0, take: 5, sort: [] };
	public searchObj = { PageSize: 5, PageIndex: 0, ShortName: "", Title: "" };
	public PageIndexDis = 1;
	public title = "";
	public pages = 5;
	public questionnaireList:QuestionnaireSearch = new QuestionnaireSearch();
	public pagesOrder: any[] = [];
	constructor(
		private router: Router,
		private negAjax: NegAjax,
		private negAlert: NegAlert,
		private _service: MyService,
		private _negAuth: NegAuth,
		private _negStorage: NegStorage,
		private _negTranslate:NegTranslate
	) { }
	ngOnInit() {

		this._negStorage.memory.remove("CreateQuestionnaire");
		if (this._negStorage.memory.get("PageSize") != undefined) {
			this.searchObj.PageSize = this._negStorage.memory.get("PageSize");
		}
		if (this._negStorage.memory.get("PageIndex") != undefined) {
			this.searchObj.PageIndex = this._negStorage.memory.get("PageIndex");
		}
		if (this._negStorage.memory.get("Title") != undefined) {
			this.searchObj.Title = this._negStorage.memory.get("Title");
			this.title = this.searchObj.Title;
		}
		this.searchObj.ShortName = this._negAuth.user.UserName;
		this.getPagedQuestionnaires();

		this._negTranslate.set('list',{
			'en-us':{
				toolbar:{
					searchTitle: 'Type to search the questionnaire title',
					perPage:'items per page.',
					showPage:'Show page',
					of:'of',
					page:'',
					previous:'Previous',
					next:'Next'
					},
				questionnaire:{
					noQuestionnaire:'No questionnaire found',
					draft:'Draft',
					processing:'Processing',
					ended:'Ended',
					realName:'Real name',
					anonymous:'Anonymous',
					volumes:'Volumes',
					deadline:'Deadline',
					details:'Details',
					edit:'Edit',
					statistics:'Statistics',
					copy:'Copy',
					delete:'Delete'
					}
				},
			'zh-cn': {
				toolbar:{
					searchTitle: '键入以搜索问卷名称',
					perPage:'张问卷每页',
					showPage:'显示第',
					of:'页，共',
					page:'页',
					previous:'上一页',
					next:'下一页'
					},
				questionnaire:{
					noQuestionnaire:'未找到任何问卷',
					draft:'草稿',
					processing:'进行中',
					ended:'已结束',
					realName:'实名',
					anonymous:'匿名',
					volumes:'参与人数',
					deadline:'截止时间',
					details:'详情',
					edit:'编辑',
					statistics:'统计',
					copy:'复制',
					delete:'删除'
					}
				},
			'zh-tw':{
				toolbar:{
					searchTitle: '键入以搜索问卷名称',
					perPage:'items per page.',
					showPage:'Show page',
					of:'of',
					page:'',
					previous:'上一页',
					next:'下一页'
					},
				questionnaire:{
					noQuestionnaire:'No questionnaire found',
					draft:'Draft',
					processing:'Processing',
					ended:'Ended',
					realName:'Real name',
					anonymous:'Anonymous',
					volumes:'Volumes',
					deadline:'Deadline',
					details:'Details',
					edit:'Edit',
					statistics:'Statistics',
					copy:'Copy',
					delete:'Delete'
					}
				}
			}
		); 
	}
	public doPreview(questionnaireID) {
		this.router.navigate(['/intern-eggrolls/preview', questionnaireID]);
	}
	public changePageSize() {
		this.searchObj.PageSize = document.getElementById("pageSettings").value;
		this.searchObj.PageIndex = 0;
		this.getPagedQuestionnaires();
	}
	public toBlack() {
		document.getElementById("pages").style.color = 'black';
	}
	public adjustment() {	
		let a :any = this.PageIndexDis.toString();
		a = a.replace(/[０１２３４５６７８９]/g, function (v) { return v.charCodeAt(0) - 65296; });		
		let temp = parseInt(a);
		if (isNaN(temp) || temp > this.pages) {
			this.PageIndexDis = this.questionnaireList.PageIndex + 1;
			document.getElementById("pages").style.color = 'red';
		}
		else if (temp < 1) {
			this.PageIndexDis = this.questionnaireList.PageIndex + 1;
		}
		else {
			this.searchObj.PageIndex = temp - 1;
			this.getPagedQuestionnaires();
		}
	}
	public search() {
		this.searchObj.PageIndex = 0;
		this.searchObj.Title = this.title;
		this.searchObj.Title = this.searchObj.Title.replace(/(^\s*)/g, "");
		this.searchObj.Title = this.searchObj.Title.replace(/(\s*$)/g, "");
		this.getPagedQuestionnaires();
	}
	public getPagedQuestionnaires() {
		this.toBlack();
		this._service.getPagedQuestionnaires(this.searchObj)
			.then(({ data }) => {
				this.questionnaireList = data;
				this.pages = this.questionnaireList.Pages;
				this.PageIndexDis = this.questionnaireList.PageIndex + 1;
				this._negStorage.memory.set("PageSize",this.searchObj.PageSize);
				this._negStorage.memory.set("PageIndex",this.searchObj.PageIndex);
				this._negStorage.memory.set("Title",this.searchObj.Title);
				this.pagesOrder = [];
				if (this.questionnaireList.PageIndex - 2 < 0) {
					for (var i = 0; i < this.pages && i < 5; i++) {
						this.pagesOrder[i] = i + 1;
					}
				}
				else if (this.questionnaireList.PageIndex + 2 >= this.pages) {
					if (this.pages >= 5) {
						for (var i = this.pages - 5, j = 0; i < this.pages; i++ , j++) {
							this.pagesOrder[j] = i + 1;
						}
					}
					else {
						for (var i = 0; i < this.pages && i < 5; i++) {
							this.pagesOrder[i] = i + 1;
						}
					}
				}
				else {
					for (var i = this.questionnaireList.PageIndex - 1, j = 0; j < 5; j++ , i++) {
						this.pagesOrder[j] = i;
					}
				}
			}, error => {
				if (this.searchObj.PageIndex > 0) {
					this.searchObj.PageIndex--;
					this.getPagedQuestionnaires();
				}
				else {
					this.questionnaireList = new QuestionnaireSearch();
					this.questionnaireList.PageIndex = 0;
					this.questionnaireList.Pages = 1;
					this.pages = 1;
					this.PageIndexDis = 1;
					this.pagesOrder = [];
					this.pagesOrder[0] = 1;
				}
			});
	}
	public doDelete(questionnaireID, questionnaireTitle) {
		this.negAlert.confirm(`Sure to delete "${questionnaireTitle}"?`, () => {
			this._service.doDelete(questionnaireID).then((data) => {
				this.negAlert.success('Delete questionnaire successfully.');
				this.getPagedQuestionnaires();
			}, error => {
				this.negAlert.error('Delete questionnaire failed.');
			});
		});
	}
	public doStatistics(questionnaireID) {
		this._negStorage.memory.set('sID', questionnaireID);
		this.router.navigate(['/intern-eggrolls/statistics']);
	}
	public doEdit(questionnaireID) {
		this._negStorage.memory.set('eID', questionnaireID);
		this.router.navigate(['/intern-eggrolls/edit', questionnaireID]);
	}
	public doCopy(questionnaireID) {
		this._negStorage.memory.set('cID', questionnaireID);
		this.router.navigate(['/intern-eggrolls/edit'], { queryParams: { 'id': questionnaireID } });
	}
	public previous() {
		if (this.searchObj.PageIndex != 0) {
			this.searchObj.PageIndex--;
			this.getPagedQuestionnaires();
		}
	}
	public next() {
		if (this.PageIndexDis < this.pages) {
			this.searchObj.PageIndex++;
			this.getPagedQuestionnaires();
		}
	}
	public pageJump(page) {
		this.searchObj.PageIndex = page - 1;
		this.getPagedQuestionnaires();
	}
	public doDetails(questionnaireID) {
		this._negStorage.memory.set('dID', questionnaireID);
		this.router.navigate(['/intern-eggrolls/details', questionnaireID]);
	}
	public getStanderdDate(dateGet){
		return dateGet.substr(0,6) == '/Date(' ? this.dmft(this.date(dateGet)) : dateGet;
	}
	public dmft(d) { return d.getFullYear() + '-' + this.pad(d.getMonth() + 1) + '-' + this.pad(d.getDate()) + ' ' + this.pad(d.getHours()) + ':' +  this.pad(d.getMinutes()); }
	public date(s) { return new Date(parseFloat(/Date\(([^)]+)\)/.exec(s)[1])); }
	public pad(d) { return d < 10 ? '0'+d : d; }
}