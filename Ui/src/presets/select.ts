import { SelectPassThroughOptions } from 'primevue/select'

export const selectPreset: SelectPassThroughOptions = {
  root: {
    class: [
      'relative',
      'inline-flex',
      'w-full',
      'cursor-pointer',
      'select-none',
      'bg-white',
      'border border-lavender-300',
      'transition-colors',
      'duration-200',
      'ease-in-out',
      'rounded-lg',
      'hover:border-lavender-400',
      'focus-within:outline-none',
      'focus-within:outline-offset-0',
      'focus-within:ring-2',
      'focus-within:ring-purple-500'
    ]
  },
  label: {
    class: [
      'block',
      'flex-auto',
      'overflow-hidden',
      'whitespace-nowrap',
      'cursor-pointer',
      'overflow-ellipsis',
      'text-purple-900',
      'p-3',
      'transition',
      'duration-200'
    ]
  },
  dropdown: {
    class: [
      'flex',
      'items-center',
      'justify-center',
      'shrink-0',
      'bg-transparent',
      'text-purple-600',
      'w-12',
      'rounded-tr-lg',
      'rounded-br-lg'
    ]
  },
  overlay: {
    class: [
      'bg-white',
      'text-purple-900',
      'border border-lavender-300',
      'rounded-lg',
      'shadow-lg',
      'max-h-[200px]',
      'overflow-auto'
    ]
  },
  list: {
    class: 'py-1 list-none m-0'
  },
  option: ({ context }: any) => ({
    class: [
      'cursor-pointer',
      'font-normal',
      'overflow-hidden',
      'relative',
      'whitespace-nowrap',
      'px-4',
      'py-2',
      'text-purple-900',
      'transition',
      'duration-200',
      {
        'bg-purple-50': context.focused && !context.selected,
        'bg-purple-500 text-white': context.selected,
        'hover:bg-purple-100': !context.selected
      }
    ]
  })
}
