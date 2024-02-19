<script setup lang="ts">

import Button from "../components/Button.vue";
import {useGameStore} from "../game/GameStore.ts";
import {router} from "../router.ts";


const gameService = useGameStore();

async function newGame() {

  const result = await gameService.newGame();
  if (result) {
    await router.push('/game');
  }
}

</script>

<template>
  <div>chess</div>
  <Button text="New Game" @click="newGame()" />
</template>

<style scoped>

</style>