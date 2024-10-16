import { createSelector, createSelectorCreator, defaultMemoize } from 'reselect';
import hasDifferentItemsOrOrder from 'Utilities/Object/hasDifferentItemsOrOrder';
import createClientSideCollectionSelector from './createClientSideCollectionSelector';

function createUnoptimizedSelector(uiSection) {
  return createSelector(
    createClientSideCollectionSelector('performers', uiSection),
    (performers) => {
      const items = performers.items.map((s) => {
        return {
          id: s.id,
          foreignId: s.foreignId,
          sortTitle: s.fullName
        };
      });

      return {
        ...performers,
        items
      };
    }
  );
}

function movieListEqual(a, b) {
  return hasDifferentItemsOrOrder(a, b);
}

const createPerformerEqualSelector = createSelectorCreator(
  defaultMemoize,
  movieListEqual
);

function createPerformerClientSideCollectionItemsSelector(uiSection) {
  return createPerformerEqualSelector(
    createUnoptimizedSelector(uiSection),
    (performers) => performers
  );
}

export default createPerformerClientSideCollectionItemsSelector;
