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
          
          <!-- Loading State -->
          <div v-if="isLoadingProjects" class="flex justify-center py-8">
            <i class="pi pi-spinner pi-spin text-4xl text-purple-600"></i>
          </div>

          <!-- Error Message -->
          <Message v-if="errorMessage" severity="error" :closable="true" @close="errorMessage = ''">
            {{ errorMessage }}
          </Message>

          <!-- Projects List -->
          <div v-if="!isLoadingProjects && projects.length > 0" class="space-y-4">
            <div
              v-for="project in projects"
              :key="project.key"
              class="bg-white rounded-lg border border-lavender-300 p-6 shadow-sm hover:shadow-md transition-shadow"
            >
              <!-- Project Header -->
              <div class="flex items-start justify-between mb-3">
                <h3 class="text-xl font-bold text-purple-900">
                  <router-link
                    :to="`/projects/${project.key}`"
                    class="hover:text-purple-600 transition-colors cursor-pointer"
                  >
                    {{ project.name }} <span class="text-sm text-purple-600">({{ project.shorthand }})</span>
                  </router-link>
                </h3>
                <div class="bg-purple-100 text-purple-700 px-3 py-1 rounded-full text-sm font-medium">
                  {{ project.tickets?.length || 0 }} {{ project.tickets?.length === 1 ? 'ticket' : 'tickets' }}
                </div>
              </div>

              <!-- Description -->
              <p class="text-purple-700 mb-4">{{ project.description }}</p>

              <!-- Labels -->
              <div v-if="project.labels && project.labels.length > 0" class="flex flex-wrap gap-2">
                <span
                  v-for="label in project.labels"
                  :key="label"
                  class="bg-lavender-100 text-purple-800 px-3 py-1 rounded-full text-xs font-medium"
                >
                  <i class="pi pi-tag mr-1"></i>{{ label }}
                </span>
              </div>
              <div v-else class="text-purple-500 text-sm italic">
                No labels
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <p v-if="!isLoadingProjects && projects.length === 0 && !errorMessage" class="text-purple-700">
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
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Button from 'primevue/button'
import Message from 'primevue/message'
import { useAuth } from '@/services/auth'
import { createAuthenticatedClient } from '@/api/apiClient'
import { Project } from '@/api/apiclients/ZincApiClient'

const router = useRouter()
const { isAuthenticated } = useAuth()

const projects = ref<Project[]>([])
const isLoadingProjects = ref(false)
const errorMessage = ref('')

async function loadProjects() {
  if (!isAuthenticated.value) {
    return
  }

  isLoadingProjects.value = true
  errorMessage.value = ''

  try {
    const client = createAuthenticatedClient()
    const result = await client.projectsAll()
    projects.value = result || []
  } catch (error: any) {
    console.error('Failed to load projects:', error)
    errorMessage.value = 'Failed to load projects. Please try again.'
  } finally {
    isLoadingProjects.value = false
  }
}

// Load projects when component mounts
onMounted(() => {
  loadProjects()
})
</script>
