<template>
  <div class="pt-12 pb-8 px-4">
    <Card class="w-full max-w-2xl mx-auto bg-white/90 backdrop-blur-md shadow-2xl rounded-2xl">
      <template #title>
        <div class="flex items-center gap-2 text-2xl text-purple-900 px-8 pt-6">
          <i class="pi pi-user-plus text-purple-600"></i>
          Register New User
        </div>
      </template>
      
      <template #content>
        <form @submit.prevent="handleSubmit" class="flex flex-col gap-5 px-8 pb-6">
          <!-- Success Message -->
          <Message v-if="successMessage" severity="success" :closable="false">
            {{ successMessage }}
          </Message>
          
          <!-- Success Actions -->
          <div v-if="successMessage" class="flex flex-col gap-2">
            <Button
              label="Go to Home"
              icon="pi pi-home"
              severity="success"
              @click="router.push('/')"
              class="w-full"
            />
          </div>

          <!-- Error Message -->
          <Message v-if="errorMessage" severity="error" :closable="true" @close="errorMessage = ''">
            {{ errorMessage }}
          </Message>

          <!-- First Name and Last Name Row -->
          <div class="grid grid-cols-2 gap-4">
            <div class="flex flex-col gap-2">
              <label for="firstName" class="font-semibold text-purple-900">
                First Name
              </label>
              <InputText
                id="firstName"
                v-model="firstName"
                placeholder="Enter your first name"
                :disabled="isLoading"
                class="w-full border-2 border-lavender-300 rounded-lg px-4 py-2 shadow-sm focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-200 transition-all"
              />
            </div>

            <div class="flex flex-col gap-2">
              <label for="lastName" class="font-semibold text-purple-900">
                Last Name
              </label>
              <InputText
                id="lastName"
                v-model="lastName"
                placeholder="Enter your last name"
                :disabled="isLoading"
                class="w-full border-2 border-lavender-300 rounded-lg px-4 py-2 shadow-sm focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-200 transition-all"
              />
            </div>
          </div>

          <!-- Username -->
          <div class="flex flex-col gap-2">
            <label for="username" class="font-semibold text-purple-900">
              Username
            </label>
            <InputText
              id="username"
              v-model="username"
              placeholder="Enter your username"
              :disabled="isLoading"
              class="w-full border-2 border-lavender-300 rounded-lg px-4 py-2 shadow-sm focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-200 transition-all"
            />
          </div>

          <!-- Password -->
          <div class="flex flex-col gap-2">
            <label for="password" class="font-semibold text-purple-900">
              Password
            </label>
            <Password
              id="password"
              v-model="password"
              placeholder="Enter your password"
              :disabled="isLoading"
              :feedback="false"
              toggleMask
              inputClass="w-full border-2 border-lavender-300 rounded-lg px-4 py-2 shadow-sm focus:border-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-200 transition-all"
            />
          </div>

          <!-- Submit Button -->
          <Button
            type="submit"
            label="Register"
            icon="pi pi-user-plus"
            :loading="isLoading"
            severity="primary"
            class="w-full mt-2"
          />
        </form>
      </template>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import Card from 'primevue/card'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Message from 'primevue/message'
import { RegisterUser } from '@/api/User/RegisterUserCommand'

const router = useRouter()

const firstName = ref('')
const lastName = ref('')
const username = ref('')
const password = ref('')
const isLoading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

async function handleSubmit() {
  errorMessage.value = ''
  successMessage.value = ''
  
  // Basic validation
  if (!firstName.value || !lastName.value || !username.value || !password.value) {
    errorMessage.value = 'All fields are required'
    return
  }

  isLoading.value = true

  try {
    await RegisterUser(firstName.value, lastName.value, username.value, password.value)
    successMessage.value = 'Registration successful!'
    
    // Clear form
    firstName.value = ''
    lastName.value = ''
    username.value = ''
    password.value = ''
  } catch (error: any) {
    console.error('Registration error:', error)
    errorMessage.value = error?.message || 'Failed to register user. Please try again.'
  } finally {
    isLoading.value = false
  }
}
</script>