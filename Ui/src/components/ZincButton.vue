<template>
  <button
    :type="type"
    :disabled="disabled || loading"
    @click="$emit('click', $event)"
    :class="buttonClasses"
  >
    <i v-if="loading" class="pi pi-spinner pi-spin mr-2"></i>
    <i v-else-if="icon" :class="[icon, { 'mr-2': label }]"></i>
    <span v-if="label">{{ label }}</span>
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  label?: string
  icon?: string
  severity?: 'primary' | 'secondary' | 'danger' | 'success' | 'info'
  disabled?: boolean
  loading?: boolean
  type?: 'button' | 'submit' | 'reset'
  text?: boolean
  outlined?: boolean
  small?: boolean
  size?: 'small' | 'normal'
}

const props = withDefaults(defineProps<Props>(), {
  severity: 'primary',
  type: 'button',
  disabled: false,
  loading: false,
  text: false,
  outlined: false,
  small: false,
  size: 'normal'
})

defineEmits<{
  click: [event: MouseEvent]
}>()

const buttonClasses = computed(() => {
  const baseClasses = 'font-semibold rounded-lg transition-all inline-flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed'
  const sizeClasses = (props.small || props.size === 'small') ? 'px-4 py-2 text-sm' : 'px-6 py-3'
  
  if (props.text) {
    return `${baseClasses} ${sizeClasses} text-purple-600 hover:text-purple-800 hover:bg-purple-50 shadow-none`
  }
  
  if (props.outlined) {
    const outlinedClasses: Record<string, string> = {
      primary: 'border-2 border-purple-600 text-purple-600 hover:bg-purple-50',
      secondary: 'border-2 border-gray-400 text-gray-700 hover:bg-gray-50',
      danger: 'border-2 border-red-600 text-red-600 hover:bg-red-50',
      success: 'border-2 border-green-600 text-green-600 hover:bg-green-50',
      info: 'border-2 border-blue-600 text-blue-600 hover:bg-blue-50'
    }
    return `${baseClasses} ${sizeClasses} ${outlinedClasses[props.severity] || outlinedClasses.primary}`
  }
  
  const solidClasses: Record<string, string> = {
    primary: 'bg-purple-600 hover:bg-purple-700 text-white shadow-md hover:shadow-lg',
    secondary: 'bg-gray-200 hover:bg-gray-300 text-gray-700 shadow-md hover:shadow-lg',
    danger: 'bg-red-600 hover:bg-red-700 text-white shadow-md hover:shadow-lg',
    success: 'bg-green-600 hover:bg-green-700 text-white shadow-md hover:shadow-lg',
    info: 'bg-blue-600 hover:bg-blue-700 text-white shadow-md hover:shadow-lg'
  }
  
  return `${baseClasses} ${sizeClasses} ${solidClasses[props.severity] || solidClasses.primary}`
})
</script>
