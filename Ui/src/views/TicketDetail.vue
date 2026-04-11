<template>
  <div class="min-h-screen">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-4xl mx-auto">
        <!-- Back Button -->
        <Button
          label="Back to Project"
          icon="pi pi-arrow-left"
          severity="secondary"
          @click="router.push(`/projects/${projectShorthand}`)"
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

        <!-- Ticket Details -->
        <div v-if="!isLoading && ticket" class="bg-white/80 backdrop-blur-sm rounded-2xl shadow-xl border border-lavender-200 p-8">
          <!-- Ticket Header -->
          <div class="mb-6">
            <div class="flex items-center gap-2 mb-2">
              <span class="text-purple-600 font-semibold text-lg">{{ projectShorthand }}-{{ ticket.index }}</span>
              <span 
                class="px-3 py-1 rounded-full text-sm font-medium"
                :class="getStatusClass(ticket.status)"
              >
                {{ formatTicketStatus(ticket.status) }}
              </span>
            </div>
            <h1 class="text-4xl font-bold text-purple-900">{{ ticket.title }}</h1>
          </div>

          <!-- Description -->
          <div class="mb-6">
            <h2 class="text-xl font-semibold text-purple-900 mb-2">Description</h2>
            <p class="text-purple-700 leading-relaxed whitespace-pre-wrap">{{ ticket.description || 'No description provided' }}</p>
          </div>

          <!-- Ticket Information -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
            <!-- Reporter -->
            <div class="bg-purple-50 rounded-lg p-4 border border-purple-200">
              <div class="text-sm text-purple-600 font-medium mb-1">Reporter</div>
              <div class="text-lg font-semibold text-purple-900">{{ ticket.reporterKey || 'Unknown' }}</div>
            </div>

            <!-- Assigned To -->
            <div class="bg-purple-50 rounded-lg p-4 border border-purple-200">
              <div class="text-sm text-purple-600 font-medium mb-1">Assigned To</div>
              <div class="text-lg font-semibold text-purple-900">{{ ticket.assignedToKey || 'Unassigned' }}</div>
            </div>

            <!-- Created Date -->
            <div class="bg-purple-50 rounded-lg p-4 border border-purple-200">
              <div class="text-sm text-purple-600 font-medium mb-1">Created</div>
              <div class="text-lg font-semibold text-purple-900">{{ formatDate(ticket.createdDate) }}</div>
            </div>

            <!-- Completed Date -->
            <div v-if="ticket.completedDate" class="bg-purple-50 rounded-lg p-4 border border-purple-200">
              <div class="text-sm text-purple-600 font-medium mb-1">Completed</div>
              <div class="text-lg font-semibold text-purple-900">{{ formatDate(ticket.completedDate) }}</div>
            </div>
          </div>

          <!-- Labels -->
          <div class="mb-6">
            <h2 class="text-xl font-semibold text-purple-900 mb-3">Labels</h2>
            <div v-if="ticket.labels && ticket.labels.length > 0" class="flex flex-wrap gap-2">
              <span
                v-for="label in ticket.labels"
                :key="label"
                class="bg-lavender-100 text-purple-800 px-4 py-2 rounded-full text-sm font-medium"
              >
                <i class="pi pi-tag mr-1"></i>{{ label }}
              </span>
            </div>
            <p v-else class="text-purple-500 italic">No labels assigned to this ticket</p>
          </div>

          <!-- Comments Section -->
          <div v-if="ticket.comments && ticket.comments.length > 0" class="mt-8">
            <h2 class="text-xl font-semibold text-purple-900 mb-4">Comments</h2>
            <div class="space-y-4">
              <div
                v-for="(comment, index) in ticket.comments"
                :key="index"
                class="bg-white rounded-lg border border-lavender-300 p-4 shadow-sm"
              >
                <div class="flex items-start justify-between mb-2">
                  <div class="font-semibold text-purple-900">{{ comment.author || 'Unknown' }}</div>
                  <div class="text-sm text-purple-600">{{ formatDate(comment.createdDate) }}</div>
                </div>
                <p class="text-purple-700 whitespace-pre-wrap">{{ comment.value }}</p>
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
import { Ticket, TicketStatus } from '@/api/apiclients/ZincApiClient'

const router = useRouter()
const route = useRoute()

const ticket = ref<Ticket | null>(null)
const projectShorthand = ref('')
const isLoading = ref(false)
const errorMessage = ref('')

async function loadTicket() {
  const identifier = route.params.identifier as string
  
  if (!identifier) {
    errorMessage.value = 'No ticket identifier provided'
    return
  }

  // Parse the identifier (format: SHORTHAND-INDEX)
  const lastDashIndex = identifier.lastIndexOf('-')
  if (lastDashIndex === -1) {
    errorMessage.value = 'Invalid ticket identifier format'
    return
  }

  projectShorthand.value = identifier.substring(0, lastDashIndex)
  const ticketIndexStr = identifier.substring(lastDashIndex + 1)
  const ticketIndex = parseInt(ticketIndexStr, 10)

  if (isNaN(ticketIndex)) {
    errorMessage.value = 'Invalid ticket index'
    return
  }

  isLoading.value = true
  errorMessage.value = ''

  try {
    const client = createAuthenticatedClient()
    const project = await client.projects(projectShorthand.value)
    
    // Find the ticket by index
    const foundTicket = project.tickets?.find(t => t.index === ticketIndex)
    
    if (!foundTicket) {
      errorMessage.value = `Ticket ${identifier} not found`
      ticket.value = null
    } else {
      ticket.value = foundTicket
    }
  } catch (error: any) {
    console.error('Failed to load ticket:', error)
    errorMessage.value = 'Failed to load ticket details. Please try again.'
  } finally {
    isLoading.value = false
  }
}

function formatTicketStatus(status: TicketStatus | undefined): string {
  if (status === undefined) return 'Unknown'
  
  const statusMap: Record<number, string> = {
    [TicketStatus._0]: 'Unknown',
    [TicketStatus._1]: 'In Review',
    [TicketStatus._2]: 'Open',
    [TicketStatus._3]: 'In Progress',
    [TicketStatus._4]: 'Complete',
    [TicketStatus._5]: 'Cancelled'
  }
  
  return statusMap[status] || 'Unknown'
}

function getStatusClass(status: TicketStatus | undefined): string {
  if (status === undefined) return 'bg-gray-100 text-gray-700'
  
  const classMap: Record<number, string> = {
    [TicketStatus._0]: 'bg-gray-100 text-gray-700',
    [TicketStatus._1]: 'bg-yellow-100 text-yellow-700',
    [TicketStatus._2]: 'bg-blue-100 text-blue-700',
    [TicketStatus._3]: 'bg-purple-100 text-purple-700',
    [TicketStatus._4]: 'bg-green-100 text-green-700',
    [TicketStatus._5]: 'bg-red-100 text-red-700'
  }
  
  return classMap[status] || 'bg-gray-100 text-gray-700'
}

function formatDate(date: Date | undefined): string {
  if (!date) return 'N/A'
  
  return new Date(date).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

onMounted(() => {
  loadTicket()
})
</script>
