import {defineStore} from "pinia";
import {ref} from "vue";


defineStore('GameStore', () => {
    const gameId = ref<string>('');
    const error = ref<string>('');

    function newGame()  {

    }

});
