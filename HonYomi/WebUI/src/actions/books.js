import {
    API_BASE_URL
} from '../config';
import {
    normalizeResponseErrors
} from './utils';

export const FETCH_ALL_BOOKS_SUCCESS = 'FETCH_ALL_BOOKS_SUCCESS';
export const fetchAllBooksSuccess = data => ({
    type: FETCH_ALL_BOOKS_SUCCESS,
    data
});
export const FETCH_ALL_BOOKS_ERROR = 'FETCH_ALL_BOOKS_ERROR';
export const fetchAllBooksError = error => ({
    type: FETCH_ALL_BOOKS_ERROR,
    error
});

export const fetchAllBooks = () => (dispatch, getState) => {
    const authToken = getState().auth.authToken;
    return fetch(`${API_BASE_URL}/books/list`, {
        method: 'GET',
        headers: {
            Authorization: `Bearer ${authToken}`
        }
    }).then(res => normalizeResponseErrors(res))
      .then(res => res.json())
      .then(({data}) => dispatch(fetchAllBooksSuccess(data)))
      .catch(err => dispatch(fetchAllBooksError(err)));
};