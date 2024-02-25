import {defineStore} from "pinia";
import {computed, ref} from "vue";
import {createGameClient} from "../services/game-service/client/gameClient.ts";
import {AnonymousAuthenticationProvider} from "@microsoft/kiota-abstractions";
import {FetchRequestAdapter} from "@microsoft/kiota-http-fetchlibrary";

function createClient() {
    const anonymousAuthenticationProvider = new AnonymousAuthenticationProvider();
    const fetchProvider = new FetchRequestAdapter(anonymousAuthenticationProvider);
        fetchProvider.baseUrl = window.location.origin + '/api';

    const gameClient = createGameClient(fetchProvider);

    return gameClient;
}

interface Squares {
    [key: string]: Square;
}

interface Square {
    name: string;
    piece: string;
}

export function parseFenToSquares(fen: string): Squares {
    const squares: Squares = {};
    const rows = fen.split(' ')[0].split('/');
    rows.forEach((row, rowIndex) => {
        let columnIndex = 0;
        row.split('').forEach((char) => {
            if (isNaN(parseInt(char))) {
                const squareName = `${String.fromCharCode(97 + columnIndex)}${8 - rowIndex}`;
                squares[squareName] = {
                    name: squareName,
                    piece: char
                };
                columnIndex++;
            }
            else {
                columnIndex += parseInt(char);
            }
        });
    });

    return squares;
}

async function getCandidateMoves(gameId: string, fromSquare: string): Promise<string[]> {
    const gameClient = createClient();
    const result = await gameClient.game.byGameId(gameId).move.byFromSquare(fromSquare).get();
    return result?.toSquares ?? [];
}

export const useGameStore = defineStore('GameStore', () => {
    const gameClient = createClient();
    const gameId = ref<string>("");
    const fen = ref<string>('');
    const error = ref<string>('');
    const inProgress = ref<boolean>(false);
    const selectedSquare = ref<string>('');
    const candidateMoves = ref<string[]>([]);
    const squares = computed(() => parseFenToSquares(fen.value));



    async function newGame()  {
        inProgress.value = true;
        try {
            const game = await gameClient.game.post({});
            gameId.value = game?.gameId ?? "";
            fen.value = game?.fen ?? "";
            return true;
        }
        catch (e) {
            console.error('Error creating game', e);
            error.value = "Error creating new game."
        }
        finally {
            inProgress.value = false;
        }
        return false;
    }

    async function setSelectedSquare(square: string) {
        selectedSquare.value = square;
        inProgress.value = true;
        try {
            const candidateSquares = await getCandidateMoves(gameId.value, square);
            candidateMoves.value = candidateSquares;
        }
        catch (e) {
            console.error('Error getting candidate moves', e);
            error.value = "Error getting candidate moves."
        }
        finally {
            inProgress.value = false;
        }
    }

    async function move(move: string) {
        inProgress.value = true;
        try {
            const gameClient = createClient();
            const result = await gameClient.game.byGameId(gameId.value).move.post({move});
            fen.value = result?.fen ?? fen.value;

            console.log(fen.value, result?.fen, result?.result);
            return true;
        }
        catch (e) {
            console.error('Error moving', e);
            error.value = "Error moving."
        }
        finally {
            inProgress.value = false;
        }
        return false;
    }

    return {
        gameId,
        fen,
        error,
        inProgress,
        squares,
        selectedSquare,
        candidateMoves,
        newGame,
        move,
        setSelectedSquare
    }
});
