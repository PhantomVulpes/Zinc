<template>
  <div class="pt-12 pb-8 px-4">
    <Card class="w-full max-w-2xl mx-auto bg-white/90 backdrop-blur-md shadow-2xl rounded-2xl">
      <template #title>
        <div class="flex items-center gap-2 text-2xl text-purple-900 px-8 pt-6">
          <i class="pi pi-plus-circle text-purple-600"></i>
          Create New Project
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

          <!-- Project Name -->
          <div class="flex flex-col gap-2">
            <label for="projectName" class="font-semibold text-purple-900">
              Project Name <span class="text-red-500">*</span>
            </label>
            <InputText
              id="projectName"
              v-model="projectName"
              placeholder="Enter project name"
              :disabled="isLoading"
              class="w-full border-2 border-lavender-300 rounded-lg px-4 py-2 shadow-sm focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-200 transition-all"
            />
          </div>

          <!-- Project Shorthand -->
          <div class="flex flex-col gap-2">
            <label for="projectShorthand" class="font-semibold text-purple-900">
              Project Shorthand - used for page urls <span class="text-red-500">*</span>
            </label>
            <InputText
              id="projectShorthand"
              v-model="projectShorthand"
              placeholder="e.g., PROJ, XYZ"
              :disabled="isLoading"
              class="w-full border-2 border-lavender-300 rounded-lg px-4 py-2 shadow-sm focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-200 transition-all"
            />
            <small class="text-purple-600">
              A short identifier for the project (typically 2-5 characters)
            </small>
          </div>

          <!-- Project Description -->
          <div class="flex flex-col gap-2">
            <label for="projectDescription" class="font-semibold text-purple-900">
              Project Description <span class="text-red-500">*</span>
            </label>
            <Textarea
              id="projectDescription"
              v-model="projectDescription"
              placeholder="Describe your project..."
              :disabled="isLoading"
              rows="4"
              class="w-full border-2 border-lavender-300 rounded-lg px-4 py-2 shadow-sm focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-200 transition-all resize-none"
            />
          </div>

          <!-- Action Buttons -->
          <div class="flex gap-3 mt-2">
            <ZincButton
              type="submit"
              label="Create Project"
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
              @click="router.push('/')"
            />
          </div>
        </form>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import Card from 'primevue/card'
import InputText from 'primevue/inputtext'
import Textarea from 'primevue/textarea'
import Message from 'primevue/message'
import ZincButton from '@/components/ZincButton.vue'
import { createAuthenticatedClient } from '@/api/apiClient'
import { CreateNewProjectRequest } from '@/api/apiclients/ZincApiClient'

const router = useRouter()

const projectName = ref('')
const projectShorthand = ref('')
const projectDescription = ref('')
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

async function handleSubmit() {
  errorMessage.value = ''
  successMessage.value = ''
  
  // Validation
  if (!projectName.value?.trim()) {
    errorMessage.value = 'Project name is required'
    return
  }
  
  if (!projectShorthand.value?.trim()) {
    errorMessage.value = 'Project shorthand is required'
    return
  }
  
  if (!projectDescription.value?.trim()) {
    errorMessage.value = 'Project description is required'
    return
  }

  isLoading.value = true

  try {
    const client = createAuthenticatedClient()
    const request = new CreateNewProjectRequest({
      projectName: projectName.value.trim(),
      projectShorthand: projectShorthand.value.trim(),
      projectDescription: projectDescription.value.trim()
    })
    
    const projectKey = await client.create(request)
    
    successMessage.value = `Project created successfully! Project Key: ${projectKey}`
    
    // Clear form
    projectName.value = ''
    projectShorthand.value = ''
    projectDescription.value = ''
    
    // Redirect to home after a short delay
    setTimeout(() => {
      router.push('/')
    }, 2000)
  } catch (error: any) {
    console.error('Create project error:', error)
    errorMessage.value = error?.message || 'Failed to create project. Please try again.'
  } finally {
    isLoading.value = false
  }
}
</script>
