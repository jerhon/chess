import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
var gameServicePort = process.env['services__chess-game-service__0'] ?? "http://localhost:5238"


export default defineConfig({
  plugins: [vue()],
  server: {
    port: process.env['PORT'] ?? 5173,
    proxy: {
      '/api': {
        target: 'http://localhost:5238',
        changeOrigin: true,
      }
    }
  }
})
