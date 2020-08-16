<template>
    <div id="detail">
        <div class="container">
            <b-loading :is-full-page="true" :active.sync="isLoading" :can-cancel="false"/>

            <div class="columns is-centered is-mobile has-side-margin has-negative-top-margin">
                <div class="column is-three-quarters-tablet is-four-fifths-desktop is-four-fifths-fullhd">
                    <div class="box has-shadow">
                        <!-- Show -->
                        <div v-if="!isEditting && !isLoading">
                            <!-- Header -->
                            <div class="level is-mobile">
                                <div class="level-left is-inline">
                                    <p class="has-text-weight-bold">{{ event != null ? (shortDate(event.dueTo)) : ''}}</p>
                                    <p class="is-size-4 has-text-weight-bold limits-with-ellipses">{{ event && event.name ? event.name : $t('home.no-reason') }}</p>
                                </div>

                                <div class="level-right is-inline">
                                    <p>
                                        <b-icon class="is-size-5 has-text-primary" icon="users-alt"/>
                                        <span class="has-text-weight-bold">{{ event && event.eventsUsers ? event.eventsUsers.length : 0 }}</span>
                                    </p>
                                    <p>
                                        <b-icon class="is-size-5 has-text-primary" icon="usd-circle"/>
                                        <span class="has-text-weight-bold">R$ {{ event && event.eventsUsers ? event.eventsUsers.reduce((p, n) => p + n.contribution, 0) : 0}}</span>
                                    </p>
                                </div>
                            </div>
                            
                            <!-- List of participants -->
                            <div v-if="event && event.eventsUsers && event.eventsUsers.length > 0">
                                <div v-for="part in event.eventsUsers" :key="part.id" class="level is-mobile has-bottom-border has-vertical-margin">
                                    <div class="level-left" :class="{ 'has-pointer': canEdit }" @click="togglePayment(part)">
                                        <div class="is-circle" :class="{ 'is-filled': part.hasPaid }"/>

                                        <p class="has-text-weight-bold">{{ part.user.name }}</p>
                                    </div>

                                    <div class="level-right" :class="{ 'has-line-through': part.hasPaid }">
                                        <p class="has-text-weight-bold">{{ part.contribution | feeConversion('R$') }}</p>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Edit -->
                        <div v-if="isEditting">
                            <p :v-if="isEditting" class="title">{{ isNew ? $t('detail.new-title') : $t('detail.edit-title') }}</p>

                            <b-field grouped>
                                <b-field :label="$t('detail.date')" position="is-centered" :type="{ 'is-danger': dateEmptyError || dateInvalidError }" 
                                    :message="[{ 'Escolha a data do churras.': dateEmptyError }, { 'Data inválida.': dateInvalidError }]">
                                    <b-datepicker class="is-dark" ref="dateRef" expanded v-model="eventEdit.dueToConverted" :focused-date="new Date()"
                                        icon="calendar-alt" :placeholder="$t('detail.date-info')" :min-date="new Date()" :years-range="[0, 5]"/>
                                </b-field>

                                <b-field :label="$t('detail.name')" position="is-centered" expanded :type="{ 'is-danger': nameEmptyError }" 
                                    :message="[{ 'Escolha o nome do churras.': nameEmptyError }]">
                                    <b-input type="text" maxlength="30" :placeholder="$t('detail.name-info')" v-model="eventEdit.name"/>
                                </b-field>
                            </b-field>

                            <b-field grouped>
                                <b-field :label="$t('detail.suggested')" position="is-centered" :type="{ 'is-danger': valueEmptyError || valueInvalidError }" 
                                    :message="[{ 'Defina o valor sugerido da contribuição.': valueEmptyError }, 
                                    { 'Valor inválido (maior que o outro).': valueInvalidError }]">
                                    <!-- <b-input v-model="eventEdit.suggestedValue" v-currency="{currency: 'BRL', locale: 'pt-BR'}"/> -->
                                    <p class="control">
                                        <currency-input class="input has-width-smaller" v-model="eventEdit.suggestedValue" v-currency="{currency: 'BRL', locale: 'pt-BR'}"/>
                                    </p>
                                </b-field>

                                <b-field :label="$t('detail.suggested2')" position="is-centered" :type="{ 'is-danger': valueDrinkEmptyError || valueDrinkInvalidError }" 
                                    :message="[{ 'Defina o valor sugerido da contribuição.': valueDrinkEmptyError }, 
                                    { 'Valor inválido (menor que o outro).': valueDrinkInvalidError }]">
                                    
                                    <!-- <b-input v-model="eventEdit.suggestedValueWithDrinks" v-currency="{currency: 'BRL', locale: 'pt-BR'}"/> -->
                                    <p class="control">
                                        <currency-input class="input has-width-smaller" v-model="eventEdit.suggestedValueWithDrinks" v-currency="{currency: 'BRL', locale: 'pt-BR'}"/>
                                    </p>
                                </b-field>

                                <b-field class="is-hidden-touch" :label="$t('detail.observation')" position="is-centered" expanded>
                                    <b-input type="text" maxlength="100" :placeholder="$t('detail.observation-info')" v-model="eventEdit.observation"/>
                                </b-field>
                            </b-field>

                            <!-- {{ eventEdit.eventsUsers.length.toLocaleString('pt-BR') }} -->
                            <b-field class="is-hidden-desktop" :label="$t('detail.observation')" position="is-centered" expanded>
                                <b-input type="text" maxlength="100" :placeholder="$t('detail.observation-info')" v-model="eventEdit.observation"/>
                            </b-field>

                            <!-- List of participants -->
                            <p class="has-text-left has-text-weight-bold has-margin-5">
                                {{ $t('detail.list.title') }}
                            </p>

                            <div>
                                <b-table ref="tableEdit" class="has-margin-table" v-if="eventEdit" :data="eventEdit.eventsUsers" hoverable
                                    :default-sort-direction="defaultSortOrder" :default-sort="[sortField, sortOrder]">
                                    <template slot-scope="props">
                                        <b-table-column class="is-unselectable" width="70" cell-class="is-vertical" field="hasPaid" :label="$t('detail.list.paid')">
                                            <b-checkbox class="has-check-centered is-aligned-bottom" v-model="props.row.hasPaid"/>
                                        </b-table-column>

                                        <b-table-column class="is-unselectable" cell-class="is-vertical" field="user.name" :label="$t('detail.list.participant')">
                                            <strong>{{props.row.user.name}}</strong>
                                        </b-table-column>

                                        <b-table-column class="is-unselectable" cell-class="is-vertical" field="observation" :label="$t('detail.list.observation')">
                                            {{props.row.observation}}
                                        </b-table-column>

                                        <b-table-column class="is-unselectable" width="90" cell-class="is-vertical" field="contribution" :label="$t('detail.list.contribution')">
                                            {{props.row.contribution | feeConversion('R$')}}
                                        </b-table-column>

                                        <b-table-column class="is-unselectable" width="45" cell-class="has-pointer">
                                            <b-button class="button is-smaller" type="is-dark" icon-left="trash" @click="removeParticipant(props.row)"/>
                                        </b-table-column>
                                    </template>

                                    <template slot="empty">
                                        <div class="content has-text-grey has-text-centered has-bottom-margin">
                                            <p>{{ $t('detail.list.none') }}</p>
                                        </div>
                                    </template>
                                </b-table>

                                <div>
                                    <b-field grouped group-multiline>
                                        <b-field :label="$t('detail.list.participant')" position="is-centered" expanded :type="{ 'is-danger': partEmptyError || partInvalidError }" 
                                                :message="[{ 'Selecione o/a participante.': partEmptyError }, { 'O/a participante já está na lista.': partInvalidError }]">
                                            <b-autocomplete ref="newParticipantRef" v-model="newParticipantText" placeholder="Nome do participante" 
                                                :open-on-focus="openOnFocus" :keep-first="true" :custom-formatter="participantFormatter()"
                                                :data="filteredParticipants" @select="option => newParticipant = option" 
                                                dropdown-position="bottom" autoComplete="new-password">

                                                <template slot-scope="props">
                                                    <p>
                                                        <span class="has-text-semibold">{{ props.option.id }}</span>
                                                        <span> - </span>
                                                        <span>{{ props.option.name }}</span>
                                                    </p>
                                                </template>

                                                <template slot="empty">{{ $t('detail.list.no-result') }} {{ newParticipantText }}</template>
                                            </b-autocomplete>
                                        </b-field>

                                        <b-field :label="$t('detail.list.contribution')" :type="{ 'is-danger': contribEmptyError }" :message="[{ 'Escolha o valor da contribuição.': contribEmptyError }]">
                                            <b-field>
                                                <p class="control">
                                                    <b-button @click="withoutDrink()">{{ $t('detail.list.without-drink') }}</b-button>
                                                </p>
                                                <p class="control">
                                                    <b-button @click="withDrink()">{{ $t('detail.list.with-drink') }}</b-button>
                                                </p>

                                                <!-- <b-input type="numeric" v-model="newParticipantContribution" v-currency="{currency: 'BRL', locale: 'pt-BR'}"/> -->
                                                <p class="control">
                                                    <currency-input class="input has-width-smaller" v-model="newParticipantContribution" v-currency="{currency: 'BRL', locale: 'pt-BR'}"/>
                                                </p>
                                            </b-field>
                                        </b-field>
                                    </b-field>

                                    <b-field grouped group-multiline>
                                        <b-field :label="$t('detail.list.observation')" position="is-centered" expanded>
                                            <b-input type="text" maxlength="100" :placeholder="$t('detail.list.observation-info')" v-model="newParticipantObservation"/>
                                        </b-field>

                                        <b-field :label="$t('detail.list.paid')" position="is-centered">
                                            <b-checkbox class="is-aligned-bottom" v-model="newParticipantHasPaid">{{ $t('detail.list.yes') }}</b-checkbox>
                                        </b-field>

                                        <b-button class="has-margin-as-label" type="is-dark" icon-left="plus" @click="addParticipant()">{{ $t('detail.list.add') }}</b-button>
                                    </b-field>
                                </div>
                            </div>
                        </div>

                        <hr/>

                        <div class="level is-mobile has-top-margin">
                            <div class="level-left">
                                <!-- Leave in blank to make the right side stick to the side -->
                            </div>

                            <div class="level level-right">
                                <div v-if="!isEditting && canEdit" class="level-item">
                                    <b-button type="is-dark" icon-left="trash" @click="remove()">
                                        {{ $t('detail.remove') }}
                                    </b-button>
                                </div>

                                <div v-if="!isEditting && canEdit" class="level-item">
                                    <b-button type="is-dark" icon-left="pen" @click="edit()">
                                        {{ $t('detail.edit') }}
                                    </b-button>
                                </div>

                                <div v-if="isEditting && canEdit" class="level-item">
                                    <b-button type="is-dark" icon-left="save" @click="save()">
                                        {{ $t('detail.save') }}
                                    </b-button>
                                </div>

                                <div v-if="isEditting && canEdit" class="level-item">
                                    <b-button type="is-dark" icon-left="times" @click="cancel()">
                                        {{ $t('detail.cancel') }}
                                    </b-button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'Detalhe',

        data() {
            return {
                isLoading: true,
                isNew: false,
                isEditting: false,
                users: [],
                event: {},
                eventEdit: {
                    dueToConverted: new Date()
                },

                dateEmptyError: false,
                dateInvalidError: false,
                nameEmptyError: false,
                valueEmptyError: false,
                valueInvalidError: false,
                valueDrinkEmptyError: false,
                valueDrinkInvalidError: false,
                
                partEmptyError: false,
                partInvalidError: false,
                contribEmptyError: false,

                sortField: "properties.user.name",
                sortOrder: "asc",
                defaultSortOrder: "asc",

                newParticipantText: '',
                newParticipant: {},
                newParticipantHasPaid: false,
                newParticipantContribution: 0,
                newParticipantObservation: '',
            }
        },

        async created() {
            try {
                //When accesing this page without an ID set, treat as if the user wants to add a new event.
                this.isNew = this.$route.params.id === undefined;

                if (this.isNew) {
                    await this.edit();           
                    return;
                }
                
                if (this.$store.state.event.events == undefined) {
                    await this.$store.dispatch('event/getAll');
                }

                //Get event from store.
                this.event = this.$store.state.event.events.filter(e => { return e.id == this.$route.params.id })[0];
            } catch (e) {
                console.log('Impossible to load', e);
                this.$buefy.snackbar.open('Não foi possível carregar o evento.');
            }
            finally{
                this.isLoading = false;
            }
        },

        methods: {
            isEmpty(obj) {
                for(var i in obj) 
                    return false; 
    
                return true;
            },
            shortDate(dateString) {
                var date = new Date(dateString);

                return (date.getDate() < 10 ? "0" : '') + date.getDate() + '/' + (date.getMonth() + 1 < 10 ? "0" : '') + (date.getMonth() + 1);
            },
            participantFormatter() {
                return (e) => { return e.id + " - " + e.name; };
            },

            async togglePayment(part) {
                if (!this.canEdit) {
                    return;
                }

                this.isLoading = true;
                var previous = part.hasPaid;

                try {
                    part.hasPaid = !part.hasPaid;
                    var resp = await this.$store.dispatch('part/edit', part);

                    if (resp.code == 113) {
                        throw new Error('Not possible to toggle payment.');
                    }
                } catch (e) {
                    console.log('Error when trying to toggle payment', e);
                    part.hasPaid = previous;
                    this.$buefy.snackbar.open('Não foi possível alterar a situação.');
                }
                finally {
                    this.isLoading = false;
                }
            },
            withoutDrink() {
                this.newParticipantContribution = this.eventEdit.suggestedValue;
            },
            withDrink() {
                this.newParticipantContribution = this.eventEdit.suggestedValueWithDrinks;
            },
            async remove() {
                if (!this.canEdit) {
                    return;
                }

                this.isLoading = true;

                try {
                    var resp = await this.$store.dispatch('event/remove', this.event);

                    if (resp.code == 111)
                        throw new Error('Not possible to remove.');

                    this.$router.push('/');
                } catch (e) {
                    console.log('Impossible to remove event.', e);
                    this.$buefy.snackbar.open('Não foi possível remover o evento.');
                }
                finally {
                    this.isLoading = false;
                }
            },
            async edit() {
                try {
                    this.isLoading = true;

                    if (!this.isEmpty(this.event)){
                        this.eventEdit = JSON.parse(JSON.stringify(this.event));
                        this.eventEdit.dueToConverted = this.eventEdit.dueTo ? new Date(this.eventEdit.dueTo) : null;
                    }
                    else {
                        this.eventEdit = {
                            eventsUsers: []
                        };
                    }
                    
                    this.users = (await this.$store.dispatch('user/getAll')).users;
                    this.isEditting = true;

                    this.$nextTick().then(t => {
                        t.$refs.dateRef.focus();
                    });
                } catch (e) {
                    console.log('Not possible to edit.', e);
                    this.$buefy.snackbar.open('Não foi possível ativar o modo de edição.');
                }
                finally {
                    this.isLoading = false;
                }
            },
            addParticipant() {
                this.partEmptyError = false;
                this.partInvalidError = false;
                this.contribEmptyError = false;

                //Validations.
                if (this.isEmpty(this.newParticipant)) {
                    this.partEmptyError = true;
                    return;
                }

                if (this.eventEdit.eventsUsers.filter(f => f.userId == this.newParticipant.id).length > 0) {
                    this.partInvalidError = true;
                    return;
                }

                if (this.newParticipantContribution < 0.01) {
                    this.contribEmptyError = true;
                    return;
                }

                //Add the selected participant to the edit object.
                this.eventEdit.eventsUsers.push({
                    user: this.newParticipant,
                    userId: this.newParticipant.id,
                    hasPaid: this.newParticipantHasPaid,
                    contribution: this.newParticipantContribution,
                    observation: this.newParticipantObservation
                });

                //Clean the properties and set focus.
                this.newParticipantText = '';
                this.newParticipant = {};
                this.newParticipantHasPaid = false;
                this.newParticipantContribution = 0;
                this.newParticipantObservation = '';
                this.$refs.newParticipantRef.focus();
            },
            removeParticipant(row, confirm) {
                if (confirm == null){
                    this.$buefy.dialog.confirm({
                        title: 'Remoção de participante',
                        message: 'Você deseja <b>remover</b> o/a participante <i>' + row.user.name + '</i> do churras?',
                        confirmText: 'Sim',
                        cancelText: 'Não',
                        type: 'is-danger',
                        hasIcon: true,
                        onConfirm: () => this.removeParticipant(row, true)
                    });
                    return;
                }

                this.eventEdit.eventsUsers = this.eventEdit.eventsUsers.filter(item => item.userId !== row.userId || item.eventId !== row.eventId);
            },
            async save() {
                this.dateEmptyError = false;
                this.dateInvalidError = false;
                this.nameEmptyError = false;
                this.valueEmptyError = false;
                this.valueInvalidError = false;
                this.valueDrinkEmptyError = false;
                this.valueDrinkInvalidError = false;

                //Validations.
                if (this.eventEdit.dueToConverted == null) {
                    this.dateEmptyError = true;
                    return;
                }

                if (this.eventEdit.dueToConverted < Date.now()) {
                    this.dateInvalidError = true;
                    return;
                }

                // if (this.eventEdit.name == null) {
                //     this.nameEmptyError = true;
                //     return;
                // }

                if (this.eventEdit.suggestedValue == null || this.eventEdit.suggestedValue < 0.01) {
                    this.valueEmptyError = true;
                    return;
                }
                
                if (this.eventEdit.suggestedValueWithDrinks == null || this.eventEdit.suggestedValueWithDrinks < 0.01) {
                    this.valueDrinkEmptyError = true;
                    return;
                }

                if (this.eventEdit.suggestedValueWithDrinks < this.eventEdit.suggestedValue) {
                    this.valueInvalidError = true;
                    this.valueDrinkInvalidError = true;
                    return;
                }

                if (this.eventEdit.eventsUsers == null || this.eventEdit.eventsUsers.length == 0) {
                    this.$buefy.snackbar.open('É necessário adicionar participantes.');
                    return;
                }

                this.isLoading = true;
                this.eventEdit.dueTo = this.eventEdit.dueToConverted;

                if (this.isNew) {
                    try {
                        var resp = await this.$store.dispatch('event/create', this.eventEdit);

                        console.log(resp);
                        this.$router.push('/');
                    } catch (e) {
                        console.log('Impossible to create event', e);
                    }
                    finally {
                        this.isLoading = false;
                    }
                    return;
                }

                //Send as edit.
                try {
                    var resp2 = await this.$store.dispatch('event/edit', this.eventEdit);
                    console.log(resp2);

                    if (resp2.code == 109)
                        throw new Error('Generic error in request');

                    this.event = JSON.parse(JSON.stringify(this.eventEdit));
                    this.isEditting = false;
                } catch (e) {
                    console.log('Impossible to edit event', e);
                }
                finally {
                    this.isLoading = false;
                }
            },
            cancel() {
                if (this.isNew) {
                    this.$router.push('/');
                    return;
                }

                this.isEditting = false;
                this.eventEdit = {};
            }
        },

        computed: {
            canEdit() {
                return this.$store.state.auth.user != null && this.$store.state.auth.user.role === 'Admin';
            },

            openOnFocus() {
                //If no participant was selected yet or if the text is not equal to the selection, show the result.
                return this.isEmpty(this.newParticipant) || this.newParticipant == null || this.newParticipantText !== (this.newParticipant.id + " - " + this.newParticipant.name);
            },
            filteredParticipants() {
                let text = this.newParticipantText.toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '');

                return this.users ? this.users.filter(user => {
                    return (user.id + " - " + user.name).toLowerCase().normalize('NFD').replace(/[\u0300-\u036f]/g, '').indexOf(text) >= 0;
                }).slice(0, 20) : [];
            },
        },

        filters: {
            feeConversion(value, currency){
                return currency + ' ' + value.toLocaleString(navigator.language, { minimumFractionDigits: 2, maximumFractionDigits: 2 })
            }
        }
    }
</script>

<style lang="scss" scoped>
    .has-side-margin {
        margin-left: 10px;
        margin-right: 10px;
    }

    .has-negative-top-margin {
        margin-top: -60px;
    }

    .has-shadow {
        border-radius: 2px;
        box-shadow: 0px 0px 16px rgba(0, 0, 0, 0.06);
    }

    .column .is-padded {
        padding: 0.75rem;
    }

    .has-top-margin {
        margin-top: 1rem;
    }

    .has-bottom-margin {
        margin-bottom: 1rem;
    }

    .has-vertical-margin {
        //margin: 0.75rem 0;
        margin: 0;
        padding: 0.5rem 0 0.5rem 0;  
    }

    .is-circle {
        width: 20px;
        height: 20px;
        border-width: 2px;
        border-color: #998220;
        border-radius: 10px;
        border-style: solid;
        margin-right: 1rem;
    }

    .is-filled {
        border-color: #FFD836;
        background-color: #FFD836;
    }

    .has-bottom-border {
        border-color: #FFD836;
        border-style: solid;
        border-width: 0 0 1px 0;
    }

    .has-pointer {
        cursor: pointer; 
    }

    .has-line-through {
        text-decoration-line: line-through;
    }

    .is-vertical {
        vertical-align: middle;
    }

    .has-margin-as-label {
        margin-top: 1.75rem;
    }

    .is-aligned-bottom {
        vertical-align: bottom;
    }
</style>