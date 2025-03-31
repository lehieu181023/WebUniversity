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
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<DBContext>();

                        // Kiểm tra lỗi database trước khi backup
                        var checkDbQuery = "DBCC CHECKDB(MyDatabase) WITH NO_INFOMSGS, ALL_ERRORMSGS";
                        var checkResult = await dbContext.Database.ExecuteSqlRawAsync(checkDbQuery);
                        if (checkResult < 0)
                        {
                            _logger.LogError("[Backup] Phát hiện lỗi trong database! Dừng backup.");
                            SendEmail("Lỗi trong Database", "Backup không được thực hiện vì database có lỗi.");
                            return;
                        }

                        // Đường dẫn file backup mới
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        var backupPath = $"{_backupFolder}\\MyDatabase_{timestamp}.bak";
                        var latestBackup = $"{_backupFolder}\\MyDatabase_Latest.bak";

                        // Lưu bản backup cũ vào thư mục an toàn trước khi tạo backup mới
                        if (File.Exists(latestBackup))
                        {
                            var safeBackupPath = $"{_safeBackupFolder}\\MyDatabase_{timestamp}.bak";
                            File.Move(latestBackup, safeBackupPath);
                        }

                        // Chạy lệnh backup
                        var backupQuery = $"BACKUP DATABASE MyDatabase TO DISK = '{latestBackup}' WITH CHECKSUM, FORMAT, INIT, COMPRESSION, STATS = 10";
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

                // Đợi 24 giờ trước khi chạy lại
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private void CleanOldBackups()
        {
            var files = new DirectoryInfo(_backupFolder).GetFiles("MyDatabase_*.bak");
            foreach (var file in files)
            {
                if (file.CreationTime < DateTime.Now.AddDays(-_retentionDays))
                {
                    file.Delete();
                    _logger.LogInformation($"[Backup] Đã xóa bản backup cũ: {file.Name}");
                }
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
