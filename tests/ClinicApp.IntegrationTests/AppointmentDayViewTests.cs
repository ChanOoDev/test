using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClinicApp.IntegrationTests;

public class AppointmentDayViewTests
{
    // Auth stub identities.
    private const string ReceptionistId = "11111111-1111-1111-1111-111111111102";
    private const string DoctorId = "11111111-1111-1111-1111-111111111103";
    private const string AdminId = "11111111-1111-1111-1111-111111111101";

    // Seed master data.
    private const string Alice = "22222222-2222-2222-2222-222222222201";
    private const string Bob = "22222222-2222-2222-2222-222222222202";
    private const string PatientOne = "33333333-3333-3333-3333-333333333301";

    private static readonly HttpClient _client = new ClinicApiFactory().CreateClient();

    // Each test uses a distinct offset day to avoid cross-test contamination.
    private static DateOnly Day(int offset) => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(offset));

    private static DateTimeOffset At(DateOnly day, string time)
        => new(day.ToDateTime(TimeOnly.Parse(time), DateTimeKind.Unspecified), TimeSpan.Zero);

    private static HttpRequestMessage Book(
        string patientId, string doctorId, DateTimeOffset start,
        string roleHeader = "receptionist", string userId = ReceptionistId)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/appointments")
        {
            Content = JsonContent.Create(new
            {
                patientId, doctorId,
                startUtc = start, slotMinutes = 30, reason = "day view test",
            }),
        };
        req.Headers.Add("X-Staff-UserId", userId);
        req.Headers.Add("X-Staff-Role", roleHeader);
        return req;
    }

    private static HttpRequestMessage GetDay(DateOnly date, string? roleHeader, string? userId, string? query = null)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, $"/api/appointments?date={date:yyyy-MM-dd}{query ?? string.Empty}");
        if (roleHeader is not null) req.Headers.Add("X-Staff-Role", roleHeader);
        if (userId is not null) req.Headers.Add("X-Staff-UserId", userId);
        return req;
    }

    [Fact]
    public async Task Receptionist_gets_all_appointments_for_day()
    {
        var day = Day(30);
        Assert.Equal(HttpStatusCode.Created, (await _client.SendAsync(Book(PatientOne, Alice, At(day, "09:00")))).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await _client.SendAsync(Book(PatientOne, Bob, At(day, "10:00")))).StatusCode);

        var res = await _client.SendAsync(GetDay(day, "receptionist", ReceptionistId));
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var items = doc.RootElement.GetProperty("appointments").EnumerateArray().ToList();
        Assert.Equal(2, items.Count);
    }

    [Fact]
    public async Task Doctor_gets_only_own_appointments()
    {
        var day = Day(31);
        Assert.Equal(HttpStatusCode.Created, (await _client.SendAsync(Book(PatientOne, Alice, At(day, "11:00")))).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await _client.SendAsync(Book(PatientOne, Bob, At(day, "12:00")))).StatusCode);

        var res = await _client.SendAsync(GetDay(day, "doctor", DoctorId));
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var items = doc.RootElement.GetProperty("appointments").EnumerateArray().ToList();
        Assert.Single(items);
        // Payload-level leak assertion: the returned appointment must be Alice's.
        Assert.Equal(Alice, items[0].GetProperty("doctorId").GetString());
    }

    [Fact]
    public async Task Doctor_cannot_read_other_doctors_appointments_even_with_doctorId_filter()
    {
        var day = Day(32);
        Assert.Equal(HttpStatusCode.Created, (await _client.SendAsync(Book(PatientOne, Bob, At(day, "13:00")))).StatusCode);

        // Doctor requests day with doctorId=Bob (someone else). Must be ignored.
        var res = await _client.SendAsync(GetDay(day, "doctor", DoctorId, $"&doctorId={Bob}"));
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var items = doc.RootElement.GetProperty("appointments").EnumerateArray().ToList();
        Assert.Empty(items);
    }

    [Fact]
    public async Task Status_filter_is_applied_server_side()
    {
        var day = Day(33);
        Assert.Equal(HttpStatusCode.Created, (await _client.SendAsync(Book(PatientOne, Alice, At(day, "14:00")))).StatusCode);

        // No cancelled appointments exist; filtered query must be empty.
        var res = await _client.SendAsync(GetDay(day, "receptionist", ReceptionistId, "&status=cancelled"));
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var items = doc.RootElement.GetProperty("appointments").EnumerateArray().ToList();
        Assert.Empty(items);
    }

    [Fact]
    public async Task Empty_day_returns_200_empty_list()
    {
        var res = await _client.SendAsync(GetDay(Day(34), "receptionist", ReceptionistId));
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);

        var body = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var items = doc.RootElement.GetProperty("appointments").EnumerateArray().ToList();
        Assert.Empty(items);
    }

    [Fact]
    public async Task Unauthenticated_day_request_returns_401()
    {
        var res = await _client.SendAsync(GetDay(Day(35), null, null));
        Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
    }
}
