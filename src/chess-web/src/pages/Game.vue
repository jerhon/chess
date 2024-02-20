<script setup lang="ts">

import ChessBoard, {Square} from "../components/ChessBoard.vue";
import {useGameStore} from "../game/GameStore.ts";
import {onMounted, ref} from "vue";
import {router} from "../router.ts";
import {storeToRefs} from "pinia";

const gameStore = useGameStore();
const { gameId, fen } = storeToRefs(gameStore);

const candidateSquares = ref<string[]>([]);
const selectedSquare = ref<Square>();

async function onSquareSelected(square: Square) {
  console.log('square selected: ', square);
  if (square.piece && selectedSquare.value) {
    if (candidateSquares.value.includes(square.name)) {
      await gameStore.move(selectedSquare.value.name + square.name);
      selectedSquare.value = undefined;
      candidateSquares.value = [];
      return;
    }
  }

  selectedSquare.value = square;
  candidateSquares.value = await gameStore.getCandidateMoves(selectedSquare.value.name);
}

onMounted(() => {
  if (!gameId.value) {
    router.push('/');
  }
});

</script>

<template>
  <ChessBoard :fen="fen" @square-selected="onSquareSelected" :selected-square="selectedSquare?.name" :candidate-squares="candidateSquares" />

  <div>{{ fen }}</div>
</template>

<style scoped>

</style>