import {defineStore} from "pinia";
import {ref} from "vue";
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

export const useGameStore = defineStore('GameStore', () => {
    const gameClient = createClient();
    const gameId = ref<string>("");
    const fen = ref<string>('');
    const error = ref<string>('');
    const inProgress = ref<boolean>(false);

    async function getCandidateMoves(fromSquare: string): Promise<string[]> {
        inProgress.value = true;
        try {
            const gameClient = createClient();
            const result = await gameClient.game.byGameId(gameId.value).move.byFromSquare(fromSquare).get();
            return result?.toSquares ?? [];
        }
        catch (e) {
            console.error('Error getting candidate moves', e);
            error.value = "Error getting candidate moves."
        }
        finally {
            inProgress.value = false;
        }
        return [];
    }

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
        newGame,
        move,
        getCandidateMoves
    }
});
