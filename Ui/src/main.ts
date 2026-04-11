import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import PrimeVue from 'primevue/config'
import 'primeicons/primeicons.css'
import './style.css'
import App from './App.vue'
import Home from './views/Home.vue'
import Register from './views/Register.vue'
import Login from './views/Login.vue'
import CreateProject from './views/CreateProject.vue'
import ProjectDetail from './views/ProjectDetail.vue'
import CreateTicket from './views/CreateTicket.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'Home',
      component: Home
    },
    {
      path: '/register',
      name: 'Register',
      component: Register
    },
    {
      path: '/login',
      name: 'Login',
      component: Login
    },
    {
      path: '/projects/create',
      name: 'CreateProject',
      component: CreateProject
    },
    {
      path: '/projects/:projectShorthand',
      name: 'ProjectDetail',
      component: ProjectDetail
    },
    {
      path: '/projects/:projectShorthand/create-ticket',
      name: 'CreateTicket',
      component: CreateTicket
    }
  ]
})

createApp(App)
  .use(router)
  .use(PrimeVue, {
    unstyled: true
  })
  .mount('#app')
