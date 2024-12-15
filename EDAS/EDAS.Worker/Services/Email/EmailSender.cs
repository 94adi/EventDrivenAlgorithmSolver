namespace EDAS.Worker.Services.Email;

public class EmailSender : IEmailSender
{
    private readonly EmailConfig _emailConfig;

    public EmailSender(IOptions<EmailConfig> emailOptions)
    {
        _emailConfig = emailOptions.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var client = new HttpClient();

        var emailContent = new
        {
            from = new { email = _emailConfig.FromEmail },
            to = new[] { new { email = toEmail } },
            subject = subject,
            text = body
        };

        var jsonContent = JsonConvert.SerializeObject(emailContent);

        var request = new HttpRequestMessage(HttpMethod.Post, _emailConfig.ApiUrl)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _emailConfig.ApiKey);

        try
        {
            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
        }
    }
}
