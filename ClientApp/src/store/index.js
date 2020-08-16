import Vue from 'vue';
import Vuex from 'vuex';

import { auth } from './auth.module';
import { event } from './event.module';
import { user } from './user.module';
import { part } from './participation.module';

Vue.use(Vuex);

export default new Vuex.Store({
  modules: {
    auth,
    event,
    user,
    part
  }
});