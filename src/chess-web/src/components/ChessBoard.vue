<script setup lang="ts">

import {computed} from "vue";
import white_pawn from "../images/pieces/white_pawn.svg";
import white_bishop from "../images/pieces/white_bishop.svg";
import white_knight from "../images/pieces/white_knight.svg";
import white_rook from "../images/pieces/white_rook.svg";
import white_queen from "../images/pieces/white_queen.svg";
import white_king from "../images/pieces/white_king.svg";
import black_pawn from "../images/pieces/black_pawn.svg";
import black_bishop from "../images/pieces/black_bishop.svg";
import black_knight from "../images/pieces/black_knight.svg";
import black_rook from "../images/pieces/black_rook.svg";
import black_queen from "../images/pieces/black_queen.svg";
import black_king from "../images/pieces/black_king.svg";
import clsx from "clsx";

const ranks = ['1', '2', '3', '4', '5', '6', '7', '8'];
const files = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'];

interface Props {
  squares: Squares,
}

const props = defineProps<Props>();
const emits = defineEmits<{
  (e: 'square-clicked', square: string): void
}>();

export interface Squares {
  [key: string]: Square | undefined
}

export interface Square {
  name: string,
  piece?: string,
  highlight?: boolean,
  symbol?: 'dot',

  color: string
}

function getChessPieceSvg(chessPiece: string): string | undefined {
  switch (chessPiece) {
    case 'K':
      return white_king; // white king
    case 'Q':
      return white_queen; // white queen
    case 'R':
      return white_rook;// white rook
    case 'B':
      return white_bishop; // white bishop
    case 'N':
      return white_knight;// white knight
    case 'P':
      return white_pawn; // white pawn
    case 'k':
      return black_king; // black king
    case 'q':
      return black_queen; // black queen
    case 'r':
      return black_rook; // black rook
    case 'b':
      return black_bishop; // black bishop
    case 'n':
      return black_knight; // black knight
    case 'p':
      return black_pawn; // black pawn
    default:
      return undefined;
  }
}

interface InternalSquare extends  Square {
  svg?: string;
}


function getInternalSquares() {

  const internalSquares: InternalSquare[] = [];
  for (let rank of ranks) {
    for (let file of files) {
      const name = file + rank;
      const square = props.squares?.[name];
      const isDarkSquare = (ranks.indexOf(rank) + files.indexOf(file)) % 2 === 1;
      let color = isDarkSquare ? 'dark' : 'light';
      color = square?.highlight ? 'selected' : color;
      if (square)
      {
        const piece = props.squares[name]?.piece ?? '';
        const svg = !!piece ? getChessPieceSvg(piece) : '';
        internalSquares.push({ ...square, svg, color});
      }
      else
      {
        internalSquares.push({ name, color });
      }
    }
  }
  return internalSquares;
}

const squares = computed(() => getInternalSquares());

function selectSquare(square: string) {
  emits('square-clicked', square);
}
</script>

<template>
  <div class="chessBoard">
    <div  :class="clsx('square', square.color)" v-for="square in squares" @click="selectSquare(square.name)">
      <span class="name">{{ square.name }}</span>
      <img v-if="square.piece" class="chessPiece" :src="getChessPieceSvg(square.piece)" draggable="false" />
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

    @apply border-2 border-gray-400;
  }

  .square:hover {
    @apply border-red-400;
  }

  .dark:not(.selected):not(.candidate-move) {
    background-color: forestgreen;
  }

  .light:not(.selected):not(.candidate-move) {
    background-color: lightgrey;
  }

  .selected {
    background-color: yellow;
  }

  .candidate {
    background-color: orange;
  }

  .chessPiece {
    position: absolute;
    inset: 0 0 0 0;
    width: 100%;
    height: 100%;
    padding: 5%;

  }

  .name {
    position: absolute;
    bottom: 5px;
    left: 5px;
    color: #242424;
  }
</style>