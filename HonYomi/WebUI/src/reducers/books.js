import {FETCH_ALL_BOOKS_SUCCESS, FETCH_ALL_BOOKS_ERROR} from '../actions/books';
const initialState = {
    data : [],
    error: null
};
export default function reducer(state = initialState, action){
    if(action.type === FETCH_ALL_BOOKS_SUCCESS){
        return Object.assign({}, state, {data: action.data, error: null});
    }
    else if (action.type === FETCH_ALL_BOOKS_ERROR){
        return Object.assign({}, state, {error: action.error});
    }
    return state;
}