import { TicketStatus } from '@/api/apiclients/ZincApiClient'

export interface TicketStatusClasses {
  bg: string
  text: string
  combined: string
}

export function getTicketStatusClasses(status: TicketStatus | undefined): TicketStatusClasses {
  if (status === undefined) {
    return {
      bg: 'bg-gray-100',
      text: 'text-gray-700',
      combined: 'bg-gray-100 text-gray-700'
    }
  }
  
  const classMap: Record<number, TicketStatusClasses> = {
    [TicketStatus._0]: {
      bg: 'bg-gray-100',
      text: 'text-gray-700',
      combined: 'bg-gray-100 text-gray-700'
    },
    [TicketStatus._1]: {
      bg: 'bg-yellow-100',
      text: 'text-yellow-700',
      combined: 'bg-yellow-100 text-yellow-700'
    },
    [TicketStatus._2]: {
      bg: 'bg-blue-100',
      text: 'text-blue-700',
      combined: 'bg-blue-100 text-blue-700'
    },
    [TicketStatus._3]: {
      bg: 'bg-purple-100',
      text: 'text-purple-700',
      combined: 'bg-purple-100 text-purple-700'
    },
    [TicketStatus._4]: {
      bg: 'bg-green-100',
      text: 'text-green-700',
      combined: 'bg-green-100 text-green-700'
    },
    [TicketStatus._5]: {
      bg: 'bg-red-100',
      text: 'text-red-700',
      combined: 'bg-red-100 text-red-700'
    }
  }
  
  return classMap[status] || {
    bg: 'bg-gray-100',
    text: 'text-gray-700',
    combined: 'bg-gray-100 text-gray-700'
  }
}

export function getTicketStatusIcon(status: TicketStatus | undefined): string {
  if (status === undefined) return 'pi pi-question-circle'
  
  const iconMap: Record<number, string> = {
    [TicketStatus._0]: 'pi pi-question-circle',
    [TicketStatus._1]: 'pi pi-eye',
    [TicketStatus._2]: 'pi pi-inbox',
    [TicketStatus._3]: 'pi pi-clock',
    [TicketStatus._4]: 'pi pi-check-circle',
    [TicketStatus._5]: 'pi pi-times-circle'
  }
  
  return iconMap[status] || 'pi pi-question-circle'
}

export function formatTicketStatus(status: TicketStatus | undefined): string {
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
