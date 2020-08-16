import EventService from '../services/event.service';

export const event = {
    namespaced: true,
    state: {},

    actions: {
        getAll({ commit, dispatch }) {
            return EventService.getAll().then(
                async response => {
                    if (response.status === 401) {
                        try {
                            var resp = await dispatch('auth/refresh', null, { root: true });

                            console.log('Result of refresh', resp);

                            if (resp !== undefined)
                                return await dispatch('event/getAll', null, { root: true });
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
        },

        create({ commit, dispatch }, event) {
            return EventService.create(event).then(
                async response => {
                    if (response.status === 401) {
                        try {
                            var resp = await dispatch('auth/refresh', null, { root: true });

                            console.log('Result of refresh', resp);

                            if (resp !== undefined)
                                return await dispatch('event/create', null, { root: true });
                        } catch (e) {
                            return Promise.reject(e);
                        }
                    }

                    commit('createSuccess', response);
                    return Promise.resolve(response);
                },
                error => {
                    commit('createFailure');
                    return Promise.reject(error);
                }
            );
        },

        edit({ commit, dispatch }, event) {
            return EventService.edit(event).then(
                async response => {
                    if (response.status === 401) {
                        try {
                            var resp = await dispatch('auth/refresh', null, { root: true });

                            console.log('Result of refresh', resp);

                            if (resp !== undefined)
                                return await dispatch('event/edit', null, { root: true });
                        } catch (e) {
                            return Promise.reject(e);
                        }
                    }

                    commit('editSuccess', response);
                    return Promise.resolve(response);
                },
                error => {
                    commit('editFailure');
                    return Promise.reject(error);
                }
            );
        },

        remove({ commit, dispatch }, event) {
            return EventService.remove(event).then(
                async response => {
                    if (response.status === 401) {
                        try {
                            var resp = await dispatch('auth/refresh', null, { root: true });

                            console.log('Result of refresh', resp);

                            if (resp !== undefined)
                                return await dispatch('event/remove', null, { root: true });
                        } catch (e) {
                            return Promise.reject(e);
                        }
                    }

                    commit('removeSuccess', response);
                    return Promise.resolve(response);
                },
                error => {
                    commit('removeFailure');
                    return Promise.reject(error);
                }
            );
        }
    },

    mutations: {
        getAllSuccess(state, response) {
            state.eventLoadedTimestamp = Date.now();
            state.events = response.events;
        },

        createSuccess(state, response) {
            state.eventLoadedTimestamp = Date.now();
            state.events = response.events;
        },
        createFailure(state) {
            console.log(state);
        },

        editSuccess(state, response) {
            state.eventLoadedTimestamp = Date.now();
            state.events = response.events;
        },

        editFailure(state) {
            console.log(state);
        },

        removeSuccess(state, response) {
            state.eventLoadedTimestamp = Date.now();
            state.events = response.events;
        },

        removeFailure(state) {
            console.log(state);
        },
    }
};