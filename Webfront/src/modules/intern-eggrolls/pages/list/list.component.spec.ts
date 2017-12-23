import { TestBed, inject } from '@angular/core/testing';

import { MyFolderComponent } from './myFolder.component';

describe('a myFolder component', () => {
	let component: MyFolderComponent;

	// register all needed dependencies
	beforeEach(() => {
		TestBed.configureTestingModule({
			providers: [
				MyFolderComponent
			]
		});
	});

	// instantiation through framework injection
	beforeEach(inject([MyFolderComponent], (MyFolderComponent) => {
		component = MyFolderComponent;
	}));

	it('should have an instance', () => {
		expect(component).toBeDefined();
	});
});