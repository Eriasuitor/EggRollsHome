import { TestBed, inject } from '@angular/core/testing';

import { SelvCombComponent } from './selvComb.component';

describe('a selvComb component', () => {
	let component: SelvCombComponent;

	// register all needed dependencies
	beforeEach(() => {
		TestBed.configureTestingModule({
			providers: [
				SelvCombComponent
			]
		});
	});

	// instantiation through framework injection
	beforeEach(inject([SelvCombComponent], (SelvCombComponent) => {
		component = SelvCombComponent;
	}));

	it('should have an instance', () => {
		expect(component).toBeDefined();
	});
});