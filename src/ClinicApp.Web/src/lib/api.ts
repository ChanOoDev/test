// Local auth stub (R4) — sends staff identity headers used in dev.
// Swap for real tokens when a real identity provider is wired in.
export const AUTH_HEADERS = {
  'X-Staff-UserId': '11111111-1111-1111-1111-111111111102', // receptionist
  'X-Staff-Role': 'receptionist',
}

export interface AppointmentResponse {
  id: string
  patientId: string
  doctorId: string
  startUtc: string
  slotMinutes: number
  status: string
}

export interface CreateAppointmentInput {
  patientId: string
  doctorId: string
  startUtc: string
  slotMinutes: number
  reason?: string
}

export interface DayAppointment {
  id: string
  patientId: string
  patientName: string
  doctorId: string
  doctorName: string
  startUtc: string
  slotMinutes: number
  status: string
  reason: string
}

export interface GetDayInput {
  date: string // YYYY-MM-DD
  doctorId?: string
  status?: string
}

export async function getDayAppointments(
  input: GetDayInput,
): Promise<DayAppointment[]> {
  const params = new URLSearchParams({ date: input.date })
  if (input.doctorId) params.set('doctorId', input.doctorId)
  if (input.status) params.set('status', input.status)

  const res = await fetch(`/api/appointments?${params}`, {
    headers: { ...AUTH_HEADERS },
  })

  if (!res.ok) {
    let detail = `Request failed (${res.status})`
    let code: string | undefined
    try {
      const body = await res.json()
      if (body.detail) detail = body.detail
      if (body.title) code = body.title
    } catch {
      /* non-JSON error body */
    }
    throw new ApiError(res.status, detail, code)
  }

  const body = await res.json()
  return body.appointments as DayAppointment[]
}

export class ApiError extends Error {
  status: number
  code?: string

  constructor(status: number, message: string, code?: string) {
    super(message)
    this.status = status
    this.code = code
  }
}

export async function createAppointment(
  input: CreateAppointmentInput,
): Promise<AppointmentResponse> {
  const res = await fetch('/api/appointments', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      ...AUTH_HEADERS,
    },
    body: JSON.stringify(input),
  })

  if (!res.ok) {
    let detail = `Request failed (${res.status})`
    let code: string | undefined
    try {
      const body = await res.json()
      if (body.detail) detail = body.detail
      if (body.title) code = body.title
    } catch {
      /* non-JSON error body */
    }
    throw new ApiError(res.status, detail, code)
  }

  return res.json()
}
