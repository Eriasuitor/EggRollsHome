import { Component, OnInit } from '@angular/core';
import { Location, NgStyle } from '@angular/common';
import { NegAuth, NegAlert, NegTranslate } from '@newkit/core';

@Component({
	selector: 'manual',
	templateUrl: 'manual.component.html'
})
export class ManualComponent implements OnInit {

	constructor(
		private _negAuth: NegAuth,
		private negAlert: NegAlert,
		private _negTranslate: NegTranslate,
	) { }

	ngOnInit() {}
}