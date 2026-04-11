<template>
  <div class="min-h-screen">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-4xl mx-auto">
        <!-- Description Section -->
        <div class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl border border-lavender-200 p-8 mb-8">
          <div class="prose prose-lg max-w-none">
            <h1 class="text-4xl font-bold text-purple-900 mb-6">Zinc</h1>
            <p class="text-purple-800 text-lg leading-relaxed mb-4">
              A beautiful Vue + TypeScript application with Tailwind & PrimeVue.
            </p>
            <p class="text-purple-700 leading-relaxed mb-4">
              Get started by registering a new account and exploring the features Zinc has to offer.
            </p>
          </div>
        </div>

        <!-- Project Actions -->
        <div v-if="isAuthenticated" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl border border-lavender-200 p-8 mb-8">
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-2xl font-bold text-purple-900">Projects</h2>
            <Button
              label="New Project"
              icon="pi pi-plus"
              severity="primary"
              @click="router.push('/projects/create')"
              class="bg-purple-600 hover:bg-purple-700 text-white font-semibold px-6 py-3 rounded-lg shadow-md hover:shadow-lg transition-all inline-flex items-center gap-2"
            />
          </div>
          <p class="text-purple-700">
            Your projects will appear here. Create your first project to get started!
          </p>
        </div>

        <!-- Tech Stack Footer -->
        <div class="text-center text-sm text-purple-600 font-medium">
          Built with Vue3, PrimeVue, and Tailwind CSS 💜
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import { useAuth } from '@/services/auth'

const router = useRouter()
const { isAuthenticated } = useAuth()
</script>
