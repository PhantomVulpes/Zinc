<script setup lang="ts">
import AppHeader from './components/AppHeader.vue'
import Toast from 'primevue/toast'
</script>

<template>
  <Toast />
  <div class="flex flex-col min-h-screen bg-gradient-to-br from-lavender-50 via-purple-50 to-lavender-100">
    <AppHeader />
    <main class="flex-1">
      <router-view />
    </main>
  </div>
</template>

<style scoped>
</style>
