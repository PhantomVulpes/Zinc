<template>
  <div class="home min-h-screen flex items-center justify-center">
    <div class="max-w-2xl mx-auto">
      <div class="bg-white/80 backdrop-blur-sm rounded-3xl shadow-2xl p-12 border border-lavender-200">
        <div class="mb-8">
          <div class="inline-block p-4 bg-gradient-to-br from-lavender-400 to-purple-500 rounded-2xl mb-6">
            <i class="pi pi-bolt text-5xl text-white"></i>
          </div>
          <h1 class="text-5xl font-bold bg-gradient-to-r from-lavender-600 to-purple-600 bg-clip-text text-transparent mb-4">
            Welcome to Zinc
          </h1>
          <p class="text-lg text-purple-700 mb-8">
            A beautiful Vue + TypeScript application with Tailwind & PrimeVue
          </p>
        </div>
        
        <div class="flex gap-4 justify-center items-center mb-8">
          <Button 
            label="Get Started" 
            icon="pi pi-check" 
            class="bg-gradient-to-r from-lavender-500 to-purple-600 border-0 px-6 py-3 text-base hover:shadow-lg transition-all duration-300"
          />
          <Button 
            label="Learn More" 
            severity="secondary" 
            icon="pi pi-info-circle"
            class="border-lavender-400 text-lavender-700 hover:bg-lavender-50 px-6 py-3 text-base transition-all duration-300"
            outlined
          />
        </div>

        <div class="mt-8 pt-8 border-t border-lavender-200">
          <router-link to="/register">
            <Button 
              label="Register New User" 
              icon="pi pi-user-plus" 
              class="bg-gradient-to-r from-purple-500 to-lavender-600 border-0 px-8 py-3 text-lg hover:shadow-xl transform hover:scale-105 transition-all duration-300"
            />
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import Button from 'primevue/button'
</script>

<style scoped>
.home {
  padding: 20px;
}
</style>
