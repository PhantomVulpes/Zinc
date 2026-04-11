<template>
  <div class="pt-12 pb-8 px-4">
    <Card class="w-full max-w-md mx-auto bg-white/90 backdrop-blur-md shadow-2xl rounded-2xl">
      <template #title>
        <div class="flex items-center gap-2 text-2xl text-purple-900 px-8 pt-6">
          <i class="pi pi-sign-in text-purple-600"></i>
          Sign In
        </div>
      </template>
      
      <template #content>
        <form @submit.prevent="handleSubmit" class="flex flex-col gap-5 px-8 pb-6">
          <!-- Error Message -->
          <Message v-if="errorMessage" severity="error" :closable="true" @close="errorMessage = ''">
            {{ errorMessage }}
          </Message>

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
            label="Sign In"
            icon="pi pi-sign-in"
            :loading="isLoading"
            severity="primary"
            class="w-full mt-2"
          />

          <!-- Register Link -->
          <div class="text-center text-sm text-purple-700">
            Don't have an account?
            <a 
              @click="router.push('/register')" 
              class="font-semibold hover:text-purple-900 cursor-pointer underline"
            >
              Register here
            </a>
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
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Password from 'primevue/password'
import Message from 'primevue/message'
import { LoginUser } from '@/api/User/LoginUserCommand'
import { useAuth } from '@/services/auth'

const router = useRouter()
const { signIn } = useAuth()

const username = ref('')
const password = ref('')
const isLoading = ref(false)
const errorMessage = ref('')

async function handleSubmit() {
  errorMessage.value = ''
  
  // Basic validation
  if (!username.value || !password.value) {
    errorMessage.value = 'Both username and password are required'
    return
  }

  isLoading.value = true

  try {
    const loginResponse = await LoginUser(username.value, password.value)
    
    // Store authentication data
    signIn(loginResponse)
    
    // Redirect to home
    router.push('/')
  } catch (error: any) {
    console.error('Login error:', error)
    errorMessage.value = error?.message || 'Invalid username or password. Please try again.'
  } finally {
    isLoading.value = false
  }
}
</script>
