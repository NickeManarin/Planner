<template>
    <div id="signin">
        <b-loading :is-full-page="true" :active.sync="isLoading" :can-cancel="false"/>

        <div class="columns is-centered is-mobile is-multiline">
            <div class="column is-12 has-max-width has-horizontal-margin">
                <b-field :label="$t('signin.email')" position="is-centered" :type="{ 'is-danger': emptyEmailError || accountMissingError }" :message="emailMessages">
                    <b-input ref="emailRef" type="text" maxlength="30" :placeholder="$t('signin.email-info')" v-model="userEmail" @keyup.enter.native="goToPassword()"/>
                </b-field>

                <b-field :label="$t('signin.password')" position="is-centered" :type="{ 'is-danger': passwordEmptyError || invalidPasswordError }" :message="passwordMessages">
                    <b-input ref="passwordRef" type="password" maxlength="30" :placeholder="$t('signin.password-info')" v-model="userPassword" @keyup.enter.native="login()"/>
                </b-field>

                <b-button class="is-dark is-medium is-expanded has-vertical-margin" @click="login()">
                    <p class="is-size-6">{{ $t('signin.enter') }}</p>
                </b-button>
            </div>

            <div class="column is-12 has-text-centered">
                <router-link class="has-text-dark is-expanded has-text-centered" to="/signup">
                    {{ $t('signin.no-account') }}
                </router-link>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'SignIn',

        data() {
            return {
                isLoading: false,
                emptyEmailError: false,
                accountMissingError: false,
                passwordEmptyError: false,
                invalidPasswordError: false,
                userEmail: '', //'alice@examplo.com',
                userPassword: ''//'abc@12345'
            }
        },

        mounted() {
            //Se a página foi carregada a partir de um redirecionamento do router, por não ter login, exibe a mensagem que o login é necessário.
            if (this.$route.query.next)
                this.$buefy.snackbar.open(this.$t('signin.login-again'));

            //Coloca foco no campo de e-mail ao carregar a página.
            this.$refs.emailRef.focus();
        },

        methods: {
            isEmpty(obj) {
                for(var i in obj) 
                    return false; 
    
                return true;
            },
            goToPassword (){
                this.$refs.passwordRef.focus();
            },
            async login() {
                this.emptyEmailError = false;
                this.accountMissingError = false;
                this.passwordEmptyError = false;
                this.invalidPasswordError = false;
                
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

                    var response = await this.$store.dispatch('auth/signin', { email: this.userEmail, password: this.userPassword });
                    
                    if (this.isEmpty(response)) {
                        throw new Error('Empty response.');                    
                    }

                    switch (response.status) {
                        case 100:
                            this.$buefy.snackbar.open(this.$t('signin.error.no-user'));
                            this.accountMissingError = true;
                            break;
                        case 101:
                            this.$buefy.snackbar.open(this.$t('signin.error.empty-credentials'));
                            this.emptyEmailError = true;
                            this.passwordEmptyError = true;
                            break;
                        case 102:
                            this.$buefy.snackbar.open(this.$t('signin.error.invalid-password'));
                            this.invalidPasswordError = true;
                            break;
                    }

                    if (response.code < 300 && response.accessToken)
                        this.$router.push(this.$route.query && this.$route.query.next ? this.$route.query.next : '/');
                } catch (e) {
                    this.$buefy.snackbar.open(this.$t('signin.error.generic'));
                    console.log('Signin error.', e);
                }
                finally{
                    this.isLoading = false;
                }
            },
        },

        computed: {
            emailMessages() {
                if (this.emptyEmailError)
                    return this.$t('signin.error.email-empty');

                if (this.accountMissingError)
                    return this.$t('signin.error.no-account');

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