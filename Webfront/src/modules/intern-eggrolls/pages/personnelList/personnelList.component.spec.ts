import { TestBed, inject } from '@angular/core/testing';

import { PersonnelListComponent } from './personnelList.component';

describe('a personnelList component', () => {
	let component: PersonnelListComponent;

	// register all needed dependencies
	beforeEach(() => {
		TestBed.configureTestingModule({
			providers: [
				PersonnelListComponent
			]
		});
	});

	// instantiation through framework injection
	beforeEach(inject([PersonnelListComponent], (PersonnelListComponent) => {
		component = PersonnelListComponent;
	}));

	it('should have an instance', () => {
		expect(component).toBeDefined();
	});
});