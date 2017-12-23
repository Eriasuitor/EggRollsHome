import { TestBed, inject } from '@angular/core/testing';

import { StatisticsComponent } from './statistics.component';

describe('a statistics component', () => {
	let component: StatisticsComponent;

	// register all needed dependencies
	beforeEach(() => {
		TestBed.configureTestingModule({
			providers: [
				StatisticsComponent
			]
		});
	});

	// instantiation through framework injection
	beforeEach(inject([StatisticsComponent], (StatisticsComponent) => {
		component = StatisticsComponent;
	}));

	it('should have an instance', () => {
		expect(component).toBeDefined();
	});
});