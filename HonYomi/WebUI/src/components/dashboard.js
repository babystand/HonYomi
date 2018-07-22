import React from 'react';
import {connect} from 'react-redux';
import requiresLogin from './requires-login';
import { fetchAllBooks } from '../actions/books';

export class Dashboard extends React.Component {
    componentDidMount() {
        this.props.dispatch(fetchAllBooks());
    }

    render() {
        return (
            <div className="dashboard">
                <div className="dashboard-username">
                    Username: {this.props.username}
                </div>
                <div className="dashboard-name">Name: {this.props.name}</div>
                <div className="dashboard-protected-data">
                    Books: {this.props.books}
                </div>
            </div>
        );
    }
}

const mapStateToProps = state => {
    const {currentUser} = state.auth;
    return {
        username: state.auth.currentUser,
        books: state.books.data
    };
};

export default requiresLogin()(connect(mapStateToProps)(Dashboard));
