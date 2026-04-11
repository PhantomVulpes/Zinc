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
              <div class="text-lg font-semibold text-purple-900">{{ reporterUsername || 'Loading...' }}</div>
            </div>

            <!-- Created Date -->
            <div class="bg-purple-50 rounded-lg p-4 border border-purple-200">
              <div class="text-sm text-purple-600 font-medium mb-1">Created</div>
              <div class="text-lg font-semibold text-purple-900">{{ formatDate(ticket.createdDate) }}</div>
            </div>

            <!-- Completed Date -->
            <div v-if="ticket.status === TicketStatus._4 && ticket.completedDate" class="bg-purple-50 rounded-lg p-4 border border-purple-200">
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
          <div class="mt-8">
            <h2 class="text-xl font-semibold text-purple-900 mb-4">Comments</h2>
            
            <!-- Existing Comments -->
            <div v-if="ticket.comments && ticket.comments.length > 0" class="space-y-4 mb-4">
              <div
                v-for="(comment, index) in ticket.comments"
                :key="index"
                class="bg-white rounded-lg border border-lavender-300 p-4 shadow-sm"
              >
                <div class="flex items-start justify-between mb-2">
                  <div class="font-semibold text-purple-900">{{ getCommentAuthorName(comment.author) || 'Loading...' }}</div>
                  <div class="text-sm text-purple-600">{{ formatDate(comment.createdDate) }}</div>
                </div>
                <p class="text-purple-700 whitespace-pre-wrap">{{ comment.value }}</p>
              </div>
            </div>
            
            <p v-else class="text-purple-500 italic mb-4">No comments yet</p>
            
            <!-- Add Comment Form -->
            <div class="bg-white rounded-lg border border-lavender-300 p-4 shadow-sm">
              <div class="flex gap-3">
                <InputText
                  v-model="newComment"
                  rows="3"
                  class="flex-1 p-3 border border-lavender-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-purple-500"
                  placeholder="Write your comment here..."
                  :disabled="isSubmitting"
                />
                <Button
                  icon="pi pi-send"
                  @click="submitComment"
                  :disabled="!newComment.trim() || isSubmitting"
                  :loading="isSubmitting"
                  class="bg-purple-600 hover:bg-purple-700 text-white font-semibold px-4 py-2 rounded-lg self-start"
                />
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
import InputText from 'primevue/inputtext'
import { createAuthenticatedClient } from '@/api/apiClient'
import { Ticket, TicketStatus, AddCommentToTicketRequest } from '@/api/apiclients/ZincApiClient'

const router = useRouter()
const route = useRoute()

const ticket = ref<Ticket | null>(null)
const projectShorthand = ref('')
const reporterUsername = ref('')
const projectKey = ref('')
const newComment = ref('')
const commentAuthors = ref<Record<string, string>>({})
const isLoading = ref(false)
const isSubmitting = ref(false)
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
    
    // Store project key for adding comments
    projectKey.value = project.key || ''
    
    // Find the ticket by index
    const foundTicket = project.tickets?.find(t => t.index === ticketIndex)
    
    if (!foundTicket) {
      errorMessage.value = `Ticket ${identifier} not found`
      ticket.value = null
    } else {
      ticket.value = foundTicket
      
      // Load reporter username
      if (foundTicket.reporterKey) {
        try {
          const reporter = await client.user(foundTicket.reporterKey)
          reporterUsername.value = reporter.username || 'Unknown'
        } catch (error) {
          console.error('Failed to load reporter:', error)
          reporterUsername.value = 'Unknown'
        }
      }
      
      // Load comment author usernames
      await loadCommentAuthors(foundTicket, client)
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

async function loadCommentAuthors(ticketData: Ticket, client: any) {
  if (!ticketData.comments || ticketData.comments.length === 0) return
  
  const authorKeys = new Set<string>()
  ticketData.comments.forEach(comment => {
    if (comment.author) {
      authorKeys.add(comment.author)
    }
  })
  
  // Load all unique author usernames
  for (const authorKey of authorKeys) {
    try {
      const user = await client.user(authorKey)
      commentAuthors.value[authorKey] = user.username || 'Unknown'
    } catch (error) {
      console.error(`Failed to load user ${authorKey}:`, error)
      commentAuthors.value[authorKey] = 'Unknown'
    }
  }
}

function getCommentAuthorName(authorKey: string | undefined): string {
  if (!authorKey) return 'Unknown'
  return commentAuthors.value[authorKey] || 'Loading...'
}

async function submitComment() {
  if (!newComment.value.trim() || !ticket.value || !projectKey.value) return

  isSubmitting.value = true
  errorMessage.value = ''

  try {
    const client = createAuthenticatedClient()
    
    const request = new AddCommentToTicketRequest({
      projectKey: projectKey.value,
      ticketIndex: ticket.value.index,
      comment: newComment.value
    })
    
    await client.addComment(request)
    
    // Clear the comment field
    newComment.value = ''
    
    // Reload the ticket to show the new comment
    await loadTicket()
  } catch (error: any) {
    console.error('Failed to add comment:', error)
    errorMessage.value = 'Failed to add comment. Please try again.'
  } finally {
    isSubmitting.value = false
  }
}

onMounted(() => {
  loadTicket()
})
</script>
