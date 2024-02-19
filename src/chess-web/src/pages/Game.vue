<script setup lang="ts">

import ChessBoard from "../components/ChessBoard.vue";
import {useGameStore} from "../game/GameStore.ts";
import {onMounted, ref} from "vue";
import {router} from "../router.ts";
import {storeToRefs} from "pinia";

const gameStore = useGameStore();
const { gameId, fen } = storeToRefs(gameStore);

const selectedSquare = ref<string>("");

async function onSquareSelected(name: string) {
  if (selectedSquare.value) {
    await gameStore.move(selectedSquare.value + name);
    selectedSquare.value = "";
    console.log(' after move: ', fen.value);
  }
  else {
    selectedSquare.value = name;
  }
}

onMounted(() => {
  if (!gameId.value) {
    router.push('/');
  }
});

</script>

<template>
  <ChessBoard :fen="fen" @square-selected="onSquareSelected" :selected-square="selectedSquare" />
  <div>{{ fen }}</div>
</template>

<style scoped>

</style>