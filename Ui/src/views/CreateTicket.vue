<template>
  <div class="min-h-screen">
    <div class="container mx-auto px-4 py-12">
      <div class="max-w-2xl mx-auto">
        <!-- Back Button -->
        <ZincButton
          label="Back to Project"
          icon="pi pi-arrow-left"
          severity="secondary"
          @click="router.back()"
          class="mb-6"
          text
        />

        <Card class="bg-white/90 backdrop-blur-md shadow-2xl rounded-2xl">
          <template #title>
            <div class="flex items-center gap-2 text-2xl text-purple-900 px-8 pt-6">
              <i class="pi pi-ticket text-purple-600"></i>
              Create New Ticket
            </div>
          </template>
          
          <template #content>
            <form @submit.prevent="handleSubmit" class="flex flex-col gap-5 px-8 pb-6">
              <!-- Success Message -->
              <Message v-if="successMessage" severity="success" :closable="true" @close="successMessage = ''">
                {{ successMessage }}
              </Message>

              <!-- Error Message -->
              <Message v-if="errorMessage" severity="error" :closable="true" @close="errorMessage = ''">
                {{ errorMessage }}
              </Message>

              <!-- Ticket Title -->
              <div class="flex flex-col gap-2">
                <label for="ticketTitle" class="font-semibold text-purple-900">
                  Title <span class="text-red-500">*</span>
                </label>
                <InputText
                  id="ticketTitle"
                  ref="titleInputRef"
                  v-model="ticketTitle"
                  placeholder="Enter ticket title"
                  :disabled="isLoading"
                  class="w-full border-2 border-lavender-300 rounded-lg px-4 py-2 shadow-sm focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-200 transition-all"
                />
              </div>

              <!-- Ticket Description -->
              <div class="flex flex-col gap-2">
                <label for="ticketDescription" class="font-semibold text-purple-900">
                  Description
                </label>
                <Textarea
                  id="ticketDescription"
                  v-model="ticketDescription"
                  placeholder="Describe the ticket... (optional)"
                  :disabled="isLoading"
                  rows="6"
                  class="w-full border-2 border-lavender-300 rounded-lg px-4 py-2 shadow-sm focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-200 transition-all resize-none"
                />
              </div>

              <!-- Redirect Checkbox -->
              <div class="flex items-center gap-2">
                <Checkbox
                  id="redirectOnCreate"
                  v-model="redirectOnCreate"
                  :binary="true"
                  :disabled="isLoading"
                />
                <label for="redirectOnCreate" class="text-purple-900 cursor-pointer">
                  Redirect on ticket creation
                </label>
              </div>

              <!-- Action Buttons -->
              <div class="flex gap-3 mt-2">
                <ZincButton
                  type="submit"
                  label="Create Ticket"
                  icon="pi pi-check"
                  :loading="isLoading"
                  severity="primary"
                />
                <ZincButton
                  type="button"
                  label="Cancel"
                  icon="pi pi-times"
                  severity="secondary"
                  :disabled="isLoading"
                  @click="router.back()"
                />
              </div>
            </form>
          </template>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Message from 'primevue/message'
import Checkbox from 'primevue/checkbox'
import ZincButton from '@/components/ZincButton.vue'
import { createAuthenticatedClient } from '@/api/apiClient'
import { CreateTicketRequest } from '@/api/apiclients/ZincApiClient'

const router = useRouter()
const route = useRoute()

const ticketTitle = ref('')
const ticketDescription = ref('')
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const redirectOnCreate = ref(false)
const titleInputRef = ref<any>(null)

async function handleSubmit() {
  errorMessage.value = ''
  successMessage.value = ''
  
  // Validation
  if (!ticketTitle.value?.trim()) {
    errorMessage.value = 'Ticket title is required'
    return
  }

  const projectShorthand = route.params.projectShorthand as string
  
  if (!projectShorthand) {
    errorMessage.value = 'No project shorthand provided'
    return
  }

  isLoading.value = true

  try {
    const client = createAuthenticatedClient()
    const request = new CreateTicketRequest({
      title: ticketTitle.value.trim(),
      description: ticketDescription.value?.trim() || ''
    })
    
    await client.createTicket(projectShorthand, request)
    
    // Clear form
    ticketTitle.value = ''
    ticketDescription.value = ''
    
    if (redirectOnCreate.value) {
      // Redirect immediately
      router.push(`/projects/${projectShorthand}`)
    } else {
      // Stay on page, show success message, and focus title input
      successMessage.value = 'Ticket created successfully!'
      
      // Focus the title input for quick next ticket creation
      setTimeout(() => {
        titleInputRef.value?.$el?.focus()
      }, 100)
    }
  } catch (error: any) {
    console.error('Failed to create ticket:', error)
    errorMessage.value = 'Failed to create ticket. Please try again.'
  } finally {
    isLoading.value = false
  }
}
</script>
