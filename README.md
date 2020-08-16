# Planejador de Churrascos (BBQ Planner)

Prática de desenvolvimento de frontend + backend.  

## Dados
  
O modelo de dados se encontra em *./Planner/Data/model.drawio* e pode ser visualizado em https://app.diagrams.net/.  
Existem três tabelas, User -> Participation <- Event, e elas recebem dados iniciais (seed) como teste.  
  
Alguns exemplos de login:  
(Admin) alice@exemplo.com   abc@12345  
(User)  beto@exemplo.com    abc@12345  
  
*Apenas usuários adminstradores podem alterar dados, como exemplo de autenticação por funções de usuário (roles).*  
*Todos os usuários possuem a mesma senha, para facilitar o teste.*  

## Detalhes do backend

C# + .Net Core 3.1.  
Controllers com services injetados.  
EFCore (Seed, Migrations).  
JWT (AccessToken, RefreshToken, Roles, Redis cache).  

## Detalhes do frontend

Vue 2.6.10  
Buefy 0.8.20  
Bulma 0.8.2  
Vue-router 3.3.4  
Vuex 3.5.1  
Axios 0.19.2  
Vue-i18n 8.21.0

Para testar o frontend, primeiro execute **npm install** na pasta *ClientApp*.  