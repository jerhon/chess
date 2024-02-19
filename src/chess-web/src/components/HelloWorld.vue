<script setup lang="ts">
import ChessBoard from "./ChessBoard.vue";


function squareSelected(squareName: string) {
  console.log(squareName);
}

</script>

<template>
  <ChessBoard fen="rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1" @square-selected="squareSelected" />
</template>

<style scoped>
</style>
