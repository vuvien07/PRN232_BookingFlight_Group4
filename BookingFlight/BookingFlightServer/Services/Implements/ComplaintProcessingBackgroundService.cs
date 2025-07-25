using BookingFlightServer.Services;
using BookingFlightServer.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BookingFlightServer.Services.Implements
{
    public class ComplaintProcessingBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ComplaintProcessingBackgroundService> _logger;
        private static bool _isEnabled = true; // Flag to enable/disable processing

        public ComplaintProcessingBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<ComplaintProcessingBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public static void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;
        }

        public static bool IsEnabled => _isEnabled;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_isEnabled)
                    {
                        await ProcessPendingComplaints();
                    }
                    else
                    {
                        _logger.LogInformation("Complaint processing is disabled, skipping this cycle");
                    }
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Check every 10 seconds
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in ComplaintProcessingBackgroundService");
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Wait 30 seconds before retrying on error
                }
            }
        }

        private async Task ProcessPendingComplaints()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var complaintRepository = scope.ServiceProvider.GetRequiredService<IComplaintRepository>();
            var geminiAiService = scope.ServiceProvider.GetRequiredService<IGeminiAIService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            try
            {
                // Get all pending complaints that are at least 30 seconds old
                var thirtySecondsAgo = DateTime.Now.AddSeconds(-30);
                var pendingComplaints = await complaintRepository.GetComplaintsByStatusAsync(3); // StatusId = 3 is Pending
                
                var complaintsToProcess = pendingComplaints
                    .Where(c => c.CreateAt.HasValue && c.CreateAt.Value <= thirtySecondsAgo)
                    .ToList();

                _logger.LogInformation($"Found {complaintsToProcess.Count} complaints to process (created more than 30 seconds ago)");

                foreach (var complaint in complaintsToProcess)
                {
                    try
                    {
                        _logger.LogInformation($"Processing complaint {complaint.ComplaintId} for AI review (created at: {complaint.CreateAt})");

                        // Check if complaint is relevant using Gemini AI
                        bool isRelevant = await geminiAiService.IsComplaintRelevantAsync(complaint.Description);

                        if (!isRelevant)
                        {
                            // Update status to Reject (StatusId = 5)
                            await complaintRepository.UpdateComplaintStatusAsync(complaint.ComplaintId, 5);

                            // Generate rejection reason
                            string rejectionReason = await geminiAiService.GenerateRejectionReasonAsync(complaint.Description);

                            // Send warning email to customer
                            await SendRejectionEmail(complaint, rejectionReason, emailService);

                            _logger.LogInformation($"Complaint {complaint.ComplaintId} rejected automatically due to irrelevant content");
                        }
                        else
                        {
                            _logger.LogInformation($"Complaint {complaint.ComplaintId} is relevant and remains pending for supporter review");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing complaint {complaint.ComplaintId}");
                        // Continue processing other complaints even if one fails
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessPendingComplaints");
            }
        }

        private async Task SendRejectionEmail(BookingFlightServer.Entities.Complaint complaint, string rejectionReason, IEmailService emailService)
        {
            try
            {
                var customerEmail = complaint.Customer?.Email;
                var customerName = complaint.Customer?.Fullname;

                if (string.IsNullOrEmpty(customerEmail))
                {
                    _logger.LogWarning($"No email found for complaint {complaint.ComplaintId}, cannot send rejection notification");
                    return;
                }

                await emailService.SendComplaintRejectionEmailAsync(
                    customerEmail, 
                    customerName, 
                    complaint.ComplaintId, 
                    complaint.Description, 
                    rejectionReason, 
                    complaint.CreateAt ?? DateTime.Now);

                _logger.LogInformation($"Rejection email sent to {customerEmail} for complaint {complaint.ComplaintId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending rejection email for complaint {complaint.ComplaintId}");
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ComplaintProcessingBackgroundService is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}
