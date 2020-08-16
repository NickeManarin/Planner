import ParticipationService from '../services/participation.service';

export const part = {
    namespaced: true,
    state: { },

    actions: {       
      edit({dispatch}, part) {
            return ParticipationService.edit(part).then(
                async response => {
                    if (response.status === 401) {
                        try {
                            var resp = await dispatch('auth/refresh', null, { root: true } );

                            console.log('Result of refresh', resp);

                            if (resp !== undefined)
                                return await dispatch('event/getAll', null,  { root: true } );
                        } catch (e) {
                            return Promise.reject(e);
                        }
                    }

                    return Promise.resolve(response);
                },
                error => {
                    return Promise.reject(error);
                }
            );
        }
    }
  };