import Vue from "vue";
import VueRouter from "vue-router";
import VueScrollTo from 'vue-scrollto';
import Home from "@/views/Home.vue";

Vue.use(VueRouter);

const routes = [
    {
        path: "/",
        name: "Home",
        component: Home,
        meta: {
            title: 'Agenda de Churras',
            requiresAuth: true
        }
    },
    {
        path: "/home",
        redirect: "/"
    },

    {
        path: "/signin",
        name: "SignIn",
        //Route level code-splitting: this generates a separate chunk (signin.[hash].js) for this route which is lazy-loaded when the route is visited.
        component: () => import(/* webpackChunkName: "SignIn" */ "@/views/SignIn.vue"),
        meta: {
            title: 'Agenda de Churras - Entre',
        }
    },
    {
        path: "/login",
        redirect: "/signin"
    },

    {
        path: "/signup",
        name: "SignUp",
        component: () => import(/* webpackChunkName: "SignUp" */ "@/views/SignUp.vue"),
        meta: {
            title: 'Agenda de Churras - Cadastre-se',
        }
    },

    {
        path: "/detail/:id?",
        name: "Detalhes",
        component: () => import(/* webpackChunkName: "Detail" */ "@/views/Detail.vue"),
        meta: {
            title: 'Agenda de Churras - Detalhe',
            requiresAuth: true
        }
    },

    {
        path: "/404",
        name: "404",
        component: () => import(/* webpackChunkName: "NotFound" */ "@/views/NotFound.vue"),
        meta: {
            title: 'Agenda de Churras - 404',
        }
    },
    {
        path: "*", //Everything else will result in a 404 page.
        redirect: "/404"
    }
];

const router = new VueRouter({
    mode: "history",
    base: process.env.BASE_URL,
    routes,

    scrollBehavior (to, from, savedPosition) {
        if (to.hash) {
            this.app.$scrollTo(to.hash, 700);
            return { selector: to.hash }
        } else if (savedPosition) {
            return savedPosition;
        } else {
            //When the route changes, the page should scroll back to the top.
            this.app.$scrollTo('#app', 700);
            return { x: 0, y: 0 }
        }
    }
});

router.beforeEach((to, from, next) => {
    const user = JSON.parse(localStorage.getItem('user'));
    const isLoggedIn = user != null && user != undefined && new Date(user.refreshTokenExpiryDateUtc) > Date.now();

    //When trying to access a restricted page and not logged in, redirect to login page
    if (to.meta.requiresAuth && !isLoggedIn) {
        next(to.fullPath && to.fullPath !== '/' ? { path: '/signin', query: { next: to.fullPath, }}: '/signin');
        return;
    }

    next();
  });

router.afterEach((to, from) => {
    //If it's the same page, use the scrollBehavior (unless it's on the home page).
    if (to.hash && (to.path != from.path || to.path == "/"))
        Vue.nextTick().then(() => VueScrollTo.scrollTo(to.hash, 700));
});

export default router;