<template>
    <div id="signin">
        <b-loading :is-full-page="true" :active.sync="isLoading" :can-cancel="false"/>

        <div class="columns is-centered is-mobile is-multiline">
            <div class="column is-12 has-max-width has-horizontal-margin">
                <b-field :label="$t('signup.name')" position="is-centered" :type="{ 'is-danger': emptyNameError }" :message="nameMessages">
                    <b-input ref="nameRef" type="text" maxlength="30" :placeholder="$t('signup.name-info')" v-model="userName" @keyup.enter.native="goToEmail()"/>
                </b-field>

                <b-field :label="$t('signin.email')" position="is-centered" :type="{ 'is-danger': emptyEmailError || accountExistsError }" :message="emailMessages">
                    <b-input ref="emailRef" type="text" maxlength="30" :placeholder="$t('signin.email-info')" v-model="userEmail" @keyup.enter.native="goToPassword()"/>
                </b-field>

                <b-field :label="$t('signin.password')" position="is-centered" :type="{ 'is-danger': passwordEmptyError || invalidPasswordError }" :message="passwordMessages">
                    <b-input ref="passwordRef" type="password" maxlength="30" :placeholder="$t('signin.password-info')" v-model="userPassword" @keyup.enter.native="login()"/>
                </b-field>

                <b-button class="is-dark is-medium is-expanded has-vertical-margin" @click="login()">
                    <p class="is-size-6">{{ $t('signup.enter') }}</p>
                </b-button>
            </div>

            <div class="column is-12 has-text-centered">
                <router-link class="has-text-dark is-expanded has-text-centered" to="/signin">
                    {{ $t('signup.account') }}
                </router-link>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'SignUp',

        data() {
            return {
                isLoading: false,
                emptyNameError: false,
                emptyEmailError: false,
                accountExistsError: false,
                passwordEmptyError: false,
                invalidPasswordError: false,
                userName: '', //'Alice B',
                userEmail: '', //'alice.b@exemplo.com',
                userPassword: '' //'abc@12345'
            }
        },

        mounted() {
            //Coloca foco no campo de nome ao carregar a página.
            this.$refs.nameRef.focus();
        },

        methods: {
            isEmpty(obj) {
                for(var i in obj) 
                    return false; 
    
                return true;
            },
            goToEmail() {
                this.$refs.emailRef.focus();
            },
            goToPassword (){
                this.$refs.passwordRef.focus();
            },
            async login() {
                this.emptyNameError = false;
                this.emptyEmailError = false;
                this.invalidEmailError = false;
                this.accountExistsError = false;
                this.passwordEmptyError = false;
                
                if (this.userName === null || this.userName === '') {
                    this.emptyNameError = true;
                    return;
                }

                if (this.userEmail === null || this.userEmail === '') {
                    this.emptyEmailError = true;
                    return;
                }

                if (this.userPassword === null || this.userPassword === '') {
                    this.passwordEmptyError = true;
                    return;
                }

                try {
                    this.isLoading = true;

                    var response = await this.$store.dispatch('auth/signup', { name: this.userName, email: this.userEmail, password: this.userPassword });
                    
                    if (this.isEmpty(response)) {
                        throw new Error('Empty response.');                    
                    }

                    switch (response.status) {
                        case 107:
                            this.$buefy.snackbar.open(this.$t('signup.error.invalid-email'));
                            this.invalidEmailError = true;
                            break;
                        case 108:
                            this.$buefy.snackbar.open(this.$t('signup.error.already-in-use'));
                            this.accountExistsError = true;
                            break;
                        case 104:
                            this.$buefy.snackbar.open(this.$t('signup.error.invalid-password'));
                            this.invalidPasswordError = true;
                            break;
                    }

                    if (response.code < 300 && response.accessToken)
                        this.$router.push(this.$route.query && this.$route.query.next ? this.$route.query.next : '/');
                } catch (e) {
                    this.$buefy.snackbar.open(this.$t('signup.error.generic'));
                    console.log('Signup error.', e);
                }
                finally{
                    this.isLoading = false;
                }
            },
        },

        computed: {
            nameMessages() {
                if (this.emptyNameError)
                    return this.$t('signup.error.name-empty');

                return null;
            },

            emailMessages() {
                if (this.emptyEmailError)
                    return this.$t('signin.error.email-empty');

                if (this.accountExistsError)
                    return this.$t('signup.error.account');

                return null;
            },

            passwordMessages() {
                if (this.passwordEmptyError)
                    return this.$t('signin.error.password-empty');

                if (this.invalidPasswordError)
                    return this.$t('signin.error.invalid-password');

                return null;
            },
        },
    }
</script>

<style lang="scss" scoped>
    .has-max-width {
        max-width: 400px;
    }

    .has-horizontal-margin {
        margin: 0 10px;
    }

    .is-expanded {
        width: 100%;
    }

    .has-vertical-margin {
        margin: 20px 0;
    }
</style>

<style lang="scss">
    p.help.is-danger {
        font-weight: 800;
        color: rgb(163, 1, 1);
    }
</style>