<script setup lang="ts">

import {computed} from "vue";

enum Size {
  small,
  medium,
  large
}

const props = defineProps<{
  size: Size
}>();

const size = computed(() => {
  switch (props.size) {
    case Size.small:
      return "w-5 h-5";
    case Size.medium:
      return "w-10 h-10";
    case Size.large:
      return "w-20 h-20";
  }
});

</script>

<template>
  <div class="relative">
    <div :class="size + 'border-purple-200 border-2 rounded-full'"></div>
    <div :class="size + 'border-purple-700 border-t-2 animate-spin rounded-full absolute left-0 top-0'"></div>
  </div>
</template>

<style scoped>

</style>