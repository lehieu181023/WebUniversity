namespace WebUniversity.Models
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;

    public class BackupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BackupService> _logger;
        private readonly string _backupFolder = "D:\\Backups";
        private readonly string _safeBackupFolder = "D:\\SafeBackups";
        private readonly int _retentionDays = 7; // Giữ lại backup trong 7 ngày

        public BackupService(IServiceScopeFactory scopeFactory, ILogger<BackupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[Backup] Ứng dụng khởi động. Đợi 30 ngày trước lần backup đầu tiên.");

            try
            {
                await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("[Backup] Quá trình backup bị hủy trước khi chạy lần đầu tiên.");
                return; // Nếu service bị hủy trong lúc chờ, thoát luôn
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("[Backup] Bắt đầu thực hiện backup.");

                    await ExecuteBackup();

                    _logger.LogInformation("[Backup] Đã hoàn thành backup. Đợi 30 ngày trước lần tiếp theo.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Backup] Lỗi khi thực hiện backup.");
                }

                // Đợi 24 giờ trước khi chạy backup tiếp theo
                try
                {
                    await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("[Backup] Quá trình backup đã bị hủy.");
                    break; // Nếu service bị hủy, thoát vòng lặp
                }
            }
        }


        private async Task ExecuteBackup()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();

                    // Kiểm tra lỗi database trước khi backup
                    if (!await CheckDatabaseIntegrity(dbContext))
                    {
                        _logger.LogError("[Backup] Phát hiện lỗi trong database! Dừng backup.");
                        SendEmail("Lỗi trong Database", "Backup không được thực hiện vì database có lỗi.");
                        return;
                    }


                    // Tạo thư mục backup nếu chưa tồn tại
                    if (!Directory.Exists(_backupFolder))
                        Directory.CreateDirectory(_backupFolder);
                    if (!Directory.Exists(_safeBackupFolder))
                        Directory.CreateDirectory(_safeBackupFolder);

                    // Đường dẫn file backup mới
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var backupPath = $"{_backupFolder}\\EastAsiaUOfT_{timestamp}.bak";
                    var latestBackup = $"{_backupFolder}\\EastAsiaUOfT_Latest.bak";

                    // Di chuyển backup cũ sang thư mục an toàn
                    if (File.Exists(latestBackup))
                    {
                        var safeBackupPath = $"{_safeBackupFolder}\\EastAsiaUOfT_{timestamp}.bak";
                        File.Move(latestBackup, safeBackupPath);
                        _logger.LogInformation($"[Backup] Đã lưu backup cũ vào thư mục an toàn: {safeBackupPath}");
                    }

                    // Chạy lệnh backup
                    var backupQuery = $"BACKUP DATABASE EastAsiaUOfT TO DISK = '{latestBackup}' WITH CHECKSUM, FORMAT, INIT, STATS = 10";
                    await dbContext.Database.ExecuteSqlRawAsync(backupQuery);

                    _logger.LogInformation($"[Backup] Đã sao lưu dữ liệu vào {backupPath}");

                    // Xóa các bản backup cũ hơn _retentionDays ngày
                    CleanOldBackups();

                    // Gửi email thông báo backup thành công
                    SendEmail("Backup Thành Công", $"Database đã được backup vào {backupPath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Backup] Lỗi: {ex.Message}");
                SendEmail("Backup Thất Bại", $"Lỗi khi sao lưu database: {ex.Message}");
            }
        }
        private async Task<bool> CheckDatabaseIntegrity(DBContext dbContext)
        {
            try
            {
                using (var command = dbContext.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "DBCC CHECKDB(EastAsiaUOfT) WITH NO_INFOMSGS, ALL_ERRORMSGS";
                    command.CommandTimeout = 600; // Đặt timeout 10 phút
                    await dbContext.Database.OpenConnectionAsync();
                    await command.ExecuteNonQueryAsync();
                    return true; // Không có lỗi
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Backup] Lỗi kiểm tra DB: {ex.Message}");
                return false; // Có lỗi trong database
            }
            finally
            {
                await dbContext.Database.CloseConnectionAsync();
            }
        }

        private void CleanOldBackups()
        {
            try
            {
                var files = new DirectoryInfo(_backupFolder).GetFiles("EastAsiaUOfT_*.bak");
                foreach (var file in files)
                {
                    if (file.CreationTime < DateTime.Now.AddDays(-_retentionDays))
                    {
                        file.Delete();
                        _logger.LogInformation($"[Backup] Đã xóa bản backup cũ: {file.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Backup] Lỗi khi xóa backup cũ: {ex.Message}");
            }
        }

        private void SendEmail(string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("20214045@eaut.edu.vn", "#include"),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("20214045@eaut.edu.vn"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                mailMessage.To.Add("admin@example.com");

                smtpClient.Send(mailMessage);
                _logger.LogInformation($"[Backup] Email thông báo đã được gửi: {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[Backup] Không thể gửi email: {ex.Message}");
            }
        }
    }
}
