<script setup lang="ts">

defineProps<{
  text: string
}>();

</script>

<template>
  <button class="button">
    {{ text }}
  </button>
</template>

<style lang="postcss" scoped>
</style>