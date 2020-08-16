import axios from 'axios';
import authHeader from './auth.header';

const API_URL = 'https://localhost:5001/api/participation/';

class ParticipationService {
    edit(part) {
        return axios.post(API_URL + 'edit', part, { headers: authHeader() }).then(response => {           
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

export default new ParticipationService();