import React from 'react';
import ReactDOM from 'react-dom';

import injectTapEventPlugin from 'react-tap-event-plugin';

import AppBar from 'material-ui/lib/app-bar';


injectTapEventPlugin();

class Device extends React.Component {
    render() {
        return (
            <AppBar title="Sample Application" />
        );
    }
};

ReactDOM.render(
    <Device />,
    document.getElementById('device')
);