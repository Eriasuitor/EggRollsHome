import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { NegTranslate, NegMultiTab } from '@newkit/core';

import "./navigationBar.component.css";

@Component({
	selector: 'navigationBar',
	templateUrl: 'navigationBar.component.html'
})

export class NavigationBarComponent implements OnInit {
	constructor(
		private router: Router,
		private _negTranslate: NegTranslate,
		private negMultiTab:NegMultiTab
	) { }

	ngOnInit() {
		this.negMultiTab.setCurrentTabName('Egg Rolls')
		this._negTranslate.set('navigationBar', {
			'en-us': {
				main: {
					systemTitlt: 'Egg Rolls',
					newQuestionnaire: 'New Questionnaire'
				}
			},
			'zh-cn': {
				main: {
					systemTitlt: '蛋卷',
					newQuestionnaire: ' 新建调查表'
				}
			},
			'zh-tw': {
				main: {
					systemTitlt: 'Questionnaire System',
					newQuestionnaire: 'New Questionnaire'
				}
			}
		});
	}

	public gotoIndex() {
		// this.router.navigate(['/eggrolls/index']);
		this.negMultiTab.openPage('/eggrolls/index', null, false)
	}

	public newQuestionnaire() {
		// this.router.navigate(['/eggrolls/create']);
		this.negMultiTab.openPage('/eggrolls/create', null, false)
	}
}