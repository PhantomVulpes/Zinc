<template>
  <div class="min-h-screen">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-4xl mx-auto">
        <!-- Back Button -->
        <Button
          label="Back to Projects"
          icon="pi pi-arrow-left"
          severity="secondary"
          @click="router.push('/')"
          class="mb-6"
          text
        />

        <!-- Loading State -->
        <div v-if="isLoading" class="flex justify-center py-12">
          <i class="pi pi-spinner pi-spin text-5xl text-purple-600"></i>
        </div>

        <!-- Error Message -->
        <Message v-if="errorMessage" severity="error" :closable="true" @close="errorMessage = ''">
          {{ errorMessage }}
        </Message>

        <!-- Project Details -->
        <div v-if="!isLoading && project" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl border border-lavender-200 p-8">
          <!-- Project Header -->
          <div class="mb-6">
            <h1 class="text-4xl font-bold text-purple-900 mb-2">
              {{ project.name }} <span class="text-2xl text-purple-600">({{ project.shorthand }})</span>
            </h1>
          </div>

          <!-- Description -->
          <div class="mb-6">
            <h2 class="text-xl font-semibold text-purple-900 mb-2">Description</h2>
            <p class="text-purple-700 leading-relaxed">{{ project.description }}</p>
          </div>

          <!-- Statistics -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
            <div class="bg-purple-50 rounded-lg p-4 border border-purple-200">
              <div class="text-sm text-purple-600 font-medium mb-1">Total Tickets</div>
              <div class="text-3xl font-bold text-purple-900">{{ project.tickets?.length || 0 }}</div>
            </div>
            <div class="bg-purple-50 rounded-lg p-4 border border-purple-200">
              <div class="text-sm text-purple-600 font-medium mb-1">Status</div>
              <div class="text-xl font-semibold text-purple-900">{{ formatProjectStatus(project.status) }}</div>
            </div>
          </div>

          <!-- Labels -->
          <div class="mb-6">
            <h2 class="text-xl font-semibold text-purple-900 mb-3">Labels</h2>
            <div v-if="project.labels && project.labels.length > 0" class="flex flex-wrap gap-2">
              <span
                v-for="label in project.labels"
                :key="label"
                class="bg-lavender-100 text-purple-800 px-4 py-2 rounded-full text-sm font-medium"
              >
                <i class="pi pi-tag mr-1"></i>{{ label }}
              </span>
            </div>
            <p v-else class="text-purple-500 italic">No labels assigned to this project</p>
          </div>

          <!-- Tickets Section -->
          <div v-if="project.tickets && project.tickets.length > 0" class="mt-8">
            <h2 class="text-xl font-semibold text-purple-900 mb-4">Tickets</h2>
            <div class="space-y-3">
              <div
                v-for="ticket in project.tickets"
                :key="ticket.key"
                class="bg-white rounded-lg border border-lavender-300 p-4 shadow-sm hover:shadow-md transition-shadow"
              >
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <h3 class="font-semibold text-purple-900">{{ ticket.title }}</h3>
                    <p v-if="ticket.description" class="text-sm text-purple-700 mt-1">{{ ticket.description }}</p>
                  </div>
                  <div class="ml-4">
                    <span class="bg-purple-100 text-purple-700 px-3 py-1 rounded-full text-xs font-medium">
                      {{ formatTicketStatus(ticket.status) }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import Button from 'primevue/button'
import Message from 'primevue/message'
import { createAuthenticatedClient } from '@/api/apiClient'
import { Project, ProjectStatus, TicketStatus } from '@/api/apiclients/ZincApiClient'

const router = useRouter()
const route = useRoute()

const project = ref<Project | null>(null)
const isLoading = ref(false)
const errorMessage = ref('')

async function loadProject() {
  const projectKey = route.params.projectKey as string
  
  if (!projectKey) {
    errorMessage.value = 'No project key provided'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    const client = createAuthenticatedClient()
    project.value = await client.projects(projectKey)
  } catch (error: any) {
    console.error('Failed to load project:', error)
    errorMessage.value = 'Failed to load project details. Please try again.'
  } finally {
    isLoading.value = false
  }
}

function formatProjectStatus(status: ProjectStatus | undefined): string {
  if (status === undefined) return 'Unknown'
  
  const statusMap: Record<number, string> = {
    [ProjectStatus._0]: 'Unknown',
    [ProjectStatus._1]: 'Open',
    [ProjectStatus._2]: 'Closed',
    [ProjectStatus._3]: 'Archived'
  }
  
  return statusMap[status] || 'Unknown'
}

function formatTicketStatus(status: TicketStatus | undefined): string {
  if (status === undefined) return 'Unknown'
  
  const statusMap: Record<number, string> = {
    [TicketStatus._0]: 'Unknown',
    [TicketStatus._1]: 'Open',
    [TicketStatus._2]: 'In Progress',
    [TicketStatus._3]: 'In Review',
    [TicketStatus._4]: 'Complete',
    [TicketStatus._5]: 'Cancelled'
  }
  
  return statusMap[status] || 'Unknown'
}

onMounted(() => {
  loadProject()
})
</script>
