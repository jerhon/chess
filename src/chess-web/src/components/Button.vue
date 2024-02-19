<script setup lang="ts">

import Spinner from "./Spinner.vue";

defineProps<{
  text: string
  inProgress?: boolean
}>();

const emit = defineEmits<{
  (e: "click"): void
}>();

</script>

<template>
  <button class="button" @click="emit('click')" >
    <Spinner size="small" v-if="inProgress" />
    {{ text }}
  </button>
</template>

<style lang="postcss" scoped>
</style>