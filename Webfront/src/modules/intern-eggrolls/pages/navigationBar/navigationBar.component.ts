import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { NegTranslate } from '@newkit/core';

import "./navigationBar.component.css";

@Component({
	selector: 'navigationBar',
	templateUrl: 'navigationBar.component.html'
})

export class NavigationBarComponent implements OnInit {
	constructor(
		private router: Router,
		private _negTranslate:NegTranslate
	) { }
	ngOnInit() {
		this._negTranslate.set('navigationBar',{
			'en-us':{
				main:{
					systemTitlt: 'Questionnaire System',
					newQuestionnaire:'New Questionnaire'
					}
				},
			'zh-cn': {
				main:{
					systemTitlt: '蛋卷问卷调查系统',
					newQuestionnaire:' 新 建 调 查 表'
					}
				},
			'zh-tw':{
				main:{
					systemTitlt: 'Questionnaire System',
					newQuestionnaire:'New Questionnaire'
					}
				}
			}
		); 
	 }
	public gotoIndex() {
		this.router.navigate(['/intern-eggrolls/index']);
	}
	public newQuestionnaire() {
		this.router.navigate(['/intern-eggrolls/create']);
	}
}