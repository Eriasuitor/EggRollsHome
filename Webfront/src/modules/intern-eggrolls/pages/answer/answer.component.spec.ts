import { TestBed, inject } from '@angular/core/testing';

import { AnswerComponent } from './answer.component';

describe('a answer component', () => {
	let component: AnswerComponent;

	// register all needed dependencies
	beforeEach(() => {
		TestBed.configureTestingModule({
			providers: [
				AnswerComponent
			]
		});
	});

	// instantiation through framework injection
	beforeEach(inject([AnswerComponent], (AnswerComponent) => {
		component = AnswerComponent;
	}));

	it('should have an instance', () => {
		expect(component).toBeDefined();
	});
});