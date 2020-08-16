import axios from 'axios';
import authHeader from './auth.header';

const API_URL = 'https://localhost:5001/api/event/';

class EventService {
    getAll() {
        return axios.get(API_URL, { headers: authHeader() }).then(response => {
            return response.data;
        }).catch(async error => {
            //If unauthorized, try refreshing the token and calling this method again.
            if (error.response.status === 401)
                return error.response;

            if (error.response != undefined && error.response.status < 500)
                return error.response.data;

            throw new Error(error.response.data);
        });
    }

    create(event) {
        return axios.post(API_URL + 'create', event, { headers: authHeader() }).then(response => {           
            return response.data;
        }).catch(error => {
            if (error.response.status === 401)
                return error.response;

            if (error.response != undefined && error.response.status < 500)
                return error.response.data;

            throw new Error(error.response.data);
        });
    }

    edit(event) {
        return axios.post(API_URL + 'edit', event, { headers: authHeader() }).then(response => {           
            return response.data;
        }).catch(error => {
            if (error.response.status === 401)
                return error.response;

            if (error.response != undefined && error.response.status < 500)
                return error.response.data;

            throw new Error(error.response.data);
        });
    }

    remove(event) {
        return axios.post(API_URL + 'remove', event, { headers: authHeader() }).then(response => {           
            return response.data;
        }).catch(error => {
            if (error.response.status === 401)
                return error.response;

            if (error.response != undefined && error.response.status < 500)
                return error.response.data;

            throw new Error(error.response.data);
        });
    }
}

export default new EventService();