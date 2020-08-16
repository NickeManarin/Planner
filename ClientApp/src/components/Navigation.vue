<template>
    <nav class="hero is-almost-medium">
        <div class="hero-body has-text-centered">
            <div class="container">
                <b-loading :is-full-page="true" :active.sync="isLoading" :can-cancel="false"/>

                <div class="columns is-mobile is-centered">
                    <div class="column is-narrow">
                        <b-tooltip :label="$t('nav.back')" type="is-dark" position="is-bottom" animated>
                            <b-button v-if="$route.path !== '/signin' && $route.path !== '/signup' && $route.path !== '/'" size="is-large" 
                                type="is-transparent" icon-right="arrow-left" @click="back()"/>
                        </b-tooltip>
                    </div>

                    <div class="column is-narrow">
                        <p class="has-text-weight-heavy title">{{ $t('nav.title') }}</p>
                    </div>

                    <div class="column is-narrow">
                        <b-tooltip :label="$t('nav.logout')"  type="is-dark" position="is-bottom" animated>
                            <b-button v-if="currentUser && $route.path !== '/signin' && $route.path !== '/signup'" size="is-large" 
                                type="is-transparent" icon-right="sign-out-alt" @click="logout()"/>
                        </b-tooltip>
                    </div>
                </div>
            </div>
        </div>
    </nav>
</template>

<script>
    export default { 
        name: 'Navigation',

        data() {
            return {
                isLoading: false
            }
        },

        computed: {
            currentUser() {
                return this.$store.state.auth.user;
            }
        },

        methods: {
            back() {
                this.$router.push('/');
            },

            async logout() {
                try {
                    this.isLoading = true;
                    await this.$store.dispatch('auth/logout');
                } catch (e) {
                    console.log('Error while trying to logout', e);
                    this.$buefy.snackbar.open(this.$t('nav.error.logout'));
                }
                finally {
                    this.$router.push('/signin');
                    this.isLoading = false;
                }
            }
        }
    }
</script>

<style lang="scss" scoped>
    @media screen and (max-width: 769px) {
        .title {
            font-size: 20pt !important;
        }
    }

    .hero.is-almost-medium .hero-body {
        padding-top: 70px;
        padding-bottom: 95px;
    }

    .has-text-weight-heavy {
        font-weight: 800;
        font-size: 32pt;
    }
</style>