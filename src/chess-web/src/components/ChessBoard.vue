<script setup lang="ts">

import {computed, ref} from "vue";
import clsx from "clsx";

interface Props {
  fen: string,
  selectedSquare?: string,
  potentialMoves?: string[]
}

const props = defineProps<Props>();
const emits = defineEmits<{
  (e: 'square-selected', squareName: string): void | Promise<void>
}>();

interface Square {
  name: string,
  originalColor: string,
  color: string,
  selected: boolean,
  chessPiece?: string,
  chessPieceUTF?: string
}

function getSquareColor(i: number) {
  let fileIdx = (i % 8);
  let rankIdx = 8 - ( (i - fileIdx) / 8 );
  return ((fileIdx + rankIdx) % 2 == 0) ? 'dark square' : 'light square';
}

function getSquareName(i: number) {
  let fileIdx = (i % 8);
  let rankIdx = 8 - ( (i - fileIdx) / 8 );

  const squareName = fileToLetter(fileIdx + 1) + rankIdx;
  return squareName;
}

function fileToLetter(file: number) {
  switch (file) {
    case 1:
      return 'a';
    case 2:
      return 'b';
    case 3:
      return 'c';
    case 4:
      return 'd';
    case 5:
      return 'e';
    case 6:
      return 'f';
    case 7:
      return 'g';
    case 8:
      return 'h';
  }

  return "";
}

function getChessPieceUTF(chessPiece: string): string {
  switch (chessPiece) {
    case 'K':
      return '&#9812;'; // white king
    case 'Q':
      return '&#9813;'; // white queen
    case 'R':
      return '&#9814;'; // white rook
    case 'B':
      return '&#9815;'; // white bishop
    case 'N':
      return '&#9816;'; // white knight
    case 'P':
      return '&#9817;'; // white pawn
    case 'k':
      return '&#9818;'; // black king
    case 'q':
      return '&#9819;'; // black queen
    case 'r':
      return '&#9820;'; // black rook
    case 'b':
      return '&#9821;'; // black bishop
    case 'n':
      return '&#9822;'; // black knight
    case 'p':
      return '&#9823;'; // black pawn
    default:
      return '';
  }
}

function parseFenToSquares(fen: string) {
  const squares: Square[] = [];
  const fenParts = fen.split(' ');
  const fenBoard = fenParts[0];
  const fenRows = fenBoard.split('/');

  for (let i = 0; i < 64; i++) {
    const color = getSquareColor(i);
    const name = getSquareName(i);
    squares.push({ name, color, selected: false, originalColor: color});
  }

  let rank = 1;
  for (const fenRow of fenRows) {
    let file = 1;
    for (const fenChar of fenRow) {
      if (isNaN(parseInt(fenChar))) {
        const squareIdx = (rank - 1) * 8 + (file - 1);
        squares[squareIdx].chessPiece = fenChar;
        squares[squareIdx].chessPieceUTF = getChessPieceUTF(fenChar);
        squares[squareIdx].selected = squares[squareIdx].name == props.selectedSquare;
        file++;
      } else {
        file += parseInt(fenChar);
      }
    }
    rank++;
  }

  return squares;
}


function selectSquare(squareName: string) {
  emits('square-selected', squareName);
}

const squares = computed(() =>  parseFenToSquares(props.fen));
</script>

<template>
  <div class="chessBoard">
    <div  v-for="square in squares" :class="clsx(square.color, selectedSquare == square.name && 'selected')" @click="selectSquare(square.name)" >
      <span class="name">{{ square.name }}</span>
      <div class="chessPiece" v-html="square.chessPieceUTF"></div>
    </div>
  </div>

</template>

<style scoped>
  .chessBoard {
    display: grid;
    grid-template-columns: auto auto auto auto auto auto auto auto;
    cursor: pointer;
    user-select: none;
  }

  .square {
    position: relative;
    aspect-ratio: 1;
    padding: 5px;
    min-width: 50px;
    left: 0;
    bottom: 0;
  }

  .dark:not(.selected) {
    background-color: forestgreen;
  }

  .light:not(.selected) {
    background-color: lightgrey;
  }

  .selected {
    background-color: yellow;
  }

  .candidate-move {
    background-color: orange;
  }

  .chessPiece {
    position: absolute;
    margin: auto;
    text-align: center;
    font-size: 30px;
  }

  .name {
    position: absolute;
    bottom: 5px;
    left: 5px;
    color: #242424;
  }
</style>