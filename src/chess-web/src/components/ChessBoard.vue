<script setup lang="ts">

import {computed} from "vue";
import clsx from "clsx";
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

interface Props {
  fen: string,
  selectedSquare?: string,
  candidateSquares?: string[]
}

const props = defineProps<Props>();
const emits = defineEmits<{
  (e: 'square-selected', square: Square): void
}>();

interface InternalSquare {
  name: string,
  originalColor: string,
  color: string,
  selected: boolean,
  chessPiece?: string,
  candidate: boolean,
}

export interface Square {
  name: string,
  pieceColor: string,
  piece: string
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


function getChessPieceSvg(chessPiece: string): string {
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
      return '';
  }
}


function parseFenToSquares(fen: string) {
  const squares: InternalSquare[] = [];
  const fenParts = fen.split(' ');
  const fenBoard = fenParts[0];
  const fenRows = fenBoard.split('/');

  for (let i = 0; i < 64; i++) {
    const color = getSquareColor(i);
    const name = getSquareName(i);
    squares.push({ name, color, selected: false, originalColor: color, candidate: false});
  }

  let rank = 1;
  for (const fenRow of fenRows) {
    let file = 1;
    for (const fenChar of fenRow) {
      if (isNaN(parseInt(fenChar))) {
        const squareIdx = (rank - 1) * 8 + (file - 1);
        squares[squareIdx].chessPiece = fenChar;
        squares[squareIdx].selected = squares[squareIdx].name == props.selectedSquare;
        squares[squareIdx].candidate = props.candidateSquares?.includes(squares[squareIdx].name) || false;
        file++;
      } else {
        file += parseInt(fenChar);
      }
    }
    rank++;
  }

  return squares;
}




function selectSquare(square: InternalSquare) {

  const ret : Square = {
    name: square.name,
    pieceColor: square.chessPiece ? (square.chessPiece == square.chessPiece.toUpperCase() ? 'white' : 'black') : '',
    piece: square.chessPiece ? square.chessPiece : ''
  };

  emits('square-selected', ret);
}

const squares = computed(() =>  parseFenToSquares(props.fen));
</script>

<template>
  <div class="chessBoard">
    <div  v-for="square in squares" :class="clsx(square.color, square.selected && 'selected', square.candidate && 'candidate')" @click="selectSquare(square)" >
      <span class="name">{{ square.name }}</span>
      <img v-if="square.chessPiece" class="chessPiece" :src="getChessPieceSvg(square.chessPiece)" />
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