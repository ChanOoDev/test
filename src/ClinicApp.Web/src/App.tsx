import { useState } from 'react'
import { createAppointment, ApiError } from './lib/api'
import './App.css'

// Stub master data (R4/A6) — patients and doctors seeded in the backend.
const PATIENTS = [
  { id: '33333333-3333-3333-3333-333333333301', name: 'Patient One' },
  { id: '33333333-3333-3333-3333-333333333302', name: 'Patient Two' },
]
const DOCTORS = [
  { id: '22222222-2222-2222-2222-222222222201', name: 'Dr. Alice' },
  { id: '22222222-2222-2222-2222-222222222202', name: 'Dr. Bob' },
]

// Local time → ISO 8601 UTC string for the API.
function toUtcIso(date: Date): string {
  return date.toISOString()
}

function App() {
  const [patientId, setPatientId] = useState(PATIENTS[0].id)
  const [doctorId, setDoctorId] = useState(DOCTORS[0].id)
  const [start, setStart] = useState('')
  const [reason, setReason] = useState('')
  const [slotMinutes, setSlotMinutes] = useState(30)
  const [message, setMessage] = useState<{ kind: 'ok' | 'error'; text: string } | null>(null)
  const [submitting, setSubmitting] = useState(false)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    if (!start) {
      setMessage({ kind: 'error', text: 'Please choose a date and time.' })
      return
    }
    setSubmitting(true)
    setMessage(null)
    try {
      const created = await createAppointment({
        patientId,
        doctorId,
        startUtc: toUtcIso(new Date(start)),
        slotMinutes,
        reason: reason || undefined,
      })
      setMessage({
        kind: 'ok',
        text: `Appointment created for ${new Date(created.startUtc).toLocaleString()} (${created.status}).`,
      })
      setStart('')
      setReason('')
    } catch (err) {
      const text =
        err instanceof ApiError
          ? `${err.code ?? 'Error'}: ${err.message}`
          : 'Failed to reach the server.'
      setMessage({ kind: 'error', text })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <main className="appointment-main">
      <h1>Clinic — Book Appointment</h1>
      <form className="appointment-form" onSubmit={handleSubmit} noValidate>
        <label>
          Patient
          <select value={patientId} onChange={(e) => setPatientId(e.target.value)}>
            {PATIENTS.map((p) => (
              <option key={p.id} value={p.id}>
                {p.name}
              </option>
            ))}
          </select>
        </label>

        <label>
          Doctor
          <select value={doctorId} onChange={(e) => setDoctorId(e.target.value)}>
            {DOCTORS.map((d) => (
              <option key={d.id} value={d.id}>
                {d.name}
              </option>
            ))}
          </select>
        </label>

        <label>
          Date &amp; time
          <input
            type="datetime-local"
            value={start}
            onChange={(e) => setStart(e.target.value)}
            required
          />
        </label>

        <label>
          Duration (minutes)
          <select
            value={slotMinutes}
            onChange={(e) => setSlotMinutes(Number(e.target.value))}
          >
            <option value={30}>30</option>
            <option value={60}>60</option>
          </select>
        </label>

        <label>
          Reason
          <input
            type="text"
            value={reason}
            onChange={(e) => setReason(e.target.value)}
            maxLength={500}
            placeholder="e.g. Annual checkup"
          />
        </label>

        <button type="submit" disabled={submitting}>
          {submitting ? 'Booking…' : 'Book appointment'}
        </button>
      </form>

      {message && (
        <p className={message.kind === 'ok' ? 'ok-banner' : 'error-banner'} role="status">
          {message.text}
        </p>
      )}
    </main>
  )
}

export default App