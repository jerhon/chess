<script setup lang="ts">

import ChessBoard, { Squares }  from "../components/ChessBoard.vue";
import {useGameStore} from "../game/GameStore.ts";
import {computed, onMounted } from "vue";
import {router} from "../router.ts";
import {storeToRefs} from "pinia";

const gameStore = useGameStore();
const { gameId, fen, selectedSquare, candidateMoves, squares } = storeToRefs(gameStore);


const chessBoardSquares = computed(() => {

  var gameSquares : Squares = {};
  for (let key in squares.value) {
    const square = squares.value[key];
    gameSquares[key] = {
      name: key,
      piece: square.piece,
      highlight: square.name == selectedSquare.value,
      symbol: candidateMoves.value.includes(key) ? 'dot' : undefined
    }
  }
  return gameSquares;

})

onMounted(() => {
  if (!gameId.value) {
    router.push('/');
  }
});

</script>

<template>
  <ChessBoard  @square-clicked="gameStore.setSelectedSquare" :squares="chessBoardSquares" />

  <div>{{ fen }}</div>
</template>

<style scoped>

</style>