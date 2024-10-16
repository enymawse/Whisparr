import _ from 'lodash';
import moment from 'moment';
import PropTypes from 'prop-types';
import React, { Component } from 'react';
import { connect } from 'react-redux';
import { createSelector } from 'reselect';
import CalendarDay from './CalendarDay';

function sort(items) {
  return _.sortBy(items, (item) => {
    if (item.isGroup) {
      return moment(item.events[0].releaseDate).unix();
    }

    return moment(item.releaseDate).unix();
  });
}

function createCalendarEventsConnector() {
  return createSelector(
    (state, { date }) => date,
    (state) => state.calendar.items,
    (date, items) => {
      const filtered = _.filter(items, (item) => {
        return (item.releaseDate && moment(date).isSame(moment(item.releaseDate), 'day'));
      });

      return sort(filtered);
    }
  );
}

function createMapStateToProps() {
  return createSelector(
    (state) => state.calendar,
    createCalendarEventsConnector(),
    (calendar, events) => {
      return {
        time: calendar.time,
        view: calendar.view,
        events
      };
    }
  );
}

class CalendarDayConnector extends Component {

  //
  // Render

  render() {
    return (
      <CalendarDay
        {...this.props}
      />
    );
  }
}

CalendarDayConnector.propTypes = {
  date: PropTypes.string.isRequired
};

export default connect(createMapStateToProps)(CalendarDayConnector);
