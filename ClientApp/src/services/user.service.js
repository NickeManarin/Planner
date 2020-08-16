import axios from 'axios';
import authHeader from './auth.header';

const API_URL = 'https://localhost:5001/api/user/';

class UserService {
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
}

export default new UserService();