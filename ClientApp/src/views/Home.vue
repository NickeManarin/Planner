<template>
    <div id="home">
        <div class="container">
            <b-loading :is-full-page="true" :active.sync="isLoading" :can-cancel="false"/>

            <div class="columns is-centered is-mobile is-multiline has-side-margin">
                <div v-for="event in events" :key="event.id" class="column is-half-mobile is-two-fifths-tablet is-one-third-desktop is-one-quarter-fullhd">
                    <b-button class="is-white is-padded has-shadow has-text-left" tag="router-link" :to="'/detail/' + event.id">
                        <p class="has-text-weight-bold">{{ event.dueTo | shortDate() }}</p>
                        <p class="has-text-weight-bold limits-with-ellipses">{{ event.name ? event.name : $t('home.no-reason') }}</p>
                        <br/>

                        <div class="level is-mobile">
                            <div class="level-left">
                                <p>
                                    <b-icon class="is-size-5 has-text-primary" icon="users-alt"/>
                                    <span class="has-text-weigth-medium">{{ event.eventsUsers ? event.eventsUsers.length : 0 }}</span>
                                </p>
                            </div>

                            <div class="level-right">
                                <p>
                                    <b-icon class="is-size-5 has-text-primary" icon="usd-circle"/>
                                    <span class="has-text-weigth-medium">R$ {{ event.eventsUsers ? event.eventsUsers.reduce((p, n) => p + n.contribution, 0) : 0}}</span>
                                </p>
                            </div>
                        </div>
                    </b-button>
                </div>

                <div v-if="canEdit" class="column is-half-mobile is-two-fifths-tablet is-one-third-desktop is-one-quarter-fullhd">
                    <b-button class="is-light is-padded has-shadow" tag="router-link" to="/detail">
                        <figure class="image">
                            <b-icon class="is-size-2 has-text-primary" icon="utensils"/>
                        </figure>

                        <p class="is-size-6 has-text-dark has-text-weight-bold">{{ $t('home.add') }}</p>
                    </b-button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'Home',

        data() {
            return {
                isLoading: true,
                canEdit: false,
                events: []
            }
        },

        async mounted() {
            try {
                this.canEdit = this.$store.state.auth.user.role === 'Admin';
                
                var response = await this.$store.dispatch('event/getAll');
                this.events = response.events;
            } catch (e) {
                console.log('Error in getting events', e);
                this.$router.push({ path: '/signin', query: { next: '/' } })
            }
            finally{
                this.isLoading = false;
            }
        },

        filters: {
            shortDate(dateString) {
                var date = new Date(dateString);

                return (date.getDate() < 10 ? "0" : '') + date.getDate() + '/' + (date.getMonth() + 1 < 10 ? "0" : '') + (date.getMonth() + 1);
            },
        }
    }
</script>

<style lang="scss" scoped>
    .has-side-margin {
        margin: 10px 0;
    }

    .columns {
        margin-top: -60px;
    }

    .has-shadow {
        border-radius: 2px;
        box-shadow: 0px 0px 16px rgba(0, 0, 0, 0.06);
    }

    .column .is-padded {
        padding: 0.75rem;
    }

    .column .button {
        white-space: normal;
        border-radius: 2px;
        width: 100%;
        height: 100%;
    }

    .limits-with-ellipses {
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    }
</style>

<style lang="scss">
    .button > span {
        width: 100%;
        padding: 0px 5px;
    }
</style>