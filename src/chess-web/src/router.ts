import {createRouter, createWebHashHistory} from "vue-router";
import Home from "./pages/Home.vue";
import Game from "./pages/Game.vue";

export const router = createRouter({
    history: createWebHashHistory(),
    routes: [
        { path: '/', component: Home },
        { path: '/game', component: Game }
    ],
});
