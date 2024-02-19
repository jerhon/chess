import { createApp } from 'vue'
import {createPinia} from "pinia";
import Home from "./pages/Home.vue";
import {createRouter, createWebHashHistory } from "vue-router";
import App from "./App.vue";
import './style.css'


const router = createRouter({
    history: createWebHashHistory(),
    routes: [
        { path: '/', component: Home },
    ],
});

createApp(App)
    .use(router)
    .use(createPinia())
    .mount('#app');
