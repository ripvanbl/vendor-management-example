import React from 'react';
import { shallow } from 'enzyme';
import ResultGrid from './resultGrid';

const setup = (props = {}) => {
  return shallow(<ResultGrid {...props} />);
};

describe('Vendor Search <ResultGrid />', () => {
  let component;

  beforeEach(() => {
    component = setup();
  });

  afterEach(() => {
    component = null;
  });

  it('should render the search results grid with 4 columns', () => {
    expect(component.find('Table').length).toBe(1);
    expect(component.find('th').length).toBe(4);
  });
});
