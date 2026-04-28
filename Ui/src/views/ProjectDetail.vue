<template>
  <div class="min-h-screen">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-4xl mx-auto">
        <!-- Back Button -->
        <ZincButton
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
          <div class="mb-6 flex items-start justify-between">
            <h1 class="text-4xl font-bold text-purple-900 mb-2">
              {{ project.name }} <span class="text-2xl text-purple-600">({{ project.shorthand }})</span>
            </h1>
            <ZincButton
              label="Create Ticket"
              icon="pi pi-plus"
              severity="primary"
              @click="router.push(`/projects/${project.shorthand}/create-ticket`)"
            />
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
              <router-link
                v-for="(ticket, index) in sortedTickets"
                :key="ticket.index ?? index"
                :to="`/ticket/${project.shorthand}-${ticket.index}`"
                class="block bg-white rounded-lg border border-lavender-300 p-4 shadow-sm hover:shadow-md transition-shadow cursor-pointer no-underline"
              >
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <div class="flex items-center gap-2 mb-1">
                      <span class="text-purple-600 font-semibold text-sm">{{ project.shorthand }}-{{ ticket.index }}</span>
                    </div>
                    <h3 class="font-semibold text-purple-900">{{ ticket.title }}</h3>
                    <p v-if="ticket.description" class="text-sm text-purple-700 mt-1 mb-3">{{ ticket.description }}</p>
                    <span
                      v-if="ticket.labels && ticket.labels.length > 0"
                      v-for="label in ticket.labels"
                      :key="label"
                      class="bg-lavender-100 text-purple-800 px-4 py-2 mr-1 rounded-full text-sm font-medium"
                    >
                      <i class="pi pi-tag mr-1"></i>{{ label }}
                    </span>
                  </div>
                  <div class="ml-4">
                    <span :class="[
                      'px-3 py-1 rounded-full text-xs font-medium',
                      getTicketStatusClasses(ticket.status).combined
                    ]">
                      <i :class="['mr-1', getTicketStatusIcon(ticket.status)]"></i>
                      {{ formatTicketStatus(ticket.status) }}
                    </span>
                  </div>
                </div>
              </router-link>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import Message from 'primevue/message'
import ZincButton from '@/components/ZincButton.vue'
import { createAuthenticatedClient } from '@/api/apiClient'
import { Project, ProjectStatus, TicketStatus } from '@/api/apiclients/ZincApiClient'
import { getTicketStatusClasses, getTicketStatusIcon, formatTicketStatus } from '@/utils/ticketStatus'

const router = useRouter()
const route = useRoute()

const project = ref<Project | null>(null)
const isLoading = ref(false)
const errorMessage = ref('')

// Define custom ticket status sort order
const statusSortOrder: Record<number, number> = {
  [TicketStatus._0]: 0, // Unknown - highest priority
  [TicketStatus._3]: 1, // In Progress
  [TicketStatus._2]: 2, // Open
  [TicketStatus._1]: 3, // In Review
  [TicketStatus._4]: 4, // Complete
  [TicketStatus._5]: 5  // Cancelled
}

// Computed property to get sorted tickets
const sortedTickets = computed(() => {
  if (!project.value?.tickets) return []
  
  return [...project.value.tickets].sort((a, b) => {
    const statusA = a.status ?? TicketStatus._0
    const statusB = b.status ?? TicketStatus._0
    const orderA = statusSortOrder[statusA] ?? 999
    const orderB = statusSortOrder[statusB] ?? 999
    return orderA - orderB
  })
})

async function loadProject() {
  const projectShorthand = route.params.projectShorthand as string
  
  if (!projectShorthand) {
    errorMessage.value = 'No project shorthand provided'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    const client = createAuthenticatedClient()
    project.value = await client.projects(projectShorthand)
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

onMounted(() => {
  loadProject()
})
</script>
