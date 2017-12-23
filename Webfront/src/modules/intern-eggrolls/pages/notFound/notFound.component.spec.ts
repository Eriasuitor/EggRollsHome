import { TestBed, inject } from '@angular/core/testing';

import { NotFoundComponent } from './notFound.component';

describe('a notFound component', () => {
	let component: NotFoundComponent;

	// register all needed dependencies
	beforeEach(() => {
		TestBed.configureTestingModule({
			providers: [
				NotFoundComponent
			]
		});
	});

	// instantiation through framework injection
	beforeEach(inject([NotFoundComponent], (NotFoundComponent) => {
		component = NotFoundComponent;
	}));

	it('should have an instance', () => {
		expect(component).toBeDefined();
	});
});