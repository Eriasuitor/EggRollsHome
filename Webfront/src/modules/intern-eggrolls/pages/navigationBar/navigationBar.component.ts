import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import "./navigationBar.component.css";

@Component({
	selector: 'navigationBar',
	templateUrl: 'navigationBar.component.html'
})

export class NavigationBarComponent implements OnInit {
	constructor(
		private router: Router
	) { }
	ngOnInit() { }
	public gotoIndex() {
		this.router.navigate(['/intern-eggrolls/index']);
	}
	public newQuestionnaire() {
		this.router.navigate(['/intern-eggrolls/create']);
	}
}