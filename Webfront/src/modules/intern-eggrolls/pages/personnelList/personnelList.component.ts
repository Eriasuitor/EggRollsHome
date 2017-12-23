import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { NegStorage,NegAlert} from '@newkit/core';

import { MyService } from '../../services';

import './personnelList.component.css';

@Component({
	selector: 'personnelList',
	templateUrl: 'personnelList.component.html'
})

export class PersonnelListComponent implements OnInit {
	public personnelList: any = {};
	public personnelListPara:any = {};
	constructor(
		private _service: MyService,
		private router: Router,
		private _negStorage:NegStorage,
		private negAlert:NegAlert
	) { }
	ngOnInit() {
		this.personnelListPara = this._negStorage.memory.get("personnelListPara");
		this._negStorage.memory.remove("personnelListPara");
		if(this.personnelListPara != undefined){
			let topicID = this.personnelListPara.TopicID;
			let optionID = this.personnelListPara.OptionID;
			let questionnaireID = this.personnelListPara.QuestionnaireID;
			if (this.personnelList.data == undefined) {
				this._service.getPersonnelList(questionnaireID, topicID, optionID)
					.then(({ data }) => {
						this.personnelList.data = data;
					},
					error => this.negAlert.error("Get list failed."))
			}
		}
	}
	public getPersonnelList() {
		if (this.personnelList.data != undefined) {
			return this.personnelList.data.Answers;
		}
	}
	public back(){
		this._negStorage.memory.set('sID', this.personnelListPara.QuestionnaireID);
		// window.open("/intern-eggrolls/personnelList?" + paras, '_blank', 'width=800,height=700,top=' + iTop + ',left=' + iLeft);
		this.router.navigate(['/intern-eggrolls/statistics']);
	}
}