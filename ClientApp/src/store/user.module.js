import UserService from '../services/user.service';

export const user = {
    namespaced: true,
    state: { },

    actions: {
      getAll({ commit, dispatch }) {
          return UserService.getAll().then(
                async response => {
                    if (response.status === 401) {
                        try {
                            var resp = await dispatch('auth/refresh', null, { root: true } );

                            console.log('Result of refresh', resp);

                            if (resp !== undefined)
                                return await dispatch('user/getAll', null,  { root: true } );
                        } catch (e) {
                            return Promise.reject(e);
                        }
                    }

                    commit('getAllSuccess', response);
                    return Promise.resolve(response);
                },
                error => {
                    return Promise.reject(error);
                }
          );
      }
    },
  
    mutations: {
        getAllSuccess(state, response) {
            state.userLoadedTimestamp = Date.now();
            state.users = response.users;
        }
    }
  };