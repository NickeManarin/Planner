import axios from 'axios';
import authHeader from './auth.header';

const API_URL = 'https://localhost:5001/api/auth/';

class AuthService {
    signup(user) {
        return axios.post(API_URL + 'signup', { name: user.name, email: user.email, password: user.password }).then(response => {
            if (response.data.accessToken)
                localStorage.setItem('user', JSON.stringify(response.data));
            
            return response.data;
        }).catch(error => {
            if (error.response != undefined && error.response.status < 500)
                return error.response.data;

            throw new Error(error.response.data);
        });
    }

    signin(user) {
        return axios.post(API_URL + 'signin', { email: user.email, password: user.password }).then(response => {
            if (response.data.accessToken)
                localStorage.setItem('user', JSON.stringify(response.data));
            
            return response.data;
        }).catch(error => {
            if (error.response != undefined && error.response.status < 500)
                return error.response.data;

            throw new Error(error.response.data);
        });
    }

    refresh(user) {
        user = user ? user : JSON.parse(localStorage.getItem('user'));
        
        if (!user || !user.refreshToken)
            throw new Error('Refresh token not available.');

        if (!user.refreshTokenExpiryDateUtc || new Date(user.refreshTokenExpiryDateUtc) < Date.now()) {
            console.log('Refresh token expired!', user);
            throw new Error('Refresh token already expired.');
        }

        return axios.post(API_URL + 'refresh', { email: user.email, refreshToken: user.refreshToken }, { headers: authHeader() })
        .then(response => {
            if (response.data.accessToken) {
                user.accessToken = response.data.accessToken;
                user.accessTokenExpiryDateUtc = response.data.accessTokenExpiryDateUtc;

                localStorage.setItem('user', JSON.stringify(user));
                return user;
            }
            
            return response.data;
        });
    }
   
    logout(user) {
        user = user ? user : JSON.parse(localStorage.getItem('user'));
        
        try {
            return axios.post(API_URL + 'revoke', { email: user.email, refreshToken: user.refreshToken }, { headers: authHeader() }).catch(async error => {
                //If unauthorized, try refreshing the token and calling this method again.
                if (error.response.status === 401)
                    return error.response;

                if (error.response != undefined && error.response.status < 500)
                    return error.response.data;
       
                throw new Error(error.response.data);
            });
        } finally {
            localStorage.removeItem('user');
        }
    }
}

export default new AuthService();