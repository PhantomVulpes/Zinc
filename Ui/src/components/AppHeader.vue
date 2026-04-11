<template>
  <div class="bg-gradient-to-r from-lavender-100 via-purple-100 to-lavender-200 border-b border-lavender-300 shadow-md">
    <div class="w-full px-6 py-4">
      <div class="flex items-center justify-between max-w-7xl mx-auto">
        <!-- Left: App Title & Navigation -->
        <div class="flex items-center gap-6">
          <RouterLink to="/" class="text-xl font-bold text-purple-900 hover:text-purple-700 transition-colors cursor-pointer">
            ✨ Shadesmar's Zinc
          </RouterLink>
          <RouterLink 
            to="/browse" 
            class="text-sm font-semibold text-purple-700 hover:text-purple-900 transition-colors flex items-center gap-1"
          >
          </RouterLink>
        </div>
        
        <!-- Right: Auth Actions or User Info -->
        <div v-if="isAuthenticated" class="flex items-center gap-3">
          <!-- Admin Button (only shown for administrators) -->
          <ZincButton
            v-if="isAdmin"
            label="Admin Stuff"
            icon="pi pi-shield"
            @click="router.push('/admin')"
            severity="info"
            outlined
            size="small"
          />

          <div class="text-right">
            <div class="font-semibold text-purple-900">{{ fullName }}</div>
            <div class="flex items-center justify-end gap-1 text-xs font-medium" :class="statusClasses">
              <i :class="statusIcon"></i>
              <span>{{ statusText }}</span>
            </div>
          </div>
          
          <ZincButton
            icon="pi pi-sign-out"
            @click="handleSignOut"
            severity="secondary"
            outlined
            size="small"
          />
        </div>
        
        <div v-else class="flex gap-2">
          <ZincButton
            icon="pi pi-sign-in"
            @click="router.push('/login')"
            severity="primary"
            size="small"
          />
          <ZincButton
            icon="pi pi-user-plus"
            @click="router.push('/register')"
            severity="secondary"
            outlined
            size="small"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useAuth } from '@/services/auth'
import { useRouter } from 'vue-router'
import ZincButton from '@/components/ZincButton.vue'
import { Role } from '@/api/apiclients/ZincApiClient'

const { user, isAuthenticated, signOut } = useAuth()
const router = useRouter()

const isAdmin = computed(() => {
  return isAuthenticated.value && user.value?.role === Role._3
})

const fullName = computed(() => {
  if (!user.value) return ''
  return `${user.value.firstName} ${user.value.lastName}`
})

const statusText = computed(() => {
  if (!user.value) return ''
  
  switch (user.value.role) {
    case Role._3: return 'Administrator'
    default: return ''
  }
})

const statusIcon = computed(() => {
  if (!user.value) return ''
  
  switch (user.value.role) {
    case Role._3: return 'pi pi-shield'
    default: return ''
  }
})

const statusClasses = computed(() => {
  if (!user.value) return ''
  
  switch (user.value.role) {
    case Role._3: return 'text-purple-600'
    default: return ''
  }
})

const handleSignOut = () => {
  signOut()
  router.push('/')
}
</script>
