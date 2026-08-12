import { useCallback, useEffect, useState } from 'react'
import {
  createAppointment,
  getDayAppointments,
  ApiError,
  type DayAppointment,
} from './lib/api'
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
const STATUSES = ['scheduled', 'checked_in', 'completed', 'cancelled']

// Local time → ISO 8601 UTC string for the API.
function toUtcIso(date: Date): string {
  return date.toISOString()
}

function today(): string {
  return new Date().toISOString().slice(0, 10)
}

function formatTime(utcIso: string): string {
  return new Date(utcIso).toLocaleString()
}

function App() {
  // Create form state
  const [patientId, setPatientId] = useState(PATIENTS[0].id)
  const [doctorId, setDoctorId] = useState(DOCTORS[0].id)
  const [start, setStart] = useState('')
  const [reason, setReason] = useState('')
  const [slotMinutes, setSlotMinutes] = useState(30)
  const [message, setMessage] = useState<{ kind: 'ok' | 'error'; text: string } | null>(null)
  const [submitting, setSubmitting] = useState(false)

  // Day view state
  const [viewDate, setViewDate] = useState(today())
  const [viewDoctorId, setViewDoctorId] = useState('')
  const [viewStatus, setViewStatus] = useState('')
  const [appointments, setAppointments] = useState<DayAppointment[]>([])
  const [loading, setLoading] = useState(false)
  const [viewError, setViewError] = useState<string | null>(null)

  const loadDay = useCallback(async () => {
    setLoading(true)
    setViewError(null)
    try {
      const items = await getDayAppointments({
        date: viewDate,
        doctorId: viewDoctorId || undefined,
        status: viewStatus || undefined,
      })
      setAppointments(items)
    } catch (err) {
      const text =
        err instanceof ApiError
          ? `${err.code ?? 'Error'}: ${err.message}`
          : 'Failed to load the schedule.'
      setViewError(text)
      setAppointments([])
    } finally {
      setLoading(false)
    }
  }, [viewDate, viewDoctorId, viewStatus])

  useEffect(() => {
    void loadDay()
  }, [loadDay])

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
      void loadDay()
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

      <section className="day-view">
        <h2>Day view</h2>
        <div className="day-filters">
          <label>
            Date
            <input
              type="date"
              value={viewDate}
              onChange={(e) => setViewDate(e.target.value)}
            />
          </label>
          <label>
            Doctor
            <select value={viewDoctorId} onChange={(e) => setViewDoctorId(e.target.value)}>
              <option value="">All doctors</option>
              {DOCTORS.map((d) => (
                <option key={d.id} value={d.id}>
                  {d.name}
                </option>
              ))}
            </select>
          </label>
          <label>
            Status
            <select value={viewStatus} onChange={(e) => setViewStatus(e.target.value)}>
              <option value="">All statuses</option>
              {STATUSES.map((s) => (
                <option key={s} value={s}>
                  {s}
                </option>
              ))}
            </select>
          </label>
        </div>

        {loading && <p className="muted">Loading…</p>}
        {viewError && <p className="error-banner" role="alert">{viewError}</p>}
        {!loading && !viewError && appointments.length === 0 && (
          <p className="muted">No appointments for this day.</p>
        )}

        {appointments.length > 0 && (
          <table className="appointment-table">
            <thead>
              <tr>
                <th>Time</th>
                <th>Patient</th>
                <th>Doctor</th>
                <th>Status</th>
                <th>Reason</th>
              </tr>
            </thead>
            <tbody>
              {appointments.map((a) => (
                <tr key={a.id}>
                  <td>{formatTime(a.startUtc)}</td>
                  <td>{a.patientName}</td>
                  <td>{a.doctorName}</td>
                  <td>{a.status}</td>
                  <td>{a.reason}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>
    </main>
  )
}

export default App