<template>
  <div class="max-w-md mx-auto mt-10 px-4">
    <div class="bg-white/90 backdrop-blur-sm rounded-3xl shadow-2xl p-8 border border-lavender-200">
      <div class="text-center mb-8">
        <div class="inline-block p-3 bg-gradient-to-br from-lavender-400 to-purple-500 rounded-2xl mb-4">
          <i class="pi pi-user-plus text-4xl text-white"></i>
        </div>
        <h1 class="text-4xl font-bold bg-gradient-to-r from-lavender-600 to-purple-600 bg-clip-text text-transparent">
          Create Account
        </h1>
        <p class="text-purple-600 mt-2">Join Zinc today</p>
      </div>

      <form @submit.prevent="handleSubmit" class="space-y-5">
        <div>
          <label for="firstName" class="block text-sm font-semibold text-purple-700 mb-2">
            First Name
          </label>
          <InputText
            id="firstName"
            v-model="firstName"
            type="text"
            placeholder="Enter first name"
            required
            class="w-full border-lavender-300 focus:border-lavender-500 focus:ring-lavender-500"
          />
        </div>

        <div>
          <label for="lastName" class="block text-sm font-semibold text-purple-700 mb-2">
            Last Name
          </label>
          <InputText
            id="lastName"
            v-model="lastName"
            type="text"
            placeholder="Enter last name"
            required
            class="w-full border-lavender-300 focus:border-lavender-500 focus:ring-lavender-500"
          />
        </div>

        <div>
          <label for="username" class="block text-sm font-semibold text-purple-700 mb-2">
            Username
          </label>
          <InputText
            id="username"
            v-model="username"
            type="text"
            placeholder="Enter username"
            required
            class="w-full border-lavender-300 focus:border-lavender-500 focus:ring-lavender-500"
          />
        </div>

        <div>
          <label for="password" class="block text-sm font-semibold text-purple-700 mb-2">
            Password
          </label>
          <Password
            id="password"
            v-model="password"
            placeholder="Enter password"
            toggleMask
            required
            class="w-full"
            :inputClass="'w-full border-lavender-300'"
          />
        </div>

        <Message v-if="errorMessage" severity="error" class="mt-4">
          {{ errorMessage }}
        </Message>

        <Message v-if="successMessage" severity="success" class="mt-4">
          {{ successMessage }}
        </Message>

        <Button
          type="submit"
          label="Create Account"
          icon="pi pi-user-plus"
          :loading="isLoading"
          class="w-full mt-6 bg-gradient-to-r from-lavender-500 to-purple-600 border-0 py-3 text-lg font-semibold hover:shadow-lg transform hover:scale-[1.02] transition-all duration-300"
        />

        <div class="text-center mt-6">
          <router-link to="/" class="text-sm text-purple-600 hover:text-purple-800 font-medium transition-colors duration-200">
            ← Back to Home
          </router-link>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
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
  isLoading.value = true

  try {
    await RegisterUser(firstName.value, lastName.value, username.value, password.value)
    successMessage.value = 'Registration successful!'
    
    // Clear form
    firstName.value = ''
    lastName.value = ''
    username.value = ''
    password.value = ''
    
    // Optionally redirect to home or login after a delay
    setTimeout(() => {
      router.push('/')
    }, 2000)
  } catch (error: any) {
    errorMessage.value = error?.message || 'Failed to register user. Please try again.'
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
/* Additional styles can be added here if needed */
</style>
