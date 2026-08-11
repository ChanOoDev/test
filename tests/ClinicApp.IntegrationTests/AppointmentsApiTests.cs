using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClinicApp.IntegrationTests;

public class AppointmentsApiTests
{
    // Stub identity (R4) for authenticated requests.
    private const string ReceptionistId = "11111111-1111-1111-1111-111111111102";
    private const string DoctorId = "11111111-1111-1111-1111-111111111103";
    private const string AdminId = "11111111-1111-1111-1111-111111111101";

    // Seed master data.
    private const string Alice = "22222222-2222-2222-2222-222222222201";
    private const string Bob = "22222222-2222-2222-2222-222222222202";
    private const string PatientOne = "33333333-3333-3333-3333-333333333301";
    private const string PatientInactive = "33333333-3333-3333-3333-333333333303";

    private static readonly HttpClient _client = new ClinicApiFactory().CreateClient();

    private static HttpRequestMessage Book(
        string patientId,
        string doctorId,
        DateTimeOffset start,
        int slotMinutes = 30,
        string? reason = null,
        string? roleHeader = "receptionist",
        string? userId = ReceptionistId)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/appointments")
        {
            Content = JsonContent.Create(new
            {
                patientId,
                doctorId,
                startUtc = start,
                slotMinutes,
                reason,
            }),
        };

        if (userId is not null)
            req.Headers.Add("X-Staff-UserId", userId);
        if (roleHeader is not null)
            req.Headers.Add("X-Staff-Role", roleHeader);

        return req;
    }

    private static DateTimeOffset Future(int dayOffset)
    {
        // Compute an absolute guaranteed-future UTC timestamp (aligned to :00).
        var now = DateTimeOffset.UtcNow.AddDays(dayOffset);
        return new DateTimeOffset(now.Year, now.Month, now.Day, 20, 0, 0, TimeSpan.Zero).AddHours(6);
    }

    [Fact]
    public async Task Create_as_receptionist_returns_201()
    {
        var res = await _client.SendAsync(Book(PatientOne, Alice, Future(30)));
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var body = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        Assert.Equal("Scheduled", doc.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Create_without_auth_returns_401()
    {
        var res = await _client.SendAsync(Book(PatientOne, Alice, Future(30), roleHeader: null, userId: null));
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }

    [Fact]
    public async Task Create_as_doctor_returns_403()
    {
        var res = await _client.SendAsync(Book(PatientOne, Alice, Future(30), roleHeader: "doctor", userId: DoctorId));
        Assert.Equal(HttpStatusCode.Forbidden, res.StatusCode);
    }

    [Fact]
    public async Task Create_as_admin_returns_201()
    {
        var res = await _client.SendAsync(Book(PatientOne, Bob, Future(31), roleHeader: "admin", userId: AdminId));
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
    }

    [Fact]
    public async Task Create_past_date_returns_422()
    {
        var past = DateTimeOffset.UtcNow.AddDays(-5);
        var res = await _client.SendAsync(Book(PatientOne, Alice, past));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
    }

    [Fact]
    public async Task Create_overlapping_slot_returns_409()
    {
        var start = Future(35);
        var first = await _client.SendAsync(Book(PatientOne, Alice, start));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await _client.SendAsync(Book(PatientOne, Alice, start));
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Create_adjacent_slot_is_allowed()
    {
        var start = Future(36);
        await _client.SendAsync(Book(PatientOne, Alice, start));

        var adjacent = await _client.SendAsync(Book(PatientOne, Alice, start.AddMinutes(30)));
        Assert.Equal(HttpStatusCode.Created, adjacent.StatusCode);
    }

    [Fact]
    public async Task Create_different_doctor_same_time_is_allowed()
    {
        var start = Future(37);
        await _client.SendAsync(Book(PatientOne, Alice, start));

        var other = await _client.SendAsync(Book(PatientOne, Bob, start));
        Assert.Equal(HttpStatusCode.Created, other.StatusCode);
    }

    [Fact]
    public async Task Create_unknown_patient_returns_422()
    {
        var res = await _client.SendAsync(Book("99999999-9999-9999-9999-999999999999", Alice, Future(38)));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
    }

    [Fact]
    public async Task Create_inactive_patient_returns_422()
    {
        var res = await _client.SendAsync(Book(PatientInactive, Alice, Future(39)));
        Assert.Equal(HttpStatusCode.UnprocessableEntity, res.StatusCode);
    }
}
